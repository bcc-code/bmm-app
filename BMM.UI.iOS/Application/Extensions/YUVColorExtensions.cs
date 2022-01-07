using System;
using BMM.UI.iOS.Utils.ColorPalette;

public static class YUVColorExtensions
{
    // Distance between two colors in Y'UV plane
    public static double DistanceTo(this YUVColor color1, YUVColor color2)
    {
        var o1 = Math.Pow (color1.U - color2.U, 2);
        var o2 = Math.Pow (color1.V - color2.V, 2);

        return Math.Sqrt (o1 + o2);
    }

    // Returns color that is at specified distance from specified color in Y'UV plane
    public static YUVColor ColorAtDistanceFrom(this YUVColor color, YUVColor referenceColor, float distance)
    {
        YUVColor result = new YUVColor() {Y = color.Y, U = color.U, V = color.V};

        if(color.Y > referenceColor.Y)
        {
            // Need to increase color.Y value
            result.Y = referenceColor.Y + distance;
        }
        else
        {
            // Need to decrease color.Y value
            result.Y = referenceColor.Y - distance;
        }

        return result;
    }

    // Lighttens color by specified distance
    public static YUVColor LighttenByDistane(this YUVColor color, float distance)
    {
        YUVColor result = (YUVColor)color.Clone();

        result.Y += distance;

        if(result.Y > 1)
        {
            // Y value can't be > 1
            result.Y = 0.85f;
        }

        return result;
    }

    // Darkens color by specified distance
    public static YUVColor DarkenByDistane(this YUVColor color, float distance)
    {
        YUVColor result = (YUVColor)color.Clone();

        result.Y -= distance;

        if(result.Y < 0)
        {
            // Y value can't be < 0
            result.Y = 0.15f;
        }

        return result;
    }
}