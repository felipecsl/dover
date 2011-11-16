using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Com.Dover.Profile
{
    public class UserProfile
    {
        private ProfilePropertyList m_lstProps = new ProfilePropertyList();
        
        public UserProfile(string _sUserName)
        {
            this.UserName = _sUserName;
        }

        public UserProfile()
        {
            this.UserName = Membership.GetUser().UserName;
        }

        public ProfilePropertyList Properties
        {
            get { return this.m_lstProps; }
        }

        public string UserName { get; private set; }
    }
}
