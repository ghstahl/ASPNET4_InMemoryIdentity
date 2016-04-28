using System;
using System.CodeDom;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Biggy.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.BiggyJson
{
    public class ClientStore : BiggyStore<ClientRecord, ClientHandle>, IClientStore
    {
        public static ClientStore NewFromSetting(StoreSettings settings)
        {
            var store = new ClientStore(
                settings.Folder,
                settings.Database,
                settings.ClientCollection);
            return store;
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
            var cr = new ClientRecord(new ClientHandle(){ClientId = clientId});
            var result = await RetrieveAsync(cr.Id);
            return await Task.FromResult(result.ToClient());
        }
    }
}
