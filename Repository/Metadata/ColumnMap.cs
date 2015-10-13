using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Metadata
{
	using Infrastructure.Mapping;

	public class ColumnMap : IColumnMap
	{
		private string _columnName;
		private string _fieldName;
		private PropertyInfo _field;
		private IMetadataMap _dataMap;

		public string ColumnName { get { return _columnName; } }
		public string FieldName { get { return _fieldName; } }
		public PropertyInfo Field { get { return _field; } }

		public ColumnMap(string columnName, string fieldName, IMetadataMap dataMap)
		{
			_columnName = columnName;
			_fieldName = fieldName;
			_dataMap = dataMap;
			InitField();
		}

		private void InitField()
		{
			try
			{
				_field = _dataMap.DomainClass.GetProperty(_fieldName, BindingFlags.Instance | BindingFlags.Public);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Unable to set up field:" + _fieldName, ex);
			}
		}

		public void SetField(object result, object columnValue)
		{
			try
			{
				_field.SetValue(result, columnValue);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error in setting " + _fieldName, ex);
			}
		}

		public object GetValue(object subject)
		{
			try
			{
				return _field.GetValue(subject);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error in getting " + _fieldName, ex);
			}
		}
	}
}
