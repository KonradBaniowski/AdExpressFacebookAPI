using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Facebook.DataAccess.Mapping
{
    public static class MapperFunctionOracle
    {
        /// <summary>
        /// Maps the result of a query into entities.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="queryConnection">The connection to run the query. Must be different from the one on the context.</param>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns>An entity list</returns>
        /// <exception cref="System.ArgumentNullException">
        /// context
        /// or
        /// queryConnection
        /// or
        /// sqlQuery
        /// List students = Mapper.Map(context, (new SchoolContext()).Database.Connection, "Select * from Students");
        /// </exception>
        public static List<T> Map<T>(DbContext context, DbConnection queryConnection, string sqlQuery) where T : new()
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (queryConnection == null)
                throw new ArgumentNullException("queryConnection");
            if (sqlQuery == null)
                throw new ArgumentNullException("sqlQuery");

            var connectionState = queryConnection.State;

            if (connectionState != ConnectionState.Open)
                queryConnection.Open();

            DbCommand command = queryConnection.CreateCommand();
            command.CommandText = sqlQuery;
            DbDataReader reader = command.ExecuteReader();

            List<T> entities = new List<T>();

            while (reader.Read())
            {
                entities.Add(InternalMap<T>(context, reader));
            }

            if (connectionState != ConnectionState.Open)
                queryConnection.Close();

            return entities;

        }

        private static T InternalMap<T>(DbContext context, DbDataReader reader) where T : new()
        {

            T entityObject = new T();

            InternalMapEntity(context, reader, entityObject);

            return entityObject;
        }

        private static void InternalMapEntity(DbContext context, DbDataReader reader, object entityObject)
        {
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            var metadataWorkspace = ((EntityConnection)objectContext.Connection).GetMetadataWorkspace();

            IEnumerable<EntitySetMapping> entitySetMappingCollection = metadataWorkspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace).Single().EntitySetMappings;
            IEnumerable<AssociationSetMapping> associationSetMappingCollection = metadataWorkspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace).Single().AssociationSetMappings;

            var entitySetMappings = entitySetMappingCollection.First(o => o.EntityTypeMappings.Select(e => e.EntityType.Name).Contains(entityObject.GetType().Name));

            var entityTypeMapping = entitySetMappings.EntityTypeMappings[0];
            string tableName = entityTypeMapping.EntitySetMapping.EntitySet.Name;
            Console.WriteLine(tableName);

            MappingFragment mappingFragment = entityTypeMapping.Fragments[0];

            foreach (PropertyMapping propertyMapping in mappingFragment.PropertyMappings)
            {
                if (reader[((ScalarPropertyMapping)propertyMapping).Column.Name] != null)
                {
                    object value = Convert.ChangeType(reader[((ScalarPropertyMapping)propertyMapping).Column.Name], propertyMapping.Property.PrimitiveType.ClrEquivalentType);
                    entityObject.GetType().GetProperty(propertyMapping.Property.Name).SetValue(entityObject, value, null);
                    Console.WriteLine("{0} {1} {2}", propertyMapping.Property.Name, ((ScalarPropertyMapping)propertyMapping).Column, value);
                }
                
            }

            foreach (var navigationProperty in entityTypeMapping.EntityType.NavigationProperties)
            {
                PropertyInfo propertyInfo = entityObject.GetType().GetProperty(navigationProperty.Name);

                AssociationSetMapping associationSetMapping = associationSetMappingCollection.First(a => a.AssociationSet.ElementType.FullName == navigationProperty.RelationshipType.FullName);

                // associationSetMapping.AssociationTypeMapping.MappingFragment.PropertyMappings contains two elements one for direct and one for inverse relationship
                EndPropertyMapping propertyMappings = associationSetMapping.AssociationTypeMapping.MappingFragment.PropertyMappings.Cast<EndPropertyMapping>().First(p => p.AssociationEnd.Name.EndsWith("_Target"));

                object[] key = propertyMappings.PropertyMappings.Select(c => reader[c.Column.Name]).ToArray();
                object value = context.Set(propertyInfo.PropertyType).Find(key);
                propertyInfo.SetValue(entityObject, value, null);
            }

        }

    }
}