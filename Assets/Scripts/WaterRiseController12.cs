using UnityEngine;

public class WaterRiseController12 : MonoBehaviour
{
    [SerializeField] private Door targetDoor;  // Reference to the door that triggers the water rise
    [SerializeField] private GameObject waterObject;  // Water object that needs to move
    [SerializeField] private float waterRiseSpeed = 2f;  // Speed of water rising
    [SerializeField] private Vector3 targetPosition;  // Target position
    private Vector3 startPosition;  // Start position
    private bool isWaterRising = false;

    void Start()
    {
        // Initialize start position to the current position of the water object
        if (waterObject != null)
        {
            startPosition = waterObject.transform.localPosition;  // Set the initial position
            Debug.Log("Water object initialized and inactive.");
        }
        else
        {
            Debug.LogError("Water object not assigned.");
        }

        if (targetDoor == null)
        {
            Debug.LogError("Target door not assigned.");
        }

        Debug.Log("12 startPos - " + startPosition + " targetPos - " + targetPosition);
    }

    void Update()
    {
        // Check if the door is open and if the water isn't already rising
        if (targetDoor != null && targetDoor.IsOpen() && !isWaterRising)
        {
            Debug.Log("Door opened, starting water rise.");
            TriggerWaterRise();
        }

        // If water is rising, move it upwards
        if (isWaterRising)
        {
            MoveWaterUpwards();
        }
    }

    // Trigger the water rise
    private void TriggerWaterRise()
    {
        isWaterRising = true;

        // Activate the water object and make it visible
        if (waterObject != null)
        {
            waterObject.SetActive(true);  // Activate the water object
            Debug.Log("Water object is now active.");
        }
    }

    // Move the water object upwards
    private void MoveWaterUpwards()
    {
        // Move water upwards until it reaches the target position
        if (waterObject != null && waterObject.transform.localPosition != targetPosition)
        {
            waterObject.transform.localPosition = Vector3.MoveTowards(
                waterObject.transform.localPosition, targetPosition, waterRiseSpeed * Time.deltaTime);

            Debug.Log("Water is rising. Current position: " + waterObject.transform.localPosition);
        }
        else
        {
            isWaterRising = false;  // Stop water rise once it reaches the target position
            Debug.Log("Water rise complete.");
        }
    }
}
