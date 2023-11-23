using camping.Database;

SiteData siteData = new SiteData();

foreach (var s in siteData.GetSiteInfo())
{
    Console.WriteLine($"{s.Number} {s.HasShawdow} {s.HasWaterSupply}");
}
*/

ReservationReposetori reservationData = new ReservationReposetori();

foreach (var r in reservationData.GetReservationInfo())
{
    Console.WriteLine($"{r.StartDate} {r.EndDate}");
}
