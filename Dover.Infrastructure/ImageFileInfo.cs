using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Com.Dover.Infrastructure {
	public class ImageFileInfo : FileInfoBase {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}