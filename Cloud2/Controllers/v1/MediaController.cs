using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Cloud2;

namespace Cloud2.Controllers.v1
{
    [Authorize]
    public class MediaController : ApiController
    {
        //api/v1/medias/<id>
        //Deletes the media with id <id>.
        public void Delete(int id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            using (var db = new MyDbContext())
            {
                User u = db.Users.Include("vacationList").FirstOrDefault(i => i.username == myUsername); //user som är inloggad
                Media med = db.Medias.Include("memory").FirstOrDefault(i => i.ID == id);
                Memory mem = db.Memories.Include("vacation").FirstOrDefault(i => i.mediaList.Any(j=> i.id == med.memory.id));
                if(u.vacationList.Contains(mem.vacation))
                {
                    db.Medias.Remove(med);
                    db.SaveChanges();
                }
                else
                {
                    throw new HttpException(400, "Bad request, not your media");
                }
                
                
            }
        }
    }
}
