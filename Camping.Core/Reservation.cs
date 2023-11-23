public class Reservation
{
    public Reservation(int reservationID, DateTime startDate, Visitor visitor, DateTime endDate)
    {
        ReservationID = reservationID;
        StartDate = startDate;
        Guest = visitor;
        EndDate = endDate;
    }
    public int ReservationID { get; set; } = 0;
    public DateTime StartDate { get; set; } = DateTime.MinValue;
    public DateTime EndDate { get; set; }=DateTime.MinValue;
    public Visitor Guest { get; set; }
    public int AmountPeople { get; set; } = 0;
    public Site Site { get; set; } 

}

