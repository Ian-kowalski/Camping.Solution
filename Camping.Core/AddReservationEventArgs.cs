namespace camping.Core
{
    public class AddReservationEventArgs : EventArgs
    {
        public int CampSiteID;
        public DateTime StartDate;
        public DateTime EndDate;
        public AddReservationEventArgs(int campsiteID, DateTime startDate, DateTime endDate)
        {
            CampSiteID = campsiteID;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
