using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Core
{
    public class StreetSelectedOnMapEventArgs
    {
        public Street Street { get; set; }
        public StreetSelectedOnMapEventArgs(Street street)
        {
            Street = street;
        }
    }
}
