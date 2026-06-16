using System.ComponentModel.DataAnnotations;

namespace Database.ViewModels.Common;

public sealed class ErrorViewModel
{
    [Display(Name = "Error")] public string? Error { get; set; }

    [Display(Name = "Description")] public string? ErrorDescription { get; set; }
}