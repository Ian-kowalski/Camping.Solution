using Camping.Core;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;


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

            Reservations = reservationData.GetReservationInfo();

            Sites = siteData.GetSiteInfo();
            Streets = siteData.GetStreetInfo();
            Areas = siteData.GetAreaInfo();
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

        public List<Site> GetAvailableReservations(DateTime startDate, DateTime endDate)
        {
            var list = (from site in Sites
                        join reservation in Reservations on site.LocationID equals reservation.SiteID
                        where ((startDate >= reservation.StartDate && endDate <= reservation.EndDate) ||
                        (startDate >= reservation.StartDate && endDate <= reservation.EndDate) ||
                        (startDate <= reservation.StartDate && endDate >= reservation.EndDate)) &&
                        (startDate <= endDate)
            select site).ToList();
            var campList = Sites.Except(list).ToList();

            return campList;
        }

        public bool GetOtherAvailableReservations(int campSite, string startDate, string endDate, int reservationID)
        {
            return reservationData.GetOtherAvailableReservation(campSite, startDate, endDate, reservationID);
        }


        public bool UpdateReservation(int reservationID, DateTime startDate, Visitor visitor, DateTime endDate, int campSiteID)
        {
            Reservation result = (from r in Reservations
                             where r.ReservationID == reservationID
                                  select r).Single();

            result.StartDate = startDate;
            result.EndDate = endDate;
            result.SiteID = campSiteID;
            result.Visitor = visitor;


            return (reservationData.UpdateReservation(reservationID, startDate, endDate, campSiteID) &&
                reservationData.UpdateVisitor(visitor.VisitorID, visitor.FirstName, visitor.LastName, visitor.Preposition, visitor.Adress, visitor.City, visitor.PostalCode, visitor.HouseNumber, visitor.PhoneNumber));
        }

        public bool DeleteReservation(int reservationID)
        {
            Reservations.Remove((from Reservation in Reservations where Reservation.ReservationID == reservationID select Reservation).Single());
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

        

        public void UpdateSites()
        {
            Sites = siteData.GetSiteInfo();
        }
        public void UpdateStreets()
        {
            Streets = siteData.GetStreetInfo();
        }
        public void UpdateAreas()
        {
            Areas = siteData.GetAreaInfo();
        }


        public List<Reservation> UpdateReservationsList()
        {
            return Reservations;
        }
        public List<Reservation> UpdateReservationsList(string lastname)
        {
            return (from reservation in Reservations
                    where reservation.Visitor.LastName.ToLower().Contains(lastname.ToLower())
                    select reservation).ToList();
        }
        public List<Reservation> UpdateReservationsList(int reservationID)
        {
            return (from reservation in Reservations
                    where reservation.ReservationID == reservationID
                    select reservation).ToList();
        }
        public List<Reservation> UpdateReservationslist(int reservationID, string lastname)
        {
            return UpdateReservationsList(reservationID).Intersect(UpdateReservationsList(lastname)).ToList();
        }

        public bool addReservation(int campSiteID, string startDate, string endDate, string firstName, string preposition, string lastName, string adress, string city, string postalcode, string houseNumber, int phoneNumber)
        {
            CultureInfo provider = new CultureInfo("en-US");
            int visitorID = reservationData.addReservation(campSiteID, startDate, endDate, firstName, preposition, lastName, adress, city, postalcode, houseNumber, phoneNumber);
            Reservations.Add(new Reservation(reservationData.getReservationID(visitorID, startDate, endDate), DateTime.ParseExact( startDate, "MM-dd-yyyy", provider), DateTime.ParseExact(endDate, "MM-dd-yyyy", provider), new Visitor(visitorID, firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber), campSiteID));
            return visitorID >= 0;
            
        }
    }
}
