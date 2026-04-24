namespace SkyBooker.PassengerService.Dtos;

public class PassengerResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
