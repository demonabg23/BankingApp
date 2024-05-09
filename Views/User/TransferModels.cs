using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EBanking.Views.User
{
    public class TransferViewModel
    {
        public IEnumerable<SelectListItem> AccountsSelectList { get; set; }

        [Display(Name = "От сметка")]
        public int SendingAccountId { get; set; }

        [Display(Name = "Към сметка")]
        public int ReceiverAccountId { get; set; }

        [Display(Name = "Стойност")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Стойността трябва да бъде по-голяма от 0")]
        public decimal Amount { get; set; }
    }
}