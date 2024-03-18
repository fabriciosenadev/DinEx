namespace Dinex.AppService
{
    public interface IUserAppService
    {
        Task<OperationResult> CreateUserAsync(CreateUserRequest request);
        Task<OperationResult> RequestActivationLinkAsync(ActivationUrlRequest request);
        Task<OperationResult> ActivateUser(ActivationRequest request);
        Task<OperationResult<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
        Task<OperationResult<GetUserResponse>> GetByIdAsync(Guid id);
    }
}
