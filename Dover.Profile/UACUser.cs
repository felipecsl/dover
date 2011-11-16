using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web.Security;
using System.IO;
using System.Web;
using Br.Com.Quavio.Tools.Web;
using Com.Dover.Infrastructure;
using System.Drawing;

namespace Com.Dover.Profile {
	[Serializable]
	public class UACUser : MembershipUser {
		public const string UserCkFinderImagesPath = "/ckfinder/userfiles/images/{0}/";
		public const string UserHomePath = "~/Uploads/{0}/";
		public const string UserImagesPath = "~/Uploads/{0}/images/";
		public const string UserFilesPath = "~/Uploads/{0}/files/";
		
		public UACUser() {
		}

		public UACUser(
			string providerName,
			string name,
			object providerUserKey,
			string email,
			string passwordQuestion,
			string comment,
			bool isApproved,
			bool isLockedOut,
			DateTime creationDate,
			DateTime lastLoginDate,
			DateTime lastActivityDate,
			DateTime lastPasswordChangedDate,
			DateTime lastLockoutDate)
			: base(providerName, name, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate) {
		}

		public override string UserName {
			get {
				// The actual user name is stored in the user comment section for OpenId users
				return !String.IsNullOrWhiteSpace(this.Comment)
					? this.Comment
					: base.UserName;
			}
		}

		public string ActualUserName {
			get { return base.UserName; }
		}

		/// <summary>
		/// Returns all images in the specified user's images directory
		/// </summary>
		/// <returns></returns>
		public List<ImageFileInfo> GetImages() {
			if (HttpContext.Current == null) {
				throw new InvalidOperationException("Must be withing a http request");
			}

			var server = HttpContext.Current.Server;
			string userName = this.UserName;
			string relativePath = String.Format(UserImagesPath, userName);
			string galleryPath = server.MapPath(relativePath);

			var lstFiles = new List<ImageFileInfo>();

			if (!Directory.Exists(galleryPath)) {
				Directory.CreateDirectory(galleryPath);
			}

			// enumerate all files in the user's directory in the server
			foreach (var file in Directory.GetFiles(galleryPath)) {
				string absolutePath = Path.Combine(galleryPath, file);
				var imgInfo = new ImageFileInfo();
				imgInfo.Filename = Path.GetFileName(file);
				imgInfo.FullRelativePath = relativePath + imgInfo.Filename;
				imgInfo.CreationDate = new FileInfo(file).CreationTime;

				using (Image img = Image.FromFile(absolutePath)) {
					imgInfo.Width = img.Width;
					imgInfo.Height = img.Height;
				}

				lstFiles.Add(imgInfo);
			}

			return lstFiles;
		}

		public double GetDiskSpaceUsage() {
			if (HttpContext.Current == null) {
				throw new InvalidOperationException("Must be withing a http request");
			}

			var server = HttpContext.Current.Server;
			string userName = this.UserName;
			string relativePath = String.Format(UserHomePath, userName);
			string ckfinderRelativePath = String.Format(UserCkFinderImagesPath, userName);
			string homePath = server.MapPath(relativePath);
			string ckfinderImgsPath = server.MapPath(ckfinderRelativePath);

			if (!Directory.Exists(homePath)) {
				return 0;
			}

			double total = GetDirectorySize(homePath);

			// if the user's ckfinder images upload path does not exist, create it
			// since the editor won't create it for us
			if (!Directory.Exists(ckfinderImgsPath)) {
				Directory.CreateDirectory(ckfinderImgsPath);
			}

			// sum up with user's ckfinder uploaded images path
			return total + GetDirectorySize(ckfinderImgsPath);
		}

		public static double GetDirectorySize(string _path) {
			string[] files = Directory.GetFiles(_path);
			double total = 0;

			foreach (string file in files) {
				total += new FileInfo(file).Length;
			}

			//now get all the sub-directories in this directory
			string[] subDirs = Directory.GetDirectories(_path);
			
			foreach (string dir in subDirs) {
				total += GetDirectorySize(dir);
			}
			//return the total value in bytes
			return total;
		}

		/// <summary>
		/// Saves the provided file to the user's uploads folder
		/// </summary>
		/// <param name="_fileToSave">The file to save.</param>
		/// <returns>The file info for the newly created file</returns>
		public FileInfoBase SaveFile(HttpPostedFileBase _fileToSave) {
			string fullPath = Save(_fileToSave, UserFilesPath);
			string fileName = Path.GetFileName(fullPath);

			return new FileInfoBase {
				Filename = fileName,
				FullRelativePath = String.Format(UserFilesPath, this.UserName) + fileName
			};
		}

		/// <summary>
		/// Saves the provided image to the user's uploads folder
		/// </summary>
		/// <param name="_fileToSave">The _file to save.</param>
		/// <returns></returns>
		public ImageFileInfo SaveImage(HttpPostedFileBase _fileToSave) {
			string fullPath = Save(_fileToSave, UserImagesPath);

			var imgInfo = new ImageFileInfo();
			imgInfo.Filename = Path.GetFileName(fullPath);
			imgInfo.FullRelativePath = String.Format(UserImagesPath, this.UserName) + imgInfo.Filename;

			using (Image img = Image.FromFile(fullPath)) {
				imgInfo.Width = img.Width;
				imgInfo.Height = img.Height;
			}

			return imgInfo;
		}

		private string Save(
			HttpPostedFileBase _fileToSave,
			string _path) {

			if (HttpContext.Current == null) {
				throw new InvalidOperationException("Must be withing a http request");
			}

			if (_fileToSave == null) {
				throw new ArgumentNullException("_fileToSave");
			}
			
			var server = HttpContext.Current.Server;
			string uploadpath = String.Format(_path, this.UserName);

			return WebTools.SaveFile(
				_fileToSave.InputStream,
				server.MapPath(uploadpath),
				Path.GetFileName(_fileToSave.FileName));
		}
	}
}
