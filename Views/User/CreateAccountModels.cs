using System.ComponentModel.DataAnnotations;

namespace EBanking.Views.User
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Полето за наименование на сметката е задължително.")]
        public required string FriendlyName { get; set; }
    }
}