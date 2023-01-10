using Tile;
using UnityEngine;

[System.Serializable]
public struct TileMatchCondition
{
    [SerializeField] public PowerUpCondition powerUpCondition;
    [SerializeField] private Sprite sprite;
    [SerializeField] private TileTypeDefinition[] entitiesToSpawnOnMatch; // used for after match spawns like powerups

    public TileTypeDefinition GetRandomEntityToSpawn()
    {
        if (entitiesToSpawnOnMatch == null || entitiesToSpawnOnMatch.Length == 0) return null;
        return entitiesToSpawnOnMatch[Random.Range(0, entitiesToSpawnOnMatch.Length)];
    }

    public Sprite Sprite => sprite;

    public PowerUpCondition PowerUpCondition
    {
        get => powerUpCondition;
        set => powerUpCondition = value;
    }
}