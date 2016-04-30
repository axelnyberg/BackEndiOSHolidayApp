using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cloud2
{
    public class SoundMedia : Media
    {
        public float duration { get; set; }
        public string codec { get; set; }
        public float bitrate { get; set; }
        public int channels { get; set; }
        public float samplingrate { get; set; }

    }
}