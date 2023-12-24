using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCBlog.Data
{
    public class WrapUp: EntityBase
    {
        public string ProjectName { get; set; }
        public string BranchName { get; set; }
        public string UserMail { get; set; }
        public string FirstCommittedDate { get; set; }
        public string FirstCommittedDateMessage { get; set; }

        public string LastCommittedDate { get; set; }
        public string LastCommittedDateMessage { get; set; }
        public string MostCommittedDate { get; set; }
        public string MostCommittedDateCount { get; set; }
        public string MinCommittedDate { get; set; }
        public string MinCommittedDateCount { get; set; }
        public string MergedPRCount { get; set; }
        public string MostCommittedDateByDeveloper { get; set; }
        public string MostCommittedDateCountByDeveloper { get; set; }

        public string MinCommittedDateByDeveloper { get; set; }
        public string MinCommittedDateCountByDeveloper { get; set; }
        public string PushCount { get; set; }
        public string AddedLineCount { get; set; }
        public string DeletedLineCount { get; set; }
    }
}
