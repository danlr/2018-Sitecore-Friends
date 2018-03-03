namespace Hackathon.XEditor.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    public class XEditorController : Controller
    {
        public ActionResult EditorForm(Guid id)
        {
            return Content("Empty rendering");
        }
    }
}