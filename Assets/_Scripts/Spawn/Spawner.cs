

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// The spawner object.
/// </summary>
public class Spawner : MonoBehaviour
{
    #region Editor variables

    public List<GameObject> _spawnPoints = new List<GameObject>();
    public List<GameObject> _players = new List<GameObject>();

    #endregion

    #region Fields

    private bool gameStart = true;
    private Random random;

    #endregion

    #region Unity methods

    private void Start()
    {
        var spawnPoints = new List<Vector2>();
        var players = _players
            .Select(x => x.GetComponent<ISpawnPlayerInterface>())
            .ToList();

        if(_players.Count > _spawnPoints.Count)
        {
            throw new System.Exception("Player count exceeds spawn point count!");
        }

        players.ForEach(x =>
        {
            x.OnStartSpawning += StartSpawnPlayer;
            var spawnPoint = GetSpawnPoint();
            while (spawnPoints.Contains(spawnPoint))
            {
                spawnPoint = GetSpawnPoint();
            }
            SpawnPlayer(spawnPoint, x);
            gameStart = false;
        });
    }


    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the available spawn point.
    /// </summary>
    private Vector2 GetSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Count)]
            .gameObject.transform.position.ToVector2();
    }

    /// <summary>
    /// Spawns player.
    /// </summary>
    private IEnumerator SpawnWait(ISpawnPlayerInterface player)
    {
        yield return new WaitForSeconds(3f);
        var spawnPoint = GetSpawnPoint();
        SpawnPlayer(spawnPoint, player);
    }

    /// <summary>
    /// Start spawning player.
    /// </summary>
    private void SpawnPlayer(Vector2 spawnPoint, ISpawnPlayerInterface player)
    {
        player.Spawn(spawnPoint);
    }

    /// <summary>
    /// Starts 
    /// </summary>
    /// <param name="player"></param>
    private void StartSpawnPlayer(ISpawnPlayerInterface player)
    {
        var spawnPoint = GetSpawnPoint();
        StartCoroutine(SpawnWait(player));
    }

    #endregion
}
