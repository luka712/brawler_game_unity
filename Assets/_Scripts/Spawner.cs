

using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> spawnPoints;

    [SerializeField]
    private Player player;

    private Random random;

    // Use this for initialization
    void Start()
    {
        SpawnPlayer();
        player.OnDeath += SpawnPlayer;
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

    private void SpawnPlayer()
    {
        var spawnPoint = GetSpawnPoint();
        player.Spawn(spawnPoint);
        player.SetPosition(spawnPoint);
    }
}
