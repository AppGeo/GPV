//  Copyright 2012 Applied Geographics, Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.UI.HtmlControls;

public partial class Login : CustomStyledPage
{
  protected void Page_Init(object sender, EventArgs e)
  {
    if (Request.RawUrl.ToLower().Contains("mobileviewer.aspx"))
    {
      AddStylesheet("Styles/Mobile/jquery.mobile-1.1.1.min.css");
      AddStylesheet("Styles/Mobile/Mobile.css");
      AddScriptReference("Scripts/jquery-1.7.2.min.js");
      AddScriptReference("Scripts/Mobile/jquery.mobile-1.1.1.min.js");
      AddScript("$('#pnlBody').bind('pagecreate', function () { $('#Form1').attr('data-ajax', 'false'); $('td').css('padding-right', '10pt'); });");
      Header1.Visible = false;
      h1.Visible = true;
    }
    else
    {
      AddStylesheet("Styles/Common.css");
      AddStylesheet("Styles/Customize.css");
      AddStylesheet("Styles/Login.css");
    }

    Login1.UserName = AppAuthentication.GetAdminUserName();
    string script = String.Format("document.getElementById(\"{0}_{1}\").focus()", Login1.ClientID, String.IsNullOrEmpty(Login1.UserName) ? "UserName" : "Password");
    ClientScript.RegisterStartupScript(typeof(Login), "focus", script, true);
  }

  protected void Page_Load(object sender, EventArgs e)
  {
  }

  protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
  {
    e.Authenticated = AppAuthentication.FormsAuthenticate(Login1.UserName, Login1.Password);
  }

  private void AddStylesheet(string stylesheet)
  {
    HtmlGenericControl link = new HtmlGenericControl("link");
    link.Attributes["href"] = stylesheet;
    link.Attributes["type"] = "text/css";
    link.Attributes["rel"] = "stylesheet";
    head.Controls.Add(link);
  }

  private void AddScript(string scriptText)
  {
    HtmlGenericControl script = new HtmlGenericControl("script");
    script.InnerHtml = scriptText;
    body.Controls.Add(script);
  }

  private void AddScriptReference(string url)
  {
    HtmlGenericControl script = new HtmlGenericControl("script");
    script.Attributes["src"] = url;
    body.Controls.Add(script);
  }
}
