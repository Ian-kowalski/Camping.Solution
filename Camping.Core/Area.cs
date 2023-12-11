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
    public Area(int locationID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply)
        : base(locationID, atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        LocationID = locationID;
    }

}

