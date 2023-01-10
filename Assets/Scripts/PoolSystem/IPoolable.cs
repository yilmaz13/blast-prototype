public interface IPoolable
{
    /// <summary>
    /// Called when the object goes to the pool
    /// </summary>
    void OnReturnPool();

    /// <summary>
    /// Called when the object is called from the pool
    /// </summary>
    void OnPoolSpawn();
}
