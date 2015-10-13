using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Domain
{
	using Infrastructure.Domain;

	public interface IRepository<T> : IReadOnlyRepository<T>
		where T : IAggregateRoot
	{
		void Add(T entity);
		void Update(T entity);
		void Remove(T entity);
	}
}
