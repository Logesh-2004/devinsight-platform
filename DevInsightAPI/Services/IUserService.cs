using DevInsightAPI.DTOs;

namespace DevInsightAPI.Services
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllUsers();

        Task<UserDTO?> GetUserById(int id);

        Task<UserDTO> CreateUser(CreateUserDTO userDto);

        Task<UserDTO?> UpdateUser(int id, UpdateUserDTO userDto);

        Task<bool> DeleteUser(int id);
    }
}
