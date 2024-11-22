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
            var claims = _context.Claims.ToList();

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
    }

}