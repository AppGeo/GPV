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
    string script = String.Format("document.getElementById(\"{0}_UserName\").focus()", Login1.ClientID);
    ClientScript.RegisterStartupScript(typeof(Login), "focus", script, true);
  }

  protected void Page_Load(object sender, EventArgs e)
  {
  }

  protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
  {
    e.Authenticated = AppAuthentication.FormsAuthenticate(Login1.UserName, Login1.Password);
  }
}
