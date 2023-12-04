public class Visitor
{
    public Visitor(int visitorID, string firstName, string lastName, string preposition, string adress, string city, string postalCode, int houseNumber, int phoneNumber)
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

    public Visitor()
    {
        VisitorID = -1;
        FirstName = "<FirstName>";
        LastName = "<LastName>";
        Preposition = "<Preposition>";
        Adress = "<thisStreet>";
        City = "<thatCity";
        PostalCode = "1234XX";
        HouseNumber = -1;
        PhoneNumber = 1234567;
    }

    public int VisitorID { get; set; }
    public string FirstName { get; set; }
    public string LastName {  get; set; }
    public string Preposition { get; set; }
    public string Adress { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public int HouseNumber { get; set; }
    public int PhoneNumber { get; set; }
}
