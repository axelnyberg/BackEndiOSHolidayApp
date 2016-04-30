using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Cloud2.Controllers.v1
{
    [Authorize]
    public class UsersController : ApiController
    {

        ///api/v1/users/<username>
        // Returns the user with username <username>.
        public User Get(string id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            using (var db = new MyDbContext())
            {
                User u = db.Users.FirstOrDefault(i => i.username == id);
                if (u == null)
                {
                    throw new HttpException(400, "Bad request, username not found");
                }
                else
                {
                    User returnUser = new User();
                    returnUser.firstname = u.firstname;
                    returnUser.lastname = u.lastname;
                    returnUser.username = u.username;
                    returnUser.email = u.email;
                    return returnUser;
                }
                
            }      
        }

        //api/v1/users/<username>/friends
        //Returns a list of friends the user with username <username> has. 

        //OR

        //api/v1/users/<username>/vacations
        //Returns a list of vacations the user with username<username> has.
        public IHttpActionResult Get(string id, string modifier)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            using (var db = new MyDbContext())
            {
                if (modifier == "friends")
                {
                    User u = db.Users.Include("friendList").FirstOrDefault(i => i.username == myUsername);
                    User searchedUser = db.Users.Include("friendList").FirstOrDefault(i => i.username == id);
                    if(searchedUser.friendList.Contains(u) || u == searchedUser)
                    {
                        foreach (User user in searchedUser.friendList)
                        {
                            user.friendList = null;
                        }
                        return Ok(searchedUser.friendList);
                    }
                    else
                    {
                        throw new HttpException(400, "Bad request, not allowed to get their friendlist"); 
                    }  
                 
                }
                else if (modifier == "vacations")
                {
                    User u = db.Users.Include("vacationList").FirstOrDefault(i => i.username == myUsername);
                    User searchedUser = db.Users.Include("VacationList").Include("friendList").FirstOrDefault(i => i.username == id);
                    if (searchedUser.friendList.Contains(u) || u == searchedUser)
                    {
                        List<Vacation> vacList = new List<Vacation>();
                        foreach (Vacation vac in searchedUser.vacationList)
                        {
                            Vacation v = new Vacation();
                            v.title = vac.title;
                            v.place = vac.place;
                            v.start = vac.start;
                            v.end = vac.end;
                            v.description = vac.description;
                            v.id = vac.id;
                            vacList.Add(v);
                        }
                        return Ok(vacList);
                    }
                    else
                    {
                        throw new HttpException(400, "Bad request, not allowed to get their vacationlist");
                    }  
                   
                }
                else
                {
                    throw new HttpException(400, "Bad request, wrong modifier"); 
                }
            }      
        }
        ///api/v1/users/<username>/friends
        //Adds a user as a friend to the user with username <username>
        public void Post(string id, [FromBody]User value, string modifier)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            if (id == myUsername)
            {
                using (var db = new MyDbContext())
                {
                
                    User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                
                    User us = db.Users.FirstOrDefault(i => i.username == value.username);
                    u.friendList.Add(us);

                    db.SaveChanges();
                }
                
            }
            else
            {
                throw new HttpException(400, "Bad request, no authority");
            }

        }
        ///api/v1/users/<username>
        // Partially updates the user with username <username>.
        public void Patch(string id, [FromBody]User value)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            if (id == myUsername)
            {
                using (var db = new MyDbContext())
                {
                
                    User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                
                    if (value.firstname != null)
                    {
                        u.firstname = value.firstname;
                    }
                    if (value.lastname != null)
                    {
                        u.lastname = value.lastname;
                    }
                    if (value.email != null)
                    {
                        u.email = value.email;
                    }
                    db.SaveChanges();
                }
                
            }
            else
            {
                throw new HttpException(400, "Bad request, no authority");
            }

        }
        ///api/v1/users/<username>
        //Updates the user with username <username>.
        public void Put(string id, [FromBody]User value)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            if (id == myUsername)
            {
                using (var db = new MyDbContext())
                {
                
                    User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                
                    if (value.firstname == null || value.lastname == null  || value.email == null)
                    {
                        throw new HttpException(400, "Bad request, you must fill all input");
                    }
                    else
                    {
                        u.firstname = value.firstname;
                        u.lastname = value.lastname;
                        u.email = value.email;
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                throw new HttpException(400, "Bad request, no authority");
            }

        }

        ///api/v1/users/<username>
        //Deletes everything related to the user with username <username>.
        public void Delete(string id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            using (var db = new MyDbContext())
            {
                User u = db.Users.Include("vacationList.memoryList.mediaList").Include("friendList").FirstOrDefault(i => i.username == myUsername);
                if (id == myUsername)
                {
                    var vacList = u.vacationList.ToList();
                    foreach(Vacation vac in vacList)
                    {
                        var memList = vac.memoryList.ToList();
                        foreach(Memory mem in memList)
                        {
                            var medList = mem.mediaList.ToList();
                            foreach(Media med in medList)
                            {
                                db.Medias.Remove(med);
                            }
                            db.Memories.Remove(mem);
                        }
                        db.Vacations.Remove(vac);
                    }

                    var userList = db.Users.Include("friendList").Where(i => i.friendList.Any(j => j.username == u.username)).ToList();
                    foreach(User friend in userList)
                    {
                        friend.friendList.Remove(u);
                        u.friendList.Remove(friend);
                    }
                    db.Users.Remove(u);
                    db.SaveChanges();
                }
                else
                {
                    throw new HttpException(400, "Bad request, couldnt remove user!!");
                }
            }
        }

        //api/v1/users/<username>/friends/<friendsUsername>
        //Removes the user with username <friendsUsername> as a friend to the user with username <username>.
        public void Delete(string id, string modifier, string modifier2)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            if (id == myUsername)
            {
                using (var db = new MyDbContext())
                {
                
                    User u = db.Users.Include("friendList").FirstOrDefault(i => i.username == myUsername);
                    User friend = db.Users.FirstOrDefault(i => i.username == modifier2);
                    if (u.friendList.Contains(friend) && id == u.username)
                    {
                        u.friendList.Remove(friend);
                    }
                    else
                    {
                        throw new HttpException(400, "Bad request, couldnt remove friend");
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                throw new HttpException(400, "Bad request, no authority");
            }


        }
    }
}
