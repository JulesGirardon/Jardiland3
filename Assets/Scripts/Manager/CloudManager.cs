using System.Collections.Generic;
using UnityEngine;
using LitMotion;

public class CloudManager : MonoBehaviour
{
    [Header("Cloud Prefabs")]
    [Tooltip("List of cloud prefabs to spawn.")]
    public List<GameObject> cloudPrefabs;

    [Header("Spawn Settings")]
    [Tooltip("Z position range for spawning clouds.")]
    public Vector2 spawnZRange = new Vector2(-40f, 120f);
    [Tooltip("Y position for spawning clouds.")]
    public float spawnY = 12f;
    [Tooltip("Interval between cloud spawns.")]
    public float spawnInterval = 0.3f;

    [Header("Movement Settings")] 
    [Tooltip("Starting X position for clouds.")]
    public float startX = 40f;
    [Tooltip("Target X position for clouds.")]
    public float targetX = -40f;
    [Tooltip("Speed range for cloud movement.")]
    public Vector2 speedRange = new Vector2(3f, 6f);

    private void Start()
    {
        InvokeRepeating(nameof(SpawnCloud), 0f, spawnInterval);
    }

    void SpawnCloud()
    {
        if (cloudPrefabs.Count == 0)
            return;

        // Pour plus de réalisme, on choisit un prefab de nuage aléatoire
        GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Count)];

        Vector3 spawnPos = new Vector3(
            startX,
            spawnY,
            Random.Range(spawnZRange.x, spawnZRange.y)
        );

        GameObject cloud = Instantiate(prefab, spawnPos, Quaternion.identity);

        float speed = Random.Range(speedRange.x, speedRange.y);
        float travelTime = Mathf.Abs(targetX - startX) / speed;

        // Mouvement des nuages aléatoires et quand le nuage est arrivé à la cible, on le détruit
        LMotion.Create(startX, targetX, travelTime)
            .WithOnComplete(() =>
            {
                if (cloud)
                    Destroy(cloud);
            })
            .Bind(x =>
            {
                if (cloud)
                    cloud.transform.position = new Vector3(
                        x,
                        cloud.transform.position.y,
                        cloud.transform.position.z
                    );
            });
    }
}