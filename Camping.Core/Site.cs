using camping.Core;

public class Site : Location
{
    public Site(int locationID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply, int size, int streetID, CoordinatesPairs cp)
        : base(locationID, atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        LocationID = locationID;
        Size = size;
        StreetID = streetID;
        CoordinatesPairs = cp;
    }

    public Site(int locationID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply, int size, int streetID)
    : base(locationID, atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        LocationID = locationID;
        Size = size;
        StreetID = streetID;
    }

    public CoordinatesPairs CoordinatesPairs { get; set; }

    public int Size { get; set; }
    public bool HasResevation { get; set; }
    public int StreetID { get; set; }

    public bool Visible = false;
}
