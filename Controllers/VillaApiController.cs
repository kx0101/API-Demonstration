using Microsoft.AspNetCore.Mvc;

namespace apiprac
{
    [Route("api/villas")]
    [ApiController]
    public class VillasApiController : ControllerBase
    {
        private readonly ILogging _logger;

        public VillasApiController(ILogging logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.Log("Getting all villas", "");

            return Ok(VillaStore.villaList);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id <= 0)
            {
                _logger.Log("Invalid id: " + id, "error");

                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);

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

            if (VillaStore.villaList.FirstOrDefault(villa => villa.Name.ToLower() == villaDTO.Name.ToLower()) != null)
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

            if (villaDTO.Id > 0)
            {
                _logger.Log($"Invalid id: {villaDTO.Id}", "error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDTO.Id = VillaStore.villaList.OrderByDescending(villa => villa.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);

            _logger.Log($"Newly villa created {villaDTO.Name}", "");
            return CreatedAtAction(nameof(GetVillas), new { id = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                _logger.Log("Invalid id: " + id, "error");
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);

            if (villa == null)
            {
                _logger.Log("Villa is empty", "error");
                return NotFound();
            }

            VillaStore.villaList.Remove(villa);

            _logger.Log($"Villa {villa.Name} successfully", "");
            return Ok($"{villa.Name} Villa deleted successfully");
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (id == 0)
            {
                _logger.Log("Invalid id: " + id, "error");
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);

            if (villa == null)
            {
                _logger.Log("Villa is empty", "error");
                return NotFound();
            }

            villa.Name = villaDTO.Name;

            _logger.Log($"Villa {villa.Name} updated successfully", "warning");
            return Ok(villa);
        }
    }
}
