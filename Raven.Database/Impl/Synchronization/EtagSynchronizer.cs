using Raven.Abstractions.Data;
using Raven.Database.Storage;
using Raven.Json.Linq;

namespace Raven.Database.Impl.Synchronization
{
	public class EtagSynchronizer
	{
		private readonly object locker = new object();

		private Etag currentEtag;

		private Etag synchronizationEtag;

		private readonly EtagSynchronizerType type;

		private readonly ITransactionalStorage transactionalStorage;

		public EtagSynchronizer(EtagSynchronizerType type, ITransactionalStorage transactionalStorage)
		{
			this.type = type;
			this.transactionalStorage = transactionalStorage;

			LoadSynchronizationState();
		}

		public void UpdateSynchronizationState(Etag lowestEtag)
		{
			lock (locker)
			{
				if (UpdateSynchronizationStateInternal(lowestEtag))
					PersistSynchronizationState();
			}
		}

		public Etag CalculateSynchronizationEtag(Etag etag, Etag lastProcessedEtag)
		{
			if (etag == null)
			{
				if (lastProcessedEtag != null)
				{
					lock (locker)
					{
						if (currentEtag == null && lastProcessedEtag.CompareTo(synchronizationEtag) != 0)
						{
							synchronizationEtag = lastProcessedEtag;
							PersistSynchronizationState();
						}
					}

					return lastProcessedEtag;
				}

				return Etag.Empty;
			}

			if (lastProcessedEtag == null)
				return Etag.Empty;

			if (etag.CompareTo(lastProcessedEtag) < 0)
				return etag;

			return lastProcessedEtag;
		}

		public Etag GetSynchronizationEtag()
		{
			lock (locker)
			{
				if (currentEtag != null)
				{
					PersistSynchronizationState();
					synchronizationEtag = currentEtag;
					currentEtag = null;
				}

				return currentEtag;
			}
		}

		private bool UpdateSynchronizationStateInternal(Etag lowestEtag)
		{
			if (currentEtag == null || lowestEtag.CompareTo(currentEtag) < 0)
			{
				currentEtag = lowestEtag;
			}

			if (lowestEtag.CompareTo(synchronizationEtag) < 0)
				return true;

			return false;
		}

		private void PersistSynchronizationState()
		{
			transactionalStorage.Batch(
				actions => actions.Lists.Set("Raven/Etag/Synchronization", type.ToString(), RavenJObject.FromObject(new
				{
					etag = GetEtagForPersistance()
				}), UuidType.EtagSynchronization));
		}

		private void LoadSynchronizationState()
		{
			transactionalStorage.Batch(actions =>
			{
				var state = actions.Lists.Read("Raven/Etag/Synchronization", type.ToString());
				if (state == null)
				{
					currentEtag = null;
					synchronizationEtag = Etag.Empty;
					return;
				}

				var etag = Etag.Parse(state.Data.Value<string>("etag"));
				currentEtag = etag;
				synchronizationEtag = etag;
			});
		}

		private Etag GetEtagForPersistance()
		{
			Etag result;
			if (currentEtag != null)
			{
				result = currentEtag.CompareTo(synchronizationEtag) < 0
					         ? currentEtag
					         : synchronizationEtag;
			}
			else
			{
				result = synchronizationEtag;
			}

			return result ?? Etag.Empty;
		}
	}
}