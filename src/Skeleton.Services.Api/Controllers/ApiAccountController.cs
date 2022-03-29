using System.Security.Claims;
using Skeleton.Domain.Core.Bus;
using Skeleton.Infra.CrossCutting.Identity.Models;
using Skeleton.Services.Api.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NetDevPack.Identity.Jwt;
using sib_api_v3_sdk.Model;
using Task = System.Threading.Tasks.Task;

namespace Skeleton.Services.Api.Controllers
{
    public abstract class ApiAccountController : ApiController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppJwtSettings _appJwtSettings;
        private readonly SendinblueConfig _sendinblue;
        private readonly IMessageHandler _message;

        protected ApiAccountController(
            UserManager<IdentityUser> userManager,
            IOptions<AppJwtSettings> appJwtSettings,
            IOptions<SendinblueConfig> sendinblue,
            IMessageHandler message)
        {
            _userManager = userManager;
            _appJwtSettings = appJwtSettings.Value;
            _sendinblue = sendinblue.Value;
            _message = message;
        }

        protected async Task ExecutePasswordResetToken(IdentityUser user, string callbackUrl)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var name = userClaims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value!;

            await SendEmail(
                user.Email,
                name,
                int.Parse(_sendinblue.ResetYourPassword),
                new { name, callbackUrl });
        }

        protected async Task ExecuteEmailConfirmationToken(IdentityUser user, string callbackUrl)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var name = userClaims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value!;

            await SendEmail(
                user.Email,
                name,
                int.Parse(_sendinblue.ConfirmEmail),
                new { name, callbackUrl });
        }

        protected async Task ExecuteTwoFactorToken(IdentityUser user, string twoFactorCode)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var mobilePhone = userClaims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.MobilePhone))?.Value!;

            await _message.SendToQueue("sms", new
            {
                Body = $"Use {twoFactorCode} como código de segurança da conta da Skeleton.",
                From = "",
                To = $"+55{mobilePhone}"
            });
        }

        protected async Task ExecuteConfirmEmail(IdentityUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var name = userClaims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value!;

            await SendEmail(
                user.Email,
                name,
                int.Parse(_sendinblue.AccountCreatedSuccessfully),
                new { name });
        }

        protected async Task ExecuteResetPassword(IdentityUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var name = userClaims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value!;

            await SendEmail(
                user.Email,
                name,
                int.Parse(_sendinblue.PasswordResetSuccessfully),
                new { name });
        }

        private async Task SendEmail(string email, string name, int templateId, object parameters)
        {
            await _message.SendToQueue("email", new SendSmtpEmail(
                new SendSmtpEmailSender(_sendinblue.SmtpName, _sendinblue.SmtpEmail),
                new List<SendSmtpEmailTo> { new(email, name) },
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                templateId,
                parameters));
        }

        protected SignInResponse GetSignInResponse(SignInResult? result, string email)
        {
            if (result is not null && !result.Succeeded)
                return new SignInResponse
                {
                    StatusMessage = result.ToString(),
                    AuthResponse = null
                };

            var buildUserResponse = new JwtBuilder()
                .WithUserManager(_userManager)
                .WithJwtSettings(_appJwtSettings)
                .WithEmail(email)
                .WithJwtClaims()
                .WithUserClaims()
                .WithUserRoles()
                .BuildUserResponse();

            return new SignInResponse
            {
                StatusMessage = result is null ? "Succeeded" : result.ToString(),
                AuthResponse = new AuthResponse
                {
                    AccessToken = buildUserResponse.AccessToken,
                    TokenType = "Bearer",
                    ExpiresIn = buildUserResponse.ExpiresIn,
                    UniqueName = email,
                    UserToken = new UserToken
                    {
                        Id = new Guid(buildUserResponse.UserToken.Id),
                        Email = buildUserResponse.UserToken.Email,
                        Claims = buildUserResponse.UserToken.Claims
                    }
                }
            };
        }
    }
}