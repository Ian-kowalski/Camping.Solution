namespace camping.Core
{
    public class ChangeReservationEventArgs : EventArgs
    {
        public Reservation Reservation { get; set; }
        public ChangeReservationEventArgs(Reservation reservation)
        {
            Reservation = reservation;
        }
    }
}
