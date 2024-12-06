using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics.SymbolStore;

namespace MyServer
{
    class Program
    {
        static List<Socket> UserOnline = new List<Socket>();

        static Queue<PlayerInfo> PlayerInfo_List = new Queue<PlayerInfo>();


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

        private static readonly object Lock = new object();

        static void Main(string[] args)
        {
            Thread Wait = new Thread(new ThreadStart(WaitClient));
            Wait.Start();

            while (true)
            {
                if (PlayerInfo_List.Count > 0)
                {
                    SendMessage(UserOnline, PlayerInfo_List.Dequeue());
                }

            }

        }

        static void WaitClient()
        {
            IPEndPoint pos = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8866);
            Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Listener.Bind(pos);
            Listener.Listen(10);

            Console.WriteLine("SERVER IS WORKING...");

            while (true)
            {
                Socket sClient = Listener.Accept();
                UserOnline.Add(sClient);
                Console.WriteLine("A Client has Connected!");

                Thread ListenClient = new Thread(new ParameterizedThreadStart(Listen_Client));
                ListenClient.Start(sClient);
            }

        }
        static void Listen_Client(Object obj)
        {
            Socket sClient = (Socket)obj;
            byte[] bytes = new byte[1024];

            while (true)
            {

                int len = sClient.Receive(bytes);
                string str = Encoding.UTF8.GetString(bytes, 0, len);

                foreach (string s in str.Split('&'))
                {
                    if (s.Length > 96 && s.Length < 107)
                    {
                        try
                        {
                            PlayerInfo p = JsonConvert.DeserializeObject<PlayerInfo>(s);
                            lock (Lock)
                            {
                                PlayerInfo_List.Enqueue(p);
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("JSON------------->" + ex);
                        }
                       
                    }
                }
            }
        }
        static string CheckPlayerInfoFormat(string str)
        {

            string pattern = @"^PlayerInfo\[\{\""Name\"":\""[^\""]+\"",""pos_X\"":[-+]?\d*\.\d+|\d+,""pos_Y\"":[-+]?\d*\.\d+|\d+,""pos_Z\"":[-+]?\d*\.\d+|\d+,""euler_X\"":[-+]?\d*\.\d+|\d+,""euler_Y\"":[-+]?\d*\.\d+|\d+,""euler_Z\"":[-+]?\d*\.\d+|\d+}\]$";


            if (System.Text.RegularExpressions.Regex.IsMatch(str, pattern) && str.Length > 100f && str.Length < 110)
            {

                return "Right";
            }
            else
            {

                return "Error: Invalid format";
            }
        }
         static void SendMessage(List<Socket> UserOnline, PlayerInfo PlayerInfo)
        {
            List<Socket> UserOnline_Send = new List<Socket>(UserOnline);
            foreach (Socket SClient in UserOnline_Send)
            {

                string str = JsonConvert.SerializeObject(PlayerInfo);
                byte[] bytes = Encoding.UTF8.GetBytes(str + "&");
                SClient.Send(bytes);
                Console.WriteLine("Send To Client" + str);
            }
           
        }
    }
}