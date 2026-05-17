using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public static Vector3 spawnPosition;

    void Awake()
    {
        spawnPosition = transform.position;
    }
}