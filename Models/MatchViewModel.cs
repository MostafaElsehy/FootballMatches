using System.ComponentModel.DataAnnotations;

namespace FootballMatches.Models
{
    public class MatchViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الفريق المحلي مطلوب")]
        [Display(Name = "الفريق المحلي")]
        public string? HomeTeam { get; set; }

        [Required(ErrorMessage = "اسم الفريق الضيف مطلوب")]
        [Display(Name = "الفريق الضيف")]
        public string? AwayTeam { get; set; }

        [Display(Name = "شعار الفريق المحلي")]
        public string? HomeTeamLogo { get; set; }

        [Display(Name = "شعار الفريق الضيف")]
        public string? AwayTeamLogo { get; set; }

        [Required(ErrorMessage = "موعد المباراة مطلوب")]
        [Display(Name = "موعد المباراة")]
        [DataType(DataType.DateTime)]
        public DateTime MatchTime { get; set; }

        [Required(ErrorMessage = "اسم البطولة مطلوب")]
        [Display(Name = "البطولة")]
        public string? Competition { get; set; }

        [Required(ErrorMessage = "الدولة مطلوبة")]
        [Display(Name = "الدولة")]
        public string? Country { get; set; }

        [Display(Name = "القناة الناقلة")]
        public string? Channel { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
} 