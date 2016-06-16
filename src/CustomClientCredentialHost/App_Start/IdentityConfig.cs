using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using CustomClientCredentialHost.Models;
using P5.AspNet.Identity.Cassandra;
using P5.CassandraStore.DAO;

namespace CustomClientCredentialHost
{

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }
    public class MailtrapEmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            var client = new SmtpClient("mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("305e9a87cd4651", "16416ff1adf3f7"),
                EnableSsl = true
            };

            var @from = new MailAddress("no-reply@CustomClientCredentialHost.info", "My Awesome Email Admin");
            var to = new MailAddress(message.Destination);

            var mail = new MailMessage(@from, to)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true,
            };

            client.Send(mail);

            return Task.FromResult(0);


        }
    }
    public class PapercutEmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var @from = new MailAddress("no-reply@CustomClientCredentialHost.info", "My Awesome Email Admin");
            var to = new MailAddress(message.Destination);

            var mailMessage = new MailMessage(@from, to)
            {
                Subject = message.Subject,
                Body = message.Body,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = false
            };

            mailMessage.Bcc.Add("boss@company.com");
            SmtpClient client = new SmtpClient("127.0.0.1", 32525);
            NetworkCredential info = new NetworkCredential("mail@jonathanchannon.com", "reallysecurepassword");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = info;

            client.Send(mailMessage);

            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public class PapercutSmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var @from = new MailAddress("no-reply@CustomClientCredentialHost.info", "My Awesome SMS Admin");
            var to = new MailAddress("someguy@somedomain.com");

            var mailMessage = new MailMessage(@from, to)
            {
                Subject = message.Subject,
                Body = message.Body,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = false
            };

            mailMessage.Bcc.Add("boss@company.com");
            SmtpClient client = new SmtpClient("127.0.0.1", 32525);
            NetworkCredential info = new NetworkCredential("mail@jonathanchannon.com", "reallysecurepassword");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = info;

            // Plug in your SMS service here to send a text message.


            client.Send(mailMessage);

            return Task.FromResult(0);
        }
    }

    public class MailtrapSmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            var client = new SmtpClient("mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("305e9a87cd4651", "16416ff1adf3f7"),
                EnableSsl = true
            };

            var @from = new MailAddress("no-reply@CustomClientCredentialHost.info", "My Awesome SMS Admin");
            var to = new MailAddress("someguy@somedomain.com");

            var mail = new MailMessage(@from, to)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true,
            };

            client.Send(mail);

            return Task.FromResult(0);
        }
    }


    public class CassandraAspNetApplicationConstants
    {
        public const string TenantGuidS = "45d29c4d-9b8b-47ff-baa6-c8ae7ef657d3";
        public static Guid TenantGuid = Guid.Parse(TenantGuidS);
    }
    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<CassandraUser, Guid>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(CassandraUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
        public override Guid ConvertIdFromString(string id)
        {
            if (string.IsNullOrEmpty(id)) return Guid.Empty;

            return new Guid(id);
        }

        public override string ConvertIdToString(Guid id)
        {
            if (id.Equals(Guid.Empty)) return string.Empty;

            return id.ToString();
        }
    }
    public class ApplicationUserManager : UserManager<CassandraUser, Guid>
    {
        public ApplicationUserManager()
            : base(new CassandraUserStore(new CassandraDao(CassandraAspNetIdentityOptions.CassandraConfig), CassandraAspNetApplicationConstants.TenantGuid))
        {
        }

        public IUserStoreAdmin<CassandraUser, Guid> AdminStore
        {
            get { return this.Store as IUserStoreAdmin<CassandraUser, Guid>; }
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager();
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<CassandraUser,Guid>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<CassandraUser, Guid>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<CassandraUser, Guid>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new PapercutEmailService();
            manager.SmsService = new PapercutSmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<CassandraUser, Guid>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

    }

#if ENTITY_ASPNET_IDENTITY
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
#endif
}
