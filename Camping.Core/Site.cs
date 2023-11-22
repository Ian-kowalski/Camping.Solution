public class Site
{
    public Site(int number, bool outletPresent, bool atWater, bool petsAllowed, bool hasShadow, bool hasWaterSupply, int surfaceArea)
    {
        Number = number;
        OutletPresent = outletPresent;
        AtWater = atWater;
        PetsAllowed = petsAllowed;
        HasShawdow = hasShadow;
        HasWaterSupply = hasWaterSupply;
        SurfaceArea = surfaceArea;
    }

    public int Number { get; set; }
    public int SurfaceArea { get; set; }
    public bool HasResevation { get; set; }
    public bool AtWater {  get; set; }
    public bool HasShawdow { get; set; }
    public bool HasWaterSupply { get; set; }

    //TODO: add facities

    public bool OutletPresent { get; set; }
    public bool PetsAllowed { get; set; }
    //TODO: aad rest of info
}
