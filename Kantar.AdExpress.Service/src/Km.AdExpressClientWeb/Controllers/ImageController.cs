using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    public class ImageController : Controller
    {
        // GET: Image
        public ActionResult GetPostImage(string idPostFacebook, string idPost)
        {
            //var url = "http://192.168.158.145/POSTS/";
            var hostName = HttpContext.Request.UrlReferrer.Authority;
            idPostFacebook = idPostFacebook.ToString().PadLeft(10, '0');
            var url = hostName + "/PostsFacebook/";
            //var srvURL = itemId.Substring(0, 1) + "/" + itemId.Substring(1, 3) + "/" + itemId + "_Post";
            //string srvUrl = $@"{idPostFacebook.Substring(idPostFacebook.Length - 4, 2)}/{idPostFacebook.Substring(idPostFacebook.Length - 2, 2)}/";
            var path = Path.Combine("http://", url, idPostFacebook.Substring(idPostFacebook.Length - 4, 2), idPostFacebook.Substring(idPostFacebook.Length - 2, 2), "Post_" + idPost + ".png");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(path);
            request.Method = "HEAD";
            //if (System.IO.File.Exists(path))
            if (exist(request))
            {
                return Redirect(path);
            }
            return File("~/Content/img/no_visu.jpg", "image/jpg");
        }

        public ActionResult GetPageImage(string itemId)
        {
            var hostName = HttpContext.Request.UrlReferrer.Authority;
            var url = hostName + "/PostsFacebook/";
            var srvUrl = "Icons/new_" + itemId;
            var path = Path.Combine("http://", url, srvUrl + ".jpg");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(path);
            request.Method = "HEAD";
            if (exist(request))
            {
                return Redirect(path);
            }
            return File("~/Content/img/no_visu.jpg", "image/jpg");
        }

        private bool exist(HttpWebRequest request)
        {
            try
            {
                request.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}