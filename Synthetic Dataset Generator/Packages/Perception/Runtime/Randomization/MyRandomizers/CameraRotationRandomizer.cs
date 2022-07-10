using UnityEngine.Perception.Randomization.Randomizers;

namespace UnityEngine.Perception.Randomization.Scenarios
{
    /// <summary>
    /// Camera rotation Randomizer
    /// </summary>
    public class CameraRotationRandomizer : Randomizer
    {
        #region - SerializeFields -

        // Main Camera
        [Tooltip("The main camera of the project.")]
        [SerializeField] private Camera camera;

        [Header("Rotation Settings")]
        // 3D point around which the camera will rotate
        [Tooltip("3D point around which the camera will rotate.")]
        [SerializeField] private Vector3 pointToRotateAround;

        // Degrees of each camera rotation. (360 % degrees) must be equal 0
        [Tooltip("The degrees of each camera rotation. \n 360 mod Rotation Degrees must equal 0.")]
        [Range(1, 360)]
        [SerializeField] private float rotationDegrees;

        // Axes around which the camera will rotate
        [Tooltip("The axes around which the camera will rotate.")]
        [SerializeField] private Vector3 rotationAxes;

        #endregion

        #region - Methods -

        /// <summary>
        /// Called on the first iteration of the Simulation Scenario
        /// </summary>
        protected override void OnScenarioStart()
        {
            // Rotating the camera once in the opposite direction so that it resets back to its original rotation on the first iteration
            // of the simulation scenario in order for the first capture to be at a rotation of 0 degrees
            camera.transform.RotateAround(pointToRotateAround, rotationAxes, -rotationDegrees);
        }

        /// <summary>
        /// Called on each iteration of the Simulation Scenario
        /// </summary>
        protected override void OnIterationStart()
        {
            // Rotating the camera around a user-defined point
            camera.transform.RotateAround(pointToRotateAround, rotationAxes, rotationDegrees);
        }

        #endregion
    }
}
