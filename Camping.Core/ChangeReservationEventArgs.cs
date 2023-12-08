using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
