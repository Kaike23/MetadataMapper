using System;

namespace Infrastructure.Identity
{
	using Infrastructure.Domain;

	public interface IIdentityMap
	{
		IEntity Get(Guid key);
		void Add(Guid key, IEntity value);
		void Remove(Guid key);
		void Clear();
	}
}
