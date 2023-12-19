namespace camping.Core
{
    public class AvailableCampsitesEventArgs : EventArgs
    {
        public List<Site> AvailableSites = new List<Site>();
        public AvailableCampsitesEventArgs(List<Site> availableSites)
        {
            AvailableSites = availableSites;
        }
    }
}
