internal class Reservation
{
    public DateTime startDate { get; set; }
    public int StayDuration { get; set; }
    public Visitor Visitor { get; set; }
    public int AmountPeople { get; set; }
    public Site Site { get; set; }

}

