using PolyclinicCore;

namespace PolyclinicDomain.Entities;

public class User
{
    public string Email { get; private set; }
    public string Password { get; private set; }
    public RoleUser Role { get; private set; }

    public User(string email, string password, RoleUser role)
    {
        Email = email;
        Password = password;
        Role = role;
    }
}