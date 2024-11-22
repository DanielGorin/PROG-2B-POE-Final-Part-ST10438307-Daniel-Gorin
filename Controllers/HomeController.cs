using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Data;
using PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Models;
using System.Diagnostics;

namespace PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context; // Database context

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Home page
        public IActionResult Index()
        {
            return View();
        }

        // Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Error handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Admin page: Display list of claims
        public IActionResult Admin()
        {
            // Fetch all claims from the database
            var claims = _context.Claims
                                   .OrderByDescending(c => c.Status == "Pending")
                                   .ThenByDescending(c => c.DateLogged)
                                   .ThenBy(c => c.ClaimantName)
                                   .ToList();

            // Pass the claims list to the Admin view
            return View(new { Claims = claims });
        }

        // Lecturer sign-in page
        public IActionResult LecturerSignIn(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                // Handle missing input data
                ViewBag.ErrorMessage = "First Name and Last Name are required.";
                return View();
            }

            // Store lecturer data in session
            HttpContext.Session.SetString("LecturerFirstName", firstName);
            HttpContext.Session.SetString("LecturerLastName", lastName);

            // Redirect to the Lecturer page
            return RedirectToAction("Lecturer");
        }

        // Lecturer page: Display list of claims for the lecturer
        public IActionResult Lecturer()
        {
            // Retrieve lecturer data from session
            string firstName = HttpContext.Session.GetString("LecturerFirstName");
            string lastName = HttpContext.Session.GetString("LecturerLastName");

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                // Redirect to sign-in page if session data is missing
                return RedirectToAction("LecturerSignIn");
            }

            // Fetch claims for the lecturer
            string fullName = $"{firstName} {lastName}";
            var claims = _context.Claims
                                 .Where(c => c.ClaimantName == fullName)
                                 .OrderByDescending(c => c.Status == "Pending")
                                 .ThenByDescending(c => c.DateLogged)
                                 .ToList();

            return View(claims);
        }
        public IActionResult ViewClaim(int claimId)
        {
            // Fetch the claim from the database by ID
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == claimId);

            if (claim == null)
            {
                // Handle the case where the claim is not found
                return NotFound();
            }

            // Pass the claim to the view
            return View(claim);
        }
        public IActionResult LecturerViewClaim(int claimId)
        {
            // Fetch the claim from the database by ID
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == claimId);

            if (claim == null)
            {
                // Handle the case where the claim is not found
                return NotFound("The selected claim could not be found.");
            }

            // Pass the claim to the view
            return View(claim);
        }
        // Display the Submit Claim form
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            // Retrieve lecturer data from session
            string firstName = HttpContext.Session.GetString("LecturerFirstName");
            string lastName = HttpContext.Session.GetString("LecturerLastName");

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                // Redirect to sign-in page if session data is missing
                return RedirectToAction("LecturerSignIn");
            }

            // Pass data to the view
            ViewBag.FullName = $"{firstName} {lastName}";
            ViewBag.TodayDate = DateTime.Now.ToString("yyyy-MM-dd");

            return View();
        }
        //submition function
        [HttpPost]
        public IActionResult SubmitClaim(decimal HourlyRate, int HoursWorked, string ClaimantComments)
        {
            // Retrieve lecturer's full name from session
            string firstName = HttpContext.Session.GetString("LecturerFirstName");
            string lastName = HttpContext.Session.GetString("LecturerLastName");

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                // Redirect to sign-in page if session data is missing
                return RedirectToAction("LecturerSignIn");
            }

            string fullName = $"{firstName} {lastName}";

            // Create a new claim
            var newClaim = new Claims
            {
                ClaimantName = fullName,
                HourlyRate = HourlyRate,
                HoursWorked = HoursWorked,
                ClaimantComments = ClaimantComments,
                DateLogged = DateTime.Now,
                Status = "Pending" // Default status for new claims
            };

            // Save the new claim to the database
            _context.Claims.Add(newClaim);
            _context.SaveChanges();

            // Redirect back to the Lecturer page
            return RedirectToAction("Lecturer");
        }
        public IActionResult ApproveClaim(int claimId)
        {
            // Fetch the claim by ID
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == claimId);

            if (claim == null)
            {
                return NotFound("Claim not found.");
            }

            // Update the claim's status to "Approved"
            claim.Status = "Approved";
            _context.SaveChanges();

            // Redirect back to the ViewClaim page
            return RedirectToAction("ViewClaim", new { claimId = claimId });
        }

        public IActionResult DenyClaim(int claimId)
        {
            // Fetch the claim by ID
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == claimId);

            if (claim == null)
            {
                return NotFound("Claim not found.");
            }

            // Update the claim's status to "Denied"
            claim.Status = "Denied";
            _context.SaveChanges();

            // Redirect back to the ViewClaim page
            return RedirectToAction("ViewClaim", new { claimId = claimId });
        }
        public IActionResult DeleteClaim(int claimId)
        {
            // Fetch the claim from the database by ID
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == claimId);

            if (claim == null)
            {
                return NotFound("Claim not found.");
            }

            // Remove the claim from the database
            _context.Claims.Remove(claim);
            _context.SaveChanges();

            // Redirect to a page (e.g., Admin or Lecturer)
            return RedirectToAction("Admin"); // Redirect back to Admin page after deletion
        }
        //page used to display information about individual claimants
        public IActionResult HR()
        {
            // Group claims by claimant name
            var hrData = _context.Claims
                .GroupBy(c => c.ClaimantName)
                .Select(group => new
                {
                    FullName = group.Key,
                    EarliestClaim = group.Min(c => c.DateLogged),
                    TotalEarned = group
                        .Where(c => c.Status == "Approved")
                        .Sum(c => c.HourlyRate * c.HoursWorked),
                    TotalPending = group
                        .Where(c => c.Status == "Pending")
                        .Sum(c => c.HourlyRate * c.HoursWorked)
                })
                .ToList();

            // Pass HR data to the view
            return View(hrData);
        }
        //page displaying detailed infromation about a specific claimant
        public IActionResult ViewClaimant(string claimantName)
        {
            if (string.IsNullOrEmpty(claimantName))
            {
                return NotFound("Claimant name is required.");
            }

            // Fetch all claims for the specified claimant
            var claimantClaims = _context.Claims.Where(c => c.ClaimantName == claimantName).ToList();

            if (!claimantClaims.Any())
            {
                return NotFound($"No claims found for claimant: {claimantName}");
            }

            // Calculate details for the claimant
            var totalEarned = claimantClaims.Where(c => c.Status == "Approved")
                                            .Sum(c => c.HourlyRate * c.HoursWorked);

            var totalPending = claimantClaims.Where(c => c.Status == "Pending")
                                             .Sum(c => c.HourlyRate * c.HoursWorked);

            var totalHoursWorked = claimantClaims.Where(c=> c.Status == "Approved")
                                            .Sum(c => c.HoursWorked);
            var averageHourlyRate = totalHoursWorked > 0
                ? claimantClaims.Average(c => c.HourlyRate)
                : 0;

            var acceptedClaims = claimantClaims.Count(c => c.Status == "Approved");
            var deniedClaims = claimantClaims.Count(c => c.Status == "Denied");
            var totalClaims = acceptedClaims+deniedClaims;
            var acceptedPercentage = totalClaims > 0
                ? (acceptedClaims / (double)totalClaims) * 100
                : 0;

            var earliestClaim = claimantClaims.Min(c => c.DateLogged);
            var latestClaim = claimantClaims.Max(c => c.DateLogged);

            // Pass data to the view
            var model = new
            {
                ClaimantName = claimantName,
                TotalEarned = totalEarned,
                TotalPending = totalPending,
                TotalHoursWorked = totalHoursWorked,
                AverageHourlyRate = averageHourlyRate,
                AcceptedPercentage = acceptedPercentage,
                EarliestClaim = earliestClaim,
                LatestClaim = latestClaim
            };

            return View(model);
        }
        //page displaying a lecturers own infromation
        public IActionResult Dashboard()
        {
            // Retrieve lecturer data from session
            string firstName = HttpContext.Session.GetString("LecturerFirstName");
            string lastName = HttpContext.Session.GetString("LecturerLastName");

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return RedirectToAction("LecturerSignIn");
            }

            string fullName = $"{firstName} {lastName}";

            // Fetch all claims for the lecturer
            var lecturerClaims = _context.Claims.Where(c => c.ClaimantName == fullName).ToList();

            if (!lecturerClaims.Any())
            {
                return View(new
                {
                    LecturerName = fullName,
                    TotalEarned = 0,
                    TotalPending = 0,
                    TotalHoursWorked = 0,
                    AverageHourlyRate = 0,
                    AcceptedPercentage = 0,
                    EarliestClaim = (DateTime?)null,
                    LatestClaim = (DateTime?)null
                });
            }

            // Calculate dashboard data
            var totalEarned = lecturerClaims
                .Where(c => c.Status == "Approved")
                .Sum(c => c.HourlyRate * c.HoursWorked);

            var totalPending = lecturerClaims
                .Where(c => c.Status == "Pending")
                .Sum(c => c.HourlyRate * c.HoursWorked);

            var totalHoursWorked = lecturerClaims
                .Where(c => c.Status == "Approved")
                .Sum(c => c.HoursWorked);

            var averageHourlyRate = totalHoursWorked > 0
                ? lecturerClaims.Where(c => c.Status == "Approved").Average(c => c.HourlyRate)
                : 0;

            var acceptedClaims = lecturerClaims.Count(c => c.Status == "Approved");
            var totalClaims = lecturerClaims.Count();
            var acceptedPercentage = totalClaims > 0
                ? (acceptedClaims / (double)totalClaims) * 100
                : 0;

            var earliestClaim = lecturerClaims.Min(c => c.DateLogged);
            var latestClaim = lecturerClaims.Max(c => c.DateLogged);

            // Pass data to the view
            var model = new
            {
                LecturerName = fullName,
                TotalEarned = totalEarned,
                TotalPending = totalPending,
                TotalHoursWorked = totalHoursWorked,
                AverageHourlyRate = averageHourlyRate,
                AcceptedPercentage = acceptedPercentage,
                EarliestClaim = earliestClaim,
                LatestClaim = latestClaim
            };

            return View(model);
        }


    }


}