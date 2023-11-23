using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Core
{
    public interface IReservationData : INotifyPropertyChanged
    {
        public List<Reservation> GetReservationInfo();
        public bool addReservation(int campSiteID, string startDate, string endDate, string firstName, string preposition, string lastName, string adress, string city, string postalcode, int houseNumber, int phoneNumber);
        public bool addReservationLine(int campSiteID, int reservationID);
        public int getReservationID(int visitorID, string startDate, string endDate);

        public bool GetAvailableReservation(int campSite, string startDate, string endDate);
    }
}