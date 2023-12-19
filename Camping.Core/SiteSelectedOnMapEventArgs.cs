using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Core
{
    public class SiteSelectedOnMapEventArgs
    {
        public Site Site { get; set; }
        public SiteSelectedOnMapEventArgs(Site site)
        {
            Site = site;
        }
    }
}
