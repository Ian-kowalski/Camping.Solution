using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Core
{
    public abstract class Location
    {
        public int AtWater { get; set; }
        public int HasShadow { get; set; }
        public int HasWaterSupply { get; set; }

        public int OutletPresent { get; set; }
        public int PetsAllowed { get; set; }


        protected Location(int atWater, int hasShadow, int hasWaterSupply, int outletPresent, int petsAllowed)
        {
            AtWater = atWater;
            HasShadow = hasShadow;
            HasWaterSupply = hasWaterSupply;
            OutletPresent = outletPresent;
            PetsAllowed = petsAllowed;
        }
    }
}
