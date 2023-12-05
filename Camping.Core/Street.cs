using camping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Street : Location
{
    public Street(int streetID, int areaID, int outletPresent, int atWater, int petsAllowed, int hasShadow, int hasWaterSupply)
        : base(atWater, hasShadow, hasWaterSupply, outletPresent, petsAllowed)
    {
        StreetID = streetID;
        AreaID = areaID;
    }

    public int StreetID { get; set; }
    public int AreaID { get; set; }
    public bool InheritsHasWaterSupply { get; set; }
    public bool InheritsOutletPresent { get; set; }
    public bool InheritsPetsAllowed { get; set; }
    public bool InheritsHasShadow { get; set; }
    public bool InheritsAtWater { get; set; }

    public bool Visible = false;
}
