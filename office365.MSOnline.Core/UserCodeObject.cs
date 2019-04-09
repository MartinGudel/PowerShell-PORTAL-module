using System;
using Newtonsoft.Json;

namespace office365.PORTAL.Core { 

    public class UserCodeObject
    {
        public string user_code { get; set; }
        public string device_code { get; set; }
        public string verification_url { get; set; }
        public string expires_in { get; set; }
        public string interval { get; set; }
        public string message { get; set; }
    }
}