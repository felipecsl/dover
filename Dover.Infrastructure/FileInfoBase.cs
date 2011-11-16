using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Dover.Infrastructure {
	public class FileInfoBase {

		/// <summary>
		/// Gets the full relative path (relative to the application path eg.: /uac/Uploads/quavio/file.ext)
		/// </summary>
		/// <value>The full relative path.</value>
		public string FullRelativePath { get; set; }

		/// <summary>
		/// Gets the filename with extension and without the path
		/// </summary>
		/// <value>The filename.</value>
		public string Filename { get; set; }

		public DateTime CreationDate { get; set; }
	}
}
