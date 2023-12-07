using camping.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace camping.WPF
{
    public class AddReservation
    {
        public AddReservation() { }

        public event EventHandler<EventArgs> ReservationAdded;
    }
}
