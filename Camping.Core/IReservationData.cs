﻿using System;
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
        public List<Reservation> GetReservationInfo(DateTime dateTime);
        public bool addReservation(int campSiteID, string startDate, string endDate, string firstName, string preposition, string lastName, string adress, string city, string postalcode, int houseNumber, int phoneNumber);
        public bool addReservationLine(int campSiteID, int reservationID);
        public int getReservationID(int visitorID, string startDate, string endDate);
        public bool GetAvailableReservation(int campSite, string startDate, string endDate);
        public bool GetOtherAvailableReservation(int campSite, string startDate, string endDate, int reservationID);
        public bool UpdateReservation(int reservationID, DateTime startDate, DateTime endDate, int campSiteID);
        public bool UpdateReservationLines(int campSiteID, int reservationID);
        public bool UpdateVisitor(int visitorID, string firstName, string lastName, string preposition, string adress, string city, string postalcode, int houseNumber, int phoneNumber);
        public bool DeleteReservation(int reservationID);
    }
}