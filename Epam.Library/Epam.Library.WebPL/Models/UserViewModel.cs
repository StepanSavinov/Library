using System.ComponentModel.DataAnnotations;

namespace Epam.Library.WebPL.Models;

public class UserViewModel
{
    [Required] 
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}