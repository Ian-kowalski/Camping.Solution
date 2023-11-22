using camping.Database;

SiteData siteData = new SiteData();

foreach (var s in siteData.GetSiteInfo())
{
    Console.WriteLine($"{s.Number} {s.HasShawdow} {s.HasWaterSupply}");
}