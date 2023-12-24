using MVCBlog.Data;
using System.Collections.Generic;

namespace MVCBlog.Web.Models
{
    public class WrapUpViewModel
    {
        public WrapUp Entry { get; set; }

        public List<WrapUpHtml> ListOfHtml { get; set; }

    }
}