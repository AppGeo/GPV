<%-- 
  Copyright 2012 Applied Geographics, Inc.

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
--%>

<%@ Application Language="C#" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Security.Principal" %>

<script runat="server">

  protected void Application_BeginRequest(Object sender, EventArgs e)
  {
    Response.AddHeader("X-UA-Compatible", "IE=edge");
  }

  protected void Application_PostMapRequestHandler(Object sender, EventArgs e)
  {
    if (!AppSettings.AppIsAvailable && HttpContext.Current.CurrentHandler is Page && Request.Path.IndexOf("AppStatus.aspx") == -1)
    {
      Server.Transfer("Status.aspx");
    }
  }

  protected void Application_AuthenticateRequest(object sender, EventArgs e)
  {
    if (AppAuthentication.Mode == AuthenticationMode.Certificate && !Request.IsAuthenticated && Request.ClientCertificate.IsPresent)
    {
      string userName = Request.ClientCertificate.Subject;
      string field = ConfigurationManager.AppSettings["CertificateUserField"];

      if (!String.IsNullOrEmpty(field))
      {
        userName = userName.Replace(", ", ",").Split(',').FirstOrDefault(o => o.StartsWith(field + "="));

        if (userName != null)
        {
          userName = userName.Substring(field.Length + 1);
        }
      }

      if (userName != null)
      {
        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(userName, false, 30);
        HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(authTicket), new string[] { });

        string encTicket = FormsAuthentication.Encrypt(authTicket);
        Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
      }
    }
  }
       
</script>
