﻿using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.GraphIoT.Core.Database.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Core.Database
{
    public class DatabaseBackupService<TDbContext> where TDbContext : DbContext
    {
        private readonly TDbContext db;

        public DatabaseBackupService(TDbContext databaseContext)
        {
            db = databaseContext;
        }

        public class BackupRoot
        {
            [JsonPropertyName("tables")]
            public List<BackupTable> Tables { get; set; } = null!;
        }

        public class BackupTable
        {
            [JsonPropertyName("table")]
            public string Table { get; set; } = null!;

            [JsonPropertyName("columns")]
            public List<string> Columns { get; set; } = null!;

            [JsonPropertyName("rows")]
            public List<List<object>> Rows { get; set; } = null!;
        }

        /// <summary>
        /// Returns an object that is serializable to JSON containing tables of the database context
        /// that derive from <see cref="ITimeSeriesDbEntity"/> and have a <see cref="ITimeSeriesDbEntity.Key"/>
        /// between the given start and end date. If tableFilter is not null, only tables with a name
        /// contained in this list are exported.
        /// </summary>
        public IEnumerable<BackupTable> CreateBackup(DateTime start, DateTime end, ICollection<string> tableFilter)
        {
            return GetTables(db, tableFilter).Select(table => new BackupTable()
            {
                Table = table.property.Name,
                Columns = GetColumnNames(table.value),
                Rows = GetColumns(table.value, start, end)
            });
        }

        /// <summary>
        /// Restore by reading the given stream and parsing the json backup and saving it into the respective
        /// database tables.
        /// </summary>
        public async Task RestoreBackup(Stream backupToLoad, CancellationToken cancellationToken = default)
        {
            var backupData = await JsonSerializer.DeserializeAsync<BackupRoot>(backupToLoad);

            if (backupData is null)
                throw new IOException("Backup could not be parsed - wrong format");

            var tableFilters = backupData?.Tables?.Select(table => table.Table.Trim())?.ToList() ?? new List<string>();

            foreach (var (property, value) in GetTables(db, tableFilters))
            {
                var tableData = backupData?.Tables.Where(tableToRestore => tableToRestore.Table == property.Name).FirstOrDefault();
                if (tableData == null)
                    continue;

                var columnNames = GetColumnNames(value).ToList();
                if (tableData.Columns.Count != columnNames.Count)
                    throw new IOException($"Backup has different number of columns than current database in table \"{property.Name}\"");

                if (!tableData.Columns.All(x => x == columnNames[tableData.Columns.IndexOf(x)]))
                    throw new IOException($"Backup has different columns or in different order than current database in table \"{property.Name}\"");

                var columnProps = GetColumnProperties(value).ToList();
                foreach (var row in tableData.Rows)
                    RestoreBackupRow(value, row, property, columnProps);
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        private void RestoreBackupRow(IQueryable<object> tableDbSet, List<object> rowData, PropertyInfo tableProperty, IList<PropertyInfo> columnProperties)
        {
            var entityType = tableProperty.PropertyType.GenericTypeArguments.First();
            var newEntity = Activator.CreateInstance(entityType);
            if (newEntity is null)
                throw new Exception($"Could not instantiate {entityType}");

            for (int i = 0; i < Math.Min(columnProperties.Count, rowData.Count); i++)
                columnProperties[i].SetPropertyValue(newEntity, rowData[i]);

            var methodInfoBase = ReflectionExtensions.GetMethodInfo<DbSet<TimeSeriesDbEntityBase>, TDbContext, TimeSeriesDbEntityBase, TimeSeriesDbEntityBase>((a, b, c) => a.AddOrUpdate(b, c));
            var methodInfo = methodInfoBase.GetGenericMethodDefinition().MakeGenericMethod(typeof(TDbContext), entityType);

            methodInfo.Invoke(null, new object[] { tableDbSet, db, newEntity });
        }

        private IEnumerable<(PropertyInfo property, IQueryable<object> value)> GetTables(DbContext dbContext, ICollection<string> tableFilter)
        {
            var tableProperties = dbContext.GetType().GetProperties()
                    .Where(prop => prop.PropertyType.IsSubclassOfGeneric(typeof(DbSet<>)))
                    .Where(prop => tableFilter.Count <= 0 ? true : tableFilter.Contains(prop.Name, StringComparer.InvariantCultureIgnoreCase));

            return tableProperties.Select(prop => (prop, prop.GetGetMethod()?.Invoke(dbContext, null) as IQueryable<object>))
                .Where(x => x.Item2 != null).Select(x => (x.prop, x.Item2!));
        }

        private IEnumerable<PropertyInfo> GetColumnProperties(IQueryable<object> table)
        {
            return table.GetType().GenericTypeArguments.First().GetProperties()
                .Where(prop => !Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                .Where(prop => !Attribute.IsDefined(prop, typeof(ForeignKeyAttribute)))
                .Where(prop => prop.CanRead && prop.CanWrite);
        }

        private List<string> GetColumnNames(IQueryable<object> table)
        {
            return GetColumnProperties(table).Select(prop => prop.Name).ToList();
        }

        private List<List<object>> GetColumns(IQueryable<object> table, DateTime start, DateTime end)
        {
            var columnProperties = GetColumnProperties(table);

            IQueryable<object> rows;
            if (table is IQueryable<ITimeSeriesDbEntity> timeSeriesTable)
                rows = timeSeriesTable.AsNoTracking().Where(x => x.Key >= start && x.Key <= end);
            else
                rows = table;

            return rows.ToList().Select(row => columnProperties.Select(prop => prop.GetGetMethod())
                                                               .Where(getter => getter != null)
                                                               .Select(getter => getter!.Invoke(row, null)).ToList()).ToList();
        }
    }
}