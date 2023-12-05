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
    public bool InheritsHasWaterSupply {  get; set; }
    public bool InheritsOutletPresent { get; set; }
    public bool InheritsPetsAllowed { get; set; }
    public bool InheritsHasShadow { get; set; }
    public bool InheritsAtWater { get; set; }

    public bool Visible = false;
}
