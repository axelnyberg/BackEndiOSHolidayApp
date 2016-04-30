using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud2
{
    public class Memory
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string place { get; set; }
        public int time { get; set; }
        public Position position { get; set; }
        public Vacation vacation { get; set; }
        public List<Media> mediaList {get; set;}
    }
}