using Microsoft.AspNetCore.Mvc;
using FootballMatches.Models;
using FootballMatches.Services;
using System.Text.Json;

namespace FootballMatches.Controllers
{
    public class AdminController : Controller
    {
        private readonly MatchManagementService _matchService;
        private const string AdminSessionKey = "AdminLoggedIn";

        public AdminController(MatchManagementService matchService)
        {
            _matchService = matchService;
        }

        // صفحة تسجيل الدخول
        public IActionResult Login()
        {
            if (IsAdminLoggedIn())
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_matchService.ValidateAdmin(model.Username ?? "", model.Password ?? ""))
                {
                    HttpContext.Session.SetString(AdminSessionKey, "true");
                    return RedirectToAction("Dashboard");
                }
                ModelState.AddModelError("", "اسم المستخدم أو كلمة المرور غير صحيحة");
            }
            return View(model);
        }

        // لوحة التحكم
        public IActionResult Dashboard()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var matches = _matchService.GetAllMatches();
            ViewBag.TotalMatches = matches.Count;
            ViewBag.TodayMatches = matches.Count(m => m.MatchTime.Date == DateTime.Today);
            ViewBag.UpcomingMatches = matches.Count(m => m.MatchTime > DateTime.Now);

            return View(matches);
        }

        // إدارة المباريات
        public IActionResult Matches()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var matches = _matchService.GetAllMatches();
            return View(matches);
        }

        // إضافة مباراة جديدة
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            return View(new MatchViewModel { MatchTime = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MatchViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var match = new Match
                {
                    HomeTeam = model.HomeTeam,
                    AwayTeam = model.AwayTeam,
                    HomeTeamLogo = model.HomeTeamLogo,
                    AwayTeamLogo = model.AwayTeamLogo,
                    MatchTime = model.MatchTime,
                    Competition = model.Competition,
                    Country = model.Country,
                    Channel = model.Channel
                };

                _matchService.AddMatch(match);
                TempData["Success"] = "تم إضافة المباراة بنجاح";
                return RedirectToAction("Matches");
            }

            return View(model);
        }

        // تعديل مباراة
        public IActionResult Edit(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var match = _matchService.GetMatchById(id);
            if (match == null)
            {
                return NotFound();
            }

            var model = new MatchViewModel
            {
                Id = match.Id,
                HomeTeam = match.HomeTeam,
                AwayTeam = match.AwayTeam,
                HomeTeamLogo = match.HomeTeamLogo,
                AwayTeamLogo = match.AwayTeamLogo,
                MatchTime = match.MatchTime,
                Competition = match.Competition,
                Country = match.Country,
                Channel = match.Channel
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MatchViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var match = new Match
                {
                    Id = model.Id,
                    HomeTeam = model.HomeTeam,
                    AwayTeam = model.AwayTeam,
                    HomeTeamLogo = model.HomeTeamLogo,
                    AwayTeamLogo = model.AwayTeamLogo,
                    MatchTime = model.MatchTime,
                    Competition = model.Competition,
                    Country = model.Country,
                    Channel = model.Channel
                };

                _matchService.UpdateMatch(match);
                TempData["Success"] = "تم تحديث المباراة بنجاح";
                return RedirectToAction("Matches");
            }

            return View(model);
        }

        // حذف مباراة
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            _matchService.DeleteMatch(id);
            TempData["Success"] = "تم حذف المباراة بنجاح";
            return RedirectToAction("Matches");
        }

        // إعدادات الأدمن
        public IActionResult Settings()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var admin = _matchService.GetAdminUser();
            var matches = _matchService.GetAllMatches();
            ViewBag.TotalMatches = matches.Count;
            ViewBag.TodayMatches = matches.Count(m => m.MatchTime.Date == DateTime.Today);
            ViewBag.UpcomingMatches = matches.Count(m => m.MatchTime > DateTime.Now);
            
            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string? currentPassword, string? newPassword)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var admin = _matchService.GetAdminUser();
            if (admin.Password == currentPassword && !string.IsNullOrEmpty(newPassword))
            {
                _matchService.UpdateAdminPassword(newPassword);
                TempData["Success"] = "تم تغيير كلمة المرور بنجاح";
            }
            else
            {
                TempData["Error"] = "كلمة المرور الحالية غير صحيحة";
            }

            return RedirectToAction("Settings");
        }

        // تسجيل الخروج
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(AdminSessionKey);
            return RedirectToAction("Login");
        }

        // التحقق من تسجيل دخول الأدمن
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString(AdminSessionKey) == "true";
        }
    }
} 