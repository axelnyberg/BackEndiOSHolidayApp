using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cloud2
{
    public class AccessTokenRespone{
       public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}