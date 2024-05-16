

namespace AccountProvider.Models;

public class UserSignInRequest
{

    public string Email { get; set; } = null!;
    public string Password { get; set; } =null!;
    public bool IsPresistent { get; set;} 

}
