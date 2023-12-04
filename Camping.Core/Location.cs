using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Core
{
    public abstract class Location
    {
        public bool AtWater { get; set; }
        public bool HasShadow { get; set; }
        public bool HasWaterSupply { get; set; }

        public bool OutletPresent { get; set; }
        public bool PetsAllowed { get; set; }

        protected Location(bool atWater, bool hasShadow, bool hasWaterSupply, bool outletPresent, bool petsAllowed)
        {
            AtWater = atWater;
            HasShadow = hasShadow;
            HasWaterSupply = hasWaterSupply;
            OutletPresent = outletPresent;
            PetsAllowed = petsAllowed;
        }
    }
}
