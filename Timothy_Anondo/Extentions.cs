using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Reflection;

namespace Timothy_Anondo
{
    public static class Extentions
    {

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        //http://stackoverflow.com/questions/20270599/entity-framework-refresh-context
        //https://msdn.microsoft.com/en-us/library/system.data.entity.infrastructure.dbentityentry.reload(v=vs.113).aspx#M:System.Data.Entity.Infrastructure.DbEntityEntry.Reload
        public static void ReloadEntity<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            context.Entry(entity).Reload();
        }

        public static void ReloadNavigationProperty<TEntity, TElement>(this DbContext context, TEntity entity, Expression<Func<TEntity, ICollection<TElement>>> navigationProperty) where TEntity : class where TElement : class
        {
            context.Entry(entity).Collection<TElement>(navigationProperty).Query();
        }


        /// <summary>
        /// Refresh non-detached entities
        /// </summary>
        /// <param name="dbContext">context of the entities</param>
        /// <param name="refreshMode">store or client wins</param>
        /// <param name="entityType">when specified only entities of that type are refreshed. when null all non-detached entities are modified</param>
        /// <returns></returns>
        public static DbContext RefreshEntites(this DbContext dbContext, RefreshMode refreshMode, Type entityType)
        {
            //https://christianarg.wordpress.com/2013/06/13/entityframework-refreshall-loaded-entities-from-database/
            var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            var refreshableObjects = objectContext.ObjectStateManager
                .GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged)
                .Where(x => entityType == null || x.Entity.GetType() == entityType)
                .Where(entry => entry.EntityKey != null)
                .Select(e => e.Entity)
                .ToArray();

            objectContext.Refresh(RefreshMode.StoreWins, refreshableObjects);

            return dbContext;
        }

        public static DbContext RefreshAllEntites(this DbContext dbContext, RefreshMode refreshMode)
        {
            return RefreshEntites(dbContext: dbContext, refreshMode: refreshMode, entityType: null); //null entityType is a wild card
        }

        public static DbContext RefreshEntites<TEntity>(this DbContext dbContext, RefreshMode refreshMode)
        {
            return RefreshEntites(dbContext: dbContext, refreshMode: refreshMode, entityType: typeof(TEntity));
        }

        public static System.Collections.IEnumerable DynamicSqlQuery(this Database database, string sql, params object[] parameters)
        {
            TypeBuilder builder = createTypeBuilder("MyDynamicAssembly", "MyDynamicModule", "MyDynamicType");

            using (System.Data.IDbCommand command = database.Connection.CreateCommand())
            {
                try
                {
                    database.Connection.Open();
                    command.CommandText = sql;
                    command.CommandTimeout = command.Connection.ConnectionTimeout;
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    using (System.Data.IDataReader reader = command.ExecuteReader())
                    {
                        var schema = reader.GetSchemaTable();

                        foreach (System.Data.DataRow row in schema.Rows)
                        {
                            string name = (string)row["ColumnName"];
                            //var a=row.ItemArray.Select(d=>d.)
                            Type type = (Type)row["DataType"];
                            if (type != typeof(string) && (bool)row.ItemArray[schema.Columns.IndexOf("AllowDbNull")])
                            {
                                type = typeof(Nullable<>).MakeGenericType(type);
                            }
                            createAutoImplementedProperty(builder, name, type);
                        }
                    }
                }
                finally
                {
                    database.Connection.Close();
                    command.Parameters.Clear();
                }
            }

            Type resultType = builder.CreateType();

            return database.SqlQuery(resultType, sql, parameters);
        }


        public static object DynamicSqlQuery2(this Database database, string sql, params object[] parameters)
        {
            TypeBuilder builder = createTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");
            createAutoImplementedProperty(builder, "Id", typeof(string));
            createAutoImplementedProperty(builder, "FirstName", typeof(string));
            createAutoImplementedProperty(builder, "LastName", typeof(int));

            Type resultType = builder.CreateType();

            dynamic queryResult = (database.SqlQuery(resultType, sql) as IEnumerable<object>).ToList();
            return queryResult;
        }


        private static TypeBuilder createTypeBuilder(string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName),
                                       AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        private static void createAutoImplementedProperty(TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(
                string.Concat(PrivateFieldPrefix, propertyName),
                              propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(
                propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(
                string.Concat(GetterPrefix, propertyName),
                propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(
                string.Concat(SetterPrefix, propertyName),
                propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }
    }
}
