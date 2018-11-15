using HuntingSpots.Model.Domain;
using HuntingSpots.Model.Service;
using HuntingSpots.Model.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HuntingSpots.WebLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly ILocationService _locationService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _locationService = new LocationService(Program.GcpProjectId);
        }

        public IActionResult Index()
        {
            // Sends a message to configured loggers, including the Stackdriver logger.
            // The Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker logger will log all controller actions with
            // log level information. This log is for additional information.
            //_logger.LogInformation("Home page hit!");

            var all = _locationService.GetAllLocations();

            return View("List", all);
        }

        public IActionResult Map()
        {
            var all = _locationService.GetAllLocations();

            ViewData["Locations"] = JsonConvert.SerializeObject(all);

            return View();
        }

        public IActionResult Error()
        {
            // Log messages with different log levels.
            _logger.LogError("Error page hit!");
            return View();
        }

        public IActionResult Edit(long id)
        {
            var location = _locationService.Get(id);

            return View("Edit", location);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var location = _locationService.Get(id.Value);
            
            if (await TryUpdateModelAsync(location))
            {
                try
                {
                    _locationService.UpsertLocation(location);

                    return RedirectToAction("Index");
                }
                catch (Exception /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(location);
        }
        
        public IActionResult Create(decimal? latitude, decimal? longitude)
        {
            var location = new Location
            {
                Latitude = latitude ?? 0,
                Longitude = longitude ?? 0
            };

            return View("Create", location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(nameof(Location.Name), nameof(Location.Latitude), nameof(Location.Longitude))]Location location)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _locationService.UpsertLocation(location);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(location);
        }
        
        public ActionResult Delete(long? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return BadRequest();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var location = _locationService.Get(id.Value);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            try
            {
                _locationService.DeleteLocation(id);
            }
            catch (Exception/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }
    }
}
