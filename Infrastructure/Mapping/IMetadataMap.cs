using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping
{
	public interface IMetadataMap
	{
		Type DomainClass { get; }
		string TableName { get; }
		List<IColumnMap> Columns { get; }

		void AddColumn(string columnName, string fieldName);
		string ColumnList();
		string UpdateList();
		string InsertList();

		string GetColumnForField(string field);
	}
}
