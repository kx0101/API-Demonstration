namespace apiprac
{
    public interface IUserRepository
    {
        bool IsUserUnique(string username);

        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);

        Task<LocalUser> Register(RegisterRequestDTO registerRequestDTO);
    }
}
