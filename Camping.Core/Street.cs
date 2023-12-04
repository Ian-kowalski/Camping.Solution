using camping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Street : Location
{
    public Street(int streetID, int areaID, bool outletPresent, bool atWater, bool petsAllowed, bool hasShadow, bool hasWaterSupply)
        : base(atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        StreetID = streetID;
        AreaID = areaID;
    }

    public int StreetID { get; set; }
    public int AreaID { get; set; }
    public bool Inherits {  get; set; }

    public bool Visible = false;
}
