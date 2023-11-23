using camping.Database;
using camping.Core;

SiteData siteData = new SiteData();

/*
foreach (var s in siteData.GetSiteInfo())
{
    Console.WriteLine($"{s.Number} {s.HasShawdow} {s.HasWaterSupply}");
}
*/


ReservationData reservationData = new ReservationData();

/*foreach (var r in reservationData.GetReservationInfo())
{
    Console.WriteLine($"{r.StartDate} {r.EndDate}");
}*/
RetrieveData res = new(siteData, reservationData);

foreach (int r in res.CheckDate())
{
    Console.WriteLine(r);
}