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
using camping.Core;

public class Area : Location
{
    public Area(int areaID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply)
        : base(atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        AreaID = areaID;
    }

    public int AreaID { get; set; }

}

