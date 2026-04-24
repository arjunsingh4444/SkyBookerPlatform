using System.ComponentModel.DataAnnotations;

namespace SkyBooker.PassengerService.Dtos;

public class PassengerDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    public string PassportNumber { get; set; } = string.Empty;

    public string Nationality { get; set; } = string.Empty;
}
