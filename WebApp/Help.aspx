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

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
  <title>Help</title>
  <link href="Styles/Help.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Customize.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript"> if (typeof (JSON) == "undefined") { location.href = "Incompatible.htm"; } </script>
  <script type="text/javascript" src="Scripts/jquery-2.1.4.min.js"></script>
  <script type="text/javascript" src="Scripts/WebFonts.js"></script>
  <script type="text/javascript" src="Scripts/Help.js"></script>
</head>
<body>
    <div id="pnlTableofContents">
      <ul class="Menu" id="listContents">
        <li class="MenuItem"><a href="#uicomponents">Major UI Components</a></li>
        <li class="MenuItem"><a href="#functionpanel">Function Panel Features</a></li>
        <li class="MenuItem"><a href="#maptools">Interacting with the Map</a></li>
        <%--<li class="MenuItem"><a href="#search">Searching for Features on the Map</a></li>--%>
        <li class="MenuItem"><a href="#select">Selecting Features on the Map</a></li>
        <li class="MenuItem"><a href="#markup">Creating and Managing Map Markup</a></li>
        <li class="MenuItem"><a href="#share">Sharing Maps</a></li>
        <li class="MenuItem"><a href="#other">Other Capabilities</a></li>
      </ul>
    </div>
    <div id="pnlHelpContent" class="mobile">

      <%--UI Components--%>
      <h2 class="MenuItem mobile" id="uicomponents">Major UI Components</h2>
      <p>The user interface (UI) for the General Purpose Viewer(GPV) is divided into several panels.</p>
      <img src="Images/Help/Interface.png" class="interface" alt="Interface" />
      <p class="mobile">
        <span class="component">Map Panel:</span> The GPV application will always have a Map Panel. This is the map 
        itself and the basic controls needed to navigate, identify features, and change map themes. When the viewer 
        is updating the map, an animated progress bar will appear at the top of the map panel.
      </p>
      <p>
        <span class="component">Function Panel:</span> To the left of Map Panel is the Function Panel which provides 
        access to various GPV functions. Selecting a function from the menu will update the panel with additional features in
        the same panel. To return to the initial function menu click the < icon in the header.
      </p>
      <p class="mobile">
        <span class="component">Details Panel:</span> The Details Panel will appear with more information about a feature
        when it is selected, either through the Selection Panel or using the identify tool. To hide the panel click the > icon in the header.
      </p>
      <table class="mobile">
        <tbody class="mobile">
        <tr>
          <td>
            <img src="Images/Help/FunctionToggle.png" alt="function panel toggle" /></td>
          <td><span class="component">Function Panel Toggle:</span> Click to toggle the Function Panel in and out of the page.</td>
        </tr>
        <tr class="mobile">
          <td>
            <img src="Images/Help/DetailsToggle.png" alt="detail panel toggle" /></td>
          <td><span class="component">Details Panel Toggle:</span> Click to toggle the identify / selection Details Panel in and out of the page</td>
        </tr>
        <tr class="mobile">
          <td>
            <img src="Images/Help/Help.png" alt="zoom in and out" /></td>
          <td><span class="component">Help:</span> Click to get access to help documents.</td>
        </tr>  
        <tr class="mobile">
          <td>
            <img src="Images/Help/getLinks.png" alt="zoom in and out" /></td>
          <td><span class="component">Get Link:</span>Provides a link to the current map page.  You can copy this link and paste it into a messaging
        application to share it with others.  All the information about the page - the current map tab, zoom level, selected object, and markup - 
        are encrypted in this link.  When the link is clicked, the GPV will reappear in its current state.</td>
        </tr>
          </tbody>
      </table>

      <%--Function Panels--%>
      <h2 class="MenuItem" id="functionpanel">Function Panel Features</h2>

      <p class="txtLeft showSearch search">
        <img class="imgRight" src="Images/Help/SearchPanel.png" alt="search panel" />
        <span class="component">Search:</span> There are two sections of the Search Panel. The top section 
        allows users to define their search criteria and the bottom section presents the results of the search. 
        Users can select one or more of the search results from the bottom section any see each on the map using 
        the “Show on Map” buttons. This visualization of search results will automatically open the Selection Panel 
        with the features selected from the search results. At any time users can navigate back to the Search panel 
        to continue searching for additional results, where the original search criteria and results will be preserved until reset.
      </p>
      <p class="txtRight selection">
        <img class="imgLeft showSelection" src="Images/Help/SelectionPanel.png" alt="Selection Panel" />
        <span class="component">Selection:</span> There are two sections of the Selection Panels. The top section 
        allows users to select features (mapped objects) from the map and bottom section presents information about 
        the selected features. The top panel contains drop down lists that control the selection of one or two map 
        layers and a filter for limiting the data shown. The bottom panel has one or more columns and shows detailed 
        data for the objects highlighted on the map.
      </p>
      <p class="txtLeft showLegend legend">
        <img class="imgRight" src="Images/Help/LegendPanel.png" alt="legend panel" />
        <span class="component">Legend Panel:</span> The current application provides a panel which displays both the 
        legend for the current map and controls to manage additional basemap options. Depending on the GPV configuration, 
        parts of the legend may be expandable and collapsible and the basemap controls may be visible. Click check boxes 
        or radio buttons to turn thematic layers or basemaps on and off. The changes to the layers do not appear immediately 
        on the map, but changes to basemaps appear immediately. Click the Refresh Map button at the top of the legend to see 
        layer changes. Layer names that appear as a link are clickable and will open a window that contains more information 
        about that layer.
      </p>
      <p class="txtRight showLocation location">
        <img class="imgLeft" src="Images/Help/LocationPanel.png" alt="location panel" />
        <span class="component">Location Panel:</span> There are two sections of the Selection Panels. The top section 
        allows users to select features (mapped objects) from the map and bottom section presents information about the 
        selected features. The top panel contains drop down lists that control the selection of one or two map layers and a 
        filter for limiting the data shown. The bottom panel has one or more columns and shows detailed data for the 
        objects highlighted on the map. 
      </p>
      <p class="txtLeft showMarkup markup">
        <img class="imgRight" src="Images/Help/MarkupPanel.png" alt="markup panel" />
        <span class="component">Markup Panel:</span> The current application provides a panel for creating and managing 
        markups on the map. Groups of points, lines and polygons and text can be added to the map and saved in a database and can 
        later be retrieved and viewed by other users.
      </p>
      <p class="txtRight share">
        <img class="imgLeft" src="Images/Help/SharePanel.png" alt="share panel" />
        <span class="component">Share Panel:</span> The current application provides a panel for communicating the content 
        presented. There are a number of communication options including printing and electronic output formats. Additional 
        details about these features can be found in the <a href="#other">Communicating with Others Section</a>.
      </p>
      <p>The Search, Selection, Legend, Location, Markup, and Share Panels occupy the same space on the UI.</p>

      <%--Map Tools--%>
      <h2 class="MenuItem mobile" id="maptools">Interacting with the Map</h2>

      <table class="mobile">
        <tbody class="mobile">       
        <tr class="mobile">
          <td>
            <img src="Images/Help/BasemapsOverLays.png" alt="Basemaps and Overlays" /></td>
          <td><span class="component">Overlays and BaseMaps:</span> Controls to manage additional basemap options visible. 
            Click check boxes or radio buttons to turn thematic layers or basemaps on and off,the changes to basemaps appear immediately.</td>
        </tr>
        <tr class="mobile">
          <td>
            <img src="Images/Help/ZoomButtons.png" alt="zoom in and out" /></td>
          <td><span class="component">Zoom In/Out:</span> Click the 
            minus button to zoom in and the plus button to zoom out.</td>
        </tr>
        <tr class="mobile">
          <td>
            <img src="Images/Help/FullExtentButton.png" alt="full extent" /></td>
          <td><span class="component">Full Map Extent:</span> Click to zoom 
             to the full extent of the map.</td>
        </tr>
        <tr class="mobile">
          <td>
            <img src="Images/Help/CurrentLocation.png" alt="zoom to current location" /></td>
          <td>
            <span class="component">Zoom to Current Location:</span> The map will re-center on the location provided by the browser or device. 
            GPS must be enabled.
          </td>
        </tr>
        <tr class="mobile">
          <td>
            <img src="Images/Help/Overview.png" alt="overview map" /></td>
          <td>
            <span class="component">Overview Map:</span> This will expand or contract the overview map. The highlighted box in the overview map 
            shows the current map extent in relation to the full extent. This box can be dragged to change the map extent.
          </td>
        </tr>
        <tr class="mobile">
          <td>
            <img src="Images/Help/MapTheme.png" alt="map theme" /></td>
          <td>
            <span class="component">Map Theme:</span> This drop down list contains a list of available map themes. The text will contain the name of the 
            selected theme. Choose a new theme from the list to update the map.
          </td>
        </tr>
        <tr class="mobile">
          <td>
            <img src="Images/Help/PanTool.png" alt="map tools / pan tool" /></td>
          <td><span class="component">Map Tools:</span> By default 
            the Pan tool is selected. The pan tool allows dragging of the map to change the current extent. Clicking on this 
            will also expand the map tool menu.</td>
        </tr>
        <tr class="mobile">
          <td>
            <img src="Images/Help/IdentifyTool.png" alt="identify tool" /></td>
          <td><span class="component">Identify Tool:</span> When 
            this is selected, hover over a map feature will display a popup with information about that feature. Clicking on a 
            feature will open the Details Panel with more information. The Details Panel contains a print button to print this 
            information. *NOTE: Not all map features are identifiable.</td>
        </tr>
          <tr class="mobile">
          <td>
            <img src="Images/Help/SelectTool.png" alt="identify tool" /></td>
          <td><span class="component">Select Tool:</span> When 
            this is selected, Select Panel opens from the left side of the UI and shows detailed data from the 
        feature highlighted in the selection data grid.</td>
        </tr>
          <tr class="mobile">
          <td>
            <img src="Images/Help/DrawTool.png" alt="identify tool" /></td>
          <td><span class="component">Draw Tool:</span> When 
            this is selected, Draw Panel will be opened for creating and managing markups on the map..</td>
        </tr>
        <tr class="selection">
          <td>
            <img src="Images/Help/SelectFeatures.png" alt="select features tool" /></td>
          <td><span class="component">Select Features Tool:</span> When this is selected click on features from the target or 
            selection layers on the map. Either click features individually or drag a box to pick multiple features. Hold down 
            on the Shift key while clicking or dragging to add features to the selected set. Hold down on the Control (Ctrl) key to remove features.</td>
        </tr>
        <tr>
          <td>
            <img src="Images/Help/DrawPoint.png" alt="draw point" /></td>
          <td><span class="component">Draw Point:</span> Click on the map to draw a small, circular point symbol.</td>
        </tr>
        <tr>
          <td>
            <img src="Images/Help/DrawLine.png" alt="draw line" /></td>
          <td><span class="component">Draw Line:</span> Click on the map multiple times to draw a line with multiple vertices. 
            Double-click to end the line.</td>
        </tr>
        <tr>
          <td>
            <img src="Images/Help/DrawPolygon.png" alt="draw polygon" /></td>
          <td><span class="component">Draw Polygon:</span> Click on the map multiple times to draw a semi-transparent polygon. 
            Double-click to end the polygon.</td>
        </tr>
        <tr>
          <td>
            <img src="Images/Help/DrawCircle.png" alt="draw circle" /></td>
          <td><span class="component">Draw Circle:</span> Hold the mouse down on the map to define the center of a circle, 
            drag to size the circle, and release to finish drawing the circle. <span class="alert">Markup only.</span></td>
        </tr>
        <tr>
          <td>
            <img src="Images/Help/DrawText.png" alt="draw text" /></td>
          <td><span class="component">Draw Text:</span> Click on the map to place text and show input box. 
            Type text in input box. Press enter to complete the placement of text. To add a text glow with a background color, 
            check the box next to the Text Glow color selector on the Markup Panel and select glow color prior to placing text on the map.</td>
        </tr>
        <tr>
          <td>
            <img src="Images/Help/DrawCoordinates.png" alt="draw coordinates" /></td>
          <td><span class="component">Draw Coordinates:</span> Click on the map to draw a point with coordinates.</td>
        </tr>
        <tr>
          <td>
            <img src="Images/Help/MeasureLength.png" alt="draw measured length" /></td>
          <td><span class="component">Draw Measured Length:</span> Click on the map multiple times to draw a line with 
            multiple vertices. Text showing the length of the line will display with the line. Double-click to end the line.</td>
        </tr>
        <tr>
          <td>
            <img src="Images/Help/MeasureArea.png" alt="draw measured area" /></td>
          <td><span class="component">Draw Measured Area:</span>
          Click on the map multiple times to draw a semi-transparent polygon. 
            Text showing the area of the polygon will display inside the polygon. Double-click to end the polygon.
        </tr>
        <tr>
          <td>
            <img src="Images/Help/DeleteMarkup.png" alt="delete markup" /></td>
          <td><span class="component">Delete Markup:</span> Click on individual markup shapes and text to delete them.</td>
        </tr>
        <tr class="markup">
          <td>
            <img src="Images/Help/PickColor.png" alt="pick color" /></td>
          <td><span class="component">Pick Color:</span> Click on a markup shape to set its color as the current drawing color. <span class="alert">Markup only.</span></td>
        </tr>
        <tr class="markup">
          <td>
            <img src="Images/Help/FillwColor.png" alt="fill with color" /></td>
          <td><span class="component">Fill With Color:</span> Click on a markup shapes to change its color to the current drawing color. <span class="alert">Markup only.</span></td>
        </tr>
          </tbody>
      </table>


      <%--<h2 class="MenuItem" id="search">Searching for Features on the Map</h2>--%>

      <%--Selection Panel--%>
      <h2 class="MenuItem selection" id="select">Selecting Features on the Map</h2>

      <p class="selection">
        Features on the map from certain layers can be clicked to display information about them. Depending on how the GPV 
      is configured and linked to other web applications, a set of selected features may be preset 
      when the GPV opens - the functions on the Selection Panel allow for changing this selected set.
      </p>

      <p class="selection">
        <img src="Images/Help/SelectingFeaturesOptions.png" class="imgLeft" alt="selecting features" />The top Selection Panel 
      specifies how to pick features on the map. This is done by setting the five pull down lists, to the right, so that together 
      a readable sentence is formed which describes how the Select Features map tool will function.
      </p>

      <p class="selection">
        <span class="component">Action:</span> Specify what the Select Feature map tool will do when the map is clicked. 
        Three types of actions are supported:
      </p>
      <ul class="bullet-list selection">
        <li><span class="component">Select:</span> Pick features directly from the Target layer.</li>
        <li><span class="component">Find all [within a distance]:</span> Pick features from the Selection layer and let the application find features on the 
          Target layer within the distance specified by Proximity.</li>
        <li><span class="component">Find the one...five [nearest]:</span> Pick features from the Selection layer and let the application find the 
          nearest features on the Target layer, regardless of their distance.</li>
      </ul>

      <p class="selection">
        <span class="component">Target:</span> Specifies the target layer. Data for features picked from this layer 
        will be shown in the data grid below. Target features will highlight with the same color as the background of 
        this pull down list (unless they have been filtered, in which case they will appear gray).
      </p>

      <p class="selection">
        <span class="component">Proximity:</span> "Find all" specifies the distance around features 
        in the selection layer to search for features in the target layer. If set to zero, target features will only be found inside or 
        directly adjacent to the selection features.
      </p>
      <p class="selection">
        <span class="component">Selection:</span> Will set the chosen selection layer. Features picked from this layer will be used to 
        search for features in the target layer. Selection features will highlight with the same color as the background of this pull down list.
      </p>
      <p class="selection">
        <span class="component">Filter:</span> Applies a filter to the set of target features picked on the map. Data for 
        features that fulfill the criteria of the filter will be displayed in the grid below this control.
      </p>

      <table class="selection">
        <tbody>
          <tr>
            <td>
              <img src="Images/Help/SelectAllButton.png" alt="select all" /></td>
            <td><span class="component">Select All in View:</span> Select all features from the target or selection layers within the current view of the map.</td>
          </tr>
          <tr>
            <td>
              <img src="Images/Help/ClearSelectedFeatures.png" alt="clear selection" /></td>
            <td><span class="component">Clear Selected Features:</span> Clear the selected sets of target and selection features.</td>
          </tr>
          <tr>
            <td>
              <img src="Images/Help/ZoomToSelection.png" alt="select all" /></td>
            <td><span class="component">Zoom To Selected Features:</span> (on the Map Panel) Zoom to the combined extent of all target and selection features.</td>
          </tr>
        </tbody>
      </table>

      <h2 class="MenuItem sub selection">Using Selected Features</h2>
      <p class="selection">
        If the Selection Function is available in the GPV application instance and a feature has been selected on the map, the capabilities below 
        can be used to show data about those features.
      </p>
      <p class="selection">
        <img src="Images/Help/SelectionDataGrid.png" class="imgRight" alt="selection data grid" />
        <span class="component">Data Grid:</span> This displays data about the selected target features that have passed the filter criteria. 
        The columns of data shown will vary with the target layer and the selected filter. Clicking on a row in this grid will highlight that row, 
        highlight the corresponding feature on the map, and bring up detailed data in the Details Panel on the right side of the UI. 
        Double-clicking will do the same plus zoom to the feature. The total number of rows in the grid is displayed at the lower left of the grid.
      </p>
      <p class="selection">
        <img src="Images/Help/CreateMailingLabels.png" alt="mailing labels" class="imgRight" /><span class="component">To Mailing Labels:</span> When enabled, this allows for the generation of mailing labels for the selected target features. 
        A window will pop up with options for setting label format, printing direction and font characteristics. The labels are delivered as a PDF file which 
        can be printed or saved to disk. It is likely that only certain layers (such as parcels or buildings) will be enabled for mailing labels in 
        the application.
      </p>
      <p class="selection">
        <span class="component">To Spreadsheet:</span> This exports the data for the selected target features currently shown in the data grid to a 
        comma-separated value (CSV) file or an Excel spreadsheet (XLS) depending on the configuration of this application. This may launch a program, 
        such as Microsoft Excel or OpenOffice Calc, for viewing the spreadsheet. If not, a prompt will appear to save the file to disk. 
        NOTE: Later versions of Excel may display a notice regarding the XLS file format. This can be safely ignored and the file can be opened.
      </p>
      <p class="selection">
        <span class="component">Details Panel:</span> The Panel opens from the left side of the UI and shows detailed data from the 
        feature highlighted in the selection data grid. Depending on the application, one or more data set options will be available in the 
        dropdown list for different types of data. Some of the data shown in the panel may be displayed as links to other pages (which will 
        popup in another window) or which change the target layer, selection layer, and selected features. It is also possible to have small 
        images within the listed data. These may be links to other pages or documents. Next to the data set dropdown is a button that allows for printing
        the data shown in the Details Panel.
      </p>

      <%--Markup Panel--%>
      <h2 class="MenuItem markup" id="markup">Creating and Managing Map Markup</h2>

      <p class="markup">
        <img src="Images/Help/MarkupPanelOptions.png" alt="markup panel options" class="imgRight" />
        Drawn shapes and text (markup) can be added to the map for others to 
        see. Everyone has the ability to edit and delete the markup created by any users. Given the wide-open nature of this environment, 
        organizations may set guidelines for the proper treatment of markup. Please contact your administrator for details.
      </p>

      <p class="markup">
        <span class="component">Your Name:</span> A name or identifier must be provided to create markup. This 
        name will appear in the grid below for all markup created.
      </p>
      <p class="markup">
        <span class="component">Category:</span> Markup is placed in categories for ease of management. Select an appropriate 
        category from this list before creating new markup. Upon selecting a category the grid will update to show all available 
        markup groups in that category.
      </p>
      <p class="markup">
        <span class="component">Markup Group:</span> Shapes and text that created are stored in named groups. 
        The following functions help manage these markup groups.
      </p>
      <ul class="bullet-list markup">
        <li><span class="component">New:</span> Creates a new, untitled markup group in the selected category assigned to your name. Once clicked the tools relating to markup in the Map
          Tools list above are available to create the markup graphics.</li>
        <li><span class="component">Zoom To:</span> Zooms the map to the extent of all the shapes and text in the current markup group. Use this return to the markup group if the map 
          extent has changed.</li>
        <li><span class="component">Delete:</span> Deletes the current markup group. The markup will be removed from the map and grid.</li>
        <li><span class="component">To KML:</span> Exports the current markup group to a file which can be displayed in Google Earth.</li>
      </ul>
      <p class="markup">
        <span class="component">Title:</span> The title of the current markup group. This defaults to [untitled] for new markup groups. 
        Change this to a short phase which accurately describes the markup group.
      </p>
      <p class="markup">
        <img src="Images/Help/MarkupColorSelector.png" alt="color selector" class="imgRight" />
        <span class="component">Style:</span> Style colors can be applied separately to both new markup and new text glow color by clicking on the 
        respective buttons in the Markup Panel. A color Selection Panel will display the hue, saturation and value of the current color. Move the sliders to change 
        any of these color properties then click OK to set this as the new color.
      </p>

      <%--Share Panel--%>
      <h2 class="MenuItem share" id="share">Sharing Maps</h2>

      <p class="share">
        <img src="Images/Help/PrintPanel.png" alt="print map" class="imgLeft" /><span class="component">Print:</span> Displays a utility for 
        creating a PDF version of the current map suitable for printing or archiving. Options can be set to select page layout and 
        preservation of either the scale or the width of the current map. Depending on the configuration, options may be enabled to provide text, 
        such as a title and/or notes, which will appear in specific locations on the printable page.
      </p>

      <p class="share">
        <span class="component">Email Link:</span> Provides a link to the current map page.  You can copy this link and paste it into a messaging
        application to share it with others.  All the information about the page - the current map tab, zoom level, selected object, and markup - 
        are encrypted in this link.  When the link is clicked, the GPV will reappear in its current state.
      </p>

      <p class="share">
        <span class="component">Go To:</span> Presents a pull down list of other web-based map viewers.  Select one then click Go to see the
        the current map area in that viewer.
      </p>

      <p class="share">
        <span class="component">Export:</span> Shows a pull down list with image format options that allow for saving of the current map view to a 
        file. Select "as Image" to save the map as a PNG or JPEG image file. Select "as KML" to save the map in a format that can be viewed in 
        Google Earth. Click Save Map to download and save the file.
      </p>

      <%--Other Capabilities--%>
      <h2 class="MenuItem" id="other">Other Capabilities</h2>
      <p>
        <span class="component">Mobile:</span> When the application is launched from a mobile phone, a responsive mobile version is presented. 
        This mobile version has a limited subset of features; users of the mobile version can only perform basic map navigation, identify key features, 
        change the map theme, and zoom to their current location.
      </p>
    </div>
</body>
</html>
