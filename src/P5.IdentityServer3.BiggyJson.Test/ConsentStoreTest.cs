using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P5.IdentityServer3.BiggyJson.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ConsentStoreTest
    {
        static void InsertTestData(ConsentStore store, int count = 1)
        {

            for (int i = 0; i < count; ++i)
            {
                for (int sub = 0; sub < 10; ++sub)
                {
                    Consent consent = new Consent() { ClientId = "CLIENTID:"+i, Scopes = new List<string>() { "a", "b" }, Subject = "SUBJECT:"+sub };
                    ConsentRecord consentRecord = new ConsentRecord(consent);
                    store.CreateAsync(consentRecord.Record);
                }
            }
        }
        private string _targetFolder;
        private ConsentStore _consentStore;

        [TestInitialize]
        public void Setup()
        {
            _targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
            _consentStore = new ConsentStore(StoreSettings.UsingFolder(_targetFolder));
            InsertTestData(_consentStore, 10);
        }
        [TestMethod]
         public void TestCreateAsync()
        {
            string testData = System.IO.File.ReadAllText(Path.Combine(_targetFolder, @"clients.json"));

            Consent consent = new Consent() { ClientId = "CLIENTID", Scopes = new List<string>() { "a", "b" }, Subject = "SUBJECT" };
            ConsentRecord consentRecord = new ConsentRecord(consent);
            _consentStore.CreateAsync(consentRecord.Record);

            var result = _consentStore.LoadAsync(consent.Subject, consent.ClientId);
            ConsentRecord consentRecordStored = new ConsentRecord(result.Result);


            Assert.AreEqual(consentRecord.Id,consentRecordStored.Id);

        }

        [TestMethod]
         public void TestUpdateAsync()
        {

            Consent consent = new Consent() { ClientId = "CLIENTID", Scopes = new List<string>() { "a", "b" }, Subject = "SUBJECT" };
            ConsentRecord consentRecord = new ConsentRecord(consent);
            _consentStore.CreateAsync(consentRecord.Record);

            var result = _consentStore.LoadAsync(consentRecord.Record.Subject, consentRecord.Record.ClientId);
            ConsentRecord consentRecordStored = new ConsentRecord(result.Result);


            Assert.AreEqual(consentRecord.Id, consentRecordStored.Id);

            consentRecord.Record.Scopes = new List<string>() {"c", "d"};
            _consentStore.UpdateAsync(consentRecord.Record);
            result = _consentStore.LoadAsync(consentRecord.Record.Subject, consentRecord.Record.ClientId);
            consentRecordStored = new ConsentRecord(result.Result);


            Assert.AreEqual(consentRecord.Id, consentRecordStored.Id);

            var query = from item in consentRecordStored.Record.Scopes
                        where item == "c"
                        select item;

            Assert.IsTrue(query.Any());
        }

        [TestMethod]
         public void TestLoadAllAsync()
        {

            var result = _consentStore.LoadAllAsync("SUBJECT:0");
            Assert.AreEqual(result.Result.Count(),10);
            foreach (var item in result.Result)
            {
                Assert.AreEqual(item.Subject, "SUBJECT:0");
            }

        }

        [TestMethod]
         public void TestRevokeAsync()
        {

            var result = _consentStore.LoadAllAsync("SUBJECT:0");
            Assert.AreEqual(result.Result.Count(), 10);
            foreach (var item in result.Result)
            {
                Assert.AreEqual(item.Subject, "SUBJECT:0");
            }
            _consentStore.RevokeAsync("SUBJECT:0", "CLIENTID:0");

            result = _consentStore.LoadAllAsync("SUBJECT:0");
            Assert.AreEqual(result.Result.Count(), 9);
            foreach (var item in result.Result)
            {
                Assert.AreEqual(item.Subject, "SUBJECT:0");
            }
        }
    }
}
