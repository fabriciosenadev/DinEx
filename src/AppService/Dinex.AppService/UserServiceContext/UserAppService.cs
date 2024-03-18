namespace Dinex.AppService
{
    public class UserAppService : IUserAppService
    {
        private const int DefaultCodeLength = 32;

        private readonly ILogger<UserAppService> _logger;

        private readonly IUserRepository _userRepository;

        private readonly IAccountService _accountService;
        private readonly ISendEmailService _sendEmailService;

        public UserAppService(
            ILogger<UserAppService> logger,
            IUserRepository userRepository,
            IAccountService accountService,
            ISendEmailService sendEmailService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _accountService = accountService;
            _sendEmailService = sendEmailService;
        }

        public async Task<OperationResult> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                _logger.LogInformation("starting CreateUserAsync");

                var result = new OperationResult();
                
                if(_userRepository.FindAsync(user => user.Email == request.Email).Result.Any())
                {
                    result.AddError("Já existe um usuário com este E-mail");
                    return result;
                }

                User newUser = request;
                if (!newUser.IsValid)
                {
                    result.AddNotifications(newUser.Notifications);
                    return result;
                }

                await _userRepository.AddAsync(newUser);

                _logger.LogInformation("finishing CreateUserAsync");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error ocurred to method: CreateUserAsync");
                return new OperationResult().SetAsInternalServerError()
                        .AddError($"An unexpected error ocurred to method: CreateUserAsync {ex}");
            }
        }

        public async Task<OperationResult> RequestActivationLinkAsync(ActivationUrlRequest request)
        {
            try
            {
                _logger.LogInformation("starting RequestAccountActivationLink");

                var result = new OperationResult();

                var user = (await _userRepository.FindAsync(x => x.Email == request.Email)).FirstOrDefault();
                if (user is null)
                {
                    result.AddError("Usuário não encontrado").SetAsNotFound();
                    return result;
                }

                if(!user.HasValidActivationCode())
                {
                    var activationCode = _accountService.GenerateCode(DefaultCodeLength);

                    user.AssingActivationCode(activationCode);
                    await _userRepository.UpdateAsync(user);
                }

                #region email to request activation link
                var emailMessageModel = new EmailMessageModel 
                {
                    DestinationName = user.Email,
                    GeneratedCode = user.ActivationCode,
                    Origin = "activation",
                    EmailTemplateFileName = "activationAccount.html",
                    TemplateFieldToName = "{name}",
                    TemplateFieldToUrl = "{activationUrl}"
                };
                var emailMessage = _sendEmailService.GetEmailMessage(emailMessageModel);

                var sendEmail = new SendEmailModel 
                { 
                    DestinationEmailAddress = user.Email,
                    DestinationName = user.FullName,
                    DestinationSubject = "Ativação de conta",
                    EmailMessage = emailMessage,
                    IsHtml = true
                };
                await _sendEmailService.Execute(sendEmail);
                #endregion

                _logger.LogInformation("finishing RequestAccountActivationLink");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error ocurred to method: RequestAccountActivationLink");
                return new OperationResult().SetAsInternalServerError()
                        .AddError($"An unexpected error ocurred to method: RequestAccountActivationLink {ex}");
            }
        }

        public async Task<OperationResult> ActivateUser(ActivationRequest request)
        {
            try
            {
                _logger.LogInformation("starting ActivateUser");

                var result = new OperationResult();

                if(string.IsNullOrWhiteSpace(request.ActivationCode))
                {
                    result.AddError("Código de ativação deve ser fornecido");
                    return result;
                }

                var user = (await _userRepository.FindAsync(x => x.Email == request.Email)).FirstOrDefault();
                if (user is null)
                {
                    result.AddError("Usuário não encontrado").SetAsNotFound();
                    return result;
                }

                if (!user.HasValidActivationCode() && user.ActivationCode != request.ActivationCode)
                {
                    result.AddError("Código de ativação inválido ou fora da validade, solicite outro código");
                    return result;
                }

                user.Activate();
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("finishing ActivateUser");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error ocurred to method: ActivateUser");
                return new OperationResult().SetAsInternalServerError()
                        .AddError($"An unexpected error ocurred to method: ActivateUser {ex}");
            }
        }

        public async Task<OperationResult<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            try
            {
                _logger.LogInformation("starting AuthenticateAsync");

                var result = new OperationResult<AuthenticationResponse>();

                var user = (await _userRepository.FindAsync(x => x.Email == request.Email)).FirstOrDefault();
                if(user is null)
                {
                    result.AddError("Usuário não encontrado").SetAsNotFound();
                    return result;
                }

                if (user.UserStatus == UserStatus.Inactive)
                {
                    result.AddError("Conta inativa");
                    return result;
                }

                if (!User.MatchValues(user.Password, request.Password))
                {
                    result.AddError("Login ou senha incorretos");
                    return result;
                }

                var token = _accountService.GenerateToken(user);
                result.SetData(new AuthenticationResponse(user, token));

                _logger.LogInformation("finishing AuthenticateAsync");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error ocurred to method: AuthenticateAsync");
                return new OperationResult<AuthenticationResponse>()
                    .SetAsInternalServerError()
                    .AddError($"An unexpected error ocurred to method: AuthenticateAsync {ex}");
            }
        }

        public async Task<OperationResult<GetUserResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var result = new OperationResult<GetUserResponse>();

                if(id == Guid.Empty)
                {
                    result.AddError("Id inválido");
                    return result;
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user is null)
                {
                    result.AddError("Usuário não encontrado").SetAsNotFound();
                    return result;
                }

                var response = (GetUserResponse)user;
                result.SetData(response);

                _logger.LogInformation("finishing GetByIdAsync");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error ocurred to method: GetByIdAsync");
                return new OperationResult<GetUserResponse>()
                    .SetAsInternalServerError()
                    .AddError($"An unexpected error ocurred to method: GetByIdAsync {ex}");
            }
        }
    }
}
