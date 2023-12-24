using System.Collections.Generic;
using System.Diagnostics;
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
    [Route("api/[controller]")]
    [ApiController]
    public class WrapUpApiController : Controller
    {
        private readonly EFUnitOfWork unitOfWork;


        public WrapUpApiController(EFUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpPost("addWrapUp")]
        public string AddWrapUp(WrapUp wrap)
        {
            var entry = unitOfWork.WrapUps.Add(wrap);
            unitOfWork.SaveChangesAsync();
            return entry.Entity.Id.ToString();
        }

        [HttpGet("getWrapUpsByEmail/{email}")]
        public async Task<List<WrapUp>> GetWrapUps(string email)
        {

            var query =unitOfWork.WrapUps
                          .Where(e => e.UserMail.Equals(email));


            return await query.ToListAsync();
        }
    }
}
