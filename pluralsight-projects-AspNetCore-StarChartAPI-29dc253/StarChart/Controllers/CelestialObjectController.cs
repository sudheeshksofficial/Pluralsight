using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id:int}" , Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
            {
                return NotFound();
            }
            else
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(Cid => Cid.OrbitedObjectId == id).ToList();

                return Ok(celestialObject);
            }

        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = _context.CelestialObjects.Where(o => o.Name == name);
            if (!celestialObject.Any())
                return NotFound();
            foreach (var item in celestialObject)
            {
                item.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(celestialObject.ToList());

        }


        [HttpGet]

        public IActionResult GetAll()
        {
            var celestialObject = _context.CelestialObjects.ToList();

            foreach (var item in celestialObject)
            {
                item.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(celestialObject);

        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id },celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id,CelestialObject celestialObject)
        {
            var temp = _context.CelestialObjects.Find(id);
            if(temp == null)
                return NotFound();
            temp.Name = celestialObject.Name;
            temp.OrbitalPeriod = celestialObject.OrbitalPeriod;
            temp.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(temp);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var temp = _context.CelestialObjects.Find(id);
            if (temp == null)
                return NotFound();
            temp.Name = name;
            _context.CelestialObjects.Update(temp);
            _context.SaveChanges();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialobject = _context.CelestialObjects.Where(o => o.OrbitedObjectId== id || o.Id == id);

            if (!celestialobject.Any())
                return NotFound();
            _context.CelestialObjects.RemoveRange(celestialobject);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
