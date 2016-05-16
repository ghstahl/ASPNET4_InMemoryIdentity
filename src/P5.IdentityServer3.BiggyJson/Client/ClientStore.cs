using System;
using System.CodeDom;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Biggy.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.BiggyJson
{
    public class ClientStore : BiggyStore<ClientRecord, ClientHandle>, IClientStore
    {
        public ClientStore(StoreSettings settings)
            : base(settings.Folder, settings.Database, settings.ClientCollection)
        {
        }
        public ClientStore(string folderStorage, string groupName = "IdentityServer3", string databaseName = "Clients")
            : base(folderStorage, groupName, databaseName)
        {
        }


        protected override Guid GetId(ClientHandle record)
        {
            return record.CreateGuid(ClientRecord.Namespace);
        }

        protected override ClientRecord NewWrap(ClientHandle record)
        {
            return new ClientRecord(record);
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var cr = new ClientRecord(record: new ClientHandle(){ClientId = clientId});
            var result = await RetrieveAsync(cr.Id);
            var client = await result.MakeClientAsyc();
            return client;
        }
    }
}
