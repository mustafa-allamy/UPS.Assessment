using Moq;
using Newtonsoft.Json;
using System.Net;
using UPS.Assessment.Common.Forms;
using UPS.Assessment.Infrastructure.Interfaces.Services;
using UPS.Assessment.Models;
using UPS.Assessment.Models.GoRestApiResponse;

namespace UPS.Assessment.UnitTests;

public static class UsersDataSetup
{

    private static Meta Meta()
    {
        var meta = new Meta()
        {
            Pagination = new Pagination()
            {
                Page = 1
            }
        };
        return meta;
    }
    private static List<User> SetupUsersList()
    {
        var usersList = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "John doe",
                Email = "john@gmail.com",
                Gender = "male",
                Status = "active"
            },
            new User
            {
                Id = 1,
                Name = "Mustafa mohsin",
                Email = "mustafa@gmail.com",
                Gender = "male",
                Status = "active"
            },
            new User
            {
                Id = 1,
                Name = "John smith",
                Email = "john2@gmail.com",
                Gender = "male",
                Status = "inactive"
            }
        };

        return usersList;
    }

    public static void SetupGetListResponse_Success(Mock<IHttpClientHelper> httpClient)
    {

        httpClient.Setup(x =>
                x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string methodHeader, string token, string url) =>
            {
                var methodParams = methodHeader.Split('?')[1];
                var filters = methodParams.Split('&');
                var nameFilter = filters.FirstOrDefault(x => x.Contains("name"))?.Split('=')[1];
                var emailFilter = filters.FirstOrDefault(x => x.Contains("email"))?.Split('=')[1];
                var genderFilter = filters.FirstOrDefault(x => x.Contains("gender"))?.Split('=')[1];
                var statusFilter = filters.FirstOrDefault(x => x.Contains("status"))?.Split('=')[1];

                return GetUsersResponse(SetupUsersList(), Meta(), nameFilter, emailFilter, genderFilter, statusFilter);
            });
    }
    private static async Task<HttpResponseMessage> GetUsersResponse(List<User> usersList,
        Meta meta, string? name, string? email, string? gender, string? status)
    {
        if (!string.IsNullOrWhiteSpace(name))
            usersList = usersList.Where(x => x.Name.Contains(name)).ToList();

        if (!string.IsNullOrWhiteSpace(email))
            usersList = usersList.Where(x => x.Email.Contains(email)).ToList();

        if (!string.IsNullOrWhiteSpace(gender))
            usersList = usersList.Where(x => x.Gender == gender).ToList();

        if (!string.IsNullOrWhiteSpace(status))
            usersList = usersList.Where(x => x.Status == status).ToList();

        return new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                JsonConvert.SerializeObject(new GoRestListResponse()
                {
                    Code = (int)HttpStatusCode.OK,
                    Meta = meta,
                    Data = usersList.ToList()
                }))
        };
    }
    public static void SetupGetListResponses_Fail(Mock<IHttpClientHelper> httpClient)
    {
        httpClient.Setup(x =>
                x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string methodHeader, string token, string url) =>
            {
                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,

                });
            });
    }

    public static void SetupAddUserSuccess(Mock<IHttpClientHelper> _httpClient)
    {
        var usersList = SetupUsersList();
        _httpClient.Setup(x =>
            x.Post(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string a, string data, string b, string c) =>
           {
               var user = JsonConvert.DeserializeObject<CreateUserForm>(data);
               usersList.Add(new User()
               {
                   Email = user.email,
                   Status = user.status,
                   Name = user.name,
                   Gender = user.gender,
                   Id = usersList.Count() + 1
               });
               return Task.FromResult(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(
                       JsonConvert.SerializeObject(new GoRestResponse<User>
                       {
                           Code = (int)HttpStatusCode.OK,
                           Meta = Meta(),
                           Data = usersList.Last()
                       }))
               });
           }
       );
    }

    public static void SetupAddUserFail(Mock<IHttpClientHelper> _httpClient)
    {
        var errors = new List<GoRestError>()
        {
            new GoRestError()
            {
                Field = "name",
                Message = "can't be blank"
            },
             new GoRestError()
            {
                Field = "email",
                Message = "is invalid"
            }
        };
        _httpClient.Setup(x =>
            x.Post(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string a, string data, string b, string c) =>
            {
                var user = JsonConvert.DeserializeObject<CreateUserForm>(data);
                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new GoRestResponse<List<GoRestError>>
                        {
                            Code = (int)HttpStatusCode.UnprocessableEntity,
                            Meta = Meta(),
                            Data = errors
                        }))
                });
            }
        );
    }


    public static void SetupDeleteUserSuccess(Mock<IHttpClientHelper> _httpClient)
    {
        _httpClient.Setup(x =>
            x.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string a, string data, string b, string c) =>
            {
                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new GoRestResponse<User>
                        {
                            Code = (int)HttpStatusCode.NoContent,
                            Meta = null,
                            Data = null
                        }))
                });
            }
        );
    }
}