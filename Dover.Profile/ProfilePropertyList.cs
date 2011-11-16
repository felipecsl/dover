using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Dover.Profile
{
    public class ProfilePropertyList : List<ProfileProperty>
    {
        /// <summary>
        /// Returns the property in the list that matches the provided name.
        /// If the property is not found, throws an InvalidOperationException
        /// </summary>
        /// <param name="_sName">The property key to search for</param>
        /// <returns>The property found</returns>
        public ProfileProperty GetProperty(string _sName)
        {
            return this.FirstOrDefault(prop => prop.Key == _sName);
        }
    }
}
