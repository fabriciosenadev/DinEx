namespace Dinex.Core
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public static explicit operator GetUserResponse(User user)
        {
            return new GetUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
            };
        }
    }
}
