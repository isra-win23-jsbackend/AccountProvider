

namespace AccountProvider.Models;

public class VerifyRequest
{
    public string Email { get; set; } = null!;
    public string VeryfyCode { get; set; } = null!;


}
