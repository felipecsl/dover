using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Schema;
using System.Xml;

namespace Com.Dover.Web.Models.DataTypes
{
	[Serializable]
	public class VideoEntry : IFieldDataType
    {
        public string ID { get; set; }

        [DisplayName("Video")]
        [Required(ErrorMessage = "Por favor, selecione um vídeo")]
        public string VideoUrl { get; set; }

        [DisplayName("Legenda")]
        public string Label { get; set; }

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteValue(VideoUrl);
		}

		#endregion
    }
}
