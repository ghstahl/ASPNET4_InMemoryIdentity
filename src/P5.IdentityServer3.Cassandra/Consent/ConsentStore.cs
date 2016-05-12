using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Cassandra.Client;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
 

namespace P5.IdentityServer3.Cassandra
{
    public class ConsentStore : IConsentStore
    {
        public async Task<IEnumerable<Consent>> LoadAllAsync(string subject)
        {
            var result = await IdentityServer3CassandraDao.FindConsentsBySubjectAsync(subject);
            return result;
        }

        public async Task RevokeAsync(string subject, string client)
        {
            await IdentityServer3CassandraDao.DeleteConsentBySubjectAndClientIdAsync(subject, client);
        }

        public async Task<Consent> LoadAsync(string subject, string client)
        {
            return await IdentityServer3CassandraDao.FindConsentBySubjectAndClientIdAsync(subject, client);
        }

        public async Task UpdateAsync(Consent consent)
        {
            List<FlattenedConsentHandle> result = new List<FlattenedConsentHandle> {new FlattenedConsentHandle(consent)};
            await IdentityServer3CassandraDao.CreateManyConsentHandleAsync(result);
        }
    }
}