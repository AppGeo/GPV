using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Markup
{
  public string Shape = null;
  public string Text = null;
  public string Color = null;
  public string Glow = null;
  public int? Measured = null;

  public Markup() { }

	public Markup(string shape, string color)
	{
    Shape = shape;
    Color = color;
	}

  public Markup(string shape, string color, int? measured)
    : this(shape, color)
  {
    Measured = measured;
  }

  public Markup(string shape, string text, string color, string glow, int? measured)
    : this(shape, color, measured)
  {
    Text = text;
    Glow = glow;
  }
}