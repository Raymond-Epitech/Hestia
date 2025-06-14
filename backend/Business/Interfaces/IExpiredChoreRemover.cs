namespace Business.Interfaces;

public interface IExpiredChoreRemover
{
    Task PurgeOldDataAsync();
}

