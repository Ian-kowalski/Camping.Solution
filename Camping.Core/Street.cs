using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Street
{
    public Street(int streetID, bool outletPresent, bool atWater, bool petsAllowed, bool hasShadow, bool hasWaterSupply, int areaID)
    {
        StreetID = streetID;
        OutletPresent = outletPresent;
        AtWater = atWater;
        PetsAllowed = petsAllowed;
        HasShadow = hasShadow;
        HasWaterSupply = hasWaterSupply;
        AreaID = areaID;
    }

    public int StreetID { get; set; }
    public int AreaID { get; set; }
    public bool HasResevation { get; set; }
    public bool AtWater { get; set; }
    public bool HasShadow { get; set; }
    public bool HasWaterSupply { get; set; }


    public bool OutletPresent { get; set; }
    public bool PetsAllowed { get; set; }
}
