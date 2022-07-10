using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;

/// <summary>
/// Camera movement Randomizer
/// </summary>
public class CameraMovementRandomizer : Randomizer
{
    #region - SerializeFields -
    // Main Camera 
    [Tooltip("The main camera of the project.")]
    [SerializeField] private Camera camera;

    // The distance on the z-axis that the camera will travel after each round of iterations
    [Tooltip("The distance on the z-axis that the camera will travel after each round of iterations.")]
    [SerializeField] private float zAxisIntervals;

    // Iterations required for the camera to move to the next interval on the z-axis
    [Tooltip("The iterations required for the camera to move to the next interval on the z-axis.")]
    [Range(1, 360)]
    [SerializeField] private int iterationsPerMove;

    // Iterations required for the camera to move to its original position
    [Tooltip("The iterations required for the camera to move back to its original position on the z-axis.")]
    [SerializeField] private int iterationsPerCameraPositionReset;

    #endregion

    #region - Non SerializeFields -

    // Counter for the number of iterations of the Fixed Length Scenario until the camera has to reset to its original position
    private int currentIterationsForCameraPositionReset = 0;

    // Counter for the number of iterations of the Fixed Length Scenario until the camera has to move to the next interval on the z-axis 
    private int currentIterationsForCameraMovement = 0;

    // Position of the camera
    private Vector3 cameraCurrentPosition;

    // Original position of the camera
    private Vector3 cameraOriginalPosition;

    #endregion

    #region - Methods -

    /// <summary>
    /// Called on the first iteration of the Simulation Scenario
    /// </summary>
    protected override void OnScenarioStart()
    {
        cameraOriginalPosition = new Vector3(0f, 1.1f, -1f);
    }

    /// <summary>
    /// Called on each iteration of the Simulation Scenario
    /// </summary>
    protected override void OnIterationStart()
    {
        // Moving the camera to its default position once the required number of iterations has been reached
        if (currentIterationsForCameraPositionReset == iterationsPerCameraPositionReset)
        {
            currentIterationsForCameraPositionReset = 0;
            currentIterationsForCameraMovement = 0;
            camera.transform.position = cameraOriginalPosition;
            // Setting the collision of the camera to false because the it would register as true when the camera reset to its original position
            camera.GetComponent<CollisionDetection>().collided = false;
        }
        // Moving the camera to the next interval once the required number of iterations has been reached
        else if (currentIterationsForCameraMovement == iterationsPerMove)
        {
            cameraCurrentPosition = camera.transform.position;
            cameraCurrentPosition.z -= zAxisIntervals;
            camera.transform.position = cameraCurrentPosition;
            currentIterationsForCameraMovement = 0;
        }
        currentIterationsForCameraMovement++;
        currentIterationsForCameraPositionReset++;
    }

    #endregion
}
