using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CustomClientCredentialHost.Models;
using P5.AspNet.Identity.Cassandra;

namespace CustomClientCredentialHost.Controllers
{

    public static class IdentityTokenHelper
    {
        public const string WellKnown_VerifyAccountEmailAction = "b3d17698-011b-4a74-aa0d-1301e01bbb8f";
        public const string WellKnown_NortonAction = "norton::action";
        public const string ValidIssuer = "Norton";

        public static ClaimsPrincipal ValidateJWT(string tokenString,TokenValidationParameters tokenValidationParameters, out SecurityToken validatedToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out validatedToken);
            return principal;
        }

        public static ClaimsPrincipal ValidateJWT(string tokenString, out SecurityToken validatedToken)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new[] { "https://www.norton.com" },
                IssuerSigningToken = new BinarySecretSecurityToken(EncryptionKey),
                ValidIssuer = "Norton",
                ValidateLifetime = true
            };
            return ValidateJWT(tokenString, tokenValidationParameters, out validatedToken);
        }

        public static string BuildJWT(IEnumerable<Claim> claims, string issuer, string appliesToAddress,
            Lifetime lifetime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                TokenIssuerName = issuer,
                AppliesToAddress = appliesToAddress,
                Lifetime = lifetime,
                SigningCredentials = new SigningCredentials(
                    new InMemorySymmetricSecurityKey(EncryptionKey),
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256"),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public static string BuildUrlEncodedEmailVerifyJWT(HttpRequestBase Request, string nameIdentifier, Lifetime lifetime)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
                            new Claim(WellKnown_NortonAction,WellKnown_VerifyAccountEmailAction),
                        }),
                TokenIssuerName = "Norton",
                AppliesToAddress = "https://www.norton.com",
                Lifetime = lifetime,
                SigningCredentials = new SigningCredentials(
                    new InMemorySymmetricSecurityKey(EncryptionKey),
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256"),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            byte[] tokenBytes = Encoding.UTF8.GetBytes(tokenString);
            var tokenBytesEncoded = HttpServerUtility.UrlTokenEncode(tokenBytes);
            return tokenString;

        }
        static byte[] _encryptionKey;
        public static byte[] EncryptionKey
        {
            get
            {
                if (_encryptionKey == null)
                {
                    string originalString = "664b9909-71c1-432c-b655-553ae2e2b5eb";
                    Guid key = Guid.Parse(originalString);
                    byte[] myUnprotectedBytes = Encoding.UTF8.GetBytes(originalString);
                    byte[] myProtectedBytes = MachineKey.Protect(myUnprotectedBytes, originalString);
                    var urlProtected = HttpServerUtility.UrlTokenEncode(myProtectedBytes);
                    var symmetricKey = Encoding.UTF8.GetBytes(urlProtected);
                    _encryptionKey = key.ToByteArray();
                }
                return _encryptionKey;

            }
        }
        static SigningCredentials _signingCredentials ;

        public static SigningCredentials SigningCredentials
        {
            get
            {
                if (_signingCredentials == null)
                {
                    _signingCredentials = new SigningCredentials(
                        new InMemorySymmetricSecurityKey(EncryptionKey),
                        "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                        "http://www.w3.org/2001/04/xmlenc#sha256");
                }
                return _signingCredentials;
            }
        }

        public static bool IsLocalIpAddress(string host)
        {
            try
            { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }


    }
    [Authorize]
    public class AccountController : Controller
    {


        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (!(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                return RedirectToAction("SendEmailConfirmationCode", "Account", new {userId = user.Id.ToString(),email = model.Email });
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("SendEmailConfirmationCode", "Account", new { userId = user.Id.ToString(), email = model.Email });

                   /*

                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link


                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking here: " + callbackUrl);

                    return RedirectToAction("Index", "Home");
                    * */
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/EmailConfirmationSent
        [AllowAnonymous]
        public ActionResult EmailConfirmationSent(string email)
        {
            return View(new ConfirmEmailViewModel { Email = email });
        }

        // GET: /Account/SendEmailConfirmationCode
        [AllowAnonymous]
        public async Task<ActionResult> SendEmailConfirmationCode(string userId, string email)
        {
            return View(new ConfirmEmailViewModel { UserId = userId, Email = email });
        }

        const string EmailConfirmationAudience = "https://www.cassandrahost.com/aud/EmailConfirmation";
        const string EmailCodeClaim = "EmailCode";
        const string ValidIssuer = "CassandraHost";
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendEmailConfirmationCode(ConfirmEmailViewModel confirmEmailViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            var userId = confirmEmailViewModel.UserId.ToGuid();

            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, confirmEmailViewModel.UserId),
                new Claim(EmailCodeClaim, code),
            };
            var now = DateTime.UtcNow;
            var lifetime = new Lifetime(now, now.AddMinutes(30));
            var jwt = IdentityTokenHelper.BuildJWT(claims, ValidIssuer, EmailConfirmationAudience, lifetime);
            var validationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new[] { EmailConfirmationAudience },
                IssuerSigningToken = new BinarySecretSecurityToken(IdentityTokenHelper.EncryptionKey),
                ValidIssuer = ValidIssuer,
                ValidateLifetime = true
            };
            SecurityToken securityToken;
            var principal = IdentityTokenHelper.ValidateJWT(jwt, validationParameters, out securityToken);


            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = confirmEmailViewModel.UserId, code = jwt }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, "Confirm your account", "You have 30 minutes to confirm your account by clicking here: " + callbackUrl);

            // Generate the token and send it
            return RedirectToAction("EmailConfirmationSent", "Account", new { email = confirmEmailViewModel.Email });


        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var validationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new[] { EmailConfirmationAudience },
                IssuerSigningToken = new BinarySecretSecurityToken(IdentityTokenHelper.EncryptionKey),
                ValidIssuer = ValidIssuer,
                ValidateLifetime = true
            };
            SecurityToken securityToken;
            var principal = IdentityTokenHelper.ValidateJWT(code, validationParameters, out securityToken);
            
            var query = from item in principal.Claims
                where item.Type == ClaimTypes.NameIdentifier
                select item;
            userId = query.Single().Value;

            query = from item in principal.Claims
                    where item.Type == EmailCodeClaim
                        select item;
            code = query.Single().Value;


            var user = await UserManager.FindByIdAsync(userId.ToGuid());
            if (user.IsEmailConfirmed)
            {
                return View("ConfirmEmail");
            }
            var result = await UserManager.ConfirmEmailAsync(userId.ToGuid(), code);
            if (result.Succeeded)
            {
                /*
                 * 1. Find any external providers associated with this user
                 * 2. Create a new user that has all these same associations, and set email confirmed to true
                 * 3. Delete the old user, as this was only a placeholder until we got an email verification.
                 */
                var email = user.Email;
                var logins = await UserManager.GetLoginsAsync(userId.ToGuid());
                var del = await UserManager.DeleteAsync(user);
                user.IsEmailConfirmed = true;
                var dd = await UserManager.CreateAsync(user);
                user = await UserManager.FindByEmailAsync(email);
                foreach (var login in logins)
                {
                    await UserManager.AddLoginAsync(user.Id, login);
                }
            }

            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            CassandraUser user = null;
            if (!string.IsNullOrEmpty(loginInfo.Email))
            {
                user = await UserManager.FindByNameAsync(loginInfo.Email);
            }
             

            if (user != null && !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                return RedirectToAction("SendEmailConfirmationCode", "Account", new { userId = user.Id.ToString(), email = loginInfo.Email });
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            if (result == SignInStatus.Failure && user != null)
            {
                // Auto association
                var dd = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            }
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                     
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}