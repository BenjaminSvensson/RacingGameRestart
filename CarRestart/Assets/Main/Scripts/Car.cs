using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
            Reset();
        }

    }

    private void Reset()
    {
        SceneManager.LoadScene("Mainscene");
    }
}
