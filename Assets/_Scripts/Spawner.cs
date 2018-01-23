

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private bool gameStart = true;

    [SerializeField]
    private List<GameObject> spawnPoints;

    [SerializeField]
    private Player player;

    private Random random;

    // Use this for initialization
    void Start()
    {
        StartSpawnPlayer();
        player.OnDeath += StartSpawnPlayer;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector2 GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)]
            .gameObject.transform.position.ToVector2();
    }

    private IEnumerator SpawnWait()
    {
        yield return new WaitForSeconds(3f);
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var spawnPoint = GetSpawnPoint();
        player.Spawn(spawnPoint);
        player.SetPosition(spawnPoint);
    }

    private void StartSpawnPlayer()
    {
        if (gameStart)
        {
            SpawnPlayer();
            gameStart = false;
        }
        else
        {
            StartCoroutine(SpawnWait());
        }
    }
}
