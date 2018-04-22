

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

        if (_players.Count > _spawnPoints.Count)
        {
            throw new System.Exception("Player count exceeds spawn point count!");
        }

        players.ForEach(x =>
        {
            x.OnDeathSpawn += DeathSpawn;
            var spawnPoint = GetRandomSpawnPoint();
            // search for new spawn point of one is alredy taken
            while (spawnPoints.Contains(spawnPoint))
            {
                spawnPoint = GetRandomSpawnPoint();
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
    private Vector2 GetRandomSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Count)]
            .gameObject.transform.position.ToVector2();
    }

    /// <summary>
    /// Gets the best spawn position
    /// </summary>
    private Vector2 GetOptimizedSpawnPoint(ISpawnPlayerInterface player)
    {
        // Loop throught players and find distance from spawn point for each spawn point and player
        var spawnPointCandidates = new Dictionary<GameObject, IEnumerable<float>>();

        foreach (var spawnPoint in _spawnPoints)
        {
            spawnPointCandidates.Add(spawnPoint,
                _players.Where(x => !x.Equals(player))
                        .Select(x => Vector3.Distance(x.transform.position, spawnPoint.transform.position)));
        }

        // select minimum from each, and then from this select max
        var minimumForSpawnPoints = spawnPointCandidates
            .ToDictionary(k => k.Key, v => v.Value.Min());

        return minimumForSpawnPoints
            .First(x => x.Value == minimumForSpawnPoints.Max(y => y.Value))
            .Key.transform.position.ToVector2();
    }

    /// <summary>
    /// Spawns player, wait for 3 seconds before spawning.
    /// </summary>
    private IEnumerator SpawnWait(ISpawnPlayerInterface player)
    {
        yield return new WaitForSeconds(3f);
        var spawnPoint = GetOptimizedSpawnPoint(player);
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
    private void DeathSpawn(ISpawnPlayerInterface player)
    {
        // wait for couple of seconds before new spawn.
        StartCoroutine(SpawnWait(player));
    }

    #endregion
}
