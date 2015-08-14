/***************************************************************
 *                    C# Class                                 *
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using Lidgren.Network;

namespace DDJexamples
{
    public class ExerciseScalars
    {
        static NetClient client;

        // current input string
        
        private static bool s_keepGoing = true;

        static float _x, _y, _z, _pitch, _yaw, _roll;
        
        //C:\Bohemia Interactive\VBS2VBS2\plugins
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern float GetX();
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern void SetX(float i);    
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern float GetY();
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern void SetY(float i);
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern float GetZ();
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern void SetZ(float i);
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern float GetPitch();
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern void SetPitch(float i);
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern float GetRoll();
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern void SetRoll(float i);
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern float GetYaw();
        [DllImport("C:\\Bohemia Interactive\\VBS2\\plugins\\VBSPlugin.dll")] public static extern void SetYaw(float i);    

        public static void update() 
        {
            _x = GetX();
            _y = GetY();
            _z = GetZ();
            _pitch = GetPitch();
            _roll = GetRoll();
            _yaw = GetYaw();
        }

        public static void writeX(int y)
        {
            SetX(y);
        }

        public static void setupNetwork()
        {
            NetConfiguration config = new NetConfiguration("apache");
            
            client = new NetClient(config);
            client.SetMessageTypeEnabled(NetMessageType.ConnectionRejected, true);
            //client.SetMessageTypeEnabled(NetMessageType.DebugMessage, true);
            //client.SetMessageTypeEnabled(NetMessageType.VerboseDebugMessage, true);
            client.Start();

            // Wait half a second to allow server to start up if run via Visual Studio
            Thread.Sleep(500);

            // Emit discovery signal
            //client.DiscoverLocalServers(14242);
            client.Connect("127.0.0.1", 14242, Encoding.ASCII.GetBytes("Hail from client"));


            Thread.Sleep(500);

            
        }

        public static void Main(string[] args)
        {
            setupNetwork();

            // create a buffer to read data into
            NetBuffer buffer = client.CreateBuffer();

            s_keepGoing = true;
            while (s_keepGoing)
            {
                NetMessageType type;

                
                
                // check if any messages has been received
                while (client.ReadMessage(buffer, out type))
                {
                    
                    switch (type)
                    {
                        case NetMessageType.ServerDiscovered:
                            // just connect to any server found!

                            // make hail
                            NetBuffer buf = client.CreateBuffer();
                            buf.Write("Hail from " + Environment.MachineName);
                            client.Connect(buffer.ReadIPEndPoint(), buf.ToArray());
                            break;
                        case NetMessageType.ConnectionRejected:
                            Console.WriteLine("Rejected: " + buffer.ReadString());
                            break;
                        case NetMessageType.DebugMessage:
                        case NetMessageType.VerboseDebugMessage:
                            Console.WriteLine(buffer.ReadString());
                            break;
                        case NetMessageType.StatusChanged:
                            string statusMessage = buffer.ReadString();                            
                            Console.WriteLine("New status:   (" + statusMessage + ")");
                            break;
                        case NetMessageType.Data:
                            // The server sent this data!
                            string msg = buffer.ReadString();
                            Console.WriteLine(msg);
                            break;
                    }
                }
                 

                update();

                if (client.Status == NetConnectionStatus.Connected)
                {
                    NetBuffer sendBuffer = new NetBuffer();
                    sendBuffer.Write(" ApacheNetworkObject:VBS2:foo:" + _x + ":" + _y + ":" + _z + ":" + _pitch + ":" + _yaw + ":" + _roll);
                    client.SendMessage(sendBuffer, NetChannel.ReliableInOrder14);
                }
                else
                {
                    Console.Out.WriteLine("Client lost connection....");
                    s_keepGoing = false;
                }

                Thread.Sleep(10);
            }

            client.Shutdown("Application exiting");
            
        }
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            s_keepGoing = false;
        }
    }
   }