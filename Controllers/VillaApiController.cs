using System.Net;
using AutoMapper;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            _logger.Log("Getting all villas", "");

            try
            {
                IEnumerable<Villa> villaList = await _dbVilla.FindByCriteriaAsync();

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetVillaById(Guid id)
        {
            try
            {
                var villa = await _dbVilla.FindByIdAsync(villa => villa.Id == id);

                if (villa == null)
                {
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (await _dbVilla.FindByIdAsync(villa => villa.Name.ToLower() == createDTO.Name.ToLower()) != null)
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

        [HttpDelete("{id:guid}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(Guid id)
        {

            try
            {
                var villa = await _dbVilla.FindByIdAsync(villa => villa.Id == id);

                if (villa == null)
                {
                    _logger.Log("Villa is empty", "error");
                    return NotFound();
                }

                await _dbVilla.RemoveAsync(villa);
                await _dbVilla.SaveAsync();

                _logger.Log($"Villa {villa.Name} successfully", "");

                _response.StatusCode = HttpStatusCode.NoContent;
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

        [HttpPut("{id:guid}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(Guid id, [FromBody] VillaUpdateDTO updateDTO)
        {

            try
            {
                var villa = await _dbVilla.FindByIdAsync(villa => villa.Id == id);

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
