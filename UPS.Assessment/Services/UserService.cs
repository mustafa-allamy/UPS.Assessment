using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UPS.Assessment.Common.Forms;
using UPS.Assessment.Infrastructure.Extensions;
using UPS.Assessment.Infrastructure.Helpers;
using UPS.Assessment.Infrastructure.Interfaces.Services;
using UPS.Assessment.Models;
using UPS.Assessment.Models.GoRestApiResponse;

namespace UPS.Assessment.Services;

public class UserService : IUserService
{
    private readonly IHttpClientHelper _httpClient;
    private readonly GoRestCred _goRestCred;
    public UserService(IHttpClientHelper httpClient, IOptions<GoRestCred> goRestCred)
    {
        _httpClient = httpClient;
        _goRestCred = goRestCred.Value;
    }

    public async Task<ServiceResponse<User>> GetUserById(int userId)
    {

        var response = await _httpClient.Get("public-api/users", userId.ToString(),
            _goRestCred.Token, _goRestCred.BaseUrl);

        if (!response.IsSuccessStatusCode)
            return new ServiceResponse<User>().Failed().WithError("Failed to call API");

        var user = JsonConvert
            .DeserializeObject<GoRestResponse<User>>(await response.Content.ReadAsStringAsync());
        if (user != null && user.Code != (int)HttpStatusCode.OK)
            return new ServiceResponse<User>().Failed().WithError("User not found!");

        return new ServiceResponse<User>().Successful().WithData(user!.Data);
    }

    public async Task<ServiceResponse<List<User>>> GetUsers(GetUsersForm form)
    {

        var methodHeader = "public-api/users?";
        methodHeader = AddSearchHeader(form, methodHeader);

        var response = await _httpClient.Get(methodHeader,
            _goRestCred.Token, _goRestCred.BaseUrl);

        if (!response.IsSuccessStatusCode)
            return new ServiceResponse<List<User>>().Failed()
                .WithError("Failed to call API");

        var users = JsonConvert
            .DeserializeObject<GoRestListResponse>(await response.Content.ReadAsStringAsync());

        return new ServiceResponse<List<User>>()
            .Successful().WithData(users!.Data).WithCount(users.Meta.Pagination.Pages);
    }
    public async Task<ServiceResponse<User>> AddUser(CreateUserForm form)
    {

        var postData = JsonConvert.SerializeObject(form);
        var response = await _httpClient
            .Post("public-api/users", postData, _goRestCred.Token, _goRestCred.BaseUrl);

        if (!response.IsSuccessStatusCode)
            return new ServiceResponse<User>()
                .Failed().WithError("Failed to call API");

        try
        {
            var user = JsonConvert
                .DeserializeObject<GoRestResponse<User>>(await response.Content.ReadAsStringAsync());
            return new ServiceResponse<User>().Successful().WithData(user.Data);
        }
        catch (Exception)
        {
            var errors = JsonConvert.DeserializeObject<GoRestResponse<List<GoRestError>>>
                (await response.Content.ReadAsStringAsync());
            return new ServiceResponse<User>().Failed()
                .WithErrors(errors.Data.Select(x =>
                    new ResponseError($"{x.Field}: {x.Message}")).ToList());
        }

    }


    public async Task<ServiceResponse> DeleteUser(int userId)
    {

        var response = await _httpClient
            .Delete("public-api/users", userId.ToString(), _goRestCred.Token, _goRestCred.BaseUrl);
        if (!response.IsSuccessStatusCode)
            return new ServiceResponse<bool>().Failed().WithError("Failed to call API");

        var user = JsonConvert
            .DeserializeObject<GoRestResponse<User>>(await response.Content.ReadAsStringAsync());
        if (user != null && user.Code != (int)HttpStatusCode.NoContent)
            return new ServiceResponse<bool>().Failed().WithError("User not found!");

        return new ServiceResponse().Successful();
    }


    private string AddSearchHeader(GetUsersForm form, string method)
    {
        if (!string.IsNullOrEmpty(form.Name))
            method += $"name={form.Name}&";

        if (!string.IsNullOrEmpty(form.Email))
            method += $"email={form.Email}&";

        if (!string.IsNullOrEmpty(form.Gender))
            method += $"gender={form.Gender}&";

        if (!string.IsNullOrEmpty(form.Status))
            method += $"status={form.Status}&";

        if (form.PageNumber is not null && form.PageNumber > 0)
            method += $"page={form.PageNumber}";
        return method;
    }
}