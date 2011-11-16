using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Com.Dover.Web.Models.DataTypes
{
	/// <summary>
	/// Marker interface for implementing a dynamic module field data type
	/// </summary>
	public interface IFieldDataType : IXmlSerializable
	{
	}
}
