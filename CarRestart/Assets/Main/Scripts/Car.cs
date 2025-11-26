using UnityEngine;
using UnityEngine.InputSystem;

public class Car : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float maxSpeed = 13f;
    [SerializeField] float turnSpeed = 180f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float brakePower = 20f;
    [SerializeField] float driftTurnMultiplier = 2f;

    [Header("Brake Lights")]
    [SerializeField] GameObject[] brakelights;

    [Header("Audio Sources")]
    [SerializeField] AudioSource idleSound;
    [SerializeField] AudioSource accelerateSound;
    [SerializeField] AudioSource brakeSound;
    [SerializeField] AudioSource driftSound;

    private float currentSpeed = 0f;
    public InputAction move;
    public InputAction brake;   

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

        float targetSpeed = input.y * maxSpeed;

        if (braking)
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakePower * Time.deltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        float actualTurnSpeed = braking || input.y < -0.1f
            ? turnSpeed * driftTurnMultiplier
            : turnSpeed;
        transform.Rotate(Vector3.up * input.x * actualTurnSpeed * Time.deltaTime);

        foreach (var light in brakelights)
            light.SetActive(braking);

        // --- Sound handling ---
        HandleSounds(input, braking, actualTurnSpeed);
    }

    private void HandleSounds(Vector2 input, bool braking, float turnSpeed)
    {
        // Idle
        if (Mathf.Abs(currentSpeed) < 0.1f && !braking)
        {
            PlayOnce(idleSound);
            StopSound(accelerateSound);
            StopSound(brakeSound);
            StopSound(driftSound);
        }
        // Accelerating forward
        else if (input.y > 0.1f && !braking)
        {
            PlayOnce(accelerateSound);
            StopSound(idleSound);
            StopSound(brakeSound);
            StopSound(driftSound);
        }
        // Braking
        else if (braking)
        {
            PlayOnce(brakeSound);
            StopSound(idleSound);
            StopSound(accelerateSound);
            StopSound(driftSound);
        }

        // Drift sound when turning while braking or reversing
        if ((braking || input.y < -0.1f) && Mathf.Abs(input.x) > 0.1f)
            PlayOnce(driftSound);
        else
            StopSound(driftSound);
    }

    private void PlayOnce(AudioSource source)
    {
        if (source != null && !source.isPlaying)
            source.Play();
    }

    private void StopSound(AudioSource source)
    {
        if (source != null && source.isPlaying)
            source.Stop();
    }
}
