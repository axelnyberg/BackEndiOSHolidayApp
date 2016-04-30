using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cloud2
{
    public class AccessTokenRequest
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public int password { get; set; }
    }
}