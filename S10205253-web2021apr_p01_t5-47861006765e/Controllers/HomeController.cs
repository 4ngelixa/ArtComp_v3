using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web_Asg.DAL;
using Web_Asg.Models;

namespace Web_Asg.Controllers
{

	public class HomeController : Controller
	{
		private JudgeDAL judgeContext = new JudgeDAL();
		private CompetitorDAL compContext = new CompetitorDAL();
		private AreaInterestDAL aiContext = new AreaInterestDAL();
		private CompetitionDAL competitionContext = new CompetitionDAL();

		private CompetitorDAL competitorContext = new CompetitorDAL();
		private List<int> IDList = new List<int>();

		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			if (HttpContext.Session.GetString("Role") == "Competitor")
            {
				return RedirectToAction("MemberMain");
			}
			else if (HttpContext.Session.GetString("Role") == "Admin")
			{
				return RedirectToAction("AdminMain");
			}
			else if (HttpContext.Session.GetString("Role") == "Judge")
			{
				return RedirectToAction("JudgeMain");
			}
			else
            {
				return View();
            }
		}

		static Competitor searchCompetitor(List<Competitor> list, string email)
		{
			foreach (Competitor c in list)
			{
				if (c.EmailAddr.ToLower() == email.ToLower())
				{
					return c;
				}
			}
			return null;
		}

		static Judge searchJudge(List<Judge> list, string email)
		{
			foreach (Judge j in list)
			{
				if (j.EmailAddr.ToLower() == email.ToLower())
				{
					return j;
				}
			}
			return null;
		}

		public ActionResult StaffLogin(IFormCollection formData, string guestSelector)
		{

			List<Competitor> competitorList = competitorContext.GetAllComps();
			List<Judge> judgeList = judgeContext.GetAllJudges();
			// Read inputs from textboxes
			// Email address converted to lowercase
			string loginID = formData["txtLoginID"].ToString().ToLower();
			string password = formData["txtPassword"].ToString();

			Competitor vLogin = searchCompetitor(competitorList, loginID);
			Judge jLogin = searchJudge(judgeList, loginID);

			// COMPETITOR LOGIN
			if (vLogin != null && password == vLogin.Password)
			{
				HttpContext.Session.SetString("LoginID", loginID);
				HttpContext.Session.SetString("Role", "Competitor");
				HttpContext.Session.SetInt32("CompetitorID", vLogin.CompetitorID);
				HttpContext.Session.SetString("Name", vLogin.Salutation + ". " + vLogin.CompetitorName);
				return RedirectToAction("MemberMain");
			}
			// GUEST LOGIN
			else if (loginID == "" && guestSelector == "guestLogin")
			{
				HttpContext.Session.SetString("LoginID", "Guest");
				// Store user role “Guest” as a string in session with the key “Role”
				HttpContext.Session.SetString("Role", "Guest");

				return RedirectToAction("GuestMain");
			}
			//Admin login
			else if (loginID == "admin1@lcu.edu.sg" && password == "p@55Admin")
			{
				HttpContext.Session.SetString("LoginID", loginID);
				// Store user role “Admin” as a string in session with the key “Role”
				HttpContext.Session.SetString("Role", "Admin");

				return RedirectToAction("AdminMain");
			}
			// Judge login
			else if (jLogin != null && password == jLogin.Password)
			{
				HttpContext.Session.SetString("LoginID", loginID);
				HttpContext.Session.SetString("Role", "Judge");
				HttpContext.Session.SetString("Name", jLogin.JudgeName);
				HttpContext.Session.SetString("Salutation", jLogin.Salutation);
				return RedirectToAction("JudgeMain");
			}
			// DO WRONGLY #2 - Never key in at all
			else
			{
				TempData["Message"] = "Invalid Login Credentials!";
				return RedirectToAction("Login");
			}
		}

		public ActionResult SignUp(IFormCollection formData)
		{

			string userType = formData["userType"].ToString();

			if (userType == "0") //competitor
			{
				return RedirectToAction("SUCompetitor", "UserSignUp");
			}
			else if (userType == "1") //judge
			{
				return RedirectToAction("JudgeSU", "UserSignUp");
			}
			else
			{
				return RedirectToAction("Index");
			}

		}

		public IActionResult MemberMain()
		{
			return View();
		}

		public IActionResult GuestMain()
		{
			return View();
		}
		public IActionResult AdminMain()
		{
			List<Competition> compList = competitionContext.GetAllComps();
			return View(compList);
		}

		public IActionResult JudgeMain()
		{
			return View();
		}

		public IActionResult SUCompetitor()
		{
			List<Competitor> competitorList = competitorContext.GetAllComps();
			ViewData["ExistingUsers"] = competitorList;

			return View();
		}

		public IActionResult SUAdmin()
		{
			return View();
		}

		public IActionResult InProgress()
		{
			return View();
		}

		public IActionResult Login()
		{
			List<SelectListItem> li = new List<SelectListItem>();
			li.Add(new SelectListItem { Text = "Competitor", Value = "0" });
			li.Add(new SelectListItem { Text = "Judge", Value = "1" });

			ViewData["userTypesList"] = li;
			return View();
		}

		public ActionResult LogOut()
		{

			DateTime startTime = Convert.ToDateTime(HttpContext.Session.GetString("LoggedInTime"));
			DateTime endTime = DateTime.Now;
			TimeSpan loginDuration = endTime - startTime;

			string strLoginDuration = "";
			if (loginDuration.Days > 0)
			{
				strLoginDuration += loginDuration.Days.ToString() + " day(s) ";
				HttpContext.Session.SetString("LoginDuration", strLoginDuration);
			}

			if (loginDuration.Hours > 0)
			{
				strLoginDuration += loginDuration.Hours.ToString() + " hour(s) ";
				HttpContext.Session.SetString("LoginDuration", strLoginDuration);
			}

			if (loginDuration.Minutes > 0)
			{
				strLoginDuration += loginDuration.Minutes.ToString() + " minute(s)";
				HttpContext.Session.SetString("LoginDuration", strLoginDuration);
			}

			if (loginDuration.Seconds > 0)
			{
				strLoginDuration += loginDuration.Minutes.ToString() + " seconds(s)";
				HttpContext.Session.SetString("LoginDuration", strLoginDuration);
			}

			// Clear all key-values pairs stored in session state
			HttpContext.Session.Clear();
			// Call the Index action of Home controller
			return RedirectToAction("Index");
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
