using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Domain
{
	public abstract class EntityBase : IEntity
	{
		public EntityBase() { }

		public EntityBase(Guid id)
		{
			Id = id;
		}

		public void SetSystemFields(long version, string modifiedBy, DateTime modified)
		{
			Version = version;
			Modified = modified;
			ModifiedBy = modifiedBy;
		}

		public Guid Id { get; set; }
		public DateTime Created { get; set; }
		public string CreatedBy { get; set; }
		public DateTime Modified { get; set; }
		public string ModifiedBy { get; set; }
		public long Version { get; set; }
	}
}
