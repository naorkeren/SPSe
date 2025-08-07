using Microsoft.AspNetCore.Mvc;
using SurfingPointServer.Models;
using SurfingPointServer.Services;
using System.Collections.Generic;

namespace SurfingPointServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeachesController : ControllerBase
    {
        // שליפת כל החופים – פתוח לכולם
        [HttpGet]
        public ActionResult<List<Beach>> GetAllBeaches()
        {
            var beaches = DBServices.GetAllBeaches();
            return Ok(beaches);
        }

        // שליפת פרטי כל החופים המורחבים – פתוח לכולם
        [HttpGet("details")]
        public ActionResult<List<BeachDetails>> GetAllBeachDetails()
        {
            var details = DBServices.GetAllBeachDetails();
            return Ok(details);
        }

        // שליפת פרטי חוף בודד לפי ID – פתוח לכולם
        [HttpGet("{id}/details")]
        public ActionResult<BeachDetails> GetBeachDetailsById(int id)
        {
            var details = DBServices.GetBeachDetailsById(id);
            if (details == null)
                return NotFound();
            return Ok(details);
        }
    }
}
