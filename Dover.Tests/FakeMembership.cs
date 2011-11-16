using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Com.Dover.Tests {
    public interface IStaticMembershipService {
        MembershipUser GetUser();

        void UpdateUser(MembershipUser user);
    }

    public class StaticMembershipService : IStaticMembershipService {
        public System.Web.Security.MembershipUser GetUser() {
            return Membership.GetUser();
        }

        public void UpdateUser(MembershipUser user) {
            Membership.UpdateUser(user);
        }
    }

}
