namespace UPS.Assessment.Models.GoRestApiResponse;

public class GoRestResponse<T>
{
    public GoRestResponse() : base() { }

    public int Code { get; set; }
    public Meta Meta { get; set; }
    public T Data { get; set; }
}