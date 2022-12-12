using ApiJWT.Models;

namespace ApiJWT.Repositories
{
    public static class UserRepository
    {
        public static User Get(string username, string password)
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(),  Username = "Maria", Password = "**//**--", Role = "manager",   CreatedAt = DateTimeOffset.Now},
                new User { Id = Guid.NewGuid(), Username = "João", Password = "**//**--", Role = "employee",  CreatedAt = DateTimeOffset.Now }
            };
            return users.Where(x => x.Username.ToLower() == username.ToLower() && x.Password == password).FirstOrDefault();
        }

    }
}