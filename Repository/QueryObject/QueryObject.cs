using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.QueryObject
{
	using Infrastructure.Domain;
	using UnitOfWork;

	public class QueryObject
	{
		private Type domainObject;
		private List<Criteria> criteriaList = new List<Criteria>();
		private UnitOfWork uow;

		public QueryObject(Type domainObject)
		{
			this.domainObject = domainObject;
		}

		public void AddCriteria(Criteria criteria)
		{
			criteriaList.Add(criteria);
		}

		public List<EntityBase> Execute(UnitOfWork uow)
		{
			this.uow = uow;
			return uow.GetMapper(domainObject).FindMany(GenerateWhereClause());
		}

		private string GenerateWhereClause()
		{
			var result = new StringBuilder();
			foreach (var criteria in criteriaList)
			{
				if (result.Length > 0)
					result.Append(" AND ");
				result.Append(criteria.GenerateSql(uow.GetMapper(domainObject).MetadataMap));
			}
			return result.ToString();
		}
	}
}
