using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace Com.Dover.Modules
{
    public interface IModule
    {
        int Id { get; set; }
        string ModuleName { get; set; }
        string DisplayName { get; set; }
		User User { get; set; }
        int ModuleType { get; set; }
        EntityCollection<Row> Rows { get; set; }
		EntityCollection<Field> Fields { get; set; }
		EntityCollection<UsageCounter> UsageCounters { get; set; }
		Account Account { get; set; }
    }
}
