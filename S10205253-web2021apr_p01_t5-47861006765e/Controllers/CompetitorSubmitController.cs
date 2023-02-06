using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.IO;
using System.Web;

using Web_Asg.Models;
using Web_Asg.DAL;

namespace Web_Asg.Controllers
{
    // Need to validate these:
    // 1) Check for start and end date. can only submit within those days

    public class CompetitorSubmitController : Controller
    {
        private List<int> CompID = new List<int> { 1, 2, 3, 4 };
        private List<int> AlrJoinedCompID = new List<int> {2,3};
        private List<string> salutationList = new List<string> { "Dr", "Mr", "Ms", "Mrs", "Mdm" };

        private List<SelectListItem> salutationSelect = new List<SelectListItem>();
        private List<int> displayJoinedCompID = new List<int>();
        private List<int> displayCompID = new List<int>();

        private CompetitionSubmissionDAL compsubContext = new CompetitionSubmissionDAL();

        public CompetitorSubmitController()
        {
            foreach(int cID in CompID)
            {
                foreach(int ajcID in AlrJoinedCompID)
                {
                    if (cID == ajcID)
                    {
                        displayJoinedCompID.Add(cID);
                    }
                    else if (cID != ajcID)
                    {
                        displayCompID.Add(cID);
                    }
                }
            }

            foreach(string sal in salutationList)
            {
                salutationSelect.Add(
                    new SelectListItem
                    {
                        Value = sal,
                        Text = sal,
                    });
            }
        }

        //SUBMIT FUNCTION
        // Redirect to View Submit Page

        public ActionResult ViewSubmit(int id)
        {

            int CompetitorID = (int)HttpContext.Session.GetInt32("CompetitorID");
            CompetitionSubmission compSub = compsubContext.GetDetails(CompetitorID, id);
            SubmissionViewModel svm = MapToSVM(compSub);
            Competition comp = MapToComp(id);

            ViewData["Competition"] = comp;
            return View(svm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ViewSubmit(SubmissionViewModel submission)
        {
            Competition comp = MapToComp(submission.CompetitionID);
            ViewData["Competition"] = comp;

            string namingConvention = "File_" + submission.CompetitorID + "_" + submission.CompetitionID;

            if (submission.fileToUpload != null &&
            submission.fileToUpload.Length > 0)
            {
                try
                {
                    // Find the filename extension of the file to be uploaded.
                    string fileExt = Path.GetExtension(
                     submission.fileToUpload.FileName);
                    if (fileExt == ".pdf")
                    {
                        // Rename the uploaded file with the staff’s name.
                        string uploadedFile = namingConvention + fileExt;
                        // Get the complete path to the images folder in server
                        string savePath = Path.Combine(
                         Directory.GetCurrentDirectory(),
                         "wwwroot\\submissions", uploadedFile);
                        // Upload the file to server
                        using (var fileSteam = new FileStream(
                         savePath, FileMode.Create))
                        {
                            await submission.fileToUpload.CopyToAsync(fileSteam);
                        }
                        submission.FileSubmitted = uploadedFile;

                        bool fileIn = compsubContext.FileSubmission(submission.FileSubmitted, submission.CompetitionID, submission.CompetitorID);
                        if (fileIn == true)
                        {
                            ViewData["Message"] = "File uploaded successfully.";
                        }
                        else
                        {
                            ViewData["Message"] = "File uploading fail!";
                        }
                    }
                    else
                    {
                        ViewData["Message"] = "You can only upload a pdf version of your submissions!";
                    }
                }
                catch (IOException)
                {
                    //File IO error, could be due to access rights denied
                    ViewData["Message"] = "File uploading fail!";
                }
                catch (Exception ex) //Other type of error
                {
                    ViewData["Message"] = ex.Message;
                }
            }
            return View(submission);
        }


        public SubmissionViewModel MapToSVM(CompetitionSubmission cs)
        {
            SubmissionViewModel svm = new SubmissionViewModel
            {
                CompetitionID = cs.CompetitionID,
                CompetitorID = cs.CompetitorID,
                FileSubmitted = "File_" + cs.CompetitorID + "_" + cs.CompetitionID + ".pdf",
                DateTimeFileUpload = cs.DateTimeFileUpload,
                Appeal = cs.Appeal,
                VoteCount = cs.VoteCount,
                Ranking = cs.Ranking
            };
            return svm;
        }

        public Competition MapToComp(int x)
        {

            CompetitionDAL compContext = new CompetitionDAL();
            List<Competition> allCompList = compContext.GetAllComps();

            foreach (Competition c in allCompList)
            {
                if (c.CompetitionID == x)
                {
                    return c;
                }
            }
            return null;

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
