using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Core
{
    public class ReservationDates
    {
        public DateTime startDate {  get; set; }
        public DateTime endDate { get; set; }

        public ReservationDates(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }
    }
}
