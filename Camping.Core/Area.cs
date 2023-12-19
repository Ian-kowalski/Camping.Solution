using camping.Core;
using System.Drawing;

public class Area : Location
{
    public string AreaColor { get; set; }
    public Area(int locationID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply, string color)
        : base(locationID, atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        LocationID = locationID;
        AreaColor = color;
    }
    public Area(int locationID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply)
    : base(locationID, atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        LocationID = locationID;
    }

}

