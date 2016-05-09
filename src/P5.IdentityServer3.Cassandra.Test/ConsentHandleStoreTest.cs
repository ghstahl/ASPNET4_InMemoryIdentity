using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using P5.IdentityServer3.Cassandra.Client;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;


using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ConsentHandleStoreTest : TestBase
    {
        private IdentityServer3CassandraDao _store;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }


        [TestMethod]
        public async Task TestUpdateAsync()
        {
            var store = new ConsentStore();
            var insertClients = await CassandraTestHelper.InsertTestData_Clients(1);
            var client = insertClients[0].Record;
            Consent consent = new Consent()
            {
                ClientId = client.ClientId,
                Scopes = new List<string>()
                {
                    "Scope 0:",
                    "Scope 1:"
                },
                Subject = Guid.NewGuid().ToString()
            };
            await store.UpdateAsync(consent);
            var result = await store.LoadAsync(consent.Subject,consent.ClientId);
            Assert.AreEqual(consent.ClientId, result.ClientId);
            Assert.AreEqual(consent.Subject, result.Subject);
        }


        [TestMethod]
        public async Task TestCreateTokenHandleAsync()
        {
            var store = new ConsentStore();

            var insert = await CassandraTestHelper.InsertTestData_Consents(1);
            var flat = insert[0];
            FlattenedConsentRecord fcr = new FlattenedConsentRecord(flat);
            var result = await IdentityServer3CassandraDao.FindConsentByIdAsync(fcr.Id);
            Assert.AreEqual(result.ClientId, flat.ClientId);
            Assert.AreEqual(result.Subject, flat.Subject);

            result = await store.LoadAsync(flat.Subject, flat.ClientId);
            Assert.AreEqual(result.ClientId, flat.ClientId);
            Assert.AreEqual(result.Subject, flat.Subject);
        }

        [TestMethod]
        public async Task TestGetAllAsync()
        {
            var store = new ConsentStore();
            var insertClients = await CassandraTestHelper.InsertTestData_Clients(2);
            // we are going to associate a bunch of tokens to this one client
            var subject = Guid.NewGuid().ToString();

            foreach (var client in insertClients)
            {
                await CassandraTestHelper.InsertTestData_Consents(client.Record.ClientId, subject, 1);
            }

            var result = await store.LoadAllAsync(subject);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task TestRevokeAsync()
        {
            var store = new ConsentStore();
            var insertClients = await CassandraTestHelper.InsertTestData_Clients(2);
            // we are going to associate a bunch of tokens to this one client
            var subject = Guid.NewGuid().ToString();

            foreach (var client in insertClients)
            {
                await CassandraTestHelper.InsertTestData_Consents(client.Record.ClientId, subject, 1);
            }

            var result = await store.LoadAllAsync(subject);
            Assert.AreEqual(2, result.Count());

            foreach (var client in insertClients)
            {
                await store.RevokeAsync(subject, client.Record.ClientId);
            }
            result = await store.LoadAllAsync(subject);
            Assert.AreEqual(0, result.Count());

        }
    }
}
