using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class BaseMap : System.Web.UI.UserControl
{
    private void AddTiles(Configuration.MapTabRow mapTabRow, AppState appState)
    {
        StringCollection visibleTiles = appState.VisibleTiles[mapTabRow.MapTabID];

        // create the top level legend control for this map tab

        HtmlGenericControl parentLegend = new HtmlGenericControl("div");
        pnlBaseMapScroll.Controls.Add(parentLegend);
        parentLegend.Attributes["data-maptab"] = mapTabRow.MapTabID;
        parentLegend.Attributes["class"] = "LegendTop";
        parentLegend.Style["display"] = mapTabRow.MapTabID == appState.MapTab ? "block" : "none";

        foreach (Configuration.MapTabTileGroupRow mapTabTileGroupRow in mapTabRow.GetMapTabTileGroupRows())
        {
            Configuration.TileGroupRow tileGroupRow = mapTabTileGroupRow.TileGroupRow;

            HtmlGenericControl legendEntry = new HtmlGenericControl("div");
            parentLegend.Controls.Add(legendEntry);
            legendEntry.Attributes["class"] = "LegendEntry";

            HtmlGenericControl legendHeader = new HtmlGenericControl("div");
            legendEntry.Controls.Add(legendHeader);
            legendHeader.Attributes["class"] = "LegendHeader";

            HtmlGenericControl visibility = new HtmlGenericControl("span");
            legendHeader.Controls.Add(visibility);
            visibility.Attributes["class"] = "LegendVisibility";

          
            
            HtmlInputCheckBox checkBox = new HtmlInputCheckBox();
           visibility.Controls.Add(checkBox);
            checkBox.Checked = visibleTiles.Contains(tileGroupRow.TileGroupID);
            checkBox.Attributes["class"] = "LegendCheck";
            checkBox.Attributes["data-tilegroup"] = tileGroupRow.TileGroupID;

            HtmlGenericControl name = new HtmlGenericControl("span");
            legendHeader.Controls.Add(name);
            name.Attributes["class"] = "LegendName";
            name.InnerText = tileGroupRow.DisplayName;


        }
    }

    public void Initialize(Configuration config, AppState appState, Configuration.ApplicationRow application)
    {

        foreach (Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
        {
            Configuration.MapTabRow mapTabRow = appMapTabRow.MapTabRow;
            AddTiles(mapTabRow, appState);
        }
    }

    protected void pnlBaseMaps_onchange(object sender, EventArgs e)
    {

    }

}
