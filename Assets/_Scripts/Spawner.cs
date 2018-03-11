

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private bool gameStart = true;

    // editor variables
    public List<GameObject> _spawnPoints;
    public List<Player> _players = new List<Player>();

    private Random random;

    // Use this for initialization
    void Start()
    {
        var spawnPoints = new List<Vector2>();
        _players.ForEach(x =>
        {
            x.OnDeath += StartSpawnPlayer;
            var spawnPoint = GetSpawnPoint();
            while (spawnPoints.Contains(spawnPoint))
            {
                spawnPoint = GetSpawnPoint();                                                                                                                                
            }
            StartSpawnPlayer(spawnPoint, x);
        });
    }

    private Vector2 GetSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Count)]
            .gameObject.transform.position.ToVector2();
    }

    private IEnumerator SpawnWait(Player player)
    {
        yield return new WaitForSeconds(3f);
        var spawnPoint = GetSpawnPoint();
        SpawnPlayer(spawnPoint, player);
    }

    private void SpawnPlayer(Vector2 spawnPoint, Player player)
    {
        player.Spawn(spawnPoint);
        player.SetPosition(spawnPoint);
    }

    private void StartSpawnPlayer(Player player)
    {
        var spawnPoint = GetSpawnPoint();
        StartSpawnPlayer(spawnPoint, player);
    }

    private void StartSpawnPlayer(Vector2 spawnPoint, Player player)
    {
        if (gameStart)
        {
            SpawnPlayer(spawnPoint, player);
            gameStart = false;
        }
        else
        {
            StartCoroutine(SpawnWait(player));
        }
    }
}
