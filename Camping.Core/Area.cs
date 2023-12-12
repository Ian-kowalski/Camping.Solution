using camping.Core;

public class Area : Location
{
    public Area(int locationID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply)
        : base(locationID, atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        LocationID = locationID;
    }

}

