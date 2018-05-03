
using System;
using UnityEngine;
/// <summary>
/// Interface for spawning player.
/// </summary>
public interface ISpawnPlayerInterface
{
    bool IsSpawning { get; }
    void OnSpawn(Vector2 position);
    event Action<ISpawnPlayerInterface> OnDeathSpawn;
}