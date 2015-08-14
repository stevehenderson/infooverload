using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using GoblinXNA.Network;
using GoblinXNA.Helpers;

namespace PilotTool
{
   

    /// <summary>
    /// An implementation of INetworkObject interface for transmitting mouse press information.
    /// </summary>
    public class ApacheNetworkObject : INetworkObject
    {
        
        /// <summary>
        /// A delegate function to be called when a mouse press event is sent over the network
        /// </summary>
        /// <param name="near">A point on the near clipping plane</param>
        /// <param name="far">A point on the far clipping plane</param>
        public delegate void UpdateApache(String cmd, String response, double x, double y, double z, double pitch, double yaw, double roll);

        #region Member Fields

        private bool readyToSend;
        private bool hold;
        private int sendFrequencyInHertz;

        private bool reliable;
        private bool inOrder;
                
        private UpdateApache callbackFunc;

        #endregion

        #region Constructors

        public ApacheNetworkObject()
        {
            readyToSend = false;
            hold = false;
            sendFrequencyInHertz = 0;

            reliable = true;
            inOrder = true;
        }

        #endregion

        #region Properties
        public String Identifier
        {
            get { return "ApacheNetworkObject"; }
        }

        public bool ReadyToSend
        {
            get { return readyToSend; }
            set { readyToSend = value; }
        }

        public bool Hold
        {
            get { return hold; }
            set { hold = value; }
        }

        public int SendFrequencyInHertz
        {
            get { return sendFrequencyInHertz; }
            set { sendFrequencyInHertz = value; }
        }

        public bool Reliable
        {
            get { return reliable; }
            set { reliable = value; }
        }

        public bool Ordered
        {
            get { return inOrder; }
            set { inOrder = value; }
        }

        public UpdateApache CallbackFunc
        {
            get { return callbackFunc; }
            set { callbackFunc = value; }
        }

        
        #endregion

        #region Public Methods
        public byte[] GetMessage()
        {
            
            // 1 byte: pressedButton
            // 12 bytes: near point (3 floats)
            // 12 bytes: far point (3 floats)
            
            byte[] data = new byte[34];

            /*
            data[0] = (byte)1;
            ByteHelper.FillByteArray(ref data, 1, BitConverter.GetBytes(100));
            ByteHelper.FillByteArray(ref data, 5, BitConverter.GetBytes(200));
            ByteHelper.FillByteArray(ref data, 9, BitConverter.GetBytes(100));
            ByteHelper.FillByteArray(ref data, 13, BitConverter.GetBytes(200));
            ByteHelper.FillByteArray(ref data, 17, BitConverter.GetBytes(200));
            ByteHelper.FillByteArray(ref data, 21, BitConverter.GetBytes(200));
             */

            return data;
        }

        public void InterpretMessage(byte[] msg)
        {
            float x, y, z, pitch, yaw, roll;

            x = y = z = pitch = yaw = roll =0;

            //GoblinXNA.UI.Notifier.AddMessage("Got a message!!");
            //Log.Write(ByteHelper.ConvertToString(msg), Log.LogLevel.Error);

            String msg_str = ByteHelper.ConvertToString(msg);
            char[] seps = { ':' };
            String[] splits = null;
            splits = msg_str.Split(seps);

            String cmd = splits[1];
            String payload = splits[2];

            if(cmd.Equals("VBS2")) {
                x = float.Parse(splits[3]);
                y = float.Parse(splits[4]);
                z = float.Parse(splits[5]);
                pitch = float.Parse(splits[6]);
                yaw = float.Parse(splits[7]);
                roll = float.Parse(splits[8]);            
            }

            //GoblinXNA.UI.Notifier.AddMessage("s=" + s);

            if (callbackFunc != null) callbackFunc(cmd, payload, x, y, z, pitch, yaw, roll);
            
            /*
            pressedButton = (int)msg[0];

            nearPoint.X = ByteHelper.ConvertToFloat(msg, 1);
            nearPoint.Y = ByteHelper.ConvertToFloat(msg, 5);
            nearPoint.Z = ByteHelper.ConvertToFloat(msg, 9);

            farPoint.X = ByteHelper.ConvertToFloat(msg, 13);
            farPoint.Y = ByteHelper.ConvertToFloat(msg, 17);
            farPoint.Z = ByteHelper.ConvertToFloat(msg, 21);
            */
            //if (callbackFunc != null) callbackFunc(0,1,2,3,4,5);
        }

        #endregion
    }
}
