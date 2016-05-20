using System.Collections.Generic;
using Cassandra.Mapping;
using FlattenedDocument.CassandraStore.DAO;
using P5.Store.Core.Models;

namespace ProductStore.Cassandra.DAO
{
    public class MyMappings : Mappings
    {
        private static bool _init { get; set; }

        public static void Init(string seedTableName)
        {
            if (!_init)
            {
                string tableById = FlattenedDocumentCassandraDao.TableByIdName(seedTableName);
                string tableByTypeAndVersion = FlattenedDocumentCassandraDao.TableByTypeAndVersionName(seedTableName);
                List<Mappings> mappings = new List<Mappings>
                {
                    new MyMappings(tableById, tableByTypeAndVersion)
                };
                MappingConfiguration.Global.Define(mappings.ToArray());
                _init = true;
            }

        }

        public MyMappings(string tableById, string tableByTypeAndVersion)
        {
            // Define mappings in the constructor of your class
            // that inherits from Mappings
            For<DocumentRecord>()
                .TableName(tableById);

        }
    }
}