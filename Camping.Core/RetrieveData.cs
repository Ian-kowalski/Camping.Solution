using Camping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Core
{
    public class RetrieveData
    {
        private ISiteData siteData;
        private IReservationData reservationData;

        private List<Site> sites;
        private List<Reservation> reservations;

        public RetrieveData(ISiteData siteData, IReservationData reservationData)
        {
            this.siteData = siteData;
            this.reservationData = reservationData;
            reservations = reservationData.GetReservationInfo();
            sites = siteData.GetSiteInfo();
        }

        public List<int> GetCampSiteID()
        {
            List<int> list = new();
            foreach(Site s in sites)
            {
                list.Add(s.CampSiteID);
            }
            return list;
        }

        public List<int> GetSurfaceArea()
        {
            List<int> list = new();
            foreach (Site s in sites)
            {
                list.Add(s.Size);
            }
            return list;
        }

        public void UpdateReservation(int reservationID, DateTime startDate, Visitor visitor, DateTime endDate)
        {
            reservationData.UpdateReservation(reservationID, startDate, visitor.VisitorID, endDate);
            reservationData.UpdateVisitor(visitor.VisitorID, visitor.FirstName, visitor.LastName, visitor.Preposition, visitor.Adress, visitor.City, visitor.PostalCode, visitor.HouseNumber, visitor.PhoneNumber);
        }

        public List<int> CheckDate()
        {
            List<int> list = new();
            foreach (var r in reservations)
            {
                if (r.StartDate <= DateTime.Today && DateTime.Today <= r.EndDate)
                {
                    foreach (int i in siteData.GetCampSiteID(r.ReservationID))
                        list.Add(i);
                }
            }
            return list;
        }

        public List<bool> GetCurrentAvailability(List<int> unavailableList)
        {
            List<bool> list = new();

            foreach(int i in GetCampSiteID())
            {
                list.Add(true);
            }
            foreach(int i in unavailableList)
            {
                list[i - 1] = false;
            }
            return list;
        }
    }
}
