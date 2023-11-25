using camping.Database;

SiteData siteData = new SiteData();

/*
foreach (var s in siteData.GetSiteInfo())
{
    Console.WriteLine($"{s.Number} {s.HasShawdow} {s.HasWaterSupply}");
}
*/


ReservationData reservationData = new ReservationData();

foreach (var r in reservationData.GetReservationInfo())
{
    Console.WriteLine($"{r.StartDate} {r.EndDate}");
}
