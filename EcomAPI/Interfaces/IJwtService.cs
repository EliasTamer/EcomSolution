using EcomAPI.Entities;

namespace EcomAPI.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
