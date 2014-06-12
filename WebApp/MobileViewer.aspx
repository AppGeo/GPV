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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MobileViewer.aspx.cs" Inherits="MobileViewer" %>

<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8">
  <title>( mV )</title>
  <meta name="description" content="General Purpose Viewer Mobile">
  <meta name="author" content="Applied Geographics, Inc.">
  <meta name="HandheldFriendly" content="True">
  <meta name="MobileOptimized" content="320">
  <meta name="viewport" content="width=device-width, minimum-scale=1">
  <!-- for iOS web apps -->
  <meta name="apple-mobile-web-app-capable" content="yes">
  <!-- this script prevents iOS web app links from opening in mobile safari: https://gist.github.com/1042026 -->
  <script>    (function (a, b, c) { if (c in b && b[c]) { var d, e = a.location, f = /^(a|html)$/i; a.addEventListener("click", function (a) { d = a.target; while (!f.test(d.nodeName)) d = d.parentNode; "href" in d && (d.href.indexOf("http") || ~d.href.indexOf(e.host)) && (a.preventDefault(), e.href = d.href) }, !1) } })(document, window.navigator, "standalone")</script>
  <!-- for Windows Phone 7 -->
  <meta http-equiv="cleartype" content="on">
  <link rel="stylesheet" href="Styles/Mobile/jquery.mobile-1.1.1.min.css">
  <link rel="stylesheet" href="Styles/Mobile/Mobile.css">
</head>
<body runat="server" id="body">
  <div id="appList" data-role="page">
    <div data-role="header">
      <h1>( mV )</h1>
    </div>
    <div data-role="content">
      <h2>Welcome to the General Purpose <span class="one-word">Mobile Viewer</span>!</h2>
      <div class="no-apps" style="display: none;">
        <p>There are no applications, please check your GPV configuration.</p>
      </div>
      <div class="have-apps" style="display: none;">
        <p>Which application would you like?</p>
        <ul data-role="listview" data-inset="true">
          <li data-role="list-divider">Applications</li>
        </ul>
      </div>
    </div>
  </div>
  <div id="app" data-role="page">
    <div data-role="header">
      <h1><span data-text="DisplayName">&nbsp;</span></h1>
    </div>
    <div data-role="content">
      <div class="no-mapTabs" style="display: none;">
        <p>There are no maps for this app, please check your GPV configuration.</p>
      </div>
      <div class="have-mapTabs" style="display: none;">
        <p>Which map would you like?</p>
        <ul data-role="listview" data-inset="true">
        </ul>
      </div>
    </div>
  </div>
  <div id="mapTab" data-role="page" data-fullscreen="true">
    <div data-role="header" data-position="fixed" data-tap-toggle="false">
      <div data-role="navbar" data-iconpos="right">
        <ul>
          <li><a id="cmdDesktop" href="Viewer.aspx" data-role="button" data-icon="grid" rel="external">Desktop</a></li>
          <li><a id="cmdLocate" href="#" data-role="button" data-icon="star">Locate</a></li>
          <li><a href="#app" data-icon="arrow-d" data-transition="slidedown"><span data-text="DisplayName">&nbsp;</span></a></li>
        </ul>
      </div>
    </div>
    <div class="geomap">
    </div>
    <div data-role="controlgroup" class="zoom-buttons">
      <a id="cmdZoomIn" href="#" data-role="button" data-icon="plus" data-iconpos="notext">zoom in</a>
      <a id="cmdZoomOut" href="#" data-role="button" data-icon="minus" data-iconpos="notext">zoom out</a>
    </div>
  </div>
  <div id="identify" data-role="page">
    <div data-role="header">
      <h1>identify</h1>
    </div>
    <div data-role="content" class="identify-content">
    </div>
  </div>
  <script src="Scripts/jquery-1.7.2.min.js"></script>
  <script src="Scripts/jquery.geo-test.min.js"></script>
  <script src="Scripts/Mobile/Mobile.js"></script>
  <script src="Scripts/Mobile/jquery.mobile-1.1.1.min.js"></script>
</body>
</html>
