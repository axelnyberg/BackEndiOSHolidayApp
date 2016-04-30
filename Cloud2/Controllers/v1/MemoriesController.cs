using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using System.Web;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace Cloud2.Controllers.v1
{
    [Authorize]
    public class MemoriesController : ApiController
    {
        public IEnumerable<Memory> Get(string id, [FromUri]string type, [FromUri]string q)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            //string type = Convert.ToString(HttpContext.Current.Request.QueryString["type"]);
            //string query = Convert.ToString(HttpContext.Current.Request.QueryString["q"]);
            using (var db = new MyDbContext())
            {
                User u = db.Users.Include("friendList").FirstOrDefault(i => i.username == myUsername);
                if (type == "user")
                {
                    List<User> queryUser = db.Users.Include("friendList").Where(i => i.username.Contains(q)).ToList();
                    List<Memory> memoryList = new List<Memory>();
                    foreach(User user in queryUser )
                    {
                         if (user.friendList.Any(i=> i.username == u.username) || user == u)
                         {
                             List<Memory> memList = db.Memories.Include("vacation.user").Where(i => i.vacation.user.username == user.username).ToList();
                             memoryList.AddRange(memList);
                         }
                    }
                    if(memoryList.Count != 0)
                    {
                        foreach (Memory mem in memoryList)
                        {
                            mem.vacation = null;
                            mem.mediaList = null;
                        }
                        return memoryList;
                    }
                    else
                    {
                        throw new HttpException(400, "Bad request, no memories for that user, or you are not friends!");
                    }
                
                }
                else if(type == "place")
                {
                    List<Memory> memoryList = new List<Memory>();
                    List<Memory> memList = db.Memories.Include("vacation.user.friendList").Where(i => i.place.Contains(q)).ToList();
                    foreach(Memory mem in memList)
                    {
                        if (mem.vacation.user.friendList.Any(i => i.username == u.username) || mem.vacation.user == u)
                        {
                            memoryList.Add(mem);
                        }

                    }
                    foreach (Memory me in memoryList)
                    {
                        me.vacation = null;
                        me.mediaList = null;
                    }

                    if (memoryList.Count != 0)
                    {
                        return memoryList;
                    }
                    else
                    {
                        throw new HttpException(400, "Bad request, no memories for that place!");
                    }

                }
                else if (type == "title")
                {
                    List<Memory> memoryList = new List<Memory>();
                    List<Memory> memList = db.Memories.Include("vacation.user.friendList").Where(i => i.title.Contains(q)).ToList();
                    foreach (Memory mem in memList)
                    {
                        if (mem.vacation.user.friendList.Any(i => i.username == u.username) || mem.vacation.user == u)
                        {
                            memoryList.Add(mem);
                        }
                    }
                    foreach (Memory me in memoryList)
                    {
                        me.vacation = null;
                        me.mediaList = null;
                    }
                    if (memoryList.Count != 0)
                    {
                        return memoryList;
                    }
                    else
                    {
                        throw new HttpException(400, "Bad request, no memories for that title!");
                    }

                }
                else
                {
                    throw new HttpException(400, "Bad request, no such type!");

                }
                
            }

            


                
        }
        //return memory by id
        public Memory Get(int id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;

            using (var db = new MyDbContext())
            {
                User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                Memory m = db.Memories.Include("vacation.user").FirstOrDefault(i => i.id == id);
                Vacation v = db.Vacations.Include("memoryList").Include("user").FirstOrDefault(i => i.memoryList.Any(j => j.id == m.id));
                User u2 = db.Users.Include("friendList").Include("vacationList").FirstOrDefault(i => i.vacationList.Any(j => j.id == v.id));
                if (u2.friendList.Any(i=> i.username == u.username) || u2 == u)
                {
                    v.user = null;
                    m.vacation = null;
                    return m;
                }
                else
                {
                    throw new HttpException(400, "Bad request, user havent added you!!");
                }
            }
        }

        //return medialist by memoryid
        public IEnumerable<Media> Get(int id, string modifier)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;


            using (var db = new MyDbContext())
            {
                User u = db.Users.FirstOrDefault(i => i.username == myUsername);
                Memory m = db.Memories.Include("vacation.user").Include("mediaList").FirstOrDefault(i => i.id == id);
                Vacation v = db.Vacations.Include("memoryList").Include("user").FirstOrDefault(i => i.memoryList.Any(j => j.id == m.id));
                User u2 = db.Users.Include("friendList").Include("vacationList").FirstOrDefault(i => i.vacationList.Any(j => j.id == v.id));
                if (u2.friendList.Any(i => i.username == u.username) || u2 == u)
                {
                    v.user = null;
                    m.vacation = null;
                    foreach(Media med in m.mediaList)
                    {
                        med.memory = null;
                    }
                    return m.mediaList;
                }
                else
                {
                    throw new HttpException(400, "Bad request, user havent added you!!");
                }
            }
        }
        // Post different type of medias
        public IHttpActionResult PostMediaObject(int id, string modifier)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;

            var dbb = new MyDbContext();
            User u = dbb.Users.FirstOrDefault(i => i.username == myUsername);
            Memory m = dbb.Memories.Include("vacation").FirstOrDefault(i => i.id == id);

            if (m.vacation.user.id == u.id)
            {  
                if (modifier == "pictures")
                {

                    var uploadedFile = HttpContext.Current.Request.Files["picture-file"];
                    if (uploadedFile != null && uploadedFile.ContentLength > 0)
                    {
                        byte[] data = new byte[uploadedFile.ContentLength];
                        uploadedFile.InputStream.Read(data, 0, uploadedFile.ContentLength);
                        Stream stream = new MemoryStream(data);
                        var filename = Convert.ToString(DateTime.Now.ToFileTime());
                        string file = uploadedFile.FileName;
                        string filetype = file.Substring(file.IndexOf('.') + 1);


                        var picture = new PictureMedia
                        {

                            fileurl = "http://mediamedia.s3.amazonaws.com/" + filename + "." + filetype,
                            container = uploadedFile.ContentType,
                            width = Convert.ToInt32(HttpContext.Current.Request.QueryString["width"]),
                            height = Convert.ToInt32(HttpContext.Current.Request.QueryString["height"])
                        };


                        string bucketName = "mediamedia";
                        string awsAccessKeyId = "AKIAJPF4ZQUNSJSIICFA";
                        string awsSecretAccessKey = "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";
                        using (var client = new AmazonS3Client(
                            awsAccessKeyId,
                            awsSecretAccessKey,
                            Amazon.RegionEndpoint.EUCentral1))
                        {
                            client.PutObject(new PutObjectRequest()
                            {
                                InputStream = stream,
                                BucketName = bucketName,
                                Key = filename + "." + filetype
                            });
                        }
                        //adds info of file in database
                        using (var db = new MyDbContext())
                        {
                            var currentMemory = db.Memories.Include("mediaList").FirstOrDefault(v => v.id == id);
                            currentMemory.mediaList.Add(picture);
                            db.SaveChanges();
                            return Ok("The swag file has been uploaded");
                        }
                    }
                }

                if (modifier == "sounds")
                {
                    var uploadedFile = HttpContext.Current.Request.Files["sound-file"];
                    if (uploadedFile != null && uploadedFile.ContentLength > 0)
                    {
                        byte[] data = new byte[uploadedFile.ContentLength];
                        uploadedFile.InputStream.Read(data, 0, uploadedFile.ContentLength);
                        Stream stream = new MemoryStream(data);
                        var filename = Convert.ToString(DateTime.Now.ToFileTime());
                        string file = uploadedFile.FileName;
                        string filetype = file.Substring(file.IndexOf('.') + 1);

                        var sound = new SoundMedia
                        {

                            fileurl = "http://mediamedia.s3.amazonaws.com/" + filename + "." + filetype,
                            container = uploadedFile.ContentType,
                            duration = Convert.ToInt32(HttpContext.Current.Request.QueryString["duration"]),
                            codec = Convert.ToString(HttpContext.Current.Request.QueryString["codec"]),
                            bitrate = Convert.ToInt32(HttpContext.Current.Request.QueryString["bitrate"]),
                            channels = Convert.ToInt32(HttpContext.Current.Request.QueryString["channels"]),
                            samplingrate = Convert.ToInt32(HttpContext.Current.Request.QueryString["samplingrate"]),

                        };


                        string bucketName = "mediamedia";
                        string awsAccessKeyId = "AKIAJPF4ZQUNSJSIICFA";
                        string awsSecretAccessKey = "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";
                        using (var client = new AmazonS3Client(
                            awsAccessKeyId,
                            awsSecretAccessKey,
                            Amazon.RegionEndpoint.EUCentral1))
                        {
                            client.PutObject(new PutObjectRequest()
                            {
                                InputStream = stream,
                                BucketName = bucketName,
                                Key = filename + "." + filetype
                            });
                        }
                        //adds info of file in database
                        using (var db = new MyDbContext())
                        {
                            var currentMemory = db.Memories.Include("mediaList").FirstOrDefault(v => v.id == id);
                            currentMemory.mediaList.Add(sound);
                            db.SaveChanges();
                            return Ok("The swag file has been uploaded");
                        }
                    }
                }

                if (modifier == "videos")
                {
                    var uploadedFile = HttpContext.Current.Request.Files["video-file"];
                    if (uploadedFile != null && uploadedFile.ContentLength > 0)
                    {
                        byte[] data = new byte[uploadedFile.ContentLength];
                        uploadedFile.InputStream.Read(data, 0, uploadedFile.ContentLength);
                        Stream stream = new MemoryStream(data);
                        var filename = Convert.ToString(DateTime.Now.ToFileTime());
                        string file = uploadedFile.FileName;
                        string filetype = file.Substring(file.IndexOf('.') + 1);

                        var video = new VideoMedia
                        {

                            fileurl = "http://mediamedia.s3.amazonaws.com/" + filename + "." + filetype,
                            container = uploadedFile.ContentType,
                            width = Convert.ToInt32(HttpContext.Current.Request.QueryString["width"]),
                            height = Convert.ToInt32(HttpContext.Current.Request.QueryString["height"]),
                            videocodec = Convert.ToString(HttpContext.Current.Request.QueryString["ideocodec"]),
                            videobitrate = Convert.ToInt32(HttpContext.Current.Request.QueryString["videobitrate"]),
                            framerate = Convert.ToInt32(HttpContext.Current.Request.QueryString["framerate"]),
                            audiocodec = Convert.ToString(HttpContext.Current.Request.QueryString["audiocodec"]),
                            audiobitrate = Convert.ToInt32(HttpContext.Current.Request.QueryString["audiobitrate"]),
                            samplingrate = Convert.ToInt32(HttpContext.Current.Request.QueryString["samplingrate"])
                        };


                        string bucketName = "mediamedia";
                        string awsAccessKeyId = "AKIAJPF4ZQUNSJSIICFA";
                        string awsSecretAccessKey = "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";
                        using (var client = new AmazonS3Client(
                            awsAccessKeyId,
                            awsSecretAccessKey,
                            Amazon.RegionEndpoint.EUCentral1))
                        {
                            client.PutObject(new PutObjectRequest()
                            {
                                InputStream = stream,
                                BucketName = bucketName,
                                Key = filename + "." + filetype
                            });
                        }
                        //adds info of file in database
                        using (var db = new MyDbContext())
                        {
                            var currentMemory = db.Memories.Include("mediaList").FirstOrDefault(v => v.id == id);
                            currentMemory.mediaList.Add(video);
                            db.SaveChanges();

                            return Ok("The swag file has been uploaded");
                        }
                    }
               }
           }
           return InternalServerError(new Exception("could not upload"));
          
        }
        //Delete a memory
        public void Delete(int id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim userClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            string myUsername = userClaim.Value;
            using (var db = new MyDbContext())
            {
                User u = db.Users.Include("vacationList.memoryList.mediaList").FirstOrDefault(i => i.username == myUsername); 
                Memory m = db.Memories.Include("mediaList").FirstOrDefault(i => i.id == id);
                
                
                if(u.vacationList.Contains(m.vacation))
                {
                    var mediaList = m.mediaList.ToList();
                    foreach(Media med in mediaList)
                    {
                        db.Medias.Remove(med);
                    }
                    db.Memories.Remove(m);
                    db.SaveChanges();
                }
                else
                {
                    throw new HttpException(400, "Bad request, not your memory");
                }
                
            }
            
        }
    }
}
