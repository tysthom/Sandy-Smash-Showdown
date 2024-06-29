using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPrediction : MonoBehaviour
{
    public GameObject predictionMarkerPrefab; // Prefab of the prediction marker object
    public int predictionSteps = 20; // Number of prediction steps
    public float predictionStepTime = 0.1f; // Time interval between prediction steps

    public GameObject predictionMarker; // Prediction marker object

    GameManager gameManagerInstance;
    public LayerMask groundLayerMask;

    private float targetY = 5;

    private void Start()
    {
        gameManagerInstance = GetComponent<BallMovement>().gameManager.GetComponent<GameManager>();
    }

    // Clears the prediction marker
    public void ClearPredictionMarker()
    {
        if (predictionMarker != null)
        {
            Destroy(predictionMarker);
        }
    }

    // Predicts and marks the landing position of the ball
    public void PredictBallLanding(Vector3 initialPosition, Vector3 initialVelocity)
    {
        ClearPredictionMarker(); // Clear previous prediction marker

        Vector3 currentPosition = initialPosition;
        Vector3 currentVelocity = initialVelocity;
        RaycastHit hit;

        float timeStep = 0.1f; // Time step for each prediction
        float maxPredictionTime = 10f; // Maximum prediction time to avoid infinite loop
        float minDistanceToGround = 0.1f; // Minimum vertical distance to ground

        float elapsedTime = 0f;

        while (elapsedTime < maxPredictionTime && currentPosition.y > minDistanceToGround)
        {
            // Apply gravity to the velocity
            currentVelocity += Physics.gravity * timeStep;

            // Calculate the next position based on current velocity and time step
            Vector3 nextPosition = currentPosition + currentVelocity * timeStep;

            if (GetComponent<BallMovement>().mostRecentAttack != BallMovement.attacks.set)
            {
                // Perform raycast from the current position to the next position to check for ground collision
                if (Physics.Raycast(currentPosition, nextPosition - currentPosition, out hit, Vector3.Distance(currentPosition, nextPosition), groundLayerMask))
                {
                    currentPosition = hit.point;
                    break;
                }
            }
            else
            {
                //Used when about to perform a spike to allows the ball and athelte to line up when in the air
                if (nextPosition.y < targetY)
                {
                    // Use a linear interpolation to find a more accurate X and Z at the exact targetY
                    float t = (targetY - currentPosition.y) / (nextPosition.y - currentPosition.y);
                    Vector3 targetPosition = Vector3.Lerp(currentPosition, nextPosition, t);
                    currentPosition = new Vector3(targetPosition.x, targetY, targetPosition.z);
                    break;
                }
            }
            

            currentPosition = nextPosition;
            elapsedTime += timeStep;
        }

        // Create prediction marker object at the predicted landing position
        predictionMarker = Instantiate(predictionMarkerPrefab, new Vector3(currentPosition.x, 0, currentPosition.z), Quaternion.identity);


        // Update AI roles
        UpdateAIRoles();

        if((gameManagerInstance.athletes[0].tag == "Player"))
        {
            gameManagerInstance.athletes[0].GetComponent<PlayerController>().UpdateNextMoveIndicators();
        }
    }

    private void UpdateAIRoles()
    {
        for (int i = 0; i < gameManagerInstance.athletes.Length; i++)
        {
            if (gameManagerInstance.athletes[i].tag == "AI")
            {
                gameManagerInstance.athletes[i].GetComponent<AINavigation>().AssignRole();
            }
        }
    }
}
