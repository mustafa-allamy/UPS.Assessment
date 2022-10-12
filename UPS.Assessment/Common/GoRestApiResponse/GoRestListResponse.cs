using System.Collections.Generic;

namespace UPS.Assessment.Models.GoRestApiResponse;

public class GoRestListResponse
{

    public int Code { get; set; }
    public Meta Meta { get; set; }
    public List<User> Data { get; set; }
}