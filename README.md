这是一个我自学的Unity+Socket多人联机解决方案。如果对你的学习有用，那我很开心。
其中NetManager.cs需要搭载在一个空物体上，是客户端的核心脚本，里面包括了向服务端发送玩家位置信息，更新其他客户端的玩家信息等方法。
Stylized Astronaut_ServerTest文件夹是Vs的工程文件，其中Program.cs是服务端的核心脚本。
当你打开服务端代码的时候可能using Newtonsoft.Json;会报错，是因为用到的Json序列化，只要在你的解决方案资源管理器里找到引用，右击管理NuGet程序包，搜索下载安装Json就可以了哦！
Stylized Astronaut_ServerTest的Bin里也有程序的Release版本也可以直接运行！
服务端没用添加用户连接与断开的异常处理，这也是为什么这是一个学习Demo的原因。如果你有兴趣，可以为它完成这个，当然在不久的将来我也会去完成。
在GitHub中我未上传Unity的全部文件（比如地图，角色预制体等），这可能对你的上手有一定的门槛，但是相信我，等你学成归来，这些或许依旧能帮助到你！
如果你想联系我，可以发送邮箱给：1281132008@qq.com 或者在Github里面发起新的话题！   By Water  Good Luck！

Bili： https://www.bilibili.com/video/BV1Hsi6Y1EW7/?share_source=copy_web&vd_source=69e34de0f892d82c28d9682b9175f252

This is a Unity + Socket multiplayer solution that I learned by myself. If it is useful for your learning, then I am very happy.
Among them, the NetManager.cs needs to be mounted on an empty object, which is the core script of the client, including methods such as sending the player's position information to the server and updating the player information of other clients.
The Stylized Astronaut_ServerTest folder is the engineering file of Vs, and the Program.cs is the core script of the server.
When you open the server code, the using Newtonsoft.Json; may report an error, because the Json serialization is used. As long as you find the reference in your solution explorer, right-click to manage the NuGet package, search, download and install Json!
There is also a Release version of the program in the Bin of Stylized Astronaut_ServerTest, which can be run directly!
The server does not add exception handling for user connection and disconnection, which is also the reason why this is a learning demo. If you are interested, you can complete this for it, and of course, I will also complete it in the near future.
In GitHub, I did not upload all the files of Unity (such as maps, character prefabs, etc.), which may have a certain threshold for your getting started, but believe me, when you return after learning, these may still be able to help you!
If you want to contact me, you can send an email to: 1281132008@qq.com or initiate a new topic in Github!      By Water  Good Luck！

Bili： https://www.bilibili.com/video/BV1Hsi6Y1EW7/?share_source=copy_web&vd_source=69e34de0f892d82c28d9682b9175f252
