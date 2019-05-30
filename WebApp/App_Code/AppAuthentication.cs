//  Copyright 2016 Applied Geographics, Inc.
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
using System.Configuration;
using System.Data.OleDb;
using System.Web.Configuration;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;

public static class AppAuthentication
{
  private static bool _hasAuthenticationMode = false;
  private static AuthenticationMode _authenticationMode = AuthenticationMode.None;

  public static AuthenticationMode Mode
  {
    get
    {
      if (!_hasAuthenticationMode)
      {
        AuthenticationSection authenticationSection = GetAuthenticationSection();

        if (authenticationSection != null)
        {
          switch (authenticationSection.Mode)
          {
            case System.Web.Configuration.AuthenticationMode.Forms:
              string formsMode = ConfigurationManager.AppSettings["FormsAuthenticationMode"];

              if (!String.IsNullOrEmpty(formsMode))
              {
                switch (formsMode.ToLower())
                {
                  case "database":
                    _authenticationMode = AuthenticationMode.Database;
                    break;

                  case "certificate":
                    _authenticationMode = AuthenticationMode.Certificate;
                    break;
                }
              }
              break;

            case System.Web.Configuration.AuthenticationMode.Windows:
              _authenticationMode = AuthenticationMode.Windows;
              break;
          }
        }

        _hasAuthenticationMode = true;
      }

      return _authenticationMode;
    }
  }

  public static bool FormsAuthenticate(string userName, string password)
  {
    bool authenticated = false;

    switch (Mode)
    {
      case AuthenticationMode.None:
        AuthenticationSection authenticationSection = GetAuthenticationSection();

        if (authenticationSection.Mode == System.Web.Configuration.AuthenticationMode.Forms && authenticationSection.Forms != null && authenticationSection.Forms.Credentials != null)
        {
          FormsAuthenticationCredentials credentials = authenticationSection.Forms.Credentials;

          if (credentials.Users.Count > 0)
          {
            if (credentials.PasswordFormat == FormsAuthPasswordFormat.SHA1)
            {
              password = HashPasswordForWebConfig(password);
            }

            authenticated = String.Compare(userName, credentials.Users[0].Name, true) == 0 && String.Compare(password, credentials.Users[0].Password) == 0;
          }
        }
        break;

      case AuthenticationMode.Database:
        using (OleDbConnection connection = AppContext.GetDatabaseConnection())
        {
          string sql = String.Format("select count(*) from {0}User where UserName = ? and Password = ? and Active = 1", WebConfigSettings.ConfigurationTablePrefix);

          using (OleDbCommand command = new OleDbCommand(sql, connection))
          {
            command.Parameters.Add("@1", OleDbType.VarWChar).Value = userName;
            command.Parameters.Add("@2", OleDbType.VarWChar).Value = password;
            authenticated = Convert.ToInt32(command.ExecuteScalar()) > 0;

            if (!authenticated)
            {
              command.Parameters[1].Value = HashPassword(password);
              authenticated = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
          }
        }
        break;
    }

    return authenticated;
  }

  public static string GetAdminUserName()
  {
    string formsMode = ConfigurationManager.AppSettings["FormsAuthenticationMode"];
    AuthenticationSection authenticationSection = GetAuthenticationSection();

    if (String.IsNullOrEmpty(formsMode) && authenticationSection != null && authenticationSection.Mode == System.Web.Configuration.AuthenticationMode.Forms)
    {
      if (authenticationSection.Forms != null && authenticationSection.Forms.Credentials != null && authenticationSection.Forms.Credentials.Users.Count > 0)
      {
        return authenticationSection.Forms.Credentials.Users[0].Name;
      }
    }

    return "";
  }

  private static AuthenticationSection GetAuthenticationSection()
  {
    return WebConfigurationManager.OpenWebConfiguration("~/Web.config").GetSection("system.web/authentication") as AuthenticationSection;
  }

  public static string HashPassword(string password)
  {
    UnicodeEncoding encoding = new UnicodeEncoding();
    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

    byte[] salt = new byte[8] { 191, 175, 87, 88, 206, 205, 95, 123 };
    byte[] source = encoding.GetBytes(password);
    byte[] saltedSource = new byte[salt.Length + source.Length];

    Array.Copy(salt, saltedSource, salt.Length);
    Array.Copy(source, 0, saltedSource, salt.Length, source.Length);

    byte[] data = sha1.ComputeHash(saltedSource);
    string hash = "";

    foreach (byte b in data)
    {
      hash += b.ToString("X2");
    }

    return hash;
  }

  public static string HashPasswordForWebConfig(string password)
  {
    UnicodeEncoding encoding = new UnicodeEncoding();
    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

    byte[] source = encoding.GetBytes(password);
    byte[] data = sha1.ComputeHash(source);
    string hash = "";

    foreach (byte b in data)
    {
      hash += b.ToString("X2");
    }

    return hash;
  }
}

public enum AuthenticationMode
{
  None,
  Windows,
  Database,
  Certificate
}
