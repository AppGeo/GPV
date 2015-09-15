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

//#define SINGLE_THREADED

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;

public class WebServiceHandler : IHttpHandler
{
#if SINGLE_THREADED
  private static object _lock = new object();
#endif

  protected HttpContext Context = null;
  protected HttpRequest Request = null;
  protected HttpResponse Response = null;

  private Configuration _config = null;

  protected Configuration Configuration
  {
    get
    {
      if (_config == null)
      {
        _config = AppContext.GetConfiguration();
      }

      return _config;
    }
  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }

  public void ProcessRequest(HttpContext context)
  {
#if SINGLE_THREADED
    lock (_lock)
    {
#endif

    Context = context;
    Request = context.Request;
    Response = context.Response;

    Response.ContentType = "text/plain";
    Response.Cache.SetCacheability(HttpCacheability.NoCache);

    string fullMethodName;

    try
    {
      string method = Request.Form["m"];

      if (method == null)
      {
        method = Request.QueryString["m"];
      }

      if (method == null)
      {
        method = "DefaultMethod";
      }

      // some service ASHX files in the GPV point to parent handler classes, so find the requested method 
      // on the parent class of the current ASHX class via reflection if not found on the current class

      Type handlerType = this.GetType();
 
      BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;

      MemberInfo[] memberInfo = handlerType.GetMember(method, MemberTypes.Method, bindingFlags);

      if (memberInfo.Length == 0)
      {
        handlerType = handlerType.BaseType;
        memberInfo = handlerType.GetMember(method, MemberTypes.Method, bindingFlags);
      }

      if (memberInfo.Length > 0 && memberInfo[0].GetCustomAttributes(false).FirstOrDefault(o => o is WebServiceMethodAttribute) != null)
      {
        fullMethodName = String.Format("{0}.{1}()", handlerType.Name, memberInfo[0].Name);
        handlerType.InvokeMember(method, bindingFlags, null, this, null);
      }
      else
      {
        throw new Exception(String.Format("Invalid method specified: {0}", method));
      }
    }
    catch (Exception ex)
    {
      if (!(ex is System.Threading.ThreadAbortException))
      {
        if (Debugger.IsAttached)
        {
          throw ex.InnerException != null ? ex.InnerException : ex;
        }

        else
        {
          Response.StatusCode = 500;

          while (ex.InnerException != null)
          {
            ex = ex.InnerException;
          }

          if (ex is AppException)
          {
            Response.AddHeader("GPV-Error",  ex.Message);
          }
        }
      }
    }
#if SINGLE_THREADED
    }
#endif
  }

  protected void ReturnJson(string key, string value)
  {
    ReturnJson<String>(key, value);
  }

  protected void ReturnJson<T>(string key, T value)
  {
    Dictionary<String, T> result = new Dictionary<String, T>();
    result.Add(key, value);
    ReturnJson(result);
  }

  protected void ReturnJson(object obj)
  {
    try
    {
      JavaScriptSerializer serializer = new JavaScriptSerializer();
      Response.ContentType = "application/json";
      Response.Write(serializer.Serialize(obj));
      Response.End();
    }
    catch { }
  }

  protected void ReturnNotFound()
  {
    Response.StatusCode = 404;
    Response.End();
  }
}

[AttributeUsage(AttributeTargets.Method)]
public class WebServiceMethodAttribute : Attribute { }