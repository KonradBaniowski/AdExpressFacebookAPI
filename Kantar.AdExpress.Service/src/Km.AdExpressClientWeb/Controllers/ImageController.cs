﻿using System;
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
        public ActionResult GetPostImage(string itemId)
        {
            var url = "http://192.168.158.145/POSTS/";
            var srvURL = itemId.Substring(0, 1) + "/" + itemId.Substring(1, 3) + "/" + itemId + "_Post";
            var path = Path.Combine(url, srvURL + ".png");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(path);
            request.Method = "HEAD";
            //if (System.IO.File.Exists(path))
            if (exist(request))
            {
                return Redirect(path);
            }
            else
            {
                return File("~/Content/img/no_visu.jpg", "image/jpg");
            }
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