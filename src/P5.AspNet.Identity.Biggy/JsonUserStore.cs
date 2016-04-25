using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Biggy.Data.Json;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace P5.AspNet.Identity.Biggy
{
    class UserLogingInfoToUserIdRecord
    {
        public string Key { get; set; }
        public string Id { get; set; }
    }
    public class JsonUserStore :
        IUserStore<CustomUser>,
        IUserPasswordStore<CustomUser>,
        IUserLoginStore<CustomUser>,
        IUserPhoneNumberStore<CustomUser>,
        IUserLockoutStore<CustomUser,string>,
        IUserTwoFactorStore<CustomUser, string>,
        IUserRoleStore<CustomUser, string>,
        IUserClaimStore<CustomUser, string>
    {
        private Dictionary<string,CustomUser> userMap = new Dictionary<string, CustomUser>();
        private IList<CustomUser> userList = new List<CustomUser>();


        private readonly string FolderStorage = string.Empty;

        private JsonStore<CustomUser> _userDb = null;
        private JsonStore<CustomUser> UserDb
        {
            get { return _userDb ?? (_userDb = new JsonStore<CustomUser>(this.FolderStorage, "Identity", "Users")); }
        }

        private JsonStore<UserLogingInfoToUserIdRecord> _userLoginInfoToIdDb = null;
        private JsonStore<UserLogingInfoToUserIdRecord> UserLoginInfoToIdDb
        {
            get { return _userLoginInfoToIdDb ?? (_userLoginInfoToIdDb = new JsonStore<UserLogingInfoToUserIdRecord>(this.FolderStorage, "Identity", "UserLoginInfoToIdDb")); }
        }

        private JsonStore<IdentityUserRole> _roleDb = null;
        private JsonStore<IdentityUserRole> RoleDb
        {
            get { return _roleDb ?? (_roleDb = new JsonStore<IdentityUserRole>(this.FolderStorage, "Identity", "Roles")); }
        }

        public JsonUserStore(string folderStorage)
        {
            this.FolderStorage = folderStorage;
        }

        public void Dispose()
        {
            this.UserDb.FlushToDisk();
            this.UserLoginInfoToIdDb.FlushToDisk();
            this.RoleDb.FlushToDisk();
        }

        public Task CreateAsync(CustomUser user)
        {
            string userId = Guid.NewGuid().ToString();
            user.Id = userId;
            this.UserDb.Add(user);

            return Task.FromResult(user);
        }

        public Task UpdateAsync(CustomUser user)
        {
            return Task.FromResult(this.UserDb.Update(user));
        }

        public Task DeleteAsync(CustomUser user)
        {
            foreach (var item in user.Logins)
            {
                var record = new UserLogingInfoToUserIdRecord()
                {
                    Key =
                        GetLoginId(new UserLoginInfo(item.LoginProvider,item.LoginProvider)),
                    Id = user.Id
                };
                this.UserLoginInfoToIdDb.Delete(record);
            }
            return Task.FromResult(this.UserDb.Delete(user));
        }

        public Task<CustomUser> FindByIdAsync(string userId)
        {
            CustomUser user = null;
            IList<CustomUser> users = this.UserDb.TryLoadData();
            if (users == null || users.Count == 0)
            {
                return Task.FromResult(user);
            }

            user = users.SingleOrDefault(f => f.Id == userId);

            return Task.FromResult(user);

//            return Task.FromResult(userList.FirstOrDefault(x => x.Id.Equals(userId)));
        }

        public Task<CustomUser> FindByNameAsync(string userName)
        {
            CustomUser user = null;
            IList<CustomUser> users = this.UserDb.TryLoadData();
            if (users == null || users.Count == 0)
            {
                return Task.FromResult(user);
            }

            user = users.SingleOrDefault(f => f.UserName == userName);

            return Task.FromResult(user);
        }

        public Task AddLoginAsync(CustomUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("user");
            }

            if (!user.Logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey))
            {
                IdentityUserLogin iul = new IdentityUserLogin
                {
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey
                };
                user.Logins.Add(iul);

                var record = new UserLogingInfoToUserIdRecord()
                {
                    Key = GetLoginId(login),
                    Id = user.Id
                };
                this.UserLoginInfoToIdDb.Add(record);

                return UpdateAsync(user);

            }

            return Task.FromResult(true);
        }

        public Task RemoveLoginAsync(CustomUser user, UserLoginInfo login)
        {

            var toBeRemoved = new List<IdentityUserLogin>();
            foreach (var item in user.Logins)
            {
                if (login.ProviderKey == item.ProviderKey && login.LoginProvider == item.LoginProvider)
                {
                    toBeRemoved.Add(item);
                }
            }

            foreach (var item in toBeRemoved)
            {
                user.Logins.Remove(item);
            }

            var record = new UserLogingInfoToUserIdRecord()
            {
                Key = GetLoginId(login),
                Id = user.Id
            };
            this.UserLoginInfoToIdDb.Delete(record);

            return Task.FromResult(true);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(CustomUser user)
        {
            var logins = from login in user.Logins
                           select new UserLoginInfo(login.LoginProvider,login.ProviderKey);

            IList<UserLoginInfo> result = new List<UserLoginInfo>(logins);

            return Task.FromResult(result);
        }


        public Task<CustomUser> FindAsync(UserLoginInfo login)
        {

            string loginId = GetLoginId(login);
            var ulpRecords = this.UserLoginInfoToIdDb.TryLoadData();
            var record = ulpRecords.SingleOrDefault(f => f.Key == GetLoginId(login));
            if(record != null)
            {
                var user = UserDb.TryLoadData().SingleOrDefault(f => f.Id == record.Id);
                return Task.FromResult(user);
            }

            return Task.FromResult<CustomUser>(null);
            //throw new NotImplementedException();
        }

        #region IUserPasswordStore
        public Task SetPasswordHashAsync(CustomUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(CustomUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(CustomUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }
        #endregion

        #region IUserPhoneNumberStore
        public Task SetPhoneNumberAsync(CustomUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult<CustomUser>(user);
        }

        public Task<string> GetPhoneNumberAsync(CustomUser user)
        {
            return Task.FromResult<string>(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(CustomUser user)
        {
            return Task.FromResult<bool>(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(CustomUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult<CustomUser>(user);
        }
        #endregion

        #region IUserLockoutStore
        public Task<DateTimeOffset> GetLockoutEndDateAsync(CustomUser user)
        {

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.LockoutEndDateUtc != null)
                return Task.FromResult<DateTimeOffset>(user.LockoutEndDateUtc.Value);

            throw new InvalidOperationException("LockoutEndDate has no value.");
        }

        public Task SetLockoutEndDateAsync(CustomUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            return Task.FromResult<CustomUser>(user);
        }

        public Task<int> IncrementAccessFailedCountAsync(CustomUser user)
        {
            user.AccessFailedCount = user.AccessFailedCount + 1;
            return Task.FromResult<int>(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(CustomUser user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult<CustomUser>(user);
        }

        public Task<int> GetAccessFailedCountAsync(CustomUser user)
        {
            return Task.FromResult<int>(user.AccessFailedCount);
        }
        public Task<bool> GetLockoutEnabledAsync(CustomUser user)
        {
            return Task.FromResult<bool>(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(CustomUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult<CustomUser>(user);
        }
        #endregion

        #region IUserTwoFactorStore
        public Task SetTwoFactorEnabledAsync(CustomUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult<CustomUser>(user);
        }

        public Task<bool> GetTwoFactorEnabledAsync(CustomUser user)
        {
            return Task.FromResult<bool>(user.TwoFactorEnabled);
        }
        #endregion

        #region IUserRoleStore
        public Task AddToRoleAsync(CustomUser user, string roleName)
        {
            user.Roles.Add(new IdentityUserRole(){RoleId = roleName,UserId = user.Id});
            return Task.FromResult<CustomUser>(user);
        }

        public Task RemoveFromRoleAsync(CustomUser user, string roleName)
        {
            var roles = from item in user.Roles
                        where roleName == item.RoleId
                        select item;
            var roleList = roles.ToList();
            foreach (var item in roleList)
            {
                user.Roles.Remove(item);
            }
            return Task.FromResult<CustomUser>(user);
        }

        public Task<IList<string>> GetRolesAsync(CustomUser user)
        {
            var roles = from item in user.Roles
                select item.RoleId;
            IList<String> result = new List<string>(roles);
            return Task.FromResult(result);
        }

        public Task<bool> IsInRoleAsync(CustomUser user, string roleName)
        {
            var roles = from item in user.Roles
                        where roleName == item.RoleId
                        select item.RoleId;
            bool result = roles.Any();
            return Task.FromResult(result);
        }
        #endregion

        #region IUserClaimStore
        public Task<IList<Claim>> GetClaimsAsync(CustomUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var claims = from claim in user.Claims
                         select new Claim(claim.ClaimType, claim.ClaimValue);

            IList<Claim> result = new List<Claim>(claims);
            return Task.FromResult(result);
        }

        public Task RemoveClaimAsync(CustomUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            foreach (var item in user.Claims.Where(item => item.ClaimType == claim.Type))
            {
                user.Claims.Remove(item);
                break;
            }
            return Task.FromResult<CustomUser>(user);
        }

        public Task AddClaimAsync(CustomUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            RemoveClaimAsync(user, claim);
            user.Claims.Add(new IdentityUserClaim() { ClaimValue = claim.Value, ClaimType = claim.Type });
            return Task.FromResult<CustomUser>(user);
        }
        #endregion

        #region HELPER
        private string GetLoginId(UserLoginInfo login)
        {
            using (var sha = new SHA1CryptoServiceProvider())
            {
                byte[] clearBytes = Encoding.UTF8.GetBytes(login.LoginProvider + "|" + login.ProviderKey);
                byte[] hashBytes = sha.ComputeHash(clearBytes);
                return ToHex(hashBytes);
            }
        }

        private string ToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                sb.Append(bytes[i].ToString("x2"));
            return sb.ToString();
        }
        #endregion

    }
}