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
    public class UserSignUpController : Controller
    {
        private AreaInterestDAL areainterestContext = new AreaInterestDAL();
        private JudgeDAL judgeContext = new JudgeDAL();
        private CompetitorDAL compContext = new CompetitorDAL();
        private AreaInterestDAL aiContext = new AreaInterestDAL();
        private CompetitionDAL competitionContext = new CompetitionDAL();

        private List<int> IDList = new List<int>();

        private CompetitorDAL competitorContext = new CompetitorDAL();
        public ActionResult Index()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") != null))
            {
                return RedirectToAction("Index", "Home");
            }
            List<AreaInterest> interestList = areainterestContext.GetAllAi();
            return View(interestList);
        }

        public ActionResult JudgeSU()
        {
            // Stop accessing the action if logged in
            if ((HttpContext.Session.GetString("Role") != null))
            {
                return RedirectToAction("Index", "Home");
            }

            // Retrieve Area of Interest lists
            List<AreaInterest> interestList = aiContext.GetAllAi();
            // Create Area of Interest Select List
            List<SelectListItem> selectInterestList = new List<SelectListItem>();

            foreach(AreaInterest interest in interestList)
            {
                selectInterestList.Add(new SelectListItem { Text = interest.Name, Value = interest.AreaInterestID.ToString() });
            }

            Judge judge = new Judge { AreaInterestID = int.Parse(selectInterestList[0].Value) };
            ViewData["selectInterestList"] = selectInterestList;
            return View(judge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JudgeSU(Judge judge)
        {
            // Get Area of Interest for drop-down list in case of the need to return to JudgeSU.cshtml view
            // Retrieve Area of Interest lists
            List<AreaInterest> interestList = aiContext.GetAllAi();
            // Create Area of Interest Select List
            List<SelectListItem> selectInterestList = new List<SelectListItem>();

            foreach (AreaInterest interest in interestList)
            {
                selectInterestList.Add(new SelectListItem { Text = interest.Name, Value = interest.AreaInterestID.ToString() });
            }
            ViewData["selectInterestList"] = selectInterestList;

            if (ModelState.IsValid)
            {
                //Add judge record to database
                judge.JudgeID = judgeContext.Add(judge);
                //Redirect user to Home/JudgeMain view
                return Redirect("Judge/JudgeMain");
            }
            else
            {
                //Input validation fails, return to the JudgeSU view to display error message
                return View(judge);
            }

        }

        // Trying
        public IActionResult SUAdmin()
        {
            return View();
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

        public IActionResult SUCompetitor()
        {
            List<Competitor> competitorList = competitorContext.GetAllComps();
            ViewData["ExistingUsers"] = competitorList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SUFillCompetitor(IFormCollection formData)
        {

            string SUemail = formData["SUemail"].ToString().ToLower();
            List<Competitor> competitorList = competitorContext.GetAllComps();
            foreach (Competitor c in competitorList)
            {
                IDList.Add(c.CompetitorID);
            }

            Competitor x = searchCompetitor(competitorList, SUemail);


            if (x != null)
            {
                TempData["Message"] = SUemail + " already has an existing account";
                return RedirectToAction("SUCompetitor");
            }
            else if (formData["SUName"].ToString() == "")
            {
                TempData["Message"] = "Please enter a proper name";
                return RedirectToAction("SUCompetitor");
            }
            else if (formData["pass"].ToString() != formData["cPass"].ToString())
            {
                TempData["Message"] = "Both Passwords do not match";
                return RedirectToAction("SUCompetitor");
            }
            else
            {

                Competitor newCompetitor = new Competitor
                {
                    CompetitorName = formData["SUName"].ToString(),
                    Salutation = formData["SUsal"].ToString(),
                    EmailAddr = formData["SUemail"].ToString(),
                    Password = formData["cPass"].ToString(),
                };

                if (ModelState.IsValid)
                {
                    //Add staff record to database
                    newCompetitor.CompetitorID = compContext.Create(newCompetitor);
                    //Redirect user to Staff/Index view
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Input validation fails, return to the Create view
                    //to display error message
                    return RedirectToAction("SUCompetitor");
                }

            }

        }
    }
}
