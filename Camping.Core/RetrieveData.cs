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

        public RetrieveData(ISiteData siteData, IReservationData reservationData)
        {
            this.siteData = siteData;
            this.reservationData = reservationData;
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

        public List<int> CheckDate()
        {
            List<int> list = new();
            foreach (var r in reservationData.GetReservationInfo())
            {
                if (r.StartDate < DateTime.Today && DateTime.Today < r.EndDate)
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
