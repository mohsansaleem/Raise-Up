using UnityEngine;
using System.Collections;

namespace WebSocket_Client
{
    public class Message 
    {

        public string Action { get; protected set; }

        public byte[] Data { get; protected set; }
    
        public Message(string action = null, string data = null) : this(action, GetBytes(data))
        {

        }
    
        public Message(string action = null, byte[] data = null)
        {
            this.Action = action;
            this.Data = data;
        }
    
        public override string ToString()
        {
            string output = "";
        
            if (Action != null)
                output += "Action: " + Action + "\n";
        
            if (Data != null)
                output += "Data: " + DataAsString + "\n";
        
            return output;
        }
    
        public string DataAsString
        {
            get
            {
                return GetString(Data);
            }
        }
    
        public static byte[] GetBytes(string str)
        {
            if (str != null)
                return System.Text.Encoding.Default.GetBytes(str);
            else
                return null;
        }
    
        public static string GetString(byte[] bytes)
        {
            if (bytes != null)
                return System.Text.Encoding.Default.GetString(bytes);
            else
                return null;
        }
    }
}