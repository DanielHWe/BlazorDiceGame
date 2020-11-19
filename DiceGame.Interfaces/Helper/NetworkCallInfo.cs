using System;
using System.Collections.Generic;
using System.Text;

namespace DiceGame.Interfaces.Helper
{
    public class NetworkCallInfo
    {
        public bool CallBySignalR { get; set; }
        public String Url { get; set; }

        public NetworkCallInfo(String url, bool callBySignalR)
        {
            CallBySignalR = callBySignalR;
            Url = url;
        }
    }
}
