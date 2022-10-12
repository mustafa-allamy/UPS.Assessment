using System.Collections.Generic;
using System.Threading.Tasks;
using UPS.Assessment.Common.Forms;
using UPS.Assessment.Infrastructure.Helpers;
using UPS.Assessment.Models;

namespace UPS.Assessment.Infrastructure.Interfaces.Services;

public interface IUserService
{
    Task<ServiceResponse<User>> GetUserById(int userId);
    Task<ServiceResponse<List<User>>> GetUsers(GetUsersForm form);
    Task<ServiceResponse<User>> AddUser(CreateUserForm form);
    Task<ServiceResponse> DeleteUser(int userId);
}