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
    public class VacationsController : ApiController
    {
        ///api/v1/vacations
        //Returns a list of vacations.
        public IEnumerable<Vacation> Get()
        {

            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            using (var db = new MyDbContext())
            {
                User u = db.Users.Include("vacationList").FirstOrDefault(i => i.username == myUsername);
                var vacationsList = db.Vacations.Include("user").Where(v => v.user.id == u.id || v.user.friendList.Any(u2 => u2.username == u.username)).ToList();
                          
                foreach (Vacation vac in vacationsList)
                {
                    vac.user = null;              
                }
                return vacationsList;
            }            
        }

        //api/v1/vacations/<id>
        //Returns the vacation with id <id> (user must have u as friend)
        public Vacation Get(int id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;

            using (var db = new MyDbContext())
            {
                User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                Vacation v = db.Vacations.Include("user").FirstOrDefault(i => i.id == id);
                User u2 = db.Users.Include("friendList").FirstOrDefault(i => i.id == v.user.id);
                if (u2.friendList.Contains(u) || u2 == u)
                {
                    v.user = null;
                    return v;
                }
                else
                {
                    throw new HttpException(400, "Bad request, user havent added you!!");
                }
            }
        }

        //api/v1/vacations/<id>/memories
        //Returns a list of memories for the vacation with id <id>.
        public IEnumerable<Memory> Get(int id,string modifier)
        {

            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;

            using (var db = new MyDbContext())
            {
                User currentUser = db.Users.FirstOrDefault(i => i.username == myUsername);
                Vacation v = db.Vacations.Include("user").Include("memoryList").ToList().FirstOrDefault(i => i.id == id);
                User vUser = db.Users.Include("friendList").FirstOrDefault(i => i.id == v.user.id);
                
                if (vUser.friendList.Contains(currentUser) || v.user.id == currentUser.id)
                {
                    return v.memoryList;
                }
                else
                {
                    throw new HttpException(400, "Bad request, user havent added you!!");
                }
            }
        }

        //api/v1/vacations
        // Creates a new vacation for the current user.
        public void Post([FromBody]Vacation value)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;

            using (var db = new MyDbContext())
            {
                User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                value.user = u;
                db.Vacations.Add(value);
                db.SaveChanges();
            }
          
            
        }

        //api/v1/vacations/<id>/memories
        //Creates a new memory for the vacation with id <id>.
        public void Post(int id, [FromBody]Memory value,string modifier)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;

            using (var db = new MyDbContext())
            {

                User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                Vacation v = db.Vacations.Include("user").Include("memoryList").FirstOrDefault(i => i.id == id);
                if ( v.user.id != u.id)
                {
                    {
                        throw new HttpException(400, "Bad request, not your vacation");
                    }
                }
                else
                {
                    v.memoryList.Add(value);
                    db.Memories.Add(value);
                    db.SaveChanges();
                }  
            }
        }

        //api/v1/vacations/<id>
        //Updates the vacation with id <id>.
        public void Put(int id, [FromBody]Vacation value)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;

            using (var db = new MyDbContext())
            {
                User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                Vacation v = db.Vacations.Include("user").FirstOrDefault(i => i.id == id);
                if( v.user.id != u.id)
                {
                    throw new HttpException(400, "Bad request, not your vacation!!");
                }
                else 
                {
                    if(value.description == null || value.end == 0 || value.place == null || value.start == 0 || value.title == null )
                    {
                         throw new HttpException(400, "Bad request, fill all forms");
                    }
                    else
                    {
                        v.description = value.description;
                        v.end = value.end;
                        v.place = value.place;
                        v.start = value.start;
                        v.title = value.title;
                        db.SaveChanges();
                    } 
                }
                
            }
        }

        //api/v1/vacations/<id>
        //Partially updates the vacation with id <id>.
        public void Patch(int id, [FromBody]Vacation value)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;

            using (var db = new MyDbContext())
            {
                User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                Vacation v = db.Vacations.Include("user").FirstOrDefault(i => i.id == id);
                if (v.user.id != u.id)
                {
                    throw new HttpException(400, "Bad request, not your vacation!!");
                }
                else
                {
                    if (value.end != 0)
                    {
                        v.end = value.end;
                    }
                    if (value.description != null)
                    {
                        v.description = value.description;
                    }
                    if (value.place != null)
                    {
                        v.place = value.place;
                    }
                    if (value.start != 0)
                    {
                        v.start = value.start;
                    }
                    if (value.title != null)
                    {
                        v.title = value.title;
                    }
                    db.SaveChanges();
                }
                
            }

        }

        //api/v1/vacations/<id>
        //Deletes the vacation with id <id>.
        public void Delete(int id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            using (var db = new MyDbContext())
            {
                User u = db.Users.Include("vacationList.memoryList.mediaList").FirstOrDefault(i => i.username == myUsername);
                Vacation vac = db.Vacations.Include("memoryList").FirstOrDefault(i => i.id == id);
                
                if (u.vacationList.Contains(vac))
                {
                    var memList = vac.memoryList.ToList();
                    foreach(Memory m in memList)
                    {
                        var mediaList = m.mediaList.ToList();
                        foreach(Media med in mediaList)
                        {
                            db.Medias.Remove(med);
                        }
                        db.Memories.Remove(m);
                    }
                    db.Vacations.Remove(vac);
                    db.SaveChanges();
                }
                else
                {
                    throw new HttpException(400, "Bad request, couldnt remove vacation!!");
                }

            }
            
        }
    }
}
