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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Help.aspx.cs" Inherits="Help" %>

<!DOCTYPE html>

<html>
<head runat="server">
  <title>Help</title>
  <%--<link href="Styles/Common.css" type="text/css" rel="stylesheet" />--%>
  <link href="Styles/Help.css" type="text/css" rel="stylesheet" />
</head>
<body>
  <form id="form1" runat="server">
    <div class="BodyText" style="width: 600px">
      <div id="pnlAbout" runat="server" visible="false">
        <p><asp:Label id="labAboutTitle" runat="server" CssClass="TitleText"/></p>
        <div id="labAboutText" runat="server"></div>
        <hr/>
      </div>
      <p class="TitleText">How to Use the General Purpose 
        Viewer</p>
      <p class="BodyText">The General Purpose Viewer (GPV) 
        is a mapping web site that provides a consistent set of mapping functions 
        across a variety of applications.&nbsp; This document describes these mapping 
        functions.&nbsp; For specific information regarding the configuration, content 
        and use of the GPV for a particular application, please contact the <a class="BodyLink" id="lnkAdminEmail" runat="server">
        administrator</a> of this web site.</p>
      <div class="BodyText"><br/>
      </div>
      <p class="SubtitleText">Major Components of the 
        General Purpose Viewer</p>
      <p class="BodyText">The interface for the GPV is 
        divided into several panels controlled by tabs. Depending on how your 
        application has been configured, some of these&nbsp;panels may not be 
        available.</p>
      <p class="BodyText"><span class="BoldText">Map Panel</span>
        -&nbsp;A GPV application will always have a map panel. This contains the map 
        itself and the basic controls needed to manage it.&nbsp; At the top left of 
        this panel are one or more map tabs. Clicking on a map tab will change the map 
        to show different thematic layers.&nbsp; If there are more tabs than can fit above the map, you can
        hold down on any tab and slide the set right or left to reveal the hidden tabs.&nbsp; When the viewer
			  is busy retrieving a new map an animated clock icon will appear temporarily
			  toward the upper-left of the map.</p>
      <p class="BodyText">
        You can resize the map panel relative to the function panel to its right by dragging the white space
        between them to the right or left.
      </p>
			<p id="pMapLevel" runat="server" class="BodyText" visible="false">The current application provides an additional 
        <span id="spnMapLevel" runat="server">Level</span> pulldown list above the map which is not shown on the image below.</p>
      <img src="Images/Help/MapPanel.png" alt="" width="555" height="528"/>
      <p class="BodyText"><span class="BoldText">Selection 
          Panels</span> - <span id="spnNoSelection" runat="server">Some applications, but not this 
        one, provide</span><span id="spnHasSelection" runat="server" visible="false">The current 
        application provides</span> two panels that let you select 
        features (mapped objects) on the map and retrieve information about them.&nbsp; 
        The top panel lets you choose one or two map layers from which you can select 
        features and a filter for limiting the data shown.&nbsp; It also has a grid for 
        displaying the data.&nbsp; The bottom panel has one or more tabs and shows 
        detailed data for one of the objects highlighted on the map and in the grid.
      </p>
      <p class="BodyText">
        You can resize the two panels relative to each other by dragging the white space
        between them up or down.
      </p>
      <img src="Images/Help/SelectionPanel.png" alt="" width="382" height="530"/>
      <p class="BodyText"><span class="BoldText">Legend Panel</span>
        - <span id="spnNoLegend" runat="server">Some applications, but not this 
        one, provide</span><span id="spnHasLegend" runat="server" visible="false">The current 
        application provides</span> a panel which displays the legend for the current map.&nbsp; 
        Depending on its configuration, you may be able to expand and collapse parts of the legend.&nbsp;
        You may also be able to click check boxes or radio buttons to turn thematic layers on and off.&nbsp; The changes to
        the layers do not appear immediately on the map when you click a check box&mdash;you must click 
        Refresh Map at the top of the legend to see those changes.&nbsp; Clicking on a layer name that appears as a link
        should lead you to a page containing detailed information about that layer.
      </p>
      <img src="Images/Help/LegendPanel.png" alt="" width="382" height="530"/>
      <p class="BodyText"><span class="BoldText">Location Panel</span>
        - <span id="spnNoLocation" runat="server">Some applications, but not this 
        one, provide</span><span id="spnHasLocation" runat="server" visible="false">The current 
        application provides</span> a panel with an overview map of your area of interest.&nbsp; The red box
        shows the current location of the main map.&nbsp; You can move and resize this box to change the
        extent of the main map.
      </p>
      <p class="BodyText">Depending on the configuration of the application, you may also have additional tabs and links
        in the bottom panel which let you zoom to an area<span id="spnZoneName" runat="server"></span>, set
        the map to a particular level<span id="spnLevelName" runat="server"></span>, or do both operations simultaneously.
      </p>
      <p class="BodyText">
        You can resize the two panels relative to each other by dragging the white space
        between them up or down.
      </p>
      <img src="Images/Help/LocationPanel.png" alt="" width="375" height="508"/>
      <p class="BodyText"><span class="BoldText">Markup Panel</span>
        - <span id="spnNoMarkup" runat="server">Some applications, but not this 
        one, provide</span><span id="spnHasMarkup" runat="server" visible="false">The current 
        application provides</span> a panel for creating and managing markups on the 
        map.&nbsp; You can draw groups of points, lines, polygons and text which are 
        saved in a database and can be retrieved for viewing by others.
      </p>
      <img src="Images/Help/MarkupPanel.png" alt="" width="383" height="519"/>
      <p class="BodyText">The Selection Panels, Legend Panel, Location Panel and Markup 
        Panel occupy the same space on the interface.&nbsp; When more than one are available in 
        an application, you can use tabs at the top to switch between them.</p>
      <div class="BodyText"><br/>
      </div>
      <p class="SubtitleText">Navigating Around the Map</p>
      <p class="BodyText">The following controls are available on the Map Panel.</p>
      <p class="BodyText"><span class="BoldText">Zoom In/Out</span>
        - Slide the vertical bar left or right to quickly zoom in or out on the center of 
        the map.&nbsp; You can click the minus button to zoom in one level and the plus button
        to zoom out one level.  Every two divisions of the bar represent a zoom factor of two.</p>
      <img src="Images/Help/ZoomBar.png" alt="" height="24" width="188" style="margin-left: 20px"/>
      <div id="pnlLevel" runat="server" visible="false">
        <p class="BodyText"><span id="spnLevel1" runat="server" class="BoldText">Level</span>
        - Lets you set the display of the map to a particular <span id="spnLevel2" runat="server">level</span>.
        </p>
        <table cellpadding="0" cellspacing="0" style="margin-left: 20px">
          <tr>
            <td style="padding-left: 6px; padding-right: 6px; padding-top: 3px; padding-bottom: 3px; border: none; background-color: #DEE0D5"">
              <asp:Label ID="labLevel" runat="server" CssClass="BodyText" Text="Level" />
              <select id="ddlLevel" runat="server" class="BodyText" style="width: 70px">
	            </select>
            </td>
          </tr>
        </table>
      </div>
      <p class="BodyText"><span class="BoldText">Scale 1" = [feet]'</span>
        - Shows the current scale of the map.&nbsp; You can enter a different scale here to zoom the map in and out.</p>
      <img src="Images/Help/Scale.png" alt="" style="margin-left: 20px" width="133" height="25"/>
      <div class="BodyText">
        <table cellspacing="0" cellpadding="0" border="0">
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -600px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Full View</span> - 
              Zooms out to the full extent of the map.</td>
          </tr>
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: 0px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Return to Previous 
                Extent</span> - Returns the map view to the last extent before the current 
              one.&nbsp; Every extent from the beginning of the mapping session is 
              remembered, so you can click this repeatly to undo multiple pan/zoom 
              operations.
            </td>
          </tr>
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -912px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Zoom In</span> - When 
              this is selected you can click on the main map, or on the overview map if the Location tab is available, to zoom in by a factor of two on that 
              point.&nbsp; You can also click, hold down, and drag a box on either map to specify
              a new extent for the main map.
            </td>
          </tr>
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -768px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Pan</span> - When 
              this is selected you can click and hold down on the main map to drag it in any 
              direction.&nbsp; If the overview map is available, you can use this tool to drag the
              red box to a new location, which will recenter the main map.</td>
          </tr>
        </table>
      </div>
      <div id="pnlZoneLevel" runat="server" visible="false">
        <p class="BodyText">The following controls are available on the Location Panel</p>
        <p id="pZoneTab" runat="server" visible="false">
          <span id="spnZoneTabName" runat="server" class="BoldText">Zone</span> - Clicking on a link under this tab
          will zoom the map to a particular <asp:Literal id="litZoneName1" runat="server">zone</asp:Literal>.
        </p> 
        <p id="pLevelTab" runat="server" visible="false">
          <span id="spnLevelTabName" runat="server" class="BoldText">Level</span> - Clicking on a link under this tab
          will set the display of the map to a particular <asp:Literal id="litLevelName1" runat="server">level</asp:Literal>. 
        </p> 
        <p id="pComboTab" runat="server" visible="false">
          <span id="spnComboTabName" runat="server" class="BoldText">Level by Zone</span> - Clicking on a link under this tab
          will zoom the map to a <asp:Literal id="litZoneName2" runat="server">zone</asp:Literal> and 
          set the display of the map to a <asp:Literal id="litLevelName2" runat="server">level</asp:Literal> at the same time.  Only
          valid combinations of <asp:Literal id="litZoneName3" runat="server">zone</asp:Literal> and 
          <asp:Literal id="litLevelName3" runat="server">level</asp:Literal> are shown.
        </p> 
        <p><span class="BoldText">All / Containing Selection Only</span> - Depending on the configuration of this application, you may see 
          a count of the number of selected features found in each <asp:Literal id="litZoneLevel" runat="server">zone and level</asp:Literal> next
          to the corresponding link.  Click Containing Selection Only to show only those links that have selected features.
        </p>
      </div>
      <p></p>
      <p class="SubtitleText">Getting Information</p>
      <div class="BodyText">
        <table cellspacing="0" cellpadding="0" border="0">
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -648px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Identify</span> - 
              When this is selected, pointing at an object on the map will display a small 
              box of information about that object.&nbsp; Clicking on
						  the object will popup a separate window with more detailed information.  The popup
						  will contain a link that lets you print the information.&nbsp; Depending on how the GPV has been set up, 
              it is possible that some objects on the map cannot be identified.</td>
          </tr>
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -120px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Display Coordinates</span> - 
              Displays the coordinates of the point you click on the map.</td>
          </tr>
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -48px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Erase Coordinates</span> - 
              Removes coordinates you have placed on the map with the Display Coordinates tool.</td>
          </tr>
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -696px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Measure Area</span> - 
              When this is selected you can click on the map multiple times to measure the 
              area of a shape. The area will be shown inside the shape. Double-click to end 
              the measurement.</td>
          </tr>
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -672px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Measure Distance</span>
              - When this is selected you can click on the map multiple times to measure the 
              accumulated length of a set of lines. The length will be shown where you first 
              clicked. Double-click to end the measurement.</td>
          </tr>
        </table>
      <p class="BodyText"><span class="BoldText">Help</span> - Displays the help for the General Purpose Viewer in a pop-up window.</p>
      </div>
      <p></p>
      <p class="SubtitleText">Communicating with Others</p>
      <div class="BodyText">
        <table cellspacing="0" cellpadding="0" border="0">
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -792px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Printable Map</span> - 
              Pops up a utility for creating a PDF version of the current map suitable for 
              printing or archiving.&nbsp; You can select which page 
              layout you prefer and specify whether you wish to preserve either the scale or 
              the width of the current map.&nbsp; Depending on the configuration, you may also
              be able to provide text, such as a title, that will appear at specific locations
              on the printable page.<br/>
              <br/>
              <img src="Images/Help/Print.png" alt="" width="490" height="185"/>
            </td>
          </tr>
          <tr>
            <td class="Icon" valign="top"><div class="Button" style="background-position: -576px 0px"></div></td>
            <td class="Description" valign="top"><span class="BoldText">Email This Page</span>
              - Starts your email client with a message containing a link to your current map 
              page. All the information about your page - the current map tab, zoom level, 
              selected object, and markup - are encrypted in this link. You can add text to 
              this message and mail it to anyone that also has access to this General Purpose 
              Viewer. When they click on the link in the message, the General Purpose Viewer 
              will appear exactly as you had set it.</td>
          </tr>
        </table>
      </div>
      <p></p>
      <div id="pnlSelection" runat="server" visible="false">
        <p class="SubtitleText">Selecting Features on the 
          Map</p>
        <p class="BodyText">You can pick features on the map from certain layers and display 
          information about them.&nbsp; Depending on how your GPV is configured and 
          linked to other web applications, you may be brought automatically to a set of 
          selected features when the GPV comes up&mdash;the functions on the Selection Panels 
          let you change this selected set.</p>
        <p class="BodyText">The top Selection Panel lets you specify how you would like to pick features 
          on the map.&nbsp; This is done by setting the five pulldown lists, shown below,
			    so that together they form a readable sentence which describes how the Select Features
			    tool to the left will function.&nbsp; See the picture under Selection Panels (above) for an example.
        </p>
        <div style="width: 355px; height: 80px; background-color: #DEE0D5">
            <div style="position: relative">
            <div class="Button" title="Select Features" style="position: absolute; left: 10px; top: 3px; background-position: -816px 0px"></div>
            <div class="Button"  title="Select All in View" style="position: absolute; left: 10px; top: 28px; background-position: -864px 0px"></div>
            <div class="Button"  title="Clear Selection" style="position: absolute; left: 10px; top: 53px; background-position: -24px 0px"></div>
            <select id="ddlAction" class="BodyText" style="position: absolute; left: 38px; top: 8px; width: 188px">
				      <option value="Action">Action</option>
			      </select>
            <select id="ddlTarget" class="BodyText" style="position: absolute; left: 229px; top: 8px; width: 105px" runat="server">
				      <option value="Target">Target</option>
			      </select>
			      <select id="ddlProximity" class="BodyText" style="position: absolute; left: 38px; top: 30px; width: 188px">
				      <option value="Proximity">Proximity</option>
			      </select>
            <select id="ddlSelection" class="BodyText" style="position: absolute; left: 229px; top: 30px; width: 105px" runat="server">
				      <option value="Selection">Selection</option>
			      </select>
            <select id="ddlQuery" class="BodyText" style="left: 38px; position: absolute; top: 52px; width: 296px">
				      <option value="Filter">Filter</option>
			      </select>
			    </div>
        </div>
        <p class="BodyText"><span class="BoldText">Action</span> - Lets you specify what the Select Features tool to the left
        will do when you click on the map.&nbsp; Three types of action are supported:</p>
        <ul>
          <li class="BodyText"><span class="EmphasizeText">Select</span> - Pick features directly
				    from the <span class="EmphasizeText">Target</span>
          layer.</li>
          <li class="BodyText"><span class="EmphasizeText">Find all [within a distance]</span> - Pick features from the <span class="EmphasizeText">Selection</span> layer 
            and let the application find features on the <span class="EmphasizeText">Target</span> layer within the distance specified by <span class="EmphasizeText">Proximity</span>.
          </li>
          <li class="BodyText"><span class="EmphasizeText">Find the one...five [nearest]</span> - Pick features from the <span class="EmphasizeText">Selection</span> layer 
            and let the application find the nearest features on the <span class="EmphasizeText">Target</span> layer, regardless of their distance.
          </li>
        </ul>
        <p class="BodyText"><span class="BoldText">Target</span> - Lets you choose the target layer.&nbsp; Data
			    for features picked from this layer will be shown in the data grid below.&nbsp;
			    Target features will highlight with the same color as the background of this pulldown
			    list (unless they have been filtered, in which case they will appear gray).</p>
		    <p class="BodyText">
			    <span class="BoldText">Proximity</span> - When <span class="EmphasizeText">Action</span> is set to "Find all",
			    this lets you specify the distance around features in the 
          selection layer to search for features in the target layer.&nbsp; If set to 
          zero, target features will only be found inside or directly adjacent to the 
          selection features.</p>
        <p class="BodyText"><span class="BoldText">Selection</span> - Lets you choose the selection layer.&nbsp; Features 
          picked from this layer will be used to&nbsp;search for&nbsp;features in the 
          target layer.&nbsp; Selection features will highlight with the same color as
			    the background of this pulldown list.</p>
		    <p class="BodyText">
			    <span class="BoldText">Filter</span> - Applies a filter to the set of target features you picked 
          on the map.&nbsp; Data for features that fulfill the criteria of the filter 
          will be displayed in the grid below this control.</p>
        <p></p>
        <div class="BodyText">
          <table cellspacing="0" cellpadding="0" border="0">
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -816px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Select Features</span>
                - When this is selected you can pick features from the target or selection layers on the map.&nbsp; Either click
						    features individually or drag a box to pick multiple features.&nbsp; Hold down on
						    the Shift key while clicking or dragging to add features to the selected set.&nbsp;
						    Hold down on the Control (Ctrl) key to remove features.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -864px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Select All in View</span>
                - Select all features from the target or selection layers within the current view of the map.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -24px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Clear Selected Features</span>
                - Clear the selected sets of target and selection 
                features.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -936px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Zoom To Selected 
                  Features</span> - (on the Map Panel) Zoom to the combined extent of all 
                target and selection features.
              </td>
            </tr>
          </table>
        </div>
        <p class="SubtitleText">Using Selected Features</p>
        <p class="BodyText">If the Selection Panels are available in your GPV application and you have 
          picked some target features on the map, you can use the capabilities below to 
          show data about those features.</p>
        <p class="BodyText"><span class="BoldText">Data Grid</span>
          &nbsp;- This displays data about the selected target features that have passed 
          the filter criteria.&nbsp; The columns of data shown will vary with the target 
          layer and the selected filter.&nbsp; Clicking on a row in this grid will 
          highlight that row, highlight the corresponding feature on the map, and bring 
          up detailed&nbsp;data in the bottom panel.&nbsp; Double-clicking will do the same plus zoom to
          the feature.&nbsp; The total number of rows in the grid is displayed at the lower left of the grid.</p>
        <img src="Images/Help/DataGrid.png" alt="" width="361" height="171"/>
        <p class="BodyText"><span class="BoldText">To Mailing Labels</span> - When enabled, this lets you generate mailing labels 
          for the selected target features.&nbsp; A window will&nbsp;popup which let's 
          you select the label format, printing direction and font characteristics.&nbsp; The labels 
          are&nbsp;delivered as a PDF file which can be printed or saved to disk.&nbsp; 
          It is likely that only certain layers (such as parcels or buildings) will be 
          enabled for mailing labels in your application.</p>
        <img src="Images/Help/MailingLabels.png" alt="" width="322" height="307"/>
        <p class="BodyText"><span class="BoldText">To Spreadsheet</span> - This exports 
          the data for the selected target features currently shown in the data grid to a 
          comma-separated value (CSV) file or an Excel spreadsheet (XLS) depending on the
          configuration of this application.&nbsp; This may launch a program, such as Microsoft 
          Excel or OpenOffice Calc, for viewing the spreadsheet.&nbsp; If not, you will be 
          prompted to save the file to disk.&nbsp; Please note that later versions of Excel
          may display a notice regarding the XLS file format.&nbsp; You can safely ignore this
          and open the file.</p>
        <p class="BodyText"><span class="BoldText">Detailed 
            Data</span> - The bottom Selection Panel shows detailed data from the feature 
          highlighted in the data grid.&nbsp; Depending on your application, one or more 
          tabs will be provided for different types of data.&nbsp; If there are more tabs than can fit above the data, you can
          hold down on any tab and slide the set right or left to reveal the hidden tabs.&nbsp; Some of the 
          data shown in the panel may be displayed as links to other pages (which will popup in another 
          window) or which change the target layer, selection layer, and selected features.&nbsp; It 
          is also possible to have small images within the listed 
          data.&nbsp; These may act as links to other pages or documents.&nbsp; The panel
          contains a link that lets you print the data.</p>
        <img src="Images/Help/DataTab.png" alt="" width="382" height="211"/>
      </div>
      <div id="pnlMarkup" runat="server" visible="false">
        <p class="SubtitleText">Creating and Managing Map 
          Markup</p>
        <p class="BodyText">You can draw shapes and text (markup) on the map for others to 
          see.&nbsp; <span id="spnMarkupOpen" runat="server" visible="true">Every user has the 
          ability to edit and delete the markup 
          created by other users.&nbsp; Given the wide-open nature of this environment, 
          your organization may set guidelines for the proper treatment of markup.&nbsp;
          Please contact your <a class="BodyLink" id="lnkAdminEmail2" runat="server">administrator</a>
          for details.</span>
          <span id="spnMarkupSecure" runat="server" visible="false">You have the option of allowing other users
          to edit and delete your markup.</span></p>
        <p class="BodyText"><span class="BoldText">Your Name</span>
          - <span id="spnMarkupNameOpen" runat="server" visible="true">You must provide a name or identifier for yourself to create markup.&nbsp;</span> 
          <span id="spnMarkupNameSecure" runat="server" visible="false">Your login name appears here and cannot be changed.&nbsp;</span> 
          This name will appear in the grid below for all markup that you create.</p>
        &nbsp;&nbsp;&nbsp;<img src="Images/Help/MarkupName.png" alt="" alt="" width="289" height="25"/>
        <p class="BodyText"><span class="BoldText">Category</span>
          - Markup is placed in categories for ease of management.&nbsp; Select an 
          appropriate category from this list before creating new markup.&nbsp; Upon 
          selecting a category the grid will update to show all available markup 
          groups in that category.</p>
        &nbsp;&nbsp;&nbsp;<img src="Images/Help/MarkupCategory.png" alt="" width="278" height="27"/>
        <p class="BodyText"><span class="BoldText">Markup Group</span>
          - Shapes and text that you create are stored in named 
          groups.&nbsp; The following functions let you manage these markup 
          groups.</p>
        <ul>
          <li class="BodyText">
            <span class="BoldText">New</span> - Creates a new, untitled markup group in the 
            selected category assigned to your name.&nbsp; Once you click this you can use 
            the various tools described below to create the markup graphics.<br/></li>
          <li class="BodyText">
            <span class="BoldText">Zoom To</span> - Zooms the map to the extent of all the 
            shapes and text in the current markup group.&nbsp; Use this if you have panned 
            away from the markup group and wish to return to it.<br/></li>
          <li class="BodyText">
            <span class="BoldText">Delete</span> - Deletes the current markup group.&nbsp; 
            The markup will be removed from the map and grid.<br/></li>
          <li class="BodyText">
            <span class="BoldText">To KML</span> - Exports the current markup group to a file
            which can be displayed in Google Earth.<br/></li>
          <li class="BodyText">
            <span class="BoldText">Title</span> - The title of the current markup 
            group.&nbsp; This defaults to [untitled] for new markup groups.&nbsp; Change 
            this to a short phrase which accurately describes the markup group.<br/>
          </li>
          <li id="liLock" runat="server" class="BodyText" visible="false">
            <span class="BoldText">Lock</span> - When checked, only you can edit or delete
            the current markup group.  When unchecked, all users can make such edits.  [not shown below]<br/>
          </li>
        </ul>
        &nbsp;&nbsp;&nbsp;<img src="Images/Help/MarkupGroup.png" alt="" width="304" height="39"/>
        <p></p>
        <div class="BodyText">
          <table cellspacing="0" cellpadding="0" border="0">
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -432px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Draw Point</span> &nbsp;- 
                When this is selected, click on the map&nbsp;to draw&nbsp;a small, circular 
                point symbol.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -384px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Draw Line</span> &nbsp;- 
                When this is selected, click on the map multiple times to draw a line with 
                multiple vertices.&nbsp; Double-click to end the line.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -480px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Draw Polygon</span>&nbsp;- 
                When this is selected, click on the map multiple times to draw a 
                semi-transparent polygon.&nbsp; Double-click to end the polygon.&nbsp;
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -240px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Draw Circle</span>&nbsp;- 
                When this is selected, press the mouse button on the map to define the center of a circle, drag to size the circle,
						    and release to finish drawing the circle.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -288px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Draw Coordinates</span>&nbsp;- 
                When this is selected, click on the map to draw a point with coordinates.  This works like the Display Coordinates
                tool above the map except that the coordinates are saved as markup.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -336px 0px"></td>
              <td class="Description" valign="top"><span class="BoldText">Draw Measured Length</span>&nbsp;- 
                When this is selected, click on the map multiple times to draw a line with 
                multiple vertices.&nbsp; Text showing the length of the line will display with the line.&nbsp; Double-click to end the line.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -192px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Draw Measured Area</span>&nbsp;- 
                When this is selected, click on the map multiple times to draw a 
                semi-transparent polygon.&nbsp; Text showing the area of the polygon will display inside the polygon.&nbsp; Double-click to end the polygon.&nbsp;
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -144px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Delete&nbsp;Markup</span>&nbsp;- 
                When this is selected, click on individual markup shapes and text&nbsp;to 
                delete them.&nbsp;
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -72px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Pick Color</span>&nbsp;- 
                When this is selected, click on a markup shape to set its color as the current drawing color.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -720px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Fill With Color</span>&nbsp;- 
                When this is selected, click on a markup shapes to change its color to the current
                drawing color.
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-color: red; background-position: 24px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Markup&nbsp;Color</span>&nbsp;- 
                Click on this to set the current drawing color for any new markup you place on the map.&nbsp; 
                A color selection panel will display the hue, saturation and value of the current color.&nbsp; 
                Move the sliders to change any of these color properties then click OK to set this as the new color.<br/>
                <br/>
                <img src="Images/Help/ColorSelector.png" alt="" width="130" height="130"/>
              </td>
            </tr>
            <tr>
              <td class="Icon" valign="top"><div class="Button" style="background-position: -528px 0px"></div></td>
              <td class="Description" valign="top"><span class="BoldText">Draw Text</span>&nbsp;- 
                When this is selected, click on the map to place the text you specified in the 
                box to the right.&nbsp; To make the text glow with a background color, check the Glow box
                and click the color selector next to Glow to choose the color.<br/>
                <br/>
                <img src="Images/Help/MarkupText.png" alt="" width="293" height="32"/>
              </td>
            </tr>
          </table>
        </div>
        <p class="BodyText"><span class="BoldText">Markup&nbsp;Grid</span>
          -&nbsp;This displays all of the available markup groups in the selected 
          category.&nbsp; Clicking on one or more rows
          in this grid will zoom the map to the extent of those markup groups and draw their
          markup.&nbsp; Clicking on just one group will 
          <span id="spnMarkupSelectOpen" runat="server" visible="true"> also open that group for editing.</span>
          <span id="spnMarkupSelectSecure" runat="server" visible="false"> open that group for editing if you are its creator.</span></p>
        <img src="Images/Help/MarkupGrid.png" alt="" width="360" height="219"/>
      </div>
      <p class="SubtitleText">Other Capabilities</p>
      <p class="BodyText"><span class="BoldText">Mobile</span> - Launches the mobile version of the GPV with the same map and extent.&nbsp; The mobile
        version has a button which returns you to this desktop version.</p>
      <p class="BodyText"><span class="BoldText">External Map Viewers</span> - The pulldown list to the lower left of the
			  map lets you launch other web-based map viewers centered and zoomed to the same
			  location as the current map.&nbsp; Select the map viewer and click Go to pop it
			  up in a new browser window.</p>
      <img src="Images/Help/ExternalMap.png" alt="" style="margin-left: 20px"" width="167" height="27"/>
      <p class="BodyText"><span class="BoldText">Save Map</span> - The pulldown list to the lower right of the
			  map lets you save the current map image to a file.  Select "as Image" to save the map as a PNG or JPEG image file.  
			  Select "as KML" to save the map in a format that can be viewed in Google Earth.  Click Save Map to download and save the file.</p>
      <img src="Images/Help/SaveMap.png" alt="" style="margin-left: 20px" width="147" height="27"/>
      <br/>
      <br/>
      <p id="pVersion" runat="server" class="SubduedText"></p>
      </div>
  </form>
</body>
</html>
