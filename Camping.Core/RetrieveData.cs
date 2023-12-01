﻿using Camping.Core;
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

        public RetrieveData(ISiteData siteData, IReservationData reservationData)
        {
            this.siteData = siteData;
            this.reservationData = reservationData;

            Sites = siteData.GetSiteInfo();
            Streets = siteData.GetStreetInfo();
            Areas = siteData.GetAreaInfo();
        }

        public List<int> GetCampSiteID()
        {
            List<int> list = new();
            foreach (Site s in Sites)
            {
                list.Add(s.CampSiteID);
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

        public List<Reservation> GetReservations() 
        {
            return reservationData.GetReservationInfo();
        }


        public List<Reservation> GetReservations(DateTime dateTime)
        {
            return reservationData.GetReservationInfo(dateTime);
        }

/*        public int GetCampSiteID(int reservationID)
        {
            return reservationData.GetCampSiteID(reservationID);
        }*/

        public bool UpdateReservation(int reservationID, DateTime startDate, Visitor visitor, DateTime endDate)
        {
            return (reservationData.UpdateReservation(reservationID, startDate, visitor.VisitorID, endDate) &&
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

    }
}
