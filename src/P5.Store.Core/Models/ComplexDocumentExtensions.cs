using System;
using ProductStore.Core;

namespace P5.Store.Core.Models
{
    public static class ComplexDocumentExtensions
    {
        public static Guid CreateGuid<T>(this ComplexDocument<T> obj, Guid @namespace) where T : class
        {
            return GuidGenerator.CreateGuid(@namespace, obj.DocumentType,obj.DocumentVersion);
        }
    }
}