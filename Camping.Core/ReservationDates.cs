namespace camping.Core
{
    public class ReservationDates
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public ReservationDates(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }
    }
}
