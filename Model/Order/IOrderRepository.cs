using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Order
{
	using Infrastructure.Domain;

	public interface IOrderRepository : IRepository<Order>
	{
		IEnumerable<Order> FindAllBy(Guid customerId);
	}
}
