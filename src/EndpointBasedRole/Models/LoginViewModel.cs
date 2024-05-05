using System.ComponentModel.DataAnnotations;

namespace EndpointBasedRole.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        public string Password { get; set; }
    }
}
