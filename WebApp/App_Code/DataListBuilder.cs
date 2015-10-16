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
using System.Web.UI.HtmlControls;
using System.Data.OleDb;
using System.Web.UI;
using System.IO;
using System.Text.RegularExpressions;

public class DataListBuilder
{
  private HtmlGenericControl _container = new HtmlGenericControl("div");
  private string _defaultLinkTarget = "_blank";

  private Regex _linkRegex = new Regex("\\[((?:http|https|ftp|file|mailto|application):[^\\s]+)\\s+([^\\]]*)\\]", RegexOptions.IgnoreCase);
  private Regex _targetRegex = new Regex("^{([^\\s]+)}\\s+(.*)$", RegexOptions.IgnoreCase);
  private Regex _imageRegex = new Regex("^((?:http|https|ftp):[^\\s]+(?:\\.jpg|\\.gif|\\.png))(?:\\s*(\\d+))?$", RegexOptions.IgnoreCase);

  public DataListBuilder()
  {
    _container = CreateDiv("DataList");
  }

  public DataListBuilder(string defaultLinkTarget) : this()
  {
    _defaultLinkTarget = defaultLinkTarget;
  }

  public HtmlGenericControl BuiltControl
  {
    get
    {
      return _container;
    }
  }

  public void AddFromReader(OleDbDataReader reader)
  {
    AddFromReader(reader, true);
  }

  public void AddFromReader(OleDbDataReader reader, bool addSpace)
  {
    do
    {
      if (reader.HasRows)
      {
        HtmlGenericControl resultSetDiv = CreateDiv("ResultSet");
        _container.Controls.Add(resultSetDiv);

        int headerColumn = reader.GetColumnIndex("Header");
        string lastHeader = null;
        
        int subheaderColumn = reader.GetColumnIndex("Subheader");
        string lastSubHeader = null;
        
        int lastColumn = reader.FieldCount - 1;

        for (int i = 0; i < 2; ++i)
        {
          if (lastColumn == headerColumn || lastColumn == subheaderColumn)
          {
            lastColumn -= 1;
          }
        }

        while (reader.Read())
        {
          HtmlGenericControl rowSetDiv = CreateDiv("RowSet");
          resultSetDiv.Controls.Add(rowSetDiv);

          bool headerAdded = false;
          bool subheaderAdded = false;

          if (headerColumn >= 0 && !reader.IsDBNull(headerColumn))
          {
            string header = reader.GetValue(headerColumn).ToString();
            headerAdded = header != lastHeader;

            if (headerAdded)
            {
              HtmlGenericControl headerDiv = CreateDiv("RowSetHeader", header);
              rowSetDiv.Controls.Add(headerDiv);
              lastHeader = header;
            }
          }

          if (subheaderColumn >= 0 && !reader.IsDBNull(subheaderColumn))
          {
            string subheader = reader.GetValue(subheaderColumn).ToString();
            subheaderAdded = subheader != lastSubHeader;

            if (subheaderAdded)
            {
              if (headerAdded && addSpace)
              {
                AddSpace(rowSetDiv, "Space2");
              }

              HtmlGenericControl subheaderDiv = CreateDiv("RowSetSubheader", subheader);
              rowSetDiv.Controls.Add(subheaderDiv);
              lastSubHeader = subheader;
            }
          }

          if (lastColumn < 0)
          {
            if (addSpace)
            {
              AddSpace(rowSetDiv, "Space2");
            }
          }
          else
          {
            if ((headerAdded || subheaderAdded) && addSpace)
            {
              AddSpace(rowSetDiv, "Space2");
            }

            for (int i = 0; i < reader.FieldCount; ++i)
            {
              if (i != headerColumn && i != subheaderColumn)
              {
                HtmlGenericControl valueSetDiv = CreateDiv("ValueSet");
                rowSetDiv.Controls.Add(valueSetDiv);

                HtmlGenericControl labelDiv = CreateDiv("Label", reader.GetName(i));
                valueSetDiv.Controls.Add(labelDiv);

                if (addSpace)
                {
                  HtmlGenericControl separatorDiv = CreateDiv("Separator", "");
                  valueSetDiv.Controls.Add(separatorDiv);
                }

                HtmlGenericControl valueDiv = CreateValueDiv(!reader.IsDBNull(i) ? reader.GetValue(i) : null);
                valueSetDiv.Controls.Add(valueDiv);

                if (i < lastColumn && addSpace)
                {
                  AddSpace(rowSetDiv, "Space1");
                }
              }
            }

            if (addSpace)
            {
              AddSpace(rowSetDiv, "Space4");
            }
          }
        }
      }
    }
    while (reader.NextResult());
  }

  private void AddSpace(HtmlGenericControl target, string cssClass)
  {
    HtmlGenericControl spacer = CreateDiv(cssClass, "");
    target.Controls.Add(spacer);
  }

  private HtmlGenericControl CreateDiv(string cssClass)
  {
    HtmlGenericControl div = new HtmlGenericControl("div");
    div.Attributes["class"] = cssClass;
    return div;
  }

  private HtmlGenericControl CreateDiv(string cssClass, string content)
  {
    HtmlGenericControl div = CreateDiv(cssClass);

    if (String.IsNullOrEmpty(content) || String.IsNullOrEmpty(content.Trim()))
    {
      div.InnerHtml = "&nbsp;";
    }
    else
    {
      div.InnerText = content;
    }

    return div;
  }

  private HtmlGenericControl CreateValueDiv(object o)
  {
    HtmlGenericControl div = CreateDiv("Value");

    if (o != null)
    {
      if (!(o is String))
      {
        div.InnerText = o.ToString();
      }
      else
      {
        // see if the string contains link definitions

        string s = (string)o;
        Match match = _linkRegex.Match(s);

        if (!match.Success)
        {
          // if not, simply display the string

          div.InnerText = s;
        }
        else
        {
          // otherwise, loop through all of the matched link definitions (in [])

          int index = 0;

          while (match.Success)
          {
            // if literal text comes before this link, add it to the container

            if (match.Groups[0].Index > index)
            {
              div.Controls.Add(new LiteralControl(s.Substring(index, match.Groups[0].Index - index)));
            }

            // create the anchor element and add it to the container

            HtmlAnchor a = new HtmlAnchor();
            a.Attributes["class"] = "CommandLink";
            a.Attributes["href"] = match.Groups[1].Value;
            a.Attributes["target"] = _defaultLinkTarget;
            div.Controls.Add(a);

            // if the link text contains a target (in {}) add that to the anchor

            string linkText = match.Groups[2].Value;
            Match targetMatch = _targetRegex.Match(linkText);

            if (targetMatch.Success)
            {
              linkText = targetMatch.Groups[1].Value;
              a.Attributes["target"] = targetMatch.Groups[2].Value;
            }

            // if the link text specifies an image, add an image element to the anchor

            Match imageMatch = _imageRegex.Match(linkText);

            if (!imageMatch.Success)
            {
              a.InnerText = linkText;
            }
            else
            {
              HtmlImage img = new HtmlImage();
              img.Src = imageMatch.Groups[1].Value;
              img.Style["width"] = imageMatch.Groups.Count > 2 ? imageMatch.Groups[2].Value + "px" : "100%";
              a.Controls.Add(img);
            }

            index = match.Groups[0].Index + match.Groups[0].Length;
            match = match.NextMatch();
          }

          //  if there is any left over text, add that to the container

          if (index < s.Length - 1)
          {
            div.Controls.Add(new LiteralControl(s.Substring(index)));
          }
        }
      }
    }

    return div;
  }

  public void RenderToStream(Stream stream)
  {
    using (StreamWriter streamWriter = new StreamWriter(stream))
    {
      using (HtmlTextWriter writer = new HtmlTextWriter(streamWriter))
      {
        _container.RenderControl(writer);
      }
    }
  }

  public void RenderToWriter(HtmlTextWriter writer)
  {
    _container.RenderControl(writer);
  }
}