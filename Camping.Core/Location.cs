﻿namespace camping.Core
{
    public abstract class Location
    {
        public int AtWater { get; set; }
        public int HasShadow { get; set; }
        public int HasWaterSupply { get; set; }

        public int OutletPresent { get; set; }
        public int PetsAllowed { get; set; }
        public int LocationID { get; set; }


        protected Location(int locationID, int atWater, int hasShadow, int hasWaterSupply, int outletPresent, int petsAllowed)
        {
            AtWater = atWater;
            HasShadow = hasShadow;
            HasWaterSupply = hasWaterSupply;
            OutletPresent = outletPresent;
            PetsAllowed = petsAllowed;
            LocationID = locationID;
        }
    }
}
