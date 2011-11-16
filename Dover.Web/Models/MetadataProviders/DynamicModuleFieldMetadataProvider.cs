using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Com.Dover.Web.Models.MetadataProviders {
    public class DynamicModuleFieldMetadataProvider : AssociatedMetadataProvider {

        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Func<object> modelAccessor,
            Type modelType,
            string propertyName) {

            ModelMetadata metadata;

            if(modelType == typeof(DynamicModuleField)) {
                var field = modelAccessor() as DynamicModuleField;
                metadata = new ModelMetadata(this, containerType, () => field.Data, field.DataType, field.PropertyName) {
                    DisplayName = field.DisplayName,
                    IsReadOnly = field.IsReadOnly,
                    IsRequired = field.IsRequired
                };
            }
            else {
                metadata = new ModelMetadata(this, containerType, modelAccessor, modelType, propertyName);
            }

            return metadata;
        }
    }
}