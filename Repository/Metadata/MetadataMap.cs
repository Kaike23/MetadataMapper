using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Metadata
{
	using Infrastructure.Mapping;

	public class MetadataMap : IMetadataMap
	{
		private Type _domainClass;
		private string _tableName;
		private List<IColumnMap> _columns;

		public Type DomainClass { get { return _domainClass; } }
		public string TableName { get { return _tableName; } }
		public List<IColumnMap> Columns { get { return _columns; } }

		public MetadataMap(Type domainClass, string tableName)
		{
			_domainClass = domainClass;
			_tableName = tableName;
			_columns = new List<IColumnMap>();
			AddColumn("Id", "Id");
		}

		public void AddColumn(string columnName, string fieldName)
		{
			_columns.Add(new ColumnMap(columnName, fieldName, this));
		}

		public string ColumnList()
		{
			var result = new StringBuilder();
			foreach (var columnMap in _columns)
			{
				result.Append(columnMap.ColumnName + ", ");
			}
			result.Length -= 2;
			return result.ToString();
		}

		public string UpdateList()
		{
			var result = new StringBuilder(" SET ");
			result.Append(_columns[0].ColumnName + " = @" + _columns[0].ColumnName + ", ");
			for (var index = 1; index < _columns.Count; index++)
			{
				result.Append(_columns[index].ColumnName + " = @" + _columns[index].ColumnName + ", ");
			}
			result.Length -= 2;
			return result.ToString();
		}

		public string InsertList()
		{
			var result = new StringBuilder("@Id");
			for (var index = 1; index < _columns.Count; index++)
			{
				result.Append(", @" + _columns[index].ColumnName);
			}
			return result.ToString();
		}


		public string GetColumnForField(string field)
		{
			var columnMap = Columns.Find(column => column.FieldName.Equals(field));
			if (columnMap == null)
				throw new ApplicationException("Unable to find column for " + field);
			return columnMap.ColumnName;
		}
	}
}
