namespace camping.Core
{
    public class AvailableCampsitesEventArgs : EventArgs
    {
        public List<Site> AvailableSites = new List<Site>();
        public List<Street> Availablestreets = new List<Street>();
        public AvailableCampsitesEventArgs(List<Site> availableSites, List<Street> availablestreets)
        {
            AvailableSites = availableSites;
            Availablestreets = availablestreets;
        }
    }
}
