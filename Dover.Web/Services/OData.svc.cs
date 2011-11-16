using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using Com.Dover.Models;
using Com.Dover.ModuleManagement;
using System.ServiceModel;

namespace Com.Dover.Services
{
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class OData : DataService<DoverEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
#if DEBUG
            config.UseVerboseErrors = true;
#endif
		}
    }
}
