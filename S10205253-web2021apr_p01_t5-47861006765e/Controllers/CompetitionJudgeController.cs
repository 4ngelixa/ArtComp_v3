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
    public class CompetitionJudgeController : Controller
    {
        private CompetitionDAL competitionContext = new CompetitionDAL();
        private AreaInterestDAL AreaInterestContext = new AreaInterestDAL();
        private JudgeDAL JudgeContext = new JudgeDAL();
        private CompetitionJudgeDAL CompJudgeContext = new CompetitionJudgeDAL();
        //get all comps from comp DAL
        public ActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<Competition> compList = competitionContext.GetAllComps();
            return View(compList);
        }
        
        public ActionResult AssignJudges(int id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<CompetitionJudge> currentJudgeList = CompJudgeContext.GetCompJudges(id);
            List<string> currentJudgeName = new List<string>();
            Competition comp = competitionContext.GetDetails(id);
            AreaInterest areaInterest = AreaInterestContext.GetDetails(comp.AreaInterestID);
            //get judges alr assigned if any
            foreach (CompetitionJudge cj in currentJudgeList)
            {
                currentJudgeName.Add(JudgeContext.GetDetails(cj.JudgeID).JudgeName);
            }
            ViewData["AreaInterest"] = areaInterest;
            ViewData["Comp"] = comp;
            ViewData["JudgeCount"] = CompJudgeContext.JudgeCount(id);
            ViewData["CurrentJudges"] = CompJudgeContext.GetCompJudges(id);
            ViewData["CurrentJudgeName"] = currentJudgeName;
            CompetitionJudgeViewModel compJudge = new CompetitionJudgeViewModel
            {
                Judges = GetJudges(comp)
            };
            return View(compJudge);
        }
        // get all available judges 
        private List<AvailJudges> GetJudges(Competition comp)
        {
            List<Judge> judgeList = JudgeContext.GetAllJudges();
            List<AvailJudges> jSelectList = new List<AvailJudges>();
            foreach (Judge j in judgeList)
            {
                if (j.AreaInterestID == comp.AreaInterestID)
                {
                    jSelectList.Add(new AvailJudges
                    {
                        JudgeName = j.JudgeName,
                        JudgeID = j.JudgeID,
                        Selected = false
                    });
                }
            }
            return jSelectList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignJudges(CompetitionJudgeViewModel cJudge)
        {
            //get all the selected judges and insert to database
            foreach (AvailJudges judgeItem in cJudge.Judges)
            {
                if (judgeItem.Selected)
                {
                    cJudge.JudgeID.Add(judgeItem.JudgeID);
                }
            }
            CompJudgeContext.AddCompJudge(cJudge);
            return RedirectToAction("Index");
        }

      
        public ActionResult Delete(int id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            Competition comp = competitionContext.GetDetails(id);
            AreaInterest areaInterest = AreaInterestContext.GetDetails(comp.AreaInterestID);
            List<AvailJudges> availableJudges = new List<AvailJudges>();
            List<CompetitionJudge> compJudge = CompJudgeContext.GetCompJudges(id);
            foreach (CompetitionJudge cj in compJudge)
            {
                availableJudges.Add(new AvailJudges
                {
                    JudgeName = JudgeContext.GetDetails(cj.JudgeID).JudgeName,
                    JudgeID = cj.JudgeID,
                    Selected = false
                });
            }
            ViewData["AreaInterest"] = areaInterest;
            ViewData["Comp"] = comp;
            CompetitionJudgeViewModel compJudges = new CompetitionJudgeViewModel
            {
                Judges = availableJudges
            };
            return View(compJudges);
        }

        // POST: CompetitionJudge/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(CompetitionJudgeViewModel cJudge)
        {
            foreach (AvailJudges judgeItem in cJudge.Judges)
            {
                if (judgeItem.Selected)
                {
                    cJudge.JudgeID.Add(judgeItem.JudgeID);
                }
            }
            CompJudgeContext.Delete(cJudge);
            return RedirectToAction("Index");
        }
    }
}
