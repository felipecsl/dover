using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Data.EntityClient;
using System.Xml.Linq;
using Com.Dover.Modules;
using Com.Dover.Web.Models;
using Com.Dover.Web.Models.Converters;
using Com.Dover.Web.Models.DataTypes;
using Br.Com.Quavio.Tools.Web;
using Br.Com.Quavio.Tools.Web.Encryption;

namespace Com.Dover.Tests {
	[TestClass]
	public class SerializationTests {

		public static string ConnectionString {
			get {
				return new EntityConnectionStringBuilder {
					Metadata = "res://Com.Dover.Modules/UAC.csdl|res://Com.Dover.Modules/UAC.ssdl|res://Com.Dover.Modules/UAC.msl",
					//ProviderConnectionString = "Data Source=sql2k803.discountasp.net;Initial Catalog=SQL2008_728981_dover;Persist Security Info=True;User ID=SQL2008_728981_dover_user;Password=uac@dmin;MultipleActiveResultSets=True",
					ProviderConnectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=uac;Persist Security Info=True;User ID=uac;Password=uac@dmin;MultipleActiveResultSets=True",
					Provider = "System.Data.SqlClient"
				}.ToString();
			}
		}
		
		[TestMethod]
		public void Test_Deserialize_String() {
			// Act
			var value = "Some string to be deserialized";
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(string));

			string result = retData as string;

			Assert.AreEqual(value, result);
		}

		[TestMethod]
		public void Test_Deserialize_Int() {
			// Arrange 
			int value = 12;
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(int));

			int result = (int)retData;

			Assert.AreEqual(value, result);
		}

		[TestMethod]
		public void Test_Deserialize_DateTime() {
			// Arrange 
			DateTime value = DateTime.Now;
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(DateTime));

			DateTime result = (DateTime)retData;

			// for some reason, the ticks are not the same. 
			// need to compare fields alone.
			Assert.AreEqual(value.Day, result.Day);
			Assert.AreEqual(value.Month, result.Month);
			Assert.AreEqual(value.Year, result.Year);
			
			Assert.AreEqual(value.Hour, result.Hour);
			Assert.AreEqual(value.Minute, result.Minute);
			Assert.AreEqual(value.Second, result.Second);
		}

		[TestMethod]
		public void Test_Deserialize_Boolean() {
			// Arrange 
			bool value = false;
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(bool));

			bool result = (bool)retData;

			Assert.AreEqual(value, result);
		}

		[TestMethod]
		public void Test_Deserialize_HtmlText() {
			// Arrange 
			HtmlText value = new HtmlText { Text = "<p>Some paragraph</p>" };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(HtmlText));

			HtmlText result = (HtmlText)retData;

			Assert.AreEqual(value.Text, result.Text);
		}

		[TestMethod]
		public void Test_Deserialize_DbImage() {
			// Arrange 
			var value = new DbImage { ImagePath = "www.google.com" };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(DbImage));

			DbImage result = (DbImage)retData;

			Assert.AreEqual(value.ImagePath, result.ImagePath);			
		}

		[TestMethod]
		public void Test_Password_Should_Set_Value() {
			var samplePassord = "some secret data";
			var data = new Password { Value = samplePassord };
			var converter = FieldValueConversion.GetConverter(data.GetType());
			var field = new DynamicModuleField { Data = data };
			var rf = new Cell() { Data = converter.Serialize(field, new ConversionContext { Cell = new Cell() }) };
			var retData = converter.Deserialize(rf);

			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(Password));

			var pwdResult = retData as Password;

			Assert.AreNotEqual(pwdResult.Value, samplePassord);

			var hash = new Hash(Hash.Provider.SHA1);
			var hashData = hash.Calculate(new Data(samplePassord), new Data("|)0ver3ncrypt10n_k3y")).Base64;

			Assert.AreEqual(pwdResult.Value, hashData);
		}

		[TestMethod]
		public void Test_Password_Should_Keep_Current_Value() {
			var samplePassord = "some secret data";
			var data = new Password { Value = Password.BogusText };
			var converter = FieldValueConversion.GetConverter(data.GetType());
			var field = new DynamicModuleField { Data = data };
			var rf = new Cell() { Data = converter.Serialize(field, new ConversionContext { Cell = new Cell { Data = "<Password>" + samplePassord + "</Password>" } }) };
			var retData = converter.Deserialize(rf);

			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(Password));

			var pwdResult = retData as Password;

			Assert.AreEqual(pwdResult.Value, samplePassord);
		}

		[TestMethod]
		public void Test_Password_Should_Change_Value() {
			var samplePassord = "some secret data";
			var data = new Password { Value = samplePassord };
			var converter = FieldValueConversion.GetConverter(data.GetType());
			var field = new DynamicModuleField { Data = data };
			var rf = new Cell() { Data = converter.Serialize(field, new ConversionContext { Cell = new Cell { Data = "<Password>9b6ca654c29750544f6c4fba45a7c0b7f5fdf0f2</Password>" } }) };
			var retData = converter.Deserialize(rf);

			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(Password));

			var pwdResult = retData as Password;

			Assert.AreNotEqual(pwdResult.Value, samplePassord);

			var hash = new Hash(Hash.Provider.SHA1);
			var hashData = hash.Calculate(new Data(samplePassord), new Data("|)0ver3ncrypt10n_k3y")).Base64;

			Assert.AreEqual(pwdResult.Value, hashData);
		}

		[TestMethod]
		public void Test_Deserialize_DbImage_With_Null() {
			// Arrange 
			var value = new DbImage { ImagePath = null };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(DbImage));

			DbImage result = (DbImage)retData;

			Assert.AreEqual(value.ImagePath, result.ImagePath);
		}

		[TestMethod]
		public void Test_Deserialize_ImageList() {
			// Arrange 
			var value = new ImageList { 
				new DbImage { ImagePath = "www.google.com" },
				new DbImage { ImagePath = "www.yahoo.com" },
				new DbImage { ImagePath = "www.terra.com.br" }
			};
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(ImageList));

			ImageList result = (ImageList)retData;

			CollectionAssert.AllItemsAreNotNull(result);
			CollectionAssert.AllItemsAreInstancesOfType(result, typeof(DbImage));
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(value[0].ImagePath, result[0].ImagePath);
			Assert.AreEqual(value[1].ImagePath, result[1].ImagePath);
			Assert.AreEqual(value[2].ImagePath, result[2].ImagePath);
		}

		[TestMethod]
		public void Test_Deserialize_ImageList_With_Null() {
			// Arrange 
			var value = new ImageList { 
				new DbImage { ImagePath = "www.google.com" },
				new DbImage { ImagePath = null },
				new DbImage { ImagePath = "www.terra.com.br" }
			};
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(ImageList));

			ImageList result = (ImageList)retData;

			CollectionAssert.AllItemsAreNotNull(result);
			CollectionAssert.AllItemsAreInstancesOfType(result, typeof(DbImage));
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(value[0].ImagePath, result[0].ImagePath);
			Assert.AreEqual(value[1].ImagePath, result[1].ImagePath);
			Assert.AreEqual(value[2].ImagePath, result[2].ImagePath);
		}

		[TestMethod]
		public void Test_Deserialize_File() {
			// Arrange 
			var value = new File { FilePath = "c:\\somefile.txt" };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(File));

			File result = (File)retData;

			Assert.AreEqual(value.FilePath, result.FilePath);
		}

		[TestMethod]
		public void Test_Deserialize_File_Null() {
			// Arrange 
			var value = new File { FilePath = null };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(File));

			File result = (File)retData;

			Assert.AreEqual(value.FilePath, result.FilePath);
		}

		[TestMethod]
		public void Test_Deserialize_DataList() {
			// Arrange 
			var value = new DataList { Items = new List<string>() { "item 1", "item 2", "some element 3" } };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(DataList));

			DataList result = (DataList)retData;

			CollectionAssert.AllItemsAreNotNull(result.Items);
			CollectionAssert.AllItemsAreInstancesOfType(result.Items, typeof(string));
			Assert.AreEqual(value.Items[0], result.Items[0]);
			Assert.AreEqual(value.Items[1], result.Items[1]);
			Assert.AreEqual(value.Items[2], result.Items[2]);
		}

		[TestMethod]
		public void Test_Deserialize_DataList_Empty() {
			// Arrange 
			var value = new DataList();
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(DataList));

			DataList result = (DataList)retData;

			Assert.AreEqual(0, result.Items.Count);
		}

		[TestMethod]
		public void Test_Deserialize_DataList_Comma_Have_To_Fix() {
			// Arrange 
			var value = new DataList { Items = new List<string>() { "item, 1", "", "some ele#;ment 3" } };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(DataList));

			DataList result = (DataList)retData;

			CollectionAssert.AllItemsAreNotNull(result.Items);
			CollectionAssert.AllItemsAreInstancesOfType(result.Items, typeof(string));
			Assert.AreEqual(value.Items[0], result.Items[0]);
			Assert.AreEqual(value.Items[1], result.Items[1]);
			Assert.AreEqual(value.Items[2], result.Items[2]);
		}

		[TestMethod]
		public void Test_Deserialize_DropdownButton() {
			// Arrange 
			var value = new DropdownButton { SelectedValue = "some selected value" };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(DropdownButton));

			DropdownButton result = (DropdownButton)retData;

			Assert.AreEqual(value.SelectedValue, result.SelectedValue);
		}

		[TestMethod]
		public void Test_Deserialize_DropdownButton_Null() {
			// Arrange 
			var value = new DropdownButton { SelectedValue = null };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(DropdownButton));

			DropdownButton result = (DropdownButton)retData;

			Assert.IsNull(result.SelectedValue);
		}

		[TestMethod]
		public void Test_Deserialize_Money() {
			// Arrange 
			var value = new Money { Value = 1.6f };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(Money));

			Money result = (Money)retData;

			Assert.AreEqual(value.Value, result.Value);
		}

		[TestMethod]
		public void Test_Deserialize_Money_Null() {
			// Arrange 
			var value = new Money { Value = null };
			object retData = SerializeAndDeserialize(value);

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(Money));

			Money result = (Money)retData;

			Assert.AreEqual(null, result.Value);
		}

		[TestMethod]
		public void Test_Deserialize_ModuleReference() {
			// Arrange 
			var fakeRepo = new FakeModuleRepository();
			var rowField = fakeRepo.GetRandomCell();
			
			rowField.Field.Metadata.Add(new FieldMetadata { 
				Key = ModuleRepository.ModuleReferenceMetadataKey, 
				Value = "\"" + rowField.Field.Module.Id + "\"" });

			var value = new ModuleReference() { Id = rowField.ID };
			var converter = FieldValueConversion.GetConverter(value.GetType());
			var field = new DynamicModuleField { Data = value };
			var rf = new Cell() { Data = converter.Serialize(field) };

			var retData = converter.Deserialize(rf, new ConversionContext {
				Repository = fakeRepo,
				Field = rowField.Field,
				Module = rowField.Field.Module
			});

			// Assert
			Assert.IsNotNull(retData);
			Assert.IsInstanceOfType(retData, typeof(ModuleReference));

			ModuleReference result = (ModuleReference)retData;

			Assert.AreEqual(rowField.Row.ID, result.Id);
		}

		/*[TestMethod]
		[Ignore]
		public void Test_Convert_Data_To_Xml() {
			using (var db = new DoverEntities(ConnectionString)) {
				foreach (var rf in db.Cell
					.Include("Field.FieldDataType")
					.Include("Field.Metadata")
					.Include("Field.Module")) {
					Type dataType = Type.GetType(rf.Field.FieldDataType.Name);
					if (dataType == null) {
						dataType = Type.GetType(rf.Field.FieldDataType.Name + ", Com.Dover., Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
					}
					var converter = FieldValueConversion.GetConverter(dataType);
					var cc = new ConversionContext {
						Repository = new ModuleRepository(new EntityConnection(ConnectionString)),
						Field = rf.Field,
						Module = rf.Field.Module
					};
					DefaultFieldValueConverter.SerializationMethod = FieldSerializationMethod.Binary;
					object obj = converter.Deserialize(rf, cc);
					DefaultFieldValueConverter.SerializationMethod = FieldSerializationMethod.Xml;
					rf.Data = converter.Serialize(new DynamicModuleField() {
						Data = obj,
						DataType = dataType
					}, cc);
				}
				db.SaveChanges();
			}
		}

		[TestMethod]
		[Ignore]
		public void Test_Migrate_ModuleReference() {
			var dataType = Type.GetType("Com.Dover.Models.DataTypes.ModuleReference, Com.Dover., Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
			var converter = FieldValueConversion.GetConverter(dataType);

			using (var db = new DoverEntities(ConnectionString)) {
				foreach (var rf in db.Cell
					.Include("Field.FieldDataType")
					.Include("Field.Metadata")
					.Include("Field.Module")) {

					if (rf.Field.FieldDataType.Name == "Com.Dover.Models.DataTypes.ModuleReference") {
						var cc = new ConversionContext {
							Repository = new ModuleRepository(new EntityConnection(ConnectionString)),
							Field = rf.Field,
							Module = rf.Field.Module
						};
						if (rf.Data != null) {
							var modRef = converter.Deserialize(rf, cc) as ModuleReference;

							Assert.IsNotNull(modRef);

							var row = db.Row.FirstOrDefault(r => r.Cells.Any(rf1 => rf1.ID == modRef.Id));

							Assert.IsNotNull(row);

							modRef.Id = row.ID;

							rf.Data = converter.Serialize(new DynamicModuleField { Data = modRef }, cc);
						}
					}
				}
				db.SaveChanges();
			}
		}

		[TestMethod]
		[Ignore]
		public void Migrate_Binary_To_String() {

			using (var db = new DoverEntities(ConnectionString)) {
				foreach (var record in db.GetRowFieldBytes()) {
					var bytes = record.Data;
					if (bytes == null || bytes.Length == 0) {
						continue;
					}

					var id = record.ID;
					string data = null;
					if ((int)bytes[bytes.Length - 1] == 0) {
						var newArray = bytes.TakeWhile((b, i) => i < bytes.Length - 1);
						data = Encoding.UTF8.GetString(newArray.ToArray());
					}
					else {
						data = Encoding.UTF8.GetString(bytes);
					}

					var rowField = db.Cell.FirstOrDefault(rf => rf.ID == id);

					Assert.IsNotNull(rowField);

					rowField.Data = data;
				}
				db.SaveChanges();
			}
		}*/

		private static object SerializeAndDeserialize(object data) {
			var converter = FieldValueConversion.GetConverter(data.GetType());
			var field = new DynamicModuleField { Data = data };
			var rf = new Cell() { Data = converter.Serialize(field) };
			
			return converter.Deserialize(rf);
		}
	}
}
