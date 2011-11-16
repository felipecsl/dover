<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DateTime>" %>
<%: Model.ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR")) %>