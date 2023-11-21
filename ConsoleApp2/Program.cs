using camping.Database;

SiteData siteData = new();


foreach (var item in siteData.GetSites("campSiteID"))
{
    Console.WriteLine(item);
}