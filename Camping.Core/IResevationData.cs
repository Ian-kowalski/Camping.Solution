using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Core
{
    public interface IReservationData
    {
        public List<Reservation> GetReservationInfo();
        public List<Reservation> GetReservationInfo(DateTime dateTime);

        public int GetReservationID(int visitorID, string startDate, string endDate);

        public int GetCampSiteID(int reservationID);

        public void AddReservation(int campSiteID, string startDate, string endDate, string firstName, string preposition, string lastName, string adress, string city, string postalcode, int houseNumber, int phoneNumber);

        public void AddReservationLine(int campSiteID, int reservationID);
    }
}
