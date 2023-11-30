using camping.Database;

SiteData siteData = new SiteData();

/*
foreach (var s in siteData.GetSiteInfo())
{
    Console.WriteLine($"{s.Number} {s.HasShawdow} {s.HasWaterSupply}");
}
*/


/*ReservationData reservationData = new ReservationData();

foreach (var r in reservationData.GetReservationInfo())
{
    Console.WriteLine($"{r.StartDate} {r.EndDate}");
}
*/

/*
DateTime d =  DateTime.Now;

Console.WriteLine(d.ToString("MM-dd-yyyy"));
*/

ReservationData reservationData = new ReservationData();

Console.WriteLine(reservationData.UpdateReservation(4, Convert.ToDateTime("15-12-2023"), 6, Convert.ToDateTime("27-11-2023")));