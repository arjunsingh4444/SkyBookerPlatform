namespace SkyBooker.PassengerService.Entities;

public class Passenger : BaseEntity
{
    public int UserId { get; set; } // The user account that saved this passenger
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
}
