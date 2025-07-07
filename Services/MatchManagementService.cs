using FootballMatches.Models;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace FootballMatches.Services
{
    public class MatchManagementService
    {
        private readonly string _matchesFilePath = "matches.json";
        private readonly string _adminFilePath = "admin.json";
        private List<Match> _matches;
        private AdminUser _adminUser;

        public MatchManagementService()
        {
            _matches = LoadMatches();
            _adminUser = LoadAdminUser();
        }

        // إدارة المستخدم الأدمن
        public AdminUser GetAdminUser()
        {
            return _adminUser;
        }

        public bool ValidateAdmin(string? username, string? password)
        {
            return _adminUser.Username == username && _adminUser.Password == HashPassword(password) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
        }

        public void UpdateAdminPassword(string? newPassword)
        {
            _adminUser.Password = HashPassword(newPassword);
            SaveAdminUser();
        }

        private string HashPassword(string? password)
        {
            if (string.IsNullOrEmpty(password)) return "";
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // إدارة المباريات
        public List<Match> GetAllMatches()
        {
            return _matches.OrderByDescending(m => m.MatchTime).ToList();
        }

        public List<Match> GetActiveMatches()
        {
            return _matches.Where(m => m.MatchTime >= DateTime.Today).OrderBy(m => m.MatchTime).ToList();
        }

        public Match? GetMatchById(int id)
        {
            return _matches.FirstOrDefault(m => m.Id == id);
        }

        public void AddMatch(Match match)
        {
            match.Id = _matches.Count > 0 ? _matches.Max(m => m.Id) + 1 : 1;
            _matches.Add(match);
            SaveMatches();
        }

        public void UpdateMatch(Match match)
        {
            var existingMatch = _matches.FirstOrDefault(m => m.Id == match.Id);
            if (existingMatch != null)
            {
                existingMatch.HomeTeam = match.HomeTeam;
                existingMatch.AwayTeam = match.AwayTeam;
                existingMatch.HomeTeamLogo = match.HomeTeamLogo;
                existingMatch.AwayTeamLogo = match.AwayTeamLogo;
                existingMatch.MatchTime = match.MatchTime;
                existingMatch.Competition = match.Competition;
                existingMatch.Country = match.Country;
                existingMatch.Channel = match.Channel;
                SaveMatches();
            }
        }

        public void DeleteMatch(int id)
        {
            var match = _matches.FirstOrDefault(m => m.Id == id);
            if (match != null)
            {
                _matches.Remove(match);
                SaveMatches();
            }
        }

        public List<Match> GetMatchesByDate(DateTime date)
        {
            return _matches.Where(m => m.MatchTime.Date == date.Date).OrderBy(m => m.MatchTime).ToList();
        }

        // حفظ وتحميل البيانات
        private List<Match> LoadMatches()
        {
            try
            {
                if (File.Exists(_matchesFilePath))
                {
                    var json = File.ReadAllText(_matchesFilePath);
                    return JsonSerializer.Deserialize<List<Match>>(json) ?? new List<Match>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading matches: {ex.Message}");
            }
            return new List<Match>();
        }

        private void SaveMatches()
        {
            try
            {
                var json = JsonSerializer.Serialize(_matches, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_matchesFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving matches: {ex.Message}");
            }
        }

        private AdminUser LoadAdminUser()
        {
            try
            {
                if (File.Exists(_adminFilePath))
                {
                    var json = File.ReadAllText(_adminFilePath);
                    return JsonSerializer.Deserialize<AdminUser>(json) ?? CreateDefaultAdmin();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading admin user: {ex.Message}");
            }
            return CreateDefaultAdmin();
        }

        private void SaveAdminUser()
        {
            try
            {
                var json = JsonSerializer.Serialize(_adminUser, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_adminFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving admin user: {ex.Message}");
            }
        }

        private AdminUser CreateDefaultAdmin()
        {
            return new AdminUser
            {
                Username = "admin",
                Password = HashPassword("admin123"),
                Name = "مدير النظام",
                Email = "admin@footballmatches.com"
            };
        }
    }
} 