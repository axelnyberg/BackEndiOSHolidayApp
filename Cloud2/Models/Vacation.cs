using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cloud2
{
    public class Vacation
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string place { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public List<Memory> memoryList { get; set; }
        public User user { get; set; }




    }
}