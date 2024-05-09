using System.ComponentModel.DataAnnotations;
namespace EBanking.Views.Register;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Потребителското име е задължително.")]
    [RegularExpression("^[a-zA-Z0-9]{4,16}$", ErrorMessage = "Потребителското име трябва да съдържа само латински букви и цифри и да бъде между 4 и 16 символа.")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Паролата е задължителна.")]
    [MinLength(8, ErrorMessage = "Паролата трябва да бъде поне 8 символа.")]
    [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$", ErrorMessage = "Паролата трябва да съдържа поне една буква и поне една цифра.")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Повторната парола е задължителна.")]
    [Compare("Password", ErrorMessage = "Паролата и повторната парола не съвпадат.")]
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Името е задължително.")]
    public required string FullName { get; set; }

    [Required(ErrorMessage = "Имейл адресът е задължителен.")]
    [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
    public required string Email { get; set; }
}