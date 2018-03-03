namespace Hackathon.XEditor.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Services;

    public class ContactApiController : Controller
    {
        private XconnectService _xconnectService;

        private const string TestSource = "test";

        public ContactApiController() : this(new XconnectService())
        {
        }

        public ContactApiController(XconnectService xconnectService)
        {
            _xconnectService = xconnectService;
        }

        public async Task<ActionResult> TestAdd()
        {
            bool result = await _xconnectService.CreateContact(TestSource, Guid.NewGuid().ToString("D"),"First", "Last", "Mr", "", "test@sitecorehackathon.com");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestReadFacets()
        {
            var facets = _xconnectService.GetAllFacets();

            return Json(facets, JsonRequestBehavior.AllowGet);
        }
    }
}
