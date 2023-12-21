using Camping.Core;


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

        public event EventHandler<EventArgs> SiteDeleted;

        public RetrieveData(ISiteData siteData, IReservationData reservationData)
        {
            this.siteData = siteData;
            this.reservationData = reservationData;

            UpdateLocations();

            UpdateReservations();
        }

        public Area GetAreaFromID(int id)
        {
            foreach (Area area in Areas)
            {
                if (area.LocationID == id)
                {
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


        public bool UpdateReservation(int reservationID, DateTime startDate, Visitor visitor, DateTime endDate, int campSiteID)
        {
            return (reservationData.UpdateReservation(reservationID, startDate, endDate, campSiteID) &&
                reservationData.UpdateReservationLines(campSiteID, reservationID) &&
                reservationData.UpdateVisitor(visitor.VisitorID, visitor.FirstName, visitor.LastName, visitor.Preposition, visitor.Adress, visitor.City, visitor.PostalCode, visitor.HouseNumber, visitor.PhoneNumber));
        }

        public bool GetDate(int siteID)
        {
            List<ReservationDates> reservations = siteData.GetAvailability(siteID);
            foreach (ReservationDates dates in reservations)
            {
                if (dates.startDate <= DateTime.Today && dates.endDate >= DateTime.Today)
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

        public bool HasUpcomingReservations(int campSiteID)
        {
            return reservationData.HasUpcomingReservations(campSiteID, DateTime.Today);
        }

        public bool HasChildren(Street street)
        {
            foreach (var site in Sites)
            {
                if (site.StreetID == street.LocationID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasChildren(Area area)
        {
            foreach (var street in Streets)
            {
                if (street.AreaID == area.LocationID)
                {
                    return true;
                }
            }
            return false;
        }


        public bool DeleteLocation(Site campSiteID)
        {

            // will not delete any campsite if it has any upcoming reservations.
            if (HasUpcomingReservations(campSiteID.LocationID)) { return false; }

            // deletes the reservation before telling the main window to update.
            bool isDeleted = siteData.DeleteCampSite(campSiteID.LocationID);

            // deletes the site from the list of site classes
            Sites.Remove(campSiteID);

            SiteDeleted?.Invoke(this, new EventArgs());

            // return true if a campsite has been deleted.
            return isDeleted;
        }


        public bool DeleteLocation(Street street)
        {
            // will not delete any campsite if it has any upcoming reservations.
            if (HasChildren(street)) { return false; }

            // deletes the reservation before telling the main window to update.
            bool isDeleted = siteData.DeleteStreet(street.LocationID);

            // deletes the site from the list of site classes
            Streets.Remove(street);

            SiteDeleted?.Invoke(this, new EventArgs());

            // return true if a campsite has been deleted.
            return isDeleted;
        }

        public bool DeleteLocation(Area area)
        {
            // will not delete any campsite if it has any upcoming reservations.
            if (HasChildren(area)) { return false; }

            // deletes the reservation before telling the main window to update.
            bool isDeleted = siteData.DeleteArea(area.LocationID);

            // deletes the site from the list of site classes
            Areas.Remove(area);

            SiteDeleted?.Invoke(this, new EventArgs());

            // return true if a campsite has been deleted.
            return isDeleted;
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

        public void UpdateReservations(string lastname)
        {
            
            List<Reservation> sortedlist = new List<Reservation>();
            sortedlist = (from reservation in Reservations
                          where reservation.Visitor.LastName.ToLower().Contains(lastname.ToLower())
                          select reservation).ToList();
            Reservations = sortedlist;
        }
        public void UpdateReservations(int reservationID)
        {
            List<Reservation> sortedlist = new List<Reservation>();
            sortedlist = (from reservation in Reservations
                          where reservation.ReservationID == reservationID
                          select reservation).ToList();
            Reservations = sortedlist;
        }

        public void UpdateReservations(int reservationID, string lastname)
        {
            UpdateReservations(lastname);
            UpdateReservations(reservationID);

        }
    }
}
