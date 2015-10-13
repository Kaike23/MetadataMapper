using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Mapping
{
	using Infrastructure.Domain;
	using Infrastructure.Identity;
	using Infrastructure.Mapping;
	using Model.Customer;
	using Model.Order;
	using Repository.Mapping.SQL;

	public class SqlMappingRegistry
	{
		private Dictionary<Type, IDataMapper> mappers = new Dictionary<Type, IDataMapper>();

		public SqlMappingRegistry()
		{
			Init();
		}

		private void Init()
		{
			RegisterMapper(typeof(Customer), new CustomerMapper());
			RegisterMapper(typeof(Order), new OrderMapper());
		}

		public void RegisterMapper(Type domainObject, IDataMapper mapper)
		{
			mappers.Add(domainObject, mapper);
		}
		public IDataMapper GetMapper(Type domainObject)
		{
			return mappers[domainObject];
		}
	}
}
