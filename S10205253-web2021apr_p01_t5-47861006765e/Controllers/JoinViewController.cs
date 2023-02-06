using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.IO;
using System.Web;

using Web_Asg.DAL;
using Web_Asg.Models;

namespace Web_Asg.Controllers
{
    // TRANSFERRING EVERYTHING FROM COMPETITOR TO JOINVIEW

    public class JoinViewController : Controller
    {
        private CompetitionDAL compContext = new CompetitionDAL();
        private CriteriaDAL criteriaContext = new CriteriaDAL();
        private CompetitionSubmissionDAL compsubContext = new CompetitionSubmissionDAL();

        private List<int> AvailCompID = new List<int>();
        private List<int> AlrJoinedCompID = new List<int> {};
        private List<string> salutationList = new List<string> { "Dr", "Mr", "Ms", "Mrs", "Mdm" };

        private List<SelectListItem> salutationSelect = new List<SelectListItem>();
        private List<int> displayJoinedCompID = new List<int>();
        private List<int> displayCompID = new List<int>();

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") == null)
            {
                HttpContext.Session.SetString("Role", "Guest");
            }
            List<Competition> compList = compContext.GetAllComps();
            List<Criteria> criteriaList = criteriaContext.GetAllCritera();
            List<CompetitionSubmission> compsubList = compsubContext.GetAllSubs();
            List<CompsubCriteriaModel> cscList = new List<CompsubCriteriaModel> {};

            List<CompsubCriteriaModel> joinedCompetitionsList = new List<CompsubCriteriaModel>();

            if (HttpContext.Session.GetString("Role") != "Guest")
            {

                // Adding Available Comps to Available Comps List
                foreach (Competition comp in compList)
                {
                    AvailCompID.Add(comp.CompetitionID);
                }
                var ex = HttpContext.Session.GetInt32("CompetitorID");
                foreach (CompetitionSubmission compr in compsubList)
                {
                    if (compr.CompetitorID == HttpContext.Session.GetInt32("CompetitorID"))
                    {
                        AlrJoinedCompID.Add(compr.CompetitionID);
                    }
                }
                foreach (int i in AlrJoinedCompID)
                {
                    AvailCompID.Remove(i);
                }

                // 1) Find the Amount of CompetitionSubmission Objects for User - tackled
                // 2) Add them into The CompSubCriteriaModel List
                // 2a) Add the list of criterias that corresponds to the competition
                // 2b) Add the list of weightage that corresponds to the competition

                // Adding CompsubCriteria to CompSubCriteria List
                foreach(CompetitionSubmission compr in compsubList)
                {
                    if (compr.CompetitorID == HttpContext.Session.GetInt32("CompetitorID"))
                    {
                        List<string> criterias = MapCriteriaListToComp(compr.CompetitionID);
                        Competition Comp = MapCompIDComp(compr.CompetitionID);

                        CompsubCriteriaModel x = new CompsubCriteriaModel
                        {
                            criterias = string.Join(",", criterias),
                            CompetitionID = compr.CompetitionID,
                            CompetitorID = compr.CompetitorID,
                            CompetitionName = Comp.CompetitionName,
                            FileSubmitted = compr.FileSubmitted,
                            DateTimeFileUpload = compr.DateTimeFileUpload,
                            Appeal = compr.Appeal,
                            VoteCount = compr.VoteCount,
                            Ranking = compr.Ranking,
                            EndDate = Comp.EndDate,
                            ResultReleasedDate = Comp.ResultReleasedDate,
                        };

                        joinedCompetitionsList.Add(x);

                    }
                }

            }
            else if (HttpContext.Session.GetString("Role") == "Guest")
            {
                foreach (Competition comp in compList)
                {
                    AvailCompID.Add(comp.CompetitionID);
                }
            }

            List<Criteria> cList = criteriaContext.GetAllCritera();

            ViewData["AvailCompID"] = AvailCompID;
            ViewData["AlrJoinedCompID"] = joinedCompetitionsList;
            ViewData["userID"] = HttpContext.Session.GetInt32("CompetitorID");
            ViewData["cscList"] = cscList;
            ViewData["Criteria"] = cList;

            return View(compList);
        }


        public List<string> MapCriteriaListToComp(int CompetitionID)
        {
            List<string> returnList = new List<string>();
            List<Criteria> allCsList = criteriaContext.GetAllCritera();
            foreach (Criteria c in allCsList)
            {
                if (c.CompetitionID == CompetitionID)
                {
                    returnList.Add(c.CriteriaName);
                }
            }
            return returnList;
        }

        public List<int> MapWeightageListToComp(int CompetitionID)
        {
            List<int> returnList = new List<int>();
            List<Criteria> allCsList = criteriaContext.GetAllCritera();
            foreach (Criteria c in allCsList)
            {
                if (c.CompetitionID == CompetitionID)
                {
                    returnList.Add(c.Weightage);
                }
            }
            return returnList;
        }

        public Competition MapCompIDComp(int id)
        {
            List<Competition> compList = compContext.GetAllComps();
            foreach (Competition c in compList)
            {
                if (c.CompetitionID == id)
                {
                    return c;
                }
            }
            return null;
        }

        public IActionResult SubmitWork()
        {
            if (HttpContext.Session.GetString("Role") == "Guest")
            {
                return RedirectToAction("Privacy", "Home");
            }
            else
            {
                return RedirectToAction("Submit", "CompetitorSubmit");
            }
        }

        public ActionResult JoinComp(int id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") == "Guest"))
            {
               return RedirectToAction("Privacy", "Home");
            }
            Competition comp = compContext.GetDetails(id);
            List<Criteria> criteriaList = criteriaContext.GetAllCritera();

            ViewData["SelectedComp"] = comp;
            ViewData["CriteriaList"] = criteriaList;
            ViewData["userID"] = HttpContext.Session.GetInt32("CompetitorID");
            ViewData["Error"] = "";

            // StaffViewModel staffVM = MapToStaffVM(staff);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult JoinComp(CompetitionSubmission compSub)
        {
            // COMP SUB IS NULL!!
            List<Competition> compList = compContext.GetAllComps();

            //Get country list for drop-down list
            //in case of the need to return to Create.cshtml view
            if (ModelState.IsValid)
            {
                Competition selectedComp = new Competition();
                foreach (Competition c in compList)
                {
                    if (c.CompetitionID == compSub.CompetitionID)
                    {
                        selectedComp = c;
                    }
                }

                //Add staff record to database
                int cfm = compsubContext.JoinCompeition(compSub);
                if (cfm == compSub.CompetitorID && DateTime.Now <= selectedComp.StartDate.Subtract(TimeSpan.FromDays(3)))
                {
                    return RedirectToAction("Index", "JoinView");
                }
                else
                {
                    ViewData["Error"] = "You are too late! Don't worry! There are many more fun competitions for you to join!";
                    return View(compSub);
                }
            }
            else
            {
                return View(compSub);
            }
        }

    }
}
