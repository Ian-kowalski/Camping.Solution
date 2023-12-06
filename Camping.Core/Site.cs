using camping.Core;

public class Site : Location
{
    public Site(int campSiteID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply, int size, int streetID)
        : base(atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        CampSiteID = campSiteID;
        Size = size;
        StreetID = streetID;
    }

    public int CampSiteID { get; set; }
    public int Size { get; set; }
    public bool HasResevation { get; set; }
    public int StreetID { get; set; }

    public bool Visible = false;
}
