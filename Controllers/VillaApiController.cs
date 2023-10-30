using Microsoft.AspNetCore.Mvc;

namespace apiprac
{
    [Route("api/villas")]
    [ApiController]
    public class VillasApiController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly ApplicationDBContext _db;

        public VillasApiController(ILogging logger, ApplicationDBContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.Log("Getting all villas", "");

            return Ok(_db.Villas);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDTO> GetVilla(Guid id)
        {
            var villa = _db.Villas.FirstOrDefault(villa => villa.Id == id);

            if (villa == null)
            {
                _logger.Log("Villa not found" + id, "error");

                return NotFound();
            }

            _logger.Log($"Returned {villa.Name}" + id, "");
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {

            if (_db.Villas.FirstOrDefault(villa => villa.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa name must be unique");

                _logger.Log("Villa name must be unique", "error");
                return BadRequest(ModelState);
            }

            if (villaDTO == null)
            {
                _logger.Log("Villa is empty", "error");
                return BadRequest(villaDTO);
            }

            if (villaDTO.Rate < 0)
            {
                _logger.Log("Rate cannot be negative", "error");
                return BadRequest(villaDTO);
            }

            if (villaDTO.Sqft < 0)
            {
                _logger.Log("Sqft cannot be negative", "error");
                return BadRequest(villaDTO);
            }

            var newVilla = new Villa
            {
                Name = villaDTO.Name,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
            };

            _db.Villas.Add(newVilla);
            _db.SaveChanges();

            _logger.Log($"Newly villa created {villaDTO.Name}", "");
            return CreatedAtAction(nameof(GetVillas), new { Name = villaDTO.Name }, villaDTO);
        }

        [HttpDelete("{id:guid}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(Guid id)
        {
            var villa = _db.Villas.FirstOrDefault(villa => villa.Id == id);

            if (villa == null)
            {
                _logger.Log("Villa is empty", "error");
                return NotFound();
            }

            _db.Villas.Remove(villa);
            _db.SaveChanges();

            _logger.Log($"Villa {villa.Name} successfully", "");
            return Ok($"{villa.Name} Villa deleted successfully");
        }

        [HttpPut("{id:guid}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(Guid id, [FromBody] VillaDTO villaDTO)
        {
            var villa = _db.Villas.FirstOrDefault(villa => villa.Id == id);

            if (villa == null)
            {
                _logger.Log("Villa is empty", "error");
                return NotFound();
            }

            if (villaDTO.Rate < 0)
            {
                _logger.Log("Rate cannot be negative", "error");
                return BadRequest(villaDTO);
            }

            if (villaDTO.Sqft < 0)
            {
                _logger.Log("Sqft cannot be negative", "error");
                return BadRequest(villaDTO);
            }

            villa.Name = villaDTO.Name;
            villa.Rate = villaDTO.Rate;
            villa.Sqft = villaDTO.Sqft;

            _db.SaveChanges();

            _logger.Log($"Villa {villa.Name} updated successfully", "warning");
            return Ok(villa);
        }
    }
}
