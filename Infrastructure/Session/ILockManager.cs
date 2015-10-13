using System;

namespace Infrastructure.Session
{
	public interface ILockManager
	{
		void GetLock(Guid entityId, LockType lockType);
		bool ReleaseLock(Guid entityId);
		bool ReleaseAllLocks();
		ISession Session { get; }
	}
}
