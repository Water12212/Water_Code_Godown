using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System;
using System.Collections;

public class NetManager : MonoBehaviour
{
    static Queue<PlayerInfo> PlayerInfo_List = new Queue<PlayerInfo>();

    static Dictionary<string, GameObject> Person = new Dictionary<string, GameObject>();

    static Socket socket;

    public static string Client_Name;
    public static string Address;
    public static string Port;
    string str_Process;
    public GameObject Client_Player_Obj;

    public Text Name;
    public Text UI_Address;
    public Text UI_Port;

    public GameObject Player_Prefab;

    //--------------------ÐÔÄÜ¿ØÖÆ
    Vector3 lastPosition = Vector3.zero;
    Vector3 lastRotation = Vector3.zero;
    float positionThreshold = 0.01f;
    float rotationThreshold = 0.01f;

    public float Lerp_Speed;

    public class PlayerInfo
    {
        public string Name;
        public float pos_X, pos_Y, pos_Z;
        public float euler_X, euler_Y, euler_Z;



        public PlayerInfo(string Name, float posx, float posy, float posz, float eulerx, float eulery, float eulerz)
        {
            this.Name = Name;
            this.pos_X = posx;
            this.pos_Y = posy;
            this.pos_Z = posz;
            this.euler_X = eulerx;
            this.euler_Y = eulery;
            this.euler_Z = eulerz;
        }
    }


    public void TrytoConnect()
    {
        Client_Name = Name.text;
        Address = UI_Address.text;
        Port = UI_Port.text;
        Connect(Address, int.Parse(Port));
        Debug.Log("Try to connect\n" + Client_Name);
    }

    public void Connect(string ip, int port)
    {

        socket.Connect(ip, port);

        Thread Listen = new Thread(new ParameterizedThreadStart(ListenServer));
        Listen.Start(socket);

    }

    void ListenServer(object obj)
    {
        Socket socket = (Socket)obj;
        byte[] bytes = new byte[1024];
        while (true)
        {
            int len = socket.Receive(bytes);
            string str = Encoding.UTF8.GetString(bytes, 0, len);

            foreach (string s in str.Split('&'))
            {
                if (s.Length > 96 && s.Length < 107)
                {
                    try
                    {
                        PlayerInfo p = JsonConvert.DeserializeObject<PlayerInfo>(s);
                        PlayerInfo_List.Enqueue(p);
                        Debug.Log("Good data");
                    }
                    catch(Exception e)
                    {
                        Debug.Log("JSON------------->" + e);
                    }
                    
                }
            }
        }
    }
    IEnumerator Lerp_PlayerTransform(GameObject playerObj, Vector3 endPosition, Quaternion endRotation)
    {
        Vector3 startPosition = playerObj.transform.position;
        Quaternion startRotation = playerObj.transform.rotation;

        float lerpTime = 0.0f;
        while (lerpTime < Lerp_Speed)
        {
            lerpTime += Time.deltaTime;
            float lerpFactor = lerpTime / Lerp_Speed;

            playerObj.transform.position = Vector3.Lerp(startPosition, endPosition, lerpFactor);
            playerObj.transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpFactor);

            yield return null; 
        }

    
        playerObj.transform.position = endPosition;
        playerObj.transform.rotation = endRotation;
    }

    void Set_PlayerInfo(PlayerInfo p)
    {
        
        if (p.Name == Client_Name) return;

        if (Person.TryGetValue(p.Name, out GameObject playerObj))
        {
           
            Vector3 endPosition = new Vector3(p.pos_X, p.pos_Y, p.pos_Z);
            Quaternion endRotation = Quaternion.Euler(p.euler_X, p.euler_Y, p.euler_Z);

          
            StartCoroutine(Lerp_PlayerTransform(playerObj, endPosition, endRotation));
        }
        else
        {
           
            Vector3 startPosition = new Vector3(p.pos_X, p.pos_Y, p.pos_Z);
            Quaternion startRotation = Quaternion.Euler(p.euler_X, p.euler_Y, p.euler_Z);
            GameObject player_Obj = Instantiate(Player_Prefab, startPosition, startRotation);
            Person.Add(p.Name, player_Obj);
        }
    }
    void SendMessage(PlayerInfo p)
    {

        string str = JsonConvert.SerializeObject(p);
        byte[] bytes = Encoding.UTF8.GetBytes(str + "&");
        socket.Send(bytes);
    }
    PlayerInfo Get_PlayerInfo()
    {
        Vector3 currentPosition = Client_Player_Obj.transform.position;
        Vector3 currentRotation = Client_Player_Obj.transform.eulerAngles;

      
        bool positionChanged = Vector3.Distance(lastPosition, currentPosition) > positionThreshold;
        
        bool rotationChanged = Vector3.Distance(lastRotation, currentRotation) > rotationThreshold;

       
        if (positionChanged || rotationChanged)
        {
           
            lastPosition = currentPosition;
            lastRotation = currentRotation;

          
            return new PlayerInfo(
                (string)Client_Name,
                (float)Mathf.Round(currentPosition.x * 100) / 100f,
                (float)Mathf.Round(currentPosition.y * 100) / 100f,
                (float)Mathf.Round(currentPosition.z * 100) / 100f,
                (float)Mathf.Round(currentRotation.x * 100) / 100f,
                (float)Mathf.Round(currentRotation.y * 100) / 100f,
                (float)Mathf.Round(currentRotation.z * 100) / 100f
            );
        }

       
        return null;
    }

    private void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }
    private void Update()
    {

        if (socket.Connected)
        {
            SendMessage(Get_PlayerInfo());

            if (PlayerInfo_List.Count > 0)
            {
                Debug.Log(PlayerInfo_List.Count);
                Set_PlayerInfo(PlayerInfo_List.Dequeue());
            }
        }
    }
}
