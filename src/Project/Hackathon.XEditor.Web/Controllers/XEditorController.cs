namespace Hackathon.XEditor.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Api.Services;

    using Models;

    public class XEditorController : Controller
    {
        private XconnectService _xconnectService;

        public XEditorController() : this(new XconnectService())
        {
        }

        public XEditorController(XconnectService xconnectService)
        {
            _xconnectService = xconnectService;
        }

        public ActionResult EditorForm(Guid id)
        {
            return Content("Empty rendering");
        }

        [HttpPost]
        public ActionResult EditorForm(EditContactRequestModel model)
        {
            var result = Task.Run(() => _xconnectService.UpdateContactInformation(model.ContactId, model.FirstName, model.LastName, model.Title, model.JobTitle, model.Phone, model.Email)).Result;
            return this.View(model);
        }
    }
}