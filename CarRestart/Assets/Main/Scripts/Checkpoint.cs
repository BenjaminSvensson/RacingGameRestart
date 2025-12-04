using JetBrains.Annotations;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject player;
    public GameObject checkpointPositionRoot;

    private Car carScript;

    public Vector3 playerRespawnPosition;

    private void Start()
    {
        playerRespawnPosition = player.transform.position;
    }

    //Set checkpoint
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            carScript.setNewCheckpointPosition();
        }
    }
} 
