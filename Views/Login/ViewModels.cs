using System.ComponentModel.DataAnnotations;

namespace EBanking.Views.Login
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Полето за потребителско име е задължително.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Полето за парола е задължително.")]
        public required string Password { get; set; }
    }
}