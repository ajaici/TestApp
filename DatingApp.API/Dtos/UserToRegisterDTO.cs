using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserToRegisterDTO
    {
        [Required]
        [StringLength(8,MinimumLength=4, ErrorMessage="You must specify the username between 4 and 8 characters")]
        public string Username { get; set; }    

        [Required]
        [StringLength(8,MinimumLength=4, ErrorMessage="You must specify the password between 4 and 8 characters")]
        public string Password { get; set; }    
    }
}