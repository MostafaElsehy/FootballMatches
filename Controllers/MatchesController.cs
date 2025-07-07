using Microsoft.AspNetCore.Mvc;
using FootballMatches.Services;
using FootballMatches.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace FootballMatches.Controllers
{
    public class MatchesController : Controller
    {
        private readonly MatchManagementService _matchService;

        public MatchesController(MatchManagementService matchService)
        {
            _matchService = matchService;
        }

        public IActionResult Index(string? date = null, string? competition = null)
        {
            DateTime targetDate = DateTime.Today;
            if (!string.IsNullOrEmpty(date))
            {
                if (date == "tomorrow") targetDate = DateTime.Today.AddDays(1);
                else if (date == "yesterday") targetDate = DateTime.Today.AddDays(-1);
                else if (date == "today") targetDate = DateTime.Today;
                else if (DateTime.TryParse(date, out var parsed)) targetDate = parsed;
            }

            // الحصول على المباريات من النظام المدير فقط
            var matches = _matchService.GetMatchesByDate(targetDate);
            
            if (!string.IsNullOrEmpty(competition))
            {
                matches = matches.Where(m => m.Competition.Contains(competition)).ToList();
            }

            var competitions = matches.Select(m => m.Competition).Distinct().ToList();
            ViewBag.Competitions = competitions;
            ViewBag.SelectedCompetition = competition;
            ViewBag.SelectedDate = targetDate.ToString("yyyy-MM-dd");
            return View(matches);
        }
    }
} 