using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Area
{
    public Area(int areaID, bool outletPresent, bool atWater, bool petsAllowed, bool hasShadow, bool hasWaterSupply)
    {
        AreaID = areaID;
        OutletPresent = outletPresent;
        AtWater = atWater;
        PetsAllowed = petsAllowed;
        HasShadow = hasShadow;
        HasWaterSupply = hasWaterSupply;
    }

    public int AreaID { get; set; }
    public bool HasResevation { get; set; }
    public bool AtWater { get; set; }
    public bool HasShadow { get; set; }
    public bool HasWaterSupply { get; set; }


    public bool OutletPresent { get; set; }
    public bool PetsAllowed { get; set; }
}

