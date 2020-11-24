﻿using ORDER_MANAGEMENT.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using ORDER_MANAGEMENT.API.Models;
using System.Net.Http;
using System.Net.Http.Formatting;
using Microsoft.AspNet.Identity.Owin;

namespace ORDER_MANAGEMENT.API.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        private readonly IUnitOfWork db;
        public UserController(IUnitOfWork unitOfWork)
        {
            db = unitOfWork;
        }
        [HttpGet]
        // GET: api/TargetInfo
        [Route("api/TargetInfo")]
        public TargetInfo TargetInfo()
        {
            var id = db.Registrations.GetRegID_ByUserName(User.Identity.Name);
            var t_info = db.Users.API_TargetInfo(id);
            return t_info;
        }

        // GET: api/getUserInfo
        [Route("api/getUserInfo")]
        public UserNameRank getUserInfo()
        {
            var id = db.Registrations.GetRegID_ByUserName(User.Identity.Name);
            var U = db.Users.API_getUserInfo(id);
            return U;
        }

        // GET: api/getUserInfo
        [Route("api/getReportTo")]
        public UserReportTo getReportTo()
        {
            var id = db.Registrations.GetRegID_ByUserName(User.Identity.Name);
            return db.Users.GetReportTo(id);
        }

        // GET: api/getUserInfo
        [Route("api/getTerritory")]
        public List<DDL> getTerritory()
        {
            var id = db.Registrations.GetRegID_ByUserName(User.Identity.Name);
            return db.Territorys.GetUserTerritory(id);
        }


        [HttpPost]
        [Route("api/UserProfileUpdate")]
        public IHttpActionResult UserProfileUpdate([FromBody]UserInfoUpdate value)
        {
            if (value == null) NotFound();

            var RegID = db.Registrations.GetRegID_ByUserName(User.Identity.Name);

            var Reg = db.Users.UserInfoUpdate(RegID, value);
            db.Registrations.Update(Reg);
            db.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("api/ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword([FromBody] ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var result = await userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            
            if(result.Succeeded) return Ok();

            return Content(HttpStatusCode.InternalServerError, result);
        }
    }
}
