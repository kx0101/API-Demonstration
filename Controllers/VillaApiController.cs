using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiprac
{
    [Route("api/villas")]
    [ApiController]
    public class VillasApiController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public VillasApiController(ILogging logger, IVillaRepository dbVilla, IMapper mapper)
        {
            _logger = logger;
            _dbVilla = dbVilla;
            _mapper = mapper;

            this._response = new APIResponse();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery] VillaFilterDTO villaFilterDTO)
        {
            _logger.Log("Getting all villas", "");

            try
            {
                VillaFilterBuilder villaFilter = new VillaFilterBuilder();

                villaFilter.setName(villaFilterDTO.Name)
                           .setRate(villaFilterDTO.Rate)
                           .setSqft(villaFilterDTO.Sqft);

                IEnumerable<Villa> villaList = await _dbVilla.FindByCriteriaAsync(villaFilter.Build());

                _response.Data = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);

            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, "error");
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaById(Guid id)
        {
            try
            {
                var villa = await _dbVilla.FindByIdAsync(villa => villa.Id == id);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Villa not found" };

                    _logger.Log("Villa not found" + id, "error");

                    return NotFound();
                }

                _logger.Log($"Returned {villa.Name}" + id, "");

                _response.Data = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, "error");
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (await _dbVilla.FindByIdAsync(villa => villa.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Villa name must be unique" };

                    _logger.Log("Villa name must be unique", "error");
                    return BadRequest(_response);
                }

                if (createDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Villa is empty" };

                    _logger.Log("Villa is empty", "error");

                    return BadRequest(_response);
                }

                if (createDTO.Rate < 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Rate cannot be negative" };

                    _logger.Log("Rate cannot be negative", "error");

                    return BadRequest(_response);
                }

                if (createDTO.Sqft < 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Sqft cannot be negative" };

                    _logger.Log("Sqft cannot be negative", "error");

                    return BadRequest(_response);
                }

                Villa villa = _mapper.Map<Villa>(createDTO);

                await _dbVilla.CreateAsync(villa);
                await _dbVilla.SaveAsync();

                _logger.Log($"Newly villa created {createDTO.Name}", "");

                _response.Data = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

                return CreatedAtAction(nameof(GetVillas), new { Name = villa.Name }, _response);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, "error");
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(Guid id)
        {

            try
            {
                var villa = await _dbVilla.FindByIdAsync(villa => villa.Id == id);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Villa not found" };

                    _logger.Log("Villa not found", "error");
                    return NotFound();
                }

                await _dbVilla.RemoveAsync(villa);
                await _dbVilla.SaveAsync();

                _logger.Log($"Villa {villa.Name} successfully", "");

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, "error");
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(Guid id, [FromBody] VillaUpdateDTO updateDTO)
        {

            try
            {
                var villa = await _dbVilla.FindByIdAsync(villa => villa.Id == id);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Villa is empty" };

                    _logger.Log("Villa is empty", "error");

                    return BadRequest(_response);
                }

                if (updateDTO.Rate < 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Rate cannot be negative" };

                    _logger.Log("Rate cannot be negative", "error");

                    return BadRequest(_response);
                }

                if (updateDTO.Sqft < 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Sqft cannot be negative" };

                    _logger.Log("Sqft cannot be negative", "error");

                    return BadRequest(_response);
                }

                Villa model = _mapper.Map<Villa>(updateDTO);
                model.Id = id;

                await _dbVilla.UpdateAsync(id, model);

                _logger.Log($"Villa {villa.Name} updated successfully", "warning");

                _response.Data = _mapper.Map<VillaDTO>(model);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, "error");
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }
    }
}
