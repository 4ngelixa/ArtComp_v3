using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.IO;
using System.Web;

using Web_Asg.DAL;
using Web_Asg.Models;

namespace Web_Asg.Controllers
{
    // Need to validate these:
    // 1) No showing of total marks and ranking
    // 2) Only one appeal allowed
    // 3) 

    // 1) Get CompScore objects for only the comeptitor and store them in List<CompScore>
    // 2) Seperate by CompetitionID and into different lists
    // - Ex, if user has comp 1 and 2, comp 1 and 2 lists of scores
    // 3) Find the weightage for each Criteria and put them into list?

    // Created Nested List for Criterias of different competitions. - each list inside the list will be criteriaIDs of diff comps
    public class ViewScoreController : Controller
    {
        private CompetitionScoreDAL compScoreContext = new CompetitionScoreDAL();
        private CompetitionDAL compContext = new CompetitionDAL();
        private CriteriaDAL criteriaContext = new CriteriaDAL();
        private CommentDAL cmtContext = new CommentDAL();
        private CompetitionSubmissionDAL compsubContext = new CompetitionSubmissionDAL();

        private List<string> userCompName = new List<string>();

        // curremty using lists
        private List<CompetitionScore> csList = new List<CompetitionScore>();
        private List<List<int>> userCompWeightage = new List<List<int>>();

        // Nested list of diff criterias of diff alrmarked comps
        private List<List<int>> criteriaIDs = new List<List<int>>();
        private List<List<int>> TotalScores = new List<List<int>>();
        private List<int> markedCompIDs = new List<int>();
        private List<string> commentsList = new List<string>();

        public IActionResult Index(int? id)
        {
            List<CompetitionScore> allscoreList = compScoreContext.GetAllScore();
            List<Competition> allcomplist = compContext.GetAllComps();
            List<Criteria> allcriteriaList = criteriaContext.GetAllCritera();

            if (HttpContext.Session.GetString("Role") == "Guest")
            {
                return RedirectToAction("Privacy", "Home");
            }

            // Find the CompetitionScore objects that belong to user
            foreach (CompetitionScore cs in allscoreList)
            {
                if (cs.CompetitorID == HttpContext.Session.GetInt32("CompetitorID"))
                {
                    csList.Add(cs);
                }
            }

            // Find the Competitions the competitions that have been marked
            foreach (CompetitionScore cs in csList)
            {
                bool alrExist = CompAlreadyExists(cs.CompetitionID, markedCompIDs);
                if (markedCompIDs.Count == 0)
                {
                    markedCompIDs.Add(cs.CompetitionID);
                }
                else if (alrExist != true)
                {
                    markedCompIDs.Add(cs.CompetitionID);
                }
                else if (alrExist == true)
                {
                    continue;
                }
            }

            // Nested list to add criteria of different competitions
            // Add Criteria ID to CriteriaID List

            foreach (int compID in markedCompIDs)
            {
                List<int> CompetitionCumCritera = getCompCriterias(compID);
                List<int> CompetitionCumWeightage = getCompWeightage(compID);
                List<int> CompetitionCumScore = getCompIBScores(compID);

                criteriaIDs.Add(CompetitionCumCritera);
                userCompWeightage.Add(CompetitionCumWeightage);
                TotalScores.Add(CompetitionCumScore);
            }

            List<int> totalScore = CalculateTotalScores(TotalScores, userCompWeightage);

            List<Comment> cmtList = cmtContext.GetComments();

            foreach (Comment com in cmtList)
            {
                foreach (int compID in markedCompIDs)
                {
                    if (com.CompetitionID == compID)
                    { commentsList.Add(com.Description); }
                }
            }

            // By here, we have,
            // nested list of criteriaIDs
            // nested list 


            ViewData["totalScores"] = totalScore;
            ViewData["CompNames"] = getCompNames(markedCompIDs);
            ViewData["Comments"] = commentsList;
            ViewData["markedComps"] = markedCompIDs;
            ViewData["DatesList"] = mapCompToEndResultsDate(markedCompIDs);


            if (id != null)
            {
                ViewData["BDCriterias"] = MapCriteriaListToComp(Convert.ToInt32(id));
                ViewData["BDScores"] = TotalScores[markedCompIDs.IndexOf(Convert.ToInt32(id))];
                ViewData["BDWeightages"] = getCompWeightage(Convert.ToInt32(id));
                ViewData["BDCompName"] = getCompNames(markedCompIDs)[markedCompIDs.IndexOf(Convert.ToInt32(id))];
            }
            else
            {
                ViewData["BDCriteria"] = new List<int>();
                ViewData["BDScores"] = new List<int>();
                ViewData["BDWeightages"] = new List<int>();
                ViewData["BDCompName"] = "";
            }

            return View();
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

        public List<List<DateTime>> mapCompToEndResultsDate(List<int> compIDs)
        {
            // 0th item in list is end date
            // 1st item in list in results date
            List<List<DateTime>> datesList = new List<List<DateTime>>();
            List<Competition> allComps = compContext.GetAllComps();

            foreach (Competition c in allComps)
            {
                foreach (int compID in compIDs)
                {
                    List<DateTime> innerDateList = new List<DateTime>();
                    if (c.CompetitionID == compID)
                    {
                        innerDateList.Add(c.EndDate);
                        innerDateList.Add(c.ResultReleasedDate);
                    }
                    datesList.Add(innerDateList);
                }
            }

            return datesList;
        }

        public List<string> getCompNames(List<int> compIDs)
        {
            List<string> compNames = new List<string>();
            List<Competition> allComps = compContext.GetAllComps();

            foreach (Competition c in allComps)
            {
                foreach(int compID in compIDs)
                {
                    if (c.CompetitionID == compID)
                    {
                        compNames.Add(c.CompetitionName);
                    }
                }
            }
            return compNames;
        }

        // CAN POSSIBLY CHANGE THiS TO STRING SO FOR 'VIEW MARKS BREAKDOWN'
        public List<int> getCompCriterias(int reqcompID)
        {
            List<int> criteriaForThatComp = new List<int>();
            foreach (CompetitionScore cs in csList)
            {
                if (cs.CompetitionID == reqcompID)
                {
                    // CompAlreadyExists - even tho not passing in compID, function takes list<int> and an int, hence applicable
                    bool alrExist = CompAlreadyExists(cs.CriteriaID, criteriaForThatComp);
                    if (criteriaForThatComp.Count == 0)
                    {
                        criteriaForThatComp.Add(cs.CriteriaID);
                    }
                    else if (alrExist != true)
                    {
                        criteriaForThatComp.Add(cs.CriteriaID);
                    }
                    else if (alrExist == true)
                    {
                        continue;
                    }
                }
            }
            return criteriaForThatComp;
        }

        public List<int> getCompWeightage(int reqcompID)
        {
            List<Criteria> allcriteriaList = criteriaContext.GetAllCritera();
            List<int> weightageForThatComp = new List<int>();
            foreach (Criteria c in allcriteriaList)
            {
                if (c.CompetitionID == reqcompID)
                {
                    if (c.CompetitionID == reqcompID)
                    {
                        weightageForThatComp.Add(c.Weightage);
                    }
                }
            }
            return weightageForThatComp;
        }

        public List<int> getCompIBScores(int reqcompID)
        {
            List<CompetitionScore> allscoresList = compScoreContext.GetAllScore();
            List<int> compIBScore = new List<int>();
            foreach (CompetitionScore c in allscoresList)
            {
                if (c.CompetitionID == reqcompID && c.CompetitorID == HttpContext.Session.GetInt32("CompetitorID"))
                {
                    // CompAlreadyExists - even tho not passing in compID, function takes list<int> and an int, hence applicable
                    compIBScore.Add(c.Score);
                }
            }
            return compIBScore;
        }

        public bool CompAlreadyExists(int c, List<int> CompIDs)
        {
            foreach (int comp in CompIDs)
            {
                if (comp == c)
                {
                    // 'C' Already exists in COMPIDs List
                    return true; 
                }
            }
            // THERE IS NO 'C' in CompIDs List
            return false;
        }

        // List that maps Weightage to Criteria IDs
        // return to main program to use for calculation
        public List<int> MapCriteriaToWeightage(List<int> criteriaIDs)
        {
            List<int> weightage = new List<int>();
            List<Criteria> allcriteriaList = criteriaContext.GetAllCritera();
            foreach (Criteria c in allcriteriaList)
            {
                foreach (int uc in criteriaIDs)
                {
                    if (c.CriteriaID == uc)
                    {
                        weightage.Add(c.Weightage);
                    }
                }
            }
            return weightage;
        }

        // Function asks for scoreList and weightageList
        // multiplies score and weightage
        // Adds it to 'TotalScore' and returns it back

        public List<int> CalculateTotalScores(List<List<int>> scoreList, List<List<int>> weightageList)
        {
            List<int> totalScores = new List<int>();
            for (int i = 0; i < scoreList.Count; i++)
            {
                int singleCompScore = CalculateComp(scoreList[i], weightageList[i]);
                if (singleCompScore >= 0)
                {
                    totalScores.Add(singleCompScore);
                }
            }
            return totalScores;
        }


        public int CalculateComp(List<int> scoreList, List<int> weightageList)
        {
            int a = 0;
            if (scoreList.Count != weightageList.Count)
            {
                return -1;
            }
            else
            {
                for (int i = 0; i < scoreList.Count; i++)
                {
                    a += scoreList[i] * weightageList[i];
                }
                return a;
            }
        }

        public ActionResult Appeal(int id)
        {
            CompetitionSubmission selectedCS = new CompetitionSubmission();
            if (HttpContext.Session.GetString("Role") == "Guest")
            {
                return RedirectToAction("Privacy", "Home");
            }

            foreach (CompetitionSubmission cs in compsubContext.GetAllSubs())
            {
                if (cs.CompetitionID == id && cs.CompetitorID == HttpContext.Session.GetInt32("CompetitorID"))
                {
                    selectedCS = cs;
                }
            }

            foreach (Competition comp in compContext.GetAllComps())
            {
                if (comp.CompetitionID == id)
                {
                    ViewData["CompName"] = comp.CompetitionName;
                }
            }

            return View(selectedCS);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Appeal(CompetitionSubmission cs)
        {
            string AppealMsg = cs.Appeal;
            int compID = cs.CompetitionID;
            int userID = cs.CompetitorID;

            bool appealDone = compsubContext.AppealMessage(AppealMsg, compID, userID);

            if (appealDone == true)
            {
                ViewData["Message"] = "Appeal Message Sent";
            }
            else
            {
                ViewData["Message"] = "Error. Unable to Send";
            }

            return View(cs);
        }

    }
}
