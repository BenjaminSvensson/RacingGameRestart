using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System;

public class Car : MonoBehaviour
{
    //Car statVariables
    [SerializeField] float speed = 13f;
    [SerializeField] float turnSpeed = 180f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float brakePower = 20f;
    [SerializeField] float driftTurnMultiplier = 2f;

    [SerializeField] GameObject[] brakelights;

    //Textpjects
    [SerializeField] TMP_Text speedText;
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text bestTimeText;
    [SerializeField] Checkpoint checkpoint;
    private Stopwatch timer;
    
   //Input actions 
    public InputAction move;
    public InputAction brake;
    public InputAction resetAction;

    //Currently set variables
    private float currentSpeed = 0f;
    private Vector3 checkPointLocation;
    private Quaternion checkPointRotation;
    private TimeSpan bestTime = TimeSpan.MaxValue;

    private void Start()
    { 
        checkPointLocation = transform.position;
        checkPointRotation = transform.rotation;

        timer = Stopwatch.StartNew();
    }
    void OnEnable()
    {
        move.Enable();
        brake.Enable();
        resetAction.Enable();
        resetAction.performed += OnReset;
    }

    void OnDisable()
    {
        move.Disable();
        brake.Disable();
        resetAction.Disable();
        resetAction.performed -= OnReset;
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

       speedText.text = currentSpeed.ToString("F0") + " Km/h";

       timerText.text = timer.Elapsed.ToString(@"mm\:ss\:ff");
    }
   
    //Checkpoint
    public void setNewCheckpointPosition()
    {
        checkPointLocation = transform.position;
        checkPointRotation = transform.rotation;
    }
    
    //RespawnCar To Last Checkpoint 
    private void Reseting() 
    {
        transform.position = checkPointLocation;
        transform.rotation = checkPointRotation;
        currentSpeed = 0f;
    }
    private void OnReset(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("Mainscene");
    }
    public void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Finish"))
        {
            if (timer.Elapsed < bestTime) 
            {
                if (checkpoint.checkpointAmount >= 3)
                {
                    bestTime = timer.Elapsed;
                    bestTimeText.text = "BestTime: " + bestTime.ToString(@"mm\:ss\:ff"); 
                }
                else
                {
                    bestTimeText.text = "Cheater!";
                }
                
            }
            timer.Reset();
            timer.Start();
        }
    }
}
