using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cloud2
{
    public class VideoMedia : Media
    {
        public int width { get; set; }
        public int height { get; set; }
        public string videocodec { get; set; }
        public float videobitrate { get; set; }
        public float framerate { get; set; }
        public string audiocodec { get; set; }
        public float audiobitrate { get; set; }
        public int channels { get; set; }
        public float samplingrate { get; set; }
    }
}