﻿using camping.Core;
using System.ComponentModel;

namespace Camping.Core
{
    public interface ISiteData : INotifyPropertyChanged
    {
        public List<Site> GetSiteInfo();
        public List<Street> GetStreetInfo();
        public List<Area> GetAreaInfo();

        public List<int> GetCampSiteID(int reservationID);

        public List<ReservationDates> GetAvailability(int siteID);
        public void UpdateFacilities(Location location);
    }
}
