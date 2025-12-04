using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Car : MonoBehaviour
{
    [SerializeField] float speed = 13f;
    [SerializeField] float turnSpeed = 180f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float brakePower = 20f;
    [SerializeField] float driftTurnMultiplier = 2f;

    [SerializeField] GameObject[] brakelights;
    [SerializeField] TextMeshPro speedText;

   
    public InputAction move;
    public InputAction brake;

    private float currentSpeed = 0f;
    private Checkpoint checkpointScript;
    private Vector3 checkPointLocation;

    private void Start()
    {
        checkPointLocation = transform.position;
    }

    void OnEnable()
    {
        move.Enable();
        brake.Enable();
    }

    void OnDisable()
    {
        move.Disable();
        brake.Disable();
    }

    private void Update()
    {
        Vector2 input = move.ReadValue<Vector2>();
        bool braking = brake.ReadValue<float>() > 0.1f;

        float targetSpeed = input.y * speed;

        if (braking)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakePower * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        float actualTurnSpeed = braking || input.y < -0.1f
            ? turnSpeed * driftTurnMultiplier
            : turnSpeed;
        transform.Rotate(Vector3.up * input.x * actualTurnSpeed * Time.deltaTime);

        foreach (var light in brakelights)
            light.SetActive(braking);

        if (transform.position.y <= -20)
        {
            Reseting();
        }

    }

    public void setNewCheckpointPosition()
    {
        checkPointLocation = transform.position;
    }
    
    //RespawnCar To Last Checkpoint 
    private void Reseting()
    {
        transform.position = checkPointLocation;
    }
}
