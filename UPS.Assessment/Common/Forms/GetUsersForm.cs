namespace UPS.Assessment.Common.Forms;

public class GetUsersForm
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Gender { get; set; }
    public string? Status { get; set; }

    public int? PageNumber { get; set; } = 1;
}