using System.Collections.Generic;

using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private bool useRandomSpawnPoint = false;

    private Vector2 _respawnPoint;

    // Called when the level starts
    private void Start()
    {
        InitializeRespawnPoint();
    }

    // Initialize the respawn point from the spawn points list
    private void InitializeRespawnPoint()
    {
        if (spawnPoints.Count > 0)
        {
            Transform selectedSpawn;

            if (useRandomSpawnPoint)
            {
                // Choose a random spawn point
                selectedSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
            }
            else
            {
                // Use the first spawn point by default
                selectedSpawn = spawnPoints[0];
            }

            // Set the respawn point to the selected spawn position
            SetRespawnPoint((Vector2)selectedSpawn.position);

            Debug.Log("Respawn point set at: " + _respawnPoint);
        }
        else
        {
            Debug.LogWarning("No spawn points available in the list!");
        }
    }

    // Method to set the respawn point
    public void SetRespawnPoint(Vector2 position)
    {
        _respawnPoint = position;
    }

    // Method to get the current respawn point
    public Vector2 GetRespawnPoint()
    {
        return _respawnPoint;
    }

    // Method to add a spawn point to the list at runtime
    public void AddSpawnPoint(Transform spawnTransform)
    {
        if (spawnTransform != null)
        {
            spawnPoints.Add(spawnTransform);
        }
    }
}
