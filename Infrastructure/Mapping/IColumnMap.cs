using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping
{
	public interface IColumnMap
	{
		string ColumnName { get; }
		string FieldName { get; }
		PropertyInfo Field { get; }

		void SetField(object result, object columnValue);
		object GetValue(object subject);
	}
}
