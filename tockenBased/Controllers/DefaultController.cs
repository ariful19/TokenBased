using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using tockenBased.Models;

namespace tockenBased.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpPost]
        [Route("api/saveUser")]
        public JsonResult<object> saveUser(UserViewModel userModel)
        {
            using (var db = new LiteDB.LiteDatabase(AppDomain.CurrentDomain.BaseDirectory + "\\mydb.db"))
            {
                if (userModel.p1 == userModel.p2)
                {
                    var u = new User { ID = Guid.NewGuid(), address = userModel.address, age = userModel.age, name = userModel.name, pwdhash = Actions.getHash(userModel.p1) };
                    var users = db.GetCollection<User>("Users");
                    users.Insert(u);
                }
            }
            return Json<object>(new { });
        }

        [HttpPost]
        [Route("api/Login")]
        public JsonResult<object> Login(dynamic user)
        {
            using (var db = new LiteDB.LiteDatabase(AppDomain.CurrentDomain.BaseDirectory + "\\mydb.db"))
            {
                string password = user.password;
                string userid = user.user;
                var users = db.GetCollection<User>("Users");
                var first = users.Find(o => o.name == userid).FirstOrDefault();
                if (first != null && Actions.IsValide(password, first.pwdhash))
                {
                    var to = new TokenObj { userid = userid, Expires = DateTime.Now.AddSeconds(20) };
                    return Json<object>(new { token = to.ToTokenString() });
                }
            }
            return Json<object>(new { token = "" });
        }

        [HttpGet]
        [Route("api/IsExpired")]
        public JsonResult<object> IsExpired(dynamic user)
        {
            if (Request.Headers.Authorization != null)
            {
                var fd = Request.Headers.Authorization;
                //TokenObj obj = TokenObj.Get(fd);
            }


            return Json<object>(new { token = "" });
        }

        [HttpGet]
        [Route("api/CanAccessA")]
        [Authorize(Users = "a")]
        public JsonResult<object> CanAccessA()
        {


            return Json<object>(new { token = "" });
        }
    }
}
