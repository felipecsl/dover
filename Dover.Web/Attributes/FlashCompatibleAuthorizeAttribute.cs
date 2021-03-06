﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;

namespace Com.Dover.Attributes {
	/// <summary>
	/// Took from http://trycatchfail.com/blog/post/2009/05/13/Using-Flash-with-ASPNET-MVC-and-Authentication.aspx
	/// A custom version of the <see cref="AuthorizeAttribute"/> that supports working
	/// around a cookie/session bug in Flash.  
	/// </summary>
	/// <remarks>
	/// Details of the bug and workaround can be found on this blog:
	/// http://geekswithblogs.net/apopovsky/archive/2009/05/06/working-around-flash-cookie-bug-in-asp.net-mvc.aspx
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class FlashCompatibleAuthorizeAttribute : AuthorizeAttribute {
		/// <summary>
		/// The key to the authentication token that should be submitted somewhere in the request.
		/// </summary>
		private const string TOKEN_KEY = "token";

		/// <summary>
		/// This changes the behavior of AuthorizeCore so that it will only authorize
		/// users if a valid token is submitted with the request.
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		protected override bool AuthorizeCore(HttpContextBase httpContext) {
			string token = httpContext.Request.Params[TOKEN_KEY];

			if (token != null) {
				var ticket = FormsAuthentication.Decrypt(token);

				if (ticket != null) {
					var identity = new FormsIdentity(ticket);
					string[] roles = System.Web.Security.Roles.GetRolesForUser(identity.Name);
					var principal = new GenericPrincipal(identity, roles);
					httpContext.User = principal;
				}
			}

			return base.AuthorizeCore(httpContext);
		}
	}
}