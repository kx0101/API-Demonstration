using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace apiprac
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _db;
        private string secretKey;

        public UserRepository(ApplicationDBContext db, IConfiguration configuration)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("AppSettings:Secret");
        }

        public bool IsUserUnique(string username)
        {
            var user = _db.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return true;
            }

            return false;
        }

        public bool VerifyPassword(LocalUser user, string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, user.Salt) == user.Password;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.Users.FirstOrDefault(x => x.UserName == loginRequestDTO.UserName);

            if (user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null,
                };
            }

            bool isUserVerified = VerifyPassword(user, loginRequestDTO.Password);

            if (!isUserVerified)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null,
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, user.Role),
                        }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = "Bearer " + tokenHandler.WriteToken(token),
                User = user,
            };

            return loginResponseDTO;
        }

        public async Task<LocalUser> Register(RegisterRequestDTO registerRequestDTO)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequestDTO.Password, salt);

            LocalUser user = new LocalUser()
            {
                UserName = registerRequestDTO.UserName,
                Password = hashedPassword,
                Name = registerRequestDTO.Name,
                Role = registerRequestDTO.Role,
                Salt = salt,
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }
    }
}
