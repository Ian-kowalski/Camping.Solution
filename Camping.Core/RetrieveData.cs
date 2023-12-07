using Camping.Core;
using System.Windows;


namespace camping.Core
{
    public class RetrieveData
    {
        private ISiteData siteData;
        private IReservationData reservationData;

        public List<Site> Sites;
        public List<Street> Streets;
        public List<Area> Areas;
        public List<Reservation> Reservations;

        public RetrieveData(ISiteData siteData, IReservationData reservationData)
        {
            this.siteData = siteData;
            this.reservationData = reservationData;

            UpdateLocations();

            UpdateReservations();
        }

        public Area GetAreaFromID(int id) {
            foreach (Area area in Areas) {
                if (area.LocationID == id) {
                    return area;
                }
            }
            throw new Exception("No area has been found with the provided ID!");
        }
        public Street GetStreetFromID(int id)
        {
            foreach (Street street in Streets)
            {
                if (street.LocationID == id)
                {
                    return street;
                }
            }
            throw new Exception("No street has been found with the provided ID!");
        }
        public Site GetSiteFromID(int id)
        {
            foreach (Site site in Sites)
            {
                if (site.LocationID == id)
                {
                    return site;
                }
            }
            throw new Exception("No site has been found with the provided ID!");
        }


        public List<int> GetCampSiteID()
        {
            List<int> list = new();
            foreach (Site s in Sites)
            {
                list.Add(s.LocationID);
            }
            return list;
        }

        public List<int> GetSurfaceArea()
        {
            List<int> list = new();
            foreach (Site s in Sites)
            {
                list.Add(s.Size);
            }
            return list;
        }

        public bool GetAvailableReservations(int campSite, string startDate, string endDate)
        {
            return reservationData.GetAvailableReservation(campSite, startDate, endDate);
        }

        public bool GetOtherAvailableReservations(int campSite, string startDate, string endDate, int reservationID)
        {
            return reservationData.GetOtherAvailableReservation(campSite, startDate, endDate, reservationID);
        }

        public List<Reservation> GetReservations(DateTime dateTime)
        {
            return reservationData.GetReservationInfo(dateTime);
        }

/*        public int GetCampSiteID(int reservationID)
        {
            return reservationData.GetCampSiteID(reservationID);
        }*/

        public bool UpdateReservation(int reservationID, DateTime startDate, Visitor visitor, DateTime endDate, int campSiteID)
        {
            return (reservationData.UpdateReservation(reservationID, startDate, endDate, campSiteID) &&
                reservationData.UpdateReservationLines(campSiteID, reservationID) &&
                reservationData.UpdateVisitor(visitor.VisitorID, visitor.FirstName, visitor.LastName, visitor.Preposition, visitor.Adress, visitor.City, visitor.PostalCode, visitor.HouseNumber, visitor.PhoneNumber));
        }

        public bool GetDate(int siteID)
        {
            List<ReservationDates> reservations = siteData.GetAvailability(siteID);
            foreach(ReservationDates dates in reservations)
            {
                if(dates.startDate <= DateTime.Today && dates.endDate >= DateTime.Today)
                {
                    return false;
                }
            }
            return true;
        }

        public bool DeleteReservation(int reservationID)
        {
            return reservationData.DeleteReservation(reservationID);
        }
        public void UpdateLocations()
        {
            Sites = siteData.GetSiteInfo();
            Streets = siteData.GetStreetInfo();
            Areas = siteData.GetAreaInfo();
        }

        public void UpdateReservations()
        {
            Reservations = reservationData.GetReservationInfo();
        }
        public void UpdateReservations(int siteID, String lastname)
        {
            Reservations = reservationData.GetReservationInfo(siteID, lastname);
        }
    }
}
