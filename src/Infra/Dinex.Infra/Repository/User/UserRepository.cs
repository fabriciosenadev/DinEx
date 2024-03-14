namespace Dinex.Infra
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DinexApiContext context) : base(context)
        {
        }
    }
}
