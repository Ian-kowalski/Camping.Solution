public class Reservation
{
    public Reservation(int reservationID, DateTime startDate, DateTime endDate, Visitor visitor, int site)
    {
        ReservationID = reservationID;
        StartDate = startDate;
        Guest = visitor;
        EndDate = endDate;
        SiteID = site;
    }
    public int ReservationID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Visitor Guest { get; set; }
    public int AmountPeople { get; set; }
    public int SiteID { get; set; }

}

