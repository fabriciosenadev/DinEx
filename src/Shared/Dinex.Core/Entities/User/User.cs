namespace Dinex.Core
{
    public class User : Entity
    {
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public UserStatus UserStatus { get; private set; }
        public string? ActivationCode { get; private set; }

        public static implicit operator User(CreateUserRequest request)
        {

            var newUser = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                UserStatus = UserStatus.Inactive,
                CreatedAt = DateTime.UtcNow
            };

            newUser.AddNotifications(
                new Contract<Notification>()
                .Requires()
                .IsNotNullOrEmpty(newUser.FullName, "User.FullName", "Informe seu nome completo")
                .IsGreaterOrEqualsThan(newUser.FullName?.Length ?? 0, 3, "User.FullName", "Informe seu nome completo")
                .IsNotNullOrEmpty(newUser.Email, "User.Email", "Informe seu melhor e-mail")
                .IsEmail(newUser.Email, "User.Email", "Informe seu melhor e-mail")
                );

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                newUser.AddNotification("User.Password", "Preencha a senha com no minimo 8 caracteres");
            }

            if (request.Password != request.ConfirmPassword)
            {
                newUser.AddNotification("User.ConfirmPassword", "Senhas devem ser iguais");
            }

            newUser.Password = Encrypt(request.Password);

            return newUser;
        }

        public static string Encrypt(string value)
        {
            var result = BCrypt.Net.BCrypt.HashPassword(value);
            return result;
        }

        public static bool MatchValues(string encryptedValue, string valueToCompare)
        {
            var result = BCrypt.Net.BCrypt.Verify(valueToCompare, encryptedValue);
            return result;
        }

        public bool HasValidActivationCode()
        {
            if(UpdatedAt is null)
                return false;

            var timeToValidate = UpdatedAt.Value.AddHours(2);
            if(timeToValidate <= DateTime.UtcNow)
                return false;

            return true;
        }

        public void AssingActivationCode(string activationCode)
        {
            ActivationCode = activationCode;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            ActivationCode = null;
            UpdatedAt = DateTime.UtcNow;
            UserStatus = UserStatus.Active;
        }
    }
}
