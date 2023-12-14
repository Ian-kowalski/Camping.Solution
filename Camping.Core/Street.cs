using camping.Core;

public class Street : Location
{
    public Street(int locationID, int areaID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply, CoordinatesPairs cp)
        : base(locationID, atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        LocationID = locationID;
        AreaID = areaID;

        CoordinatesPairs = cp;
    }

    public Street(int locationID, int areaID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply)
    : base(locationID, atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        LocationID = locationID;
        AreaID = areaID;
    }

    public CoordinatesPairs CoordinatesPairs { get; set; }

    public int AreaID { get; set; }

    public bool Visible = false;
}
