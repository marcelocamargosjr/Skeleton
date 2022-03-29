using System.Dynamic;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using NetDevPack.Identity.Jwt;
using NetDevPack.Identity.User;
using Skeleton.Domain.Core.Bus;
using Skeleton.Infra.CrossCutting.Identity.Models;
using Skeleton.Services.Api.Configurations;
using ProblemDetails = Skeleton.Services.Api.Models.ProblemDetails;

namespace Skeleton.Services.Api.Controllers.v1
{
    [Authorize]
    [Route("api/v1/account")]
    public class AccountController : ApiAccountController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAspNetUser _user;
        private readonly ClientUriConfig _clientUri;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(
            UserManager<IdentityUser> userManager,
            IOptions<AppJwtSettings> appJwtSettings,
            IOptions<SendinblueConfig> sendinblue,
            IMessageHandler message,
            SignInManager<IdentityUser> signInManager,
            IAspNetUser user,
            IOptions<ClientUriConfig> clientUri,
            IWebHostEnvironment webHostEnvironment) : base(userManager, appJwtSettings, sendinblue, message)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _user = user;
            _clientUri = clientUri.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = false,
                PhoneNumber = registerUser.PhoneNumber,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, registerUser.Name));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.MobilePhone, registerUser.PhoneNumber));

                if (user.EmailConfirmed)
                    return CustomResponse();

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var queryString = new Dictionary<string, string> { { "email", user.Email }, { "token", token } };
                var callbackUrl = QueryHelpers.AddQueryString($"{_clientUri.BaseUrl}{_clientUri.ConfirmEmail}", queryString!);

                await ExecuteEmailConfirmationToken(user, callbackUrl);

                if (_webHostEnvironment.IsDevelopment() || _webHostEnvironment.IsStaging())
                    return CustomResponse(new { debuggingWithSwagger = queryString });

                return CustomResponse();
            }

            foreach (var error in result.Errors)
            {
                AddError(error.Description);
            }

            return CustomResponse();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailUser confirmEmailUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = await _userManager.FindByNameAsync(confirmEmailUser.Email);

            if (user is null)
            {
                AddError($"O e-mail '{confirmEmailUser.Email}' não está associado a uma conta.");
                return CustomResponse();
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
                return CustomResponse();

            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailUser.Token);

            if (result.Succeeded)
            {
                await ExecuteConfirmEmail(user);

                return CustomResponse();
            }

            foreach (var error in result.Errors)
            {
                AddError(error.Description);
            }

            return CustomResponse();
        }

        [HttpPost]
        [ProducesResponseType(typeof(SignInResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = await _userManager.FindByNameAsync(loginUser.Email);

            if (user is null)
            {
                AddError("E-mail ou senha inválidos.");
                return CustomResponse();
            }

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
                return CustomResponse(GetSignInResponse(result, user.Email));

            if (result.IsLockedOut)
            {
                var signInResponse = GetSignInResponse(result, user.Email);

                dynamic response = new ExpandoObject();
                response.statusMessage = signInResponse.StatusMessage;
                response.authResponse = signInResponse.AuthResponse!;

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var queryString = new Dictionary<string, string> { { "email", user.Email }, { "token", token } };
                var callbackUrl = QueryHelpers.AddQueryString($"{_clientUri.BaseUrl}{_clientUri.ResetPassword}", queryString!);

                await ExecutePasswordResetToken(user, callbackUrl);

                if (_webHostEnvironment.IsDevelopment() || _webHostEnvironment.IsStaging())
                    response.debuggingWithSwagger = queryString;

                return CustomResponse(response);
            }

            if (result.IsNotAllowed)
            {
                var signInResponse = GetSignInResponse(result, user.Email);

                dynamic response = new ExpandoObject();
                response.statusMessage = signInResponse.StatusMessage;
                response.authResponse = signInResponse.AuthResponse!;

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var queryString = new Dictionary<string, string> { { "email", user.Email }, { "token", token } };
                var callbackUrl = QueryHelpers.AddQueryString($"{_clientUri.BaseUrl}{_clientUri.ConfirmEmail}", queryString!);

                await ExecuteEmailConfirmationToken(user, callbackUrl);

                if (_webHostEnvironment.IsDevelopment() || _webHostEnvironment.IsStaging())
                    response.debuggingWithSwagger = queryString;

                return CustomResponse(response);
            }

            if (result.RequiresTwoFactor)
            {
                var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
                if (!providers.Contains(TokenOptions.DefaultPhoneProvider))
                {
                    AddError("Provedor de verificação em duas etapas inválido.");
                    return CustomResponse();
                }

                var signInResponse = GetSignInResponse(result, user.Email);

                dynamic response = new ExpandoObject();
                response.statusMessage = signInResponse.StatusMessage;
                response.authResponse = signInResponse.AuthResponse!;

                var twoFactorCode = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                var queryString = new Dictionary<string, string> { { "email", user.Email }, { "twoFactorCode", twoFactorCode } };

                await ExecuteTwoFactorToken(user, twoFactorCode);

                if (_webHostEnvironment.IsDevelopment() || _webHostEnvironment.IsStaging())
                    response.debuggingWithSwagger = queryString;

                return CustomResponse(response);
            }

            AddError("E-mail ou senha inválidos.");
            return CustomResponse();
        }

        [HttpPost]
        [ProducesResponseType(typeof(SignInResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        [Route("login-two-step")]
        public async Task<IActionResult> LoginTwoStep([FromBody] LoginTwoStepUser loginTwoStepUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user is null)
            {
                AddError("Token inválido.");
                return CustomResponse();
            }

            var result = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultPhoneProvider, loginTwoStepUser.TwoFactorCode, false, false);

            if (result.Succeeded)
                return CustomResponse(GetSignInResponse(result, user.Email));

            if (result.IsLockedOut)
            {
                var signInResponse = GetSignInResponse(result, user.Email);

                dynamic response = new ExpandoObject();
                response.statusMessage = signInResponse.StatusMessage;
                response.authResponse = signInResponse.AuthResponse!;

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var queryString = new Dictionary<string, string> { { "email", user.Email }, { "token", token } };
                var callbackUrl = QueryHelpers.AddQueryString($"{_clientUri.BaseUrl}{_clientUri.ResetPassword}", queryString!);

                await ExecutePasswordResetToken(user, callbackUrl);

                if (_webHostEnvironment.IsDevelopment() || _webHostEnvironment.IsStaging())
                    response.debuggingWithSwagger = queryString;

                return CustomResponse(response);
            }

            AddError("Token inválido.");
            return CustomResponse();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordUser changePasswordUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = await _userManager.FindByNameAsync(_user.GetUserEmail());

            if (user is null)
            {
                AddError($"O e-mail '{_user.GetUserEmail()}' não está associado a uma conta.");
                return CustomResponse();
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordUser.OldPassword, changePasswordUser.NewPassword);

            if (result.Succeeded)
            {
                // Todo: Send notification email

                return CustomResponse();
            }

            foreach (var error in result.Errors)
            {
                AddError(error.Description);
            }

            return CustomResponse();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordUser forgotPasswordUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = await _userManager.FindByNameAsync(forgotPasswordUser.Email);

            if (user is null)
            {
                AddError($"O e-mail '{forgotPasswordUser.Email}' não está associado a uma conta.");
                return CustomResponse();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var queryString = new Dictionary<string, string> { { "email", user.Email }, { "token", token } };
            var callbackUrl = QueryHelpers.AddQueryString($"{_clientUri.BaseUrl}{_clientUri.ResetPassword}", queryString!);

            await ExecutePasswordResetToken(user, callbackUrl);

            if (_webHostEnvironment.IsDevelopment() || _webHostEnvironment.IsStaging())
                return CustomResponse(new { debuggingWithSwagger = queryString });

            return CustomResponse();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordUser resetPasswordUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = await _userManager.FindByNameAsync(resetPasswordUser.Email);

            if (user is null)
            {
                AddError($"O e-mail '{resetPasswordUser.Email}' não está associado a uma conta.");
                return CustomResponse();
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordUser.Token, resetPasswordUser.Password);

            if (result.Succeeded)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTime.MinValue);

                await ExecuteResetPassword(user);

                return CustomResponse();
            }

            foreach (var error in result.Errors)
            {
                AddError(error.Description);
            }

            return CustomResponse();
        }
    }
}