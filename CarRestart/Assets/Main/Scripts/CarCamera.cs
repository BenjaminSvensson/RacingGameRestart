using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Drag your Car object here")]
    [SerializeField] private Transform target;

    [Header("Position Settings")]
    [Tooltip("The position offset relative to the car (X, Y, Z). Recommended: (0, 5, -10)")]
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [Tooltip("How 'loose' the camera follows. Higher = looser/more lag.")]
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Rotation Settings")]
    [Tooltip("How fast the camera rotates to face the car.")]
    [SerializeField] private float rotationSpeed = 5f;
    [Tooltip("Y-offset for where the camera looks at (so it looks at the roof, not the tires).")]
    [SerializeField] private float lookAtHeight = 2f;

    [Header("Dynamic Effects (The 'Awesome' Part)")]
    [Tooltip("Enable speed-based Field of View changes.")]
    [SerializeField] private bool useDynamicFOV = true;
    [SerializeField] private float minFOV = 60f;
    [SerializeField] private float maxFOV = 85f;
    [Tooltip("How much the FOV reacts to speed.")]
    [SerializeField] private float zoomSpeedMultiplier = 2f;

    // Internal variables for SmoothDamp
    private Vector3 currentVelocity;
    private Camera cam;
    private Vector3 lastTargetPosition;
    private float calculatedSpeed;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        HandleMovement();
        HandleRotation();

        if (useDynamicFOV)
        {
            HandleDynamicFOV();
        }
    }

    private void HandleMovement()
    {
        // Calculate where the camera WANTS to be (relative to car's current position and rotation)
        // We use target.TransformPoint to convert local offset (0, 5, -10) to world coordinates based on car facing
        Vector3 targetPosition = target.TransformPoint(offset);

        // Smoothly move from current position to target position
        // SmoothDamp is better than Lerp for cameras as it handles arrival easing beautifully
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }

    private void HandleRotation()
    {
        // Calculate where to look (The car + a little bit up)
        Vector3 lookTarget = target.position + Vector3.up * lookAtHeight;

        // Determine the direction from camera to car
        Vector3 direction = lookTarget - transform.position;

        // Create the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleDynamicFOV()
    {
        // Calculate the car's actual speed based on distance moved since last frame
        // We do this instead of reading the Car script so this camera works with ANY car controller
        float distanceMoved = Vector3.Distance(target.position, lastTargetPosition);
        calculatedSpeed = distanceMoved / Time.deltaTime;

        // Update last position for next frame
        lastTargetPosition = target.position;

        // Calculate target FOV based on speed (clamped between 0 and 1)
        // We divide speed by 20 roughly as a "max speed" reference, adjust as needed
        float speedPercent = Mathf.Clamp01(calculatedSpeed / 50f);
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, speedPercent);

        // Smoothly change the FOV
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeedMultiplier);
    }
}