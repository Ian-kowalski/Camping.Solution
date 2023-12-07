using camping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Street : Location
{
    public Street(int locationID, int areaID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply)
        : base(locationID, atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        LocationID = locationID;
        AreaID = areaID;
    }

    public int AreaID { get; set; }

    public bool Visible = false;
}
