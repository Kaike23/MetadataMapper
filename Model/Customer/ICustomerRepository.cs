using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Customer
{
	using Infrastructure.Domain;

	public interface ICustomerRepository : IRepository<Customer>
	{
	}
}
