using camping.Core;
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

        public bool DeleteCampSite(int campSiteiD);
        public bool DeleteArea(int areaID);
        public bool DeleteStreet(int streetID);

        public bool AddArea(string color);

        public int AddLocation(Location location, int x1, int y1, int x2, int y2);
        public int addCoordinatesPair(Location location, int x1, int y1, int x2, int y2);
    }
}
