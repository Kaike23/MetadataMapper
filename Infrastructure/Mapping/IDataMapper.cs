using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping
{
	using Infrastructure.Domain;

	public interface IDataMapper
	{
		IMetadataMap MetadataMap { get; }

		EntityBase Find(Guid Id);
		List<EntityBase> FindMany(IStatementSource source);
		List<EntityBase> FindMany(string where);

		Guid Insert(EntityBase entity);
		void Update(EntityBase entity);
		void Delete(EntityBase entity);
	}
}
