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
using System.Collections.Specialized;

public static class HttpRequestExtensions
{
  public static Dictionary<String, String> GetNormalizedParameters(this HttpRequest request)
  {
    NameValueCollection parameters = request.HttpMethod == "GET" ? request.QueryString : request.Form;
    Dictionary<String, String> dictionary = new Dictionary<String, String>();

    if (parameters.ToString().Length > 0 && parameters.AllKeys.Count() == 1 && String.IsNullOrEmpty(parameters.AllKeys[0]))
    {
      dictionary.Add(parameters.ToString().ToLower(), null);
    }
    else
    {
      foreach (string key in parameters)
      {
        if (!String.IsNullOrEmpty(key))
        {
          dictionary.Add(key.ToLower(), parameters[key]);
        }
      }
    }

    return dictionary;
  }
}