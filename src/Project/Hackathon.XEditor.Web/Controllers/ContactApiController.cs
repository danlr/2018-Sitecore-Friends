namespace Hackathon.XEditor.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Api.Services;

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

        public async Task<ActionResult> AddTestData()
        {
            bool result = await _xconnectService.CreateContact(TestSource, Guid.NewGuid().ToString("D"),"First1", "Last1", "Mr", "", "test1@sitecorehackathon.com");
            result = await _xconnectService.CreateContact(TestSource, Guid.NewGuid().ToString("D"),"First2", "Last2", "Ms", "", "test2@sitecorehackathon.com");
            result = await _xconnectService.CreateContact(TestSource, Guid.NewGuid().ToString("D"),"First3", "Last3", "", "01234 5678", "test3@sitecorehackathon.com");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
