public class Reservation
{
    public Reservation(int reservationID, DateTime startDate, DateTime endDate, Visitor visitor)
    {
        ReservationID = reservationID;
        StartDate = startDate;
        Guest = visitor;
        EndDate = endDate;
    }
    public int ReservationID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Visitor Guest { get; set; }
    public Site Site { get; set; }

}

