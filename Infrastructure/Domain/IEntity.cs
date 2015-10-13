using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Domain
{
	public interface IEntity
	{
		Guid Id { get; }
		DateTime Created { get; set; }
		string CreatedBy { get; set; }
		DateTime Modified { get; }
		string ModifiedBy { get; }
		long Version { get; }
	}
}
