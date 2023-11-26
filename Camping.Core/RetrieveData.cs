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

        public RetrieveData(IReservationData reservationData)
        {
            this.reservationData = reservationData;
        }

        public RetrieveData(ISiteData siteData)
        {
            this.siteData = siteData;
        }

        public List<int> GetCampSiteID()
        {
            List<int> list = new();
            foreach(Site s in siteData.GetSiteInfo())
            {
                list.Add(s.Number);
            }
            return list;
        }

        public List<int> GetSurfaceArea()
        {
            List<int> list = new();
            foreach (Site s in siteData.GetSiteInfo())
            {
                list.Add(s.SurfaceArea);
            }
            return list;
        }

        public List<Reservation> GetReservations() 
        {
            return reservationData.GetReservationInfo(); ;
        }


        public List<Reservation> GetReservations(DateTime dateTime)
        {
            return reservationData.GetReservationInfo(dateTime); ;
        }

        public int GetCampSiteID(int reservationID)
        {
            return reservationData.GetCampSiteID(reservationID);
        }
    }
}
