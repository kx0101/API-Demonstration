using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiprac
{
    [Route("api/villas")]
    [ApiController]
    public class VillasApiController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly ApplicationDBContext _db;
        private readonly IMapper _mapper;

        public VillasApiController(ILogging logger, ApplicationDBContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logger.Log("Getting all villas", "");

            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();

            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDTO>> GetVilla(Guid id)
        {
            var villa = await _db.Villas.FirstOrDefaultAsync(villa => villa.Id == id);

            if (villa == null)
            {
                _logger.Log("Villa not found" + id, "error");

                return NotFound();
            }

            _logger.Log($"Returned {villa.Name}" + id, "");
            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            if (await _db.Villas.FirstOrDefaultAsync(villa => villa.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa name must be unique");

                _logger.Log("Villa name must be unique", "error");
                return BadRequest(ModelState);
            }

            if (createDTO == null)
            {
                _logger.Log("Villa is empty", "error");
                return BadRequest(createDTO);
            }

            if (createDTO.Rate < 0)
            {
                _logger.Log("Rate cannot be negative", "error");
                return BadRequest(createDTO);
            }

            if (createDTO.Sqft < 0)
            {
                _logger.Log("Sqft cannot be negative", "error");
                return BadRequest(createDTO);
            }

            Villa model = _mapper.Map<Villa>(createDTO);

            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();

            _logger.Log($"Newly villa created {createDTO.Name}", "");
            return CreatedAtAction(nameof(GetVillas), new { Name = createDTO.Name }, createDTO);
        }

        [HttpDelete("{id:guid}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(Guid id)
        {
            var villa = await _db.Villas.FirstOrDefaultAsync(villa => villa.Id == id);

            if (villa == null)
            {
                _logger.Log("Villa is empty", "error");
                return NotFound();
            }

            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();

            _logger.Log($"Villa {villa.Name} successfully", "");
            return Ok($"{villa.Name} Villa deleted successfully");
        }

        [HttpPut("{id:guid}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(Guid id, [FromBody] VillaUpdateDTO updateDTO)
        {
            var villa = await _db.Villas.FirstOrDefaultAsync(villa => villa.Id == id);

            if (villa == null)
            {
                _logger.Log("Villa is empty", "error");
                return NotFound();
            }

            if (updateDTO.Rate < 0)
            {
                _logger.Log("Rate cannot be negative", "error");
                return BadRequest(updateDTO);
            }

            if (updateDTO.Sqft < 0)
            {
                _logger.Log("Sqft cannot be negative", "error");
                return BadRequest(updateDTO);
            }

            Villa model = _mapper.Map<Villa>(updateDTO);

            await _db.SaveChangesAsync();

            _logger.Log($"Villa {villa.Name} updated successfully", "warning");
            return Ok(villa);
        }
    }
}
