using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cloud2
{
    public class User
    {
        public int id {get; set;}
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public List<User> friendList { get; set; }
        public List<Vacation> vacationList { get; set; }

        public User()
        {
            friendList = new List<User>();
            vacationList = new List<Vacation>();
        }
    }
}