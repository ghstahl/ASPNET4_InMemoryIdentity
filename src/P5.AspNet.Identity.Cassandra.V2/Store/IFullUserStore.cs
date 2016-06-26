using System;
using Microsoft.AspNet.Identity;

namespace P5.AspNet.Identity.Cassandra
{
    public interface IFullUserStore :
        IUserStore<CassandraUser, Guid>,
        IUserClaimStore<CassandraUser, Guid>,
        IUserEmailStore<CassandraUser, Guid>,
        IUserLockoutStore<CassandraUser, Guid>,
        IUserLoginStore<CassandraUser, Guid>,
        IUserPasswordStore<CassandraUser, Guid>,
        IUserPhoneNumberStore<CassandraUser, Guid>,
        IUserRoleStore<CassandraUser, Guid>,
        IUserSecurityStampStore<CassandraUser, Guid>,
        IUserAdminStore<CassandraUser, Guid>
    {

    }
}