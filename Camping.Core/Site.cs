using camping.Core;

public class Site : Location
{
    public Site(int campSiteID, bool outletPresent, bool atWater, bool petsAllowed, bool hasShadow, bool hasWaterSupply, int size, int streetID)
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
    public bool Inherits {  get; set; }

    public bool Visible = false;
}
