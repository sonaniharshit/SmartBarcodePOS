using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.ViewModels;

public class ReceiptSettingViewModel
{
    [Required]
    [Display(Name = "Receipt Footer Message")]
    public string FooterMessage { get; set; } = "Thank you, visit again!";

    [Required]
    [Display(Name = "Print Size")]
    public string PrintSize { get; set; } = "80mm";
}
