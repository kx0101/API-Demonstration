using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace apiprac
{

    [Route("api/UsersAuth")]
    [ApiController]
    public class UserApiController : Controller
    {
        private readonly IUserRepository _userRepo;
        private APIResponse _response;

        public UserApiController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            this._response = new APIResponse();
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {

            var loginResponse = await _userRepo.Login(model);

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { "Username or password is incorrect" };

                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Data = loginResponse;

            return Ok(_response);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            bool ifUserNameUnique = _userRepo.IsUserUnique(model.UserName);

            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { "Username already exists" };

                return BadRequest(_response);
            }

            var user = await _userRepo.Register(model);

            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { "Error while registering" };

                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Data = user;

            return Ok(_response);
        }
    }
}
