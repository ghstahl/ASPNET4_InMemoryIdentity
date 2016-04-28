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
    public class ConsentStoreTest
    {
        static void InsertTestData(ConsentStore store)
        {

            for (int i = 0; i < 10; ++i)
            {
                for(int sub = 0; sub < 10;++sub)
                {
                    Consent consent = new Consent() { ClientId = "CLIENTID:"+i, Scopes = new List<string>() { "a", "b" }, Subject = "SUBJECT:"+sub };
                    ConsentRecord consentRecord = new ConsentRecord(consent);
                    store.CreateAsync(consentRecord.Record);
                }
            }
        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestCreateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
            string testData = System.IO.File.ReadAllText(Path.Combine(targetFolder, @"clients.json"));
            ConsentStore store = new ConsentStore(targetFolder);
            Consent consent = new Consent() { ClientId = "CLIENTID", Scopes = new List<string>() { "a", "b" }, Subject = "SUBJECT" };
            ConsentRecord consentRecord = new ConsentRecord(consent);
            store.CreateAsync(consentRecord.Record);

            var result = store.LoadAsync(consent.Subject, consent.ClientId);
            ConsentRecord consentRecordStored = new ConsentRecord(result.Result);


            Assert.AreEqual(consentRecord.Id,consentRecordStored.Id);

        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestUpdateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
            ConsentStore store = new ConsentStore(targetFolder);
            Consent consent = new Consent() { ClientId = "CLIENTID", Scopes = new List<string>() { "a", "b" }, Subject = "SUBJECT" };
            ConsentRecord consentRecord = new ConsentRecord(consent);
            store.CreateAsync(consentRecord.Record);

            var result = store.LoadAsync(consentRecord.Record.Subject, consentRecord.Record.ClientId);
            ConsentRecord consentRecordStored = new ConsentRecord(result.Result);


            Assert.AreEqual(consentRecord.Id, consentRecordStored.Id);

            consentRecord.Record.Scopes = new List<string>() {"c", "d"};
            store.UpdateAsync(consentRecord.Record);
            result = store.LoadAsync(consentRecord.Record.Subject, consentRecord.Record.ClientId);
            consentRecordStored = new ConsentRecord(result.Result);


            Assert.AreEqual(consentRecord.Id, consentRecordStored.Id);

            var query = from item in consentRecordStored.Record.Scopes
                        where item == "c"
                        select item;

            Assert.IsTrue(query.Any());
        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestLoadAllAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
            ConsentStore store = new ConsentStore(targetFolder);
            InsertTestData(store);

            var result = store.LoadAllAsync("SUBJECT:0");
            Assert.AreEqual(result.Result.Count(),10);
            foreach (var item in result.Result)
            {
                Assert.AreEqual(item.Subject, "SUBJECT:0");
            }

        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestRevokeAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
            ConsentStore store = new ConsentStore(targetFolder);
            InsertTestData(store);

            var result = store.LoadAllAsync("SUBJECT:0");
            Assert.AreEqual(result.Result.Count(), 10);
            foreach (var item in result.Result)
            {
                Assert.AreEqual(item.Subject, "SUBJECT:0");
            }
            store.RevokeAsync("SUBJECT:0", "CLIENTID:0");

            result = store.LoadAllAsync("SUBJECT:0");
            Assert.AreEqual(result.Result.Count(), 9);
            foreach (var item in result.Result)
            {
                Assert.AreEqual(item.Subject, "SUBJECT:0");
            }
        }
    }
}
