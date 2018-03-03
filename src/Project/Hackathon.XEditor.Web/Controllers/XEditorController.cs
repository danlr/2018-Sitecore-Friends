using System.Threading.Tasks;
using System.Web;
using Hackathon.XEditor.Api.Dto;
using Hackathon.XEditor.Api.Services;

namespace Hackathon.XEditor.Web.Controllers
{
    using System;
    using System.IO;
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
            HttpPostedFileBase avatar = HttpContext.Request.Files["AvatarFile"];

            byte[] avatarBytes = null;
            if (avatar != null && avatar.ContentLength > 0)
            {
                MemoryStream target = new MemoryStream();
                avatar.InputStream.CopyTo(target);
                avatarBytes = target.ToArray();
            }

            var result = Task.Run(() => _xconnectService.UpdateContactInformation(model.ContactId, model.PersonalInformation.FirstName, model.PersonalInformation.MiddleName, model.PersonalInformation.LastName, model.PersonalInformation.Title, model.PersonalInformation.JobTitle, model.Phone, model.Email, avatarBytes)).Result;
            return Json(result);
        }

        [HttpPost]
        public JsonResult EditorFacet(FacetDto model)
        {
            model.ContactId = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[ContactIdParam];
            var result = Task.Run(() => _xconnectService.UpdateContactFacet(model)).Result;
            var message = result ? "Successfully saved" : "An error occured";
            return Json(message);
        }
    }
}