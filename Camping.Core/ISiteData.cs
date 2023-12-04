using camping.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camping.Core
{
    public interface ISiteData : INotifyPropertyChanged
    {
        public List<Site> GetSiteInfo();
        public List<Street> GetStreetInfo();
        public List<Area> GetAreaInfo();

        public List<int> GetCampSiteID(int reservationID);

        public List<ReservationDates> GetAvailability(int siteID);
    }
}
