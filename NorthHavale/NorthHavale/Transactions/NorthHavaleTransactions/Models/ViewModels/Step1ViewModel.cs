using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NorthHavaleTransactions.Models.ViewModels
{
    public class Step1ViewModel
    {
        public string NameSurname { get; set; }
        [Required(ErrorMessage = "Tutar Giriniz")]
        [Range(1, int.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır")]
        public int Amount { get; set; }
        [Required(ErrorMessage = "Banka Seçiniz")]
        public Guid BankId { get; set; }
        public List<SelectListItem> Banks { get; set; }
    }
}
