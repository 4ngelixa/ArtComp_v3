using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Asg.DAL;
using Web_Asg.Models;

namespace Web_Asg.Controllers
{
    public class JudgeController : Controller
    {
        private CriteriaDAL criteriaContext = new CriteriaDAL();
        private CompetitionDAL competitionContext = new CompetitionDAL();
        private CompetitionJudgeDAL competitionJudgeContext = new CompetitionJudgeDAL();
        private JudgeDAL judgeContext = new JudgeDAL();
        private CompetitionSubmissionDAL competitionSubContext = new CompetitionSubmissionDAL();
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private CompetitionScoreDAL competitionScoreContext = new CompetitionScoreDAL();

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult ViewCompetition(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Judge"))
            {
                return RedirectToAction("Index", "Home");
            }

            Judge judge = judgeContext.GetJudge(HttpContext.Session.GetString("LoginID"));
            List<CompetitionJudge> compJudge = competitionJudgeContext.GetCompID(judge.JudgeID);
            List<Competition> judgeCompDetails = competitionContext.GetJudgeCompDetails(compJudge);

            CompetitionCriteriaViewModel competitionCriteriaVM = new CompetitionCriteriaViewModel();

            List<Competition> competitionDone = new List<Competition>();
            List<Competition> competitionNotDone = new List<Competition>();

            // 2 Lists of competitions, with criteria done and not done
            foreach (Competition competition in judgeCompDetails)
            {
                int weightage = criteriaContext.GetTotalWeightage(competition.CompetitionID);
                if (weightage == 100)
                {
                    competitionDone.Add(competition);
                }
                else
                {
                    competitionNotDone.Add(competition);
                }            
            }

            competitionCriteriaVM.competitionDoneList = competitionDone;
            competitionCriteriaVM.competitionNotDoneList = competitionNotDone;

            // Check if CompetitionID presents in the query string
            if (id != null)
            {
                ViewData["selectedCompetitionID"] = id.Value;
                // Get list of criteria created in the competition
                competitionCriteriaVM.criteriaList = criteriaContext.GetCriteria(id.Value);

                // Check if weightage = 100, to see if there needs to be a create criteria button
                int totalweightage = 0;
                foreach (Criteria criteria in competitionCriteriaVM.criteriaList)
                {
                    totalweightage += criteria.Weightage;
                }

                ViewData["ShowButton"] = totalweightage;
            }
            else
            {
                ViewData["selectedCompetitionID"] = "";
            }
            return View(competitionCriteriaVM);
        }

        public ActionResult CreateCriteria(int? id)
        {
            ViewData["selectedCompetitionID"] = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCriteria(Criteria criteria,int? id)
        {
            if(ModelState.IsValid)
            {
                //Add judge record to database
                criteria.CriteriaID = criteriaContext.Add(criteria);
                //Redirect user to Judge/ViewCompetition view
                // To get viewmodel object again
                Judge judge = judgeContext.GetJudge(HttpContext.Session.GetString("LoginID"));
                List<CompetitionJudge> compJudge = competitionJudgeContext.GetCompID(judge.JudgeID);
                List<Competition> judgeCompDetails = competitionContext.GetJudgeCompDetails(compJudge);

                CompetitionCriteriaViewModel competitionCriteriaVM = new CompetitionCriteriaViewModel();

                List<Competition> competitionDone = new List<Competition>();
                List<Competition> competitionNotDone = new List<Competition>();

                // 2 Lists of competitions, with criteria done and not done
                foreach (Competition competition in judgeCompDetails)
                {
                    int weightage = criteriaContext.GetTotalWeightage(competition.CompetitionID);
                    if (weightage == 100)
                    {
                        competitionDone.Add(competition);
                    }
                    else
                    {
                        competitionNotDone.Add(competition);
                    }
                }

                competitionCriteriaVM.competitionDoneList = competitionDone;
                competitionCriteriaVM.competitionNotDoneList = competitionNotDone;
                // Check if CompetitionID presents in the query string
                if (id != null)
                {
                    ViewData["selectedCompetitionID"] = id.Value;
                    // Get list of criteria created in the competition
                    competitionCriteriaVM.criteriaList = criteriaContext.GetCriteria(id.Value);

                    // Check if weightage = 100, to see if there needs to be a create criteria button
                    int totalweightage = 0;
                    foreach (Criteria c in competitionCriteriaVM.criteriaList)
                    {
                        totalweightage += c.Weightage;
                    }

                    ViewData["ShowButton"] = totalweightage;
                }
                else
                {
                    ViewData["selectedCompetitionID"] = "";
                }
                return View("ViewCompetition", competitionCriteriaVM);
            }
            else
            {
                ViewData["selectedCompetitionID"] = id;
                //Input validation fails, return to the CreateCriteria view to display error message
                return View(criteria);
            }
        }

        public ActionResult ViewSubmissionList(int id)
        {
            ViewData["selectedCompetitionID"] = id;
            List<CompetitionSubmission> competitionSubmissionList = competitionSubContext.GetAllSubmissionsByComp(id);
            List<Competitor> competitorList = competitorContext.GetAllComps();
            List<Competitor> competitorsInCompetition = new List<Competitor>();

            ViewSubmissionListViewModel viewSubmissionListVM = new ViewSubmissionListViewModel();

            foreach(CompetitionSubmission submission in competitionSubmissionList)
            {
                foreach(Competitor competitor in competitorList)
                {
                    if(submission.CompetitorID == competitor.CompetitorID)
                    {
                        competitorsInCompetition.Add(competitor);
                    }
                }
            }

            Competition competition = competitionContext.GetDetails(id);

            viewSubmissionListVM.CompetitionID = id;
            viewSubmissionListVM.CompetitionName = competition.CompetitionName;
            viewSubmissionListVM.competitorList = competitorsInCompetition;
            return View(viewSubmissionListVM);
        }

        public ActionResult ViewSubmission(int competitorid, int competitionid)
        {
            ViewSubmissionViewModel viewSubmissionVM = new ViewSubmissionViewModel();
            Competition competition = competitionContext.GetDetails(competitionid);
            Competitor competitor = competitorContext.GetDetails(competitorid);
            CompetitionSubmission competitionSub = competitionSubContext.GetDetails(competitorid, competitionid);
            List<CompetitionScore> scoreList = competitionScoreContext.GetScoresOfCompetitor(competitorid, competitionid);
            viewSubmissionVM.criteriaList = criteriaContext.GetCriteria(competitionid);
            // To show value in textbox/to determine if there should be update/create button
            List<String> displayScoreList = new List<String>();

            string viewDate = "0";
            if (DateTime.Now < competition.ResultReleasedDate)
            {
                viewDate = "100";
            }

            ViewData["viewDate"] = viewDate;
            for (int i = 0; i < criteriaContext.GetCriteria(competitionid).Count(); i++)
            {
                // Default score if score is not found
                string inputscore = "";
                // If there is a score for the criteria
                foreach (CompetitionScore score in scoreList)
                {
                    if (criteriaContext.GetCriteria(competitionid)[i].CriteriaID == score.CriteriaID)
                    {
                        inputscore = score.Score.ToString();
                    }
                }
                displayScoreList.Add(inputscore);
            }

            List<SelectListItem> selectCriteriaList = new List<SelectListItem>();
            foreach (Criteria criteria in viewSubmissionVM.criteriaList)
            {
                selectCriteriaList.Add(new SelectListItem { Text = criteria.CriteriaName, Value = criteria.CriteriaID.ToString() });
            }
            ViewData["selectCriteriaList"] = selectCriteriaList;

            viewSubmissionVM.CompetitionID = competition.CompetitionID;
            viewSubmissionVM.CompetitionName = competition.CompetitionName;
            viewSubmissionVM.CompetitorID = competitorid;
            viewSubmissionVM.CompetitorName = competitor.CompetitorName;
            viewSubmissionVM.fileShown = competitionSub.FileSubmitted;
            viewSubmissionVM.scoreList = displayScoreList;

            // Check if weightage = 100, to see if there needs to be a create criteria button
            int totalweightage = 0;
            foreach (Criteria criteria in viewSubmissionVM.criteriaList)
            {
                totalweightage += criteria.Weightage;
            }
            ViewData["ShowButton"] = totalweightage;
            ViewData["selectedCompetitionID"] = competition.CompetitionID;
            ViewData["releaseDate"] = competition.ResultReleasedDate;
            return View(viewSubmissionVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewSubmission(ViewSubmissionViewModel viewSubmissionVM, int competitorid, int competitionid)
        {
            if (ModelState.IsValid)
            {
                ViewSubmissionViewModel newViewSubmissionVM = new ViewSubmissionViewModel();
                Competition competition = competitionContext.GetDetails(competitionid);
                Competitor competitor = competitorContext.GetDetails(competitorid);
                CompetitionSubmission competitionSub = competitionSubContext.GetDetails(competitorid, competitionid);
                List<CompetitionScore> scoreList = competitionScoreContext.GetScoresOfCompetitor(competitorid, competitionid);
                newViewSubmissionVM.criteriaList = criteriaContext.GetCriteria(competitionid);
                // To show value in textbox/to determine if there should be update/create button
                List<String> displayScoreList = new List<String>();
                string viewDate = "0";
                if (DateTime.Now < competition.ResultReleasedDate)
                {
                    viewDate = "100";
                }

                ViewData["viewDate"] = viewDate;

                for (int i = 0; i < criteriaContext.GetCriteria(competitionid).Count(); i++)
                {
                    // Default score if score is not found
                    string inputscore = "";
                    // If there is a score for the criteria
                    foreach (CompetitionScore score in scoreList)
                    {
                        if (criteriaContext.GetCriteria(competitionid)[i].CriteriaID == score.CriteriaID)
                        {
                            inputscore = score.Score.ToString();
                        }
                    }
                    displayScoreList.Add(inputscore);
                }
                List<Int32> criteriaIdList = new List<Int32>();
                List<SelectListItem> selectCriteriaList = new List<SelectListItem>();
                foreach (Criteria criteria in newViewSubmissionVM.criteriaList)
                {
                    criteriaIdList.Add(criteria.CriteriaID);
                    selectCriteriaList.Add(new SelectListItem { Text = criteria.CriteriaName, Value = criteria.CriteriaID.ToString() });
                }
                ViewData["selectCriteriaList"] = selectCriteriaList;             
           
                int index = criteriaIdList.IndexOf(viewSubmissionVM.CriteriaID);
                displayScoreList[index] = viewSubmissionVM.Score.ToString();
                viewSubmissionVM.scoreList = displayScoreList;
                viewSubmissionVM.criteriaList = criteriaContext.GetCriteria(competitionid);

                // Check if weightage = 100, to see if there needs to be a create criteria button
                int totalweightage = 0;
                foreach (Criteria criteria in newViewSubmissionVM.criteriaList)
                {
                    totalweightage += criteria.Weightage;
                }
                ViewData["ShowButton"] = totalweightage;

                CompetitionScore competitionScore = new CompetitionScore();
                competitionScore.CriteriaID = viewSubmissionVM.CriteriaID;
                competitionScore.CompetitorID = viewSubmissionVM.CompetitorID;
                competitionScore.CompetitionID = viewSubmissionVM.CompetitionID;
                competitionScore.Score = viewSubmissionVM.Score;

                if (competitionScoreContext.IsUnique(competitionid, competitorid, viewSubmissionVM.CriteriaID) == true)
                {
                    // Create score
                    competitionScoreContext.AddCompetitionScore(competitionScore);
                    ViewData["SuccessMessage"] = "Score has been created";
                }
                else
                {
                    // Update score
                    competitionScoreContext.UpdateCompetitionScore(competitionScore);
                    ViewData["SuccessMessage"] = "Score has been updated";
                }

                return View(viewSubmissionVM);
            }
            else
            {
                Competition competition = competitionContext.GetDetails(competitionid);
                Competitor competitor = competitorContext.GetDetails(competitorid);
                CompetitionSubmission competitionSub = competitionSubContext.GetDetails(competitorid, competitionid);
                List<CompetitionScore> scoreList = competitionScoreContext.GetScoresOfCompetitor(competitorid, competitionid);
                viewSubmissionVM.criteriaList = criteriaContext.GetCriteria(competitionid);
                // To show value in textbox/to determine if there should be update/create button
                List<String> displayScoreList = new List<String>();

                List<SelectListItem> selectCriteriaList = new List<SelectListItem>();
                foreach (Criteria criteria in viewSubmissionVM.criteriaList)
                {
                    selectCriteriaList.Add(new SelectListItem { Text = criteria.CriteriaName, Value = criteria.CriteriaID.ToString() });
                }
                ViewData["selectCriteriaList"] = selectCriteriaList;

                for (int i = 0; i < criteriaContext.GetCriteria(competitionid).Count(); i++)
                {
                    // Default score if score is not found
                    string inputscore = "";
                    // If there is a score for the criteria
                    foreach (CompetitionScore score in scoreList)
                    {
                        if (criteriaContext.GetCriteria(competitionid)[i].CriteriaID == score.CriteriaID)
                        {
                            inputscore = score.Score.ToString();
                        }
                    }
                    displayScoreList.Add(inputscore);
                }

                viewSubmissionVM.CompetitionID = competition.CompetitionID;
                viewSubmissionVM.CompetitionName = competition.CompetitionName;
                viewSubmissionVM.CompetitorID = competitorid;
                viewSubmissionVM.CompetitorName = competitor.CompetitorName;
                viewSubmissionVM.fileShown = competitionSub.FileSubmitted;
                viewSubmissionVM.scoreList = displayScoreList;
                viewSubmissionVM.criteriaList = criteriaContext.GetCriteria(competitionid);

                // Check if weightage = 100, to see if there needs to be a create criteria button
                int totalweightage = 0;
                foreach (Criteria criteria in viewSubmissionVM.criteriaList)
                {
                    totalweightage += criteria.Weightage;
                }
                ViewData["ShowButton"] = totalweightage;
                ViewData["selectedCompetitionID"] = competition.CompetitionID;
                ViewData["SuccessMessage"] = "Error";

                return View(viewSubmissionVM);
            }
        }

        public ActionResult ViewDetails(int competitorid, int competitionid)
        {
            CompetitionSubmission competitionSub = competitionSubContext.GetDetails(competitorid, competitionid);
            List<Competitor> competitorsInCompetition = new List<Competitor>();
            List<Criteria> criteriaList = criteriaContext.GetCriteria(competitionid);
            Competition competition = competitionContext.GetDetails(competitionid);
            Competitor competitor = competitorContext.GetDetails(competitorid);

            ViewScoreDetailsViewModel viewScoreDetailsVM = new ViewScoreDetailsViewModel();

            List<double> competitionScores = new List<double>();

            double judgeScore = 0;
            double votescore = 0;
            List<CompetitionScore> judgeScoreList = competitionScoreContext.GetScoresOfCompetitor(competitor.CompetitorID, competitionid);
            if (judgeScoreList.Count() == criteriaList.Count)
            {
                for (int k = 0; k < judgeScoreList.Count(); k++)
                {
                    judgeScore += judgeScoreList[k].Score * (criteriaList[k].Weightage * 0.1);
                }
                CompetitionSubmission submission = competitionSubContext.GetDetails(competitor.CompetitorID, competitionid);
                if (submission != null)
                {
                    votescore += submission.VoteCount * 0.5;
                }
            }

            for (int i = 0; i < competitorsInCompetition.Count(); i++)
            {
                double score = 0;
                List<CompetitionScore> competitionScoreList = new List<CompetitionScore>();
                competitionScoreList = competitionScoreContext.GetScoresOfCompetitor(competitorsInCompetition[i].CompetitorID, competitionid);
                if (competitionScoreList.Count() == criteriaList.Count)
                {
                    for (int k = 0; k < competitionScoreList.Count(); k++)
                    {
                        score += competitionScoreList[k].Score * (criteriaList[k].Weightage*0.1);
                    }
                    CompetitionSubmission submission = competitionSubContext.GetDetails(competitorsInCompetition[i].CompetitorID, competitionid);
                    if (submission != null)
                    {
                        score += submission.VoteCount * 0.5;
                    }
                    competitionScores.Add(score);
                }

            }
            // Total score
            double totalscore = judgeScore + votescore;

            viewScoreDetailsVM.CompetitionID = competitionid;
            viewScoreDetailsVM.CompetitionName = competition.CompetitionName;
            viewScoreDetailsVM.Competitor = competitor;
            viewScoreDetailsVM.TotalScore = totalscore;
            viewScoreDetailsVM.JudgeScore = judgeScore;
            viewScoreDetailsVM.VoteScore = votescore;
            viewScoreDetailsVM.FileShown = competitionSub.FileSubmitted;
            if (competitionSub.Ranking != null)
            {
                viewScoreDetailsVM.Ranking = (int)competitionSub.Ranking;
            }
        
            return View(viewScoreDetailsVM);
        }

        public ActionResult ViewScore(int id)
        {
            List<CompetitionSubmission> competitionSubmissionList = competitionSubContext.GetAllSubmissionsByComp(id);
            List<Competitor> competitorList = competitorContext.GetAllComps();
            List<Competitor> competitorsInCompetition = new List<Competitor>();
            List<Criteria> criteriaList = criteriaContext.GetCriteria(id);
            Competition competition = competitionContext.GetDetails(id);

            ViewScoreViewModel viewScoreVM = new ViewScoreViewModel();

            foreach (CompetitionSubmission submission in competitionSubmissionList)
            {
                foreach (Competitor competitor in competitorList)
                {
                    if (submission.CompetitorID == competitor.CompetitorID)
                    {
                        competitorsInCompetition.Add(competitor);
                    }
                }
            }

            List<Competitor> competitorScoreDone = new List<Competitor>();
            List<Competitor> competitorScoreNotDone = new List<Competitor>();
            List<double> judgeScoreList = new List<double>();
            List<double> voteScoreList = new List<double>();
            List<double> competitionScores = new List<double>();

            for (int i = 0; i < competitorsInCompetition.Count(); i++)
            {
                double vote = 0;
                double score = 0;
                double total = 0;
                List<CompetitionScore> competitionScoreList = new List<CompetitionScore>();
                competitionScoreList = competitionScoreContext.GetScoresOfCompetitor(competitorsInCompetition[i].CompetitorID, id);
                if (competitionScoreList.Count() == criteriaList.Count)
                {
                    competitorScoreDone.Add(competitorsInCompetition[i]);
                    for (int k = 0; k < competitionScoreList.Count(); k++)
                    {
                        score += competitionScoreList[k].Score * criteriaList[k].Weightage * 0.1;
                    }
                    judgeScoreList.Add(score);
                    CompetitionSubmission submission = competitionSubContext.GetDetails(competitorsInCompetition[i].CompetitorID, id);
                    if (submission != null)
                    {
                        vote += submission.VoteCount * 0.5;
                        voteScoreList.Add(vote);
                    }
                    total = score + vote;
                    competitionScores.Add(total);

                }
                else
                {
                    competitorScoreNotDone.Add(competitorsInCompetition[i]);
                }
            }

            viewScoreVM.judgeScoreList = judgeScoreList;
            viewScoreVM.voteScoreList = voteScoreList;
            viewScoreVM.competitionDoneScoreList = competitionScores;
            viewScoreVM.CompetitionID = id;
            viewScoreVM.CompetitionName = competition.CompetitionName;
            viewScoreVM.competitorDoneList = competitorScoreDone;
            viewScoreVM.competitorNotDoneList = competitorScoreNotDone;

            return View(viewScoreVM);
        }

        public ActionResult ViewRanking(int id)
        {
            List<CompetitionSubmission> competitionSubmissionList = competitionSubContext.GetAllSubmissionsByComp(id);
            List<Competitor> competitorList = competitorContext.GetAllComps();
            List<Competitor> competitorsInCompetition = new List<Competitor>();
            List<Criteria> criteriaList = criteriaContext.GetCriteria(id);
            Competition competition = competitionContext.GetDetails(id);

            ViewRankingViewModel viewRankingVM = new ViewRankingViewModel();

            foreach (CompetitionSubmission submission in competitionSubmissionList)
            {
                foreach (Competitor competitor in competitorList)
                {
                    if (submission.CompetitorID == competitor.CompetitorID)
                    {
                        competitorsInCompetition.Add(competitor);
                    }
                }
            }

            List<Competitor> competitorScoreDone = new List<Competitor>();
            List<Competitor> competitorScoreNotDone = new List<Competitor>();
            List<double> competitionScores = new List<double>();

            for (int i = 0; i < competitorsInCompetition.Count(); i++)
            {
                double score = 0;
                List<CompetitionScore> competitionScoreList = new List<CompetitionScore>();
                competitionScoreList = competitionScoreContext.GetScoresOfCompetitor(competitorsInCompetition[i].CompetitorID, id);
                if (competitionScoreList.Count() == criteriaList.Count)
                {
                    competitorScoreDone.Add(competitorsInCompetition[i]);
                    for (int k = 0; k < competitionScoreList.Count(); k++)
                    {
                        score += competitionScoreList[k].Score * (criteriaList[k].Weightage * 0.1);
                    }
                    CompetitionSubmission submission = competitionSubContext.GetDetails(competitorsInCompetition[i].CompetitorID, id);
                    if (submission != null)
                    {
                        score += submission.VoteCount * 0.5;
                    }
                    competitionScores.Add(score);
                }
                else
                {
                    competitorScoreNotDone.Add(competitorsInCompetition[i]);
                }
            }

            competitionScores = competitionScores.OrderByDescending(d => d).ToList();
            List<int> rankingList = new List<int>();
            for (int i = 0; i < competitorsInCompetition.Count(); i++)
            {
                double score = 0;
                List<CompetitionScore> competitionScoreList = new List<CompetitionScore>();
                competitionScoreList = competitionScoreContext.GetScoresOfCompetitor(competitorsInCompetition[i].CompetitorID, id);
                if (competitionScoreList.Count() == criteriaList.Count)
                {
                    for (int k = 0; k < competitionScoreList.Count(); k++)
                    {
                        score += competitionScoreList[k].Score * criteriaList[k].Weightage * 0.1;
                    }
                    CompetitionSubmission submission = competitionSubContext.GetDetails(competitorsInCompetition[i].CompetitorID, id);
                    if (submission != null)
                    {
                        score += submission.VoteCount * 0.5;
                    }

                    int rank = competitionScores.IndexOf(score) + 1;
                    rankingList.Add(rank);
                }
            }

            if(competitionSubmissionList.Count() == rankingList.Count())
            {
                ViewData["viewRanking"] = "1";
                List<CompetitionSubmission> submissionList = competitionSubContext.UpdateSubmissionRanking(competitionSubmissionList, rankingList);
            }
          
            viewRankingVM.rankingList = rankingList;
            viewRankingVM.competitionDoneScoreList = competitionScores;
            viewRankingVM.CompetitionID = id;
            viewRankingVM.CompetitionName = competition.CompetitionName;
            viewRankingVM.competitorDoneList = competitorScoreDone;
            viewRankingVM.competitorNotDoneList = competitorScoreNotDone;

            return View(viewRankingVM);
        }

    }
}
