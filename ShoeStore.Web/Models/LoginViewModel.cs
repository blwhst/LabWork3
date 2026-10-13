using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите логин")]
    [Display(Name = "Логин")]
    public string Login { get; set; } = string.Empty;

    public string? Error { get; set; }
}