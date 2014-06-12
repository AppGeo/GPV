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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPV
{
  // a WebControl substitute for <table runat="server"> that ignores the NamingContainer

  [ParseChildren(false), PersistChildren(true)]
  public class Table : WebControl
  {
    public override string ClientID
    {
      get
      {
        return ID;
      }
    }

    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        return HtmlTextWriterTag.Table;
      }
    }
  }
}