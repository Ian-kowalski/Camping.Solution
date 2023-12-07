public class Reservation
{
    public Reservation(int reservationID, DateTime startDate, DateTime endDate, Visitor visitor, int site)
    {
        ReservationID = reservationID;
        StartDate = startDate;
        Visitor = visitor;
        EndDate = endDate;
        SiteID = site;
    }

    public int ReservationID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Visitor Visitor { get; set; }
    public int AmountPeople { get; set; }
    public int SiteID { get; set; }


    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Reservation objAsPart = obj as Reservation;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }

    public override int GetHashCode()
    {
        return ReservationID;
    }

    public bool Equals(Reservation other)
    {
        if (other == null) return false;
        return (this.ReservationID.Equals(other.ReservationID));
    }

}

