public class Visitor
{
    public Visitor(int visitorID, string firstName, string lastName, string preposition, string adress, string city, string postalCode, string houseNumber, int phoneNumber)
    {
        VisitorID = visitorID;
        FirstName = firstName;
        LastName = lastName;
        Preposition = preposition;
        Adress = adress;
        City = city;
        PostalCode = postalCode;
        HouseNumber = houseNumber;
        PhoneNumber = phoneNumber;
    }

    public int VisitorID { get; set; }
    public string FirstName { get; set; }
    public string LastName {  get; set; }
    public string Preposition { get; set; }
    public string Adress { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string HouseNumber { get; set; }
    public int PhoneNumber { get; set; }
}
