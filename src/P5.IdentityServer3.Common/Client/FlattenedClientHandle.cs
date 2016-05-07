﻿using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Common.Models;

namespace P5.IdentityServer3.Common
{
    public class FlattenedClientHandle :
        AbstractClientHandle<
            string,
            string,
            string
            >
    {
        public FlattenedClientHandle()
            : base()
        {

        }

        public FlattenedClientHandle(Client client):base(client)
        {
            
        }
        public override string Serialize(List<string> stringList)
        {
            if (stringList == null)
                return null;
            var simpleDocument = new SimpleDocument<List<string>>(stringList).DocumentJson;
            return simpleDocument;
        }

        public override List<string> DeserializeStrings(string obj)
        {
            var simpleDocument = new SimpleDocument<List<string>>(obj);
            var document = (List<string>)simpleDocument.Document;
            return document;
        }

        public override string Serialize(List<Claim> claims)
        {
            var normalized = claims == null ? null : claims.ToClaimTypeRecords();
            if (normalized == null)
                return null;
            var simpleDocument = new SimpleDocument<List<ClaimTypeRecord>>(normalized).DocumentJson;
            return simpleDocument;
        }

        public override List<Claim> DeserializeClaims(string obj)
        {
            var simpleDocument = new SimpleDocument<List<ClaimTypeRecord>>(obj);
            var document = (List<ClaimTypeRecord>) simpleDocument.Document;
            var result = document.ToClaims();
            return result;
        }

        public override string Serialize(List<Secret> secrets)
        {
            if (secrets == null)
                return null;
            var simpleDocument = new SimpleDocument<List<Secret>>(secrets).DocumentJson;
            return simpleDocument;
        }

        public override List<Secret> DeserializeSecrets(string obj)
        {
            var simpleDocument = new SimpleDocument<List<Secret>>(obj);
            var document = (List<Secret>)simpleDocument.Document;
            return document;
        }
    }
}
