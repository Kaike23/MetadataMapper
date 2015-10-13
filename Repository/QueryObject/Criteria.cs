using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.QueryObject
{
	using Infrastructure.Mapping;
	using Repository.Metadata;

	public abstract class Criteria
	{
		protected string sqlOperator;
		protected string field;
		protected object value;

		public static Criteria GreaterThan(string fieldName, int value)
		{
			return new GreaterThanCriteria(fieldName, (object)value);
		}
		public static Criteria Matches(string fieldName, string pattern)
		{
			return new MatchCriteria(fieldName, pattern);
		}
		protected Criteria(string sql, string field, object value)
		{
			sqlOperator = sql;
			this.field = field;
			this.value = value;
		}

		public abstract string GenerateSql(IMetadataMap metadataMap);
	}

	public class GreaterThanCriteria : Criteria
	{
		public GreaterThanCriteria(string fieldName, object value)
			: base(" > ", fieldName, value) { }

		public override string GenerateSql(IMetadataMap metadataMap)
		{
			return string.Format("{0} {1} {2}", metadataMap.GetColumnForField(field), sqlOperator, value);
		}
	}

	public class MatchCriteria : Criteria
	{
		public MatchCriteria(string fieldName, object value)
			: base(null, fieldName, value) { }

		public override string GenerateSql(IMetadataMap metadataMap)
		{
			return string.Format("UPPER({0}) LIKE UPPER('{1}')", metadataMap.GetColumnForField(field), value);
		}
	}
}
