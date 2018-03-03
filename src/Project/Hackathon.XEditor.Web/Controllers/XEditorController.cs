﻿using System.Threading.Tasks;
using System.Web;
using Hackathon.XEditor.Api.Dto;
using Hackathon.XEditor.Api.Services;

namespace Hackathon.XEditor.Web.Controllers
{
    using System;
    using System.Web.Mvc;
    
    public class XEditorController : Controller
    {
        private XconnectService _xconnectService;

        private static string ContactIdParam = "cid";

        public XEditorController() : this(new XconnectService())
        {
        }

        public XEditorController(XconnectService xconnectService)
        {
            _xconnectService = xconnectService;
        }

        [HttpGet]
        public ActionResult EditorForm()
        {
            string cid = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[ContactIdParam];
            ContactDto model = Task.Run(() => _xconnectService.GetContact(new Guid(cid))).Result;

            return View("/Views/XEditor/UserDataEditorForm.cshtml", model);
        }

        [HttpPost]
        public JsonResult EditorForm(ContactDto model)
        {
            var result = Task.Run(() => _xconnectService.UpdateContactInformation(model.ContactId, model.PersonalInformation.FirstName, model.PersonalInformation.LastName, model.PersonalInformation.Title, model.PersonalInformation.JobTitle, model.Phone, model.Email)).Result;
            return Json(result);
        }
    }
}