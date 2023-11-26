public class Site
{
    public Site(int campSiteID, bool outletPresent, bool atWater, bool petsAllowed, bool hasShadow, bool hasWaterSupply, int size)
    {
        CampSiteID = campSiteID;
        OutletPresent = outletPresent;
        AtWater = atWater;
        PetsAllowed = petsAllowed;
        HasShadow = hasShadow;
        HasWaterSupply = hasWaterSupply;
        Size = size;
    }

    public int CampSiteID { get; set; }
    public int Size { get; set; }
    public bool HasResevation { get; set; }
    public bool AtWater {  get; set; }
    public bool HasShadow { get; set; }
    public bool HasWaterSupply { get; set; }


    public bool OutletPresent { get; set; }
    public bool PetsAllowed { get; set; }
}
