using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace apiprac
{
    [Route("api/villa-numbers")]
    [ApiController]
    public class VillasNumberApiController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public VillasNumberApiController(ILogging logger, IVillaNumberRepository dbVillaNumber, IVillaRepository dbVilla, IMapper mapper)
        {
            _logger = logger;
            _dbVillaNumber = dbVillaNumber;
            _dbVilla = dbVilla;
            _mapper = mapper;

            this._response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers([FromQuery] VillaNumberFilterDTO villaNumberFilterDTO, int page = 1, int PageSize = 10)
        {
            _logger.Log("Getting all villa numbers", "");

            try
            {
                VillaNumberFilterBuilder villaNumberFilter = new VillaNumberFilterBuilder();

                villaNumberFilter.setVillaId(villaNumberFilterDTO.VillaId)
                                 .setVillaNo(villaNumberFilterDTO.VillaNo)
                                 .setSpecialDetails(villaNumberFilterDTO.SpecialDetails);

                IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.FindByCriteriaAsync(villaNumberFilter.Build(), page, PageSize);

                _response.Data = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Page = page;
                _response.PageSize = PageSize;
                _response.TotalCount = villaNumberList.Count();

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
        public async Task<ActionResult<APIResponse>> GetVillaNumberById(Guid id)
        {
            try
            {
                var villaNumber = await _dbVillaNumber.FindByIdAsync(villa => villa.VillaNo == id);

                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "VillaNumber not found" };

                    _logger.Log("VillaNumber not found" + id, "error");

                    return NotFound(_response);
                }

                _logger.Log($"Returned {villaNumber.VillaNo}" + id, "");

                _response.Data = _mapper.Map<VillaNumberDTO>(villaNumber);
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
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
        {
            try
            {
                if (await _dbVilla.FindByIdAsync(u => u.Id == createDTO.VillaId) == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Villa ID is invalid" };

                    _logger.Log("Villa ID is invalid", "error");

                    return BadRequest(_response);
                }

                if (createDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "VillaNumber is empty" };

                    _logger.Log("VillaNumber is empty", "error");

                    return BadRequest(_response);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);

                await _dbVillaNumber.CreateAsync(villaNumber);
                await _dbVillaNumber.SaveAsync();

                _response.Data = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

                return CreatedAtAction(nameof(GetVillaNumbers), new { SpecialDetails = villaNumber.SpecialDetails }, _response);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.Log("Inner Exception: " + ex.InnerException.Message, "error");
                }

                _logger.Log(ex.Message, "error");
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(Guid id)
        {

            try
            {
                var villaNumber = await _dbVillaNumber.FindByIdAsync(villa => villa.VillaNo == id);

                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "VillaNumber not found" };

                    _logger.Log("VillaNumber not found", "error");
                    return NotFound();
                }

                await _dbVillaNumber.RemoveAsync(villaNumber);
                await _dbVillaNumber.SaveAsync();

                _logger.Log($"VillaNumber {villaNumber.VillaNo} successfully", "");

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

        [HttpPut("{id:guid}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(Guid id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {

            try
            {
                var villaNumber = await _dbVillaNumber.FindByIdAsync(villa => villa.VillaNo == id);

                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "VillaNumber not found" };

                    _logger.Log("VillaNumber is empty", "error");
                    return NotFound();
                }

                if (await _dbVilla.FindByIdAsync(u => u.Id == updateDTO.VillaId) == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Villa ID is invalid" };

                    _logger.Log("Villa ID is invalid", "error");

                    return BadRequest(_response);
                }


                VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);
                model.VillaNo = id;

                await _dbVillaNumber.UpdateAsync(id, model);

                _logger.Log($"VillaNumber {villaNumber.VillaNo} updated successfully", "warning");

                _response.Data = _mapper.Map<VillaNumberDTO>(model);
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
