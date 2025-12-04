using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Car carScript;
    public GameObject checkpointPositionRoot;

    

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

