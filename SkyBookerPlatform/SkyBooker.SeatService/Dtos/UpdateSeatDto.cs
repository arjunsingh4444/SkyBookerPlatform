using System.ComponentModel.DataAnnotations;

namespace SkyBooker.SeatService.Dtos;

public class UpdateSeatDto
{
    [Required]
    public string SeatClass { get; set; } = string.Empty;
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public string Status { get; set; } = string.Empty;
}
