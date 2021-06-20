using System.ComponentModel.DataAnnotations;

namespace StudyBuddy.Admin
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string EMail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}