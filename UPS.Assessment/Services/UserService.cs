using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UPS.Assessment.Common.Forms;
using UPS.Assessment.Infrastructure.Extensions;
using UPS.Assessment.Infrastructure.Helpers;
using UPS.Assessment.Infrastructure.Interfaces.Services;
using UPS.Assessment.Models;
using UPS.Assessment.Models.GoRestApiResponse;

namespace UPS.Assessment.Services;

public class UserService:IUserService
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
        try
        {
            var response = await _httpClient.Get("public-api/users", userId.ToString(), _goRestCred.Token, _goRestCred.BaseUrl);
            if (!response.IsSuccessStatusCode)
                return new ServiceResponse<User>().Failed();

            var user = JsonConvert.DeserializeObject<GoRestResponse<User>>(await response.Content.ReadAsStringAsync());
            if (user != null && user.Code!=200)
                return new ServiceResponse<User>().Failed().WithError("User not found!");

            return new ServiceResponse<User>().Successful().WithData(user!.Data);
        }
        catch
        {
            return new ServiceResponse<User>().Failed().WithError("Something went wrong,Don't worry it's not your fault");
        }
    }

    public async Task<ServiceResponse<List<User>>> GetUsers(GetUsersForm form)
    {
        try
        {
            var method = "public-api/users?";
            method=AddSearchHeader(form, method);

            var response = await _httpClient.Get(method,  _goRestCred.Token, _goRestCred.BaseUrl);
            if (!response.IsSuccessStatusCode)
                return new ServiceResponse<List<User>>().Failed();

            var res = response.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<GoRestListResponse>(await response.Content.ReadAsStringAsync());
            if (users != null && users.Code != 200)
                return new ServiceResponse<List<User>>().Failed().WithError("User not found!");

            return new ServiceResponse<List<User>>().Successful().WithData(users!.Data).WithCount(users.Meta.Pagination.Pages);
        }
        catch
        {
            return new ServiceResponse<List<User>>().Failed().WithError("Something went wrong,Don't worry it's not your fault");
        }
    }
    public async Task<ServiceResponse<User>> AddUser(CreateUserForm form)
    {
        try
        {
            var postData = JsonConvert.SerializeObject(form);
            var response = await _httpClient.Post("public-api/users", postData, _goRestCred.Token, _goRestCred.BaseUrl);
            if (!response.IsSuccessStatusCode)
                return new ServiceResponse<User>().Failed();

            return await DeserlizeAddUserResponse(response);
        }
        catch
        {
            return new ServiceResponse<User>().Failed().WithError("Something went wrong,Don't worry it's not your fault");
        }
    }

    private  async Task<ServiceResponse<User>> DeserlizeAddUserResponse(HttpResponseMessage response)
    {
        try
        {
            var user = JsonConvert.DeserializeObject<GoRestResponse<User>>(await response.Content.ReadAsStringAsync());
            return new ServiceResponse<User>().Successful().WithData(user!.Data);
        }
        catch
        {
            var errors =
                JsonConvert.DeserializeObject<GoRestResponse<List<GoRestError>>>(
                    await response.Content.ReadAsStringAsync());
            return new ServiceResponse<User>().Failed()
                .WithErrors(errors.Data.Select(x => new ResponseError($"{x.Field}: {x.Message}")).ToList());
        }
    }

    public async Task<ServiceResponse<bool>> DeleteUser(int userId)
    {
        try
        {
            var response = await _httpClient.Delete("public-api/users", userId.ToString(), _goRestCred.Token, _goRestCred.BaseUrl);
            if (!response.IsSuccessStatusCode)
                return new ServiceResponse<bool>().Failed();

            var user = JsonConvert.DeserializeObject<GoRestResponse<User>>(await response.Content.ReadAsStringAsync());
            if (user != null && user.Code != 204)
                return new ServiceResponse<bool>().Failed().WithError("User not found!");

            return new ServiceResponse<bool>().Successful().WithData(true);
        }
        catch
        {
            return new ServiceResponse<bool>().Failed().WithError("Something went wrong,Don't worry it's not your fault");
        }
    }


    private static string AddSearchHeader(GetUsersForm form, string method)
    {
        if (!string.IsNullOrEmpty(form.Name))
           method+= $"name={form.Name}&";

        if (!string.IsNullOrEmpty(form.Email))
           method+=$"email={form.Email}&";

        if (!string.IsNullOrEmpty(form.Gender))
           method+= $"gender={form.Gender}&";

        if (!string.IsNullOrEmpty(form.Status))
           method+= $"status={form.Status}&";

        if (form.PageNumber is not null&&form.PageNumber>0)
           method+= $"page={form.PageNumber}";
        return method;
    }
}