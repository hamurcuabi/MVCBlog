using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCBlog.Business;
using MVCBlog.Business.Commands;
using MVCBlog.Data;
using MVCBlog.Web.Models;

namespace MVCBlog.Web.Controllers
{

    public class WrapUpController : Controller
    {
        private readonly EFUnitOfWork unitOfWork;


        public WrapUpController(EFUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Route("[controller]/{guid}")]
        public async Task<ActionResult> WrapUp(string guid)
        {
            try
            {
                var entry = await this.unitOfWork.WrapUps
                                 .Where(e => (e.Id == System.Guid.Parse(guid)))
                                .SingleOrDefaultAsync();

                if (entry == null)
                {
                    return NotFound();
                }
                var list = new List<WrapUpHtml>();


                var FirstCommiedtDate = new WrapUpHtml();
                FirstCommiedtDate.Type = "1";
                FirstCommiedtDate.Icon = "fas fa-hand-holding-heart";
                FirstCommiedtDate.Title = "Your first commit was...";
                FirstCommiedtDate.Value = FormatDate(entry.FirstCommittedDate, "dddd, dd MMMM HH:mm:ss");
                FirstCommiedtDate.Description = entry.FirstCommittedDateMessage;
                list.Add(FirstCommiedtDate);

                var LastCommiedtDate = new WrapUpHtml();
                LastCommiedtDate.Type = "2";
                LastCommiedtDate.Icon = "fas fa-hands-wash";
                LastCommiedtDate.Title = "Your last commit was...";
                LastCommiedtDate.Value = FormatDate(entry.LastCommittedDate, "dddd, dd MMMM HH:mm:ss");
                LastCommiedtDate.Description = entry.LastCommittedDateMessage;
                list.Add(LastCommiedtDate);

                var MinCommittedDateByDeveloper = new WrapUpHtml();
                MinCommittedDateByDeveloper.Type = "3";
                MinCommittedDateByDeveloper.Icon = "fas fa-coffee";
                MinCommittedDateByDeveloper.Title = FormatDate(entry.MinCommittedDateByDeveloper, "dddd, dd MMMM");
                MinCommittedDateByDeveloper.Value = entry.MinCommittedDateCountByDeveloper;
                MinCommittedDateByDeveloper.Description = "The minimum committed day you did , a coffe break";
                list.Add(MinCommittedDateByDeveloper);

                var MostCommittedDateByDeveloper = new WrapUpHtml();
                MostCommittedDateByDeveloper.Type = "1";
                MostCommittedDateByDeveloper.Icon = "fas fa-dumbbell";
                MostCommittedDateByDeveloper.Title = FormatDate(entry.MostCommittedDateByDeveloper, "dddd, dd MMMM");
                MostCommittedDateByDeveloper.Value = entry.MostCommittedDateCountByDeveloper;
                MostCommittedDateByDeveloper.Description = "The most committed day you did , WELL DONE !";
                list.Add(MostCommittedDateByDeveloper);


                var PushCountByDeveloper = new WrapUpHtml();
                PushCountByDeveloper.Type = "2";
                PushCountByDeveloper.Icon = "fas fa-align-center";
                PushCountByDeveloper.Title = "Look at that !";
                PushCountByDeveloper.Value = entry.PushCount;
                PushCountByDeveloper.Description = "You pushed a lots of line code...";
                list.Add(PushCountByDeveloper);

                var AddedLineCount = new WrapUpHtml();
                AddedLineCount.Type = "3";
                AddedLineCount.Icon = "fas fa-keyboard";
                AddedLineCount.Title = "Thats insane!";
                AddedLineCount.Value = entry.AddedLineCount;
                AddedLineCount.Description = "You code a lots of line...";
                list.Add(AddedLineCount);

                var DeletedLineCount = new WrapUpHtml();
                DeletedLineCount.Type = "1";
                DeletedLineCount.Icon = "fas fa-eraser";
                DeletedLineCount.Title = "I know you must clean it up";
                DeletedLineCount.Value = entry.DeletedLineCount;
                DeletedLineCount.Description = "You deleted a lots of line...";
                list.Add(DeletedLineCount);

                var MostCommiedtDate = new WrapUpHtml();
                MostCommiedtDate.Type = "2";
                MostCommiedtDate.Icon = "fas fa-people-carry";
                MostCommiedtDate.Title = FormatDate(entry.MostCommittedDate, "dddd, dd MMMM");
                MostCommiedtDate.Value = entry.MostCommittedDateCount;
                MostCommiedtDate.Description = "The most committed day, well done TEAM !";
                list.Add(MostCommiedtDate);


                var MinCommiedtDate = new WrapUpHtml();
                MinCommiedtDate.Type = "3";
                MinCommiedtDate.Icon = "fas fa-bed";
                MinCommiedtDate.Title = FormatDate(entry.MinCommittedDate, "dddd, dd MMMM");
                MinCommiedtDate.Value = entry.MinCommittedDateCount;
                MinCommiedtDate.Description = "The minimum committed day,TEAM needs some rest";
                list.Add(MinCommiedtDate);


                var MergedPRCount = new WrapUpHtml();
                MergedPRCount.Type = "1";
                MergedPRCount.Icon = "fas fa-code-branch";
                MergedPRCount.Title = "Wow, thats huge...";
                MergedPRCount.Value = entry.MergedPRCount;
                MergedPRCount.Description = "Your team merged a lots of pull requests";
                list.Add(MergedPRCount);


                return this.View("WrapUp", new WrapUpViewModel { Entry = entry, ListOfHtml = list });
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }

        private string FormatDate(string date,string format)
        {
            var text = date;
            try
            {
                DateTime enteredDate = DateTime.Parse(date);
                text = enteredDate.ToString(format);

            }
            catch (Exception)
            {
                try
                {
                    DateTime enteredDate = DateTime.ParseExact(date, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);
                    text = enteredDate.ToString(format);
                }
                catch (Exception)
                {

                }

            }
            return text;
        }

    }
}
