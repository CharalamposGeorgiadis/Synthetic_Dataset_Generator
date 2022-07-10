using System.IO;
using System.Collections.Generic;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.Randomization.Randomizers;

namespace UnityEngine.Perception.Randomization.Scenarios
{
    /// <summary>
    /// Image naming Randomizer
    /// </summary>
    public class ImageNamingRandomizer : Randomizer
    {
        #region - SerializeFields -

        #region - Camera -
        // Main Camera
        [Tooltip("The main camera of the project.")]
        [SerializeField] private Camera camera;

        // Camera's Collision Detection component
        [Tooltip("Camera's Collision Detection component")]
        [SerializeField] CollisionDetection collision;

        // Camera's Image Name component
        [Tooltip("Camera's Image Name component")]
        [SerializeField] ImageName imageName;

        #endregion

        #region - GameObjects -

        // Parent Game Object containing all the environments
        [Tooltip("Parent Game Object containing all the environments")]
        [SerializeField] private GameObject environmentParent;

        // Parent Game Object containing all the human models
        [Tooltip("Parent Game Object containing all the human models.")]
        [SerializeField] private GameObject humanParent;

        #endregion

        #region - Floats -

        // Degrees of each camera rotation. (360 % degrees) must be equal 0
        [Tooltip("The degrees of each camera rotation. \n 360 mod Rotation Degrees must equal 0.")]
        [Range(1, 360)]
        [SerializeField] private float rotationDegrees;

        // The final distance that the camera will reach on the z-axis before returning to its original position
        [Tooltip("The final distance that the camera will reach on the z-axis before returning to its original position.")]
        [SerializeField] private float finalCameraDistance;

        #endregion

        #region - Integers -

        // Iterations required for the camera to move to its original position
        [Tooltip("The iterations required for the camera to move on the z-axis.")]
        [Range(1, 360)]
        [SerializeField] private int iterationsPerCameraMove;

        // Iterations required for the camera to move to its original position
        [Tooltip("The iterations required for the camera to move back to its original position on the z-axis.")]
        [SerializeField] private int iterationsPerCameraReset;

        #endregion

        #region - Other -

        // Directory containing all the available Animation Controllers
        [Tooltip("Directory containing all the available Animation Controllers")]
        [SerializeField] private string animatorDirectory;

        // Boolean controlling whether animations will be turned on or off
        [Tooltip("Controls whether animations will be turned on or off")]
        [SerializeField] private bool enableAnimations = false;

        #endregion

        #endregion

        #region - Non SerializeFields

        #region - GameObjects -

        // List containing all the human models
        private List<GameObject> listOfHumans = new List<GameObject>();

        // List containing all the environments
        private List<GameObject> listOfEnvironments = new List<GameObject>();

        // Current active human
        private GameObject currentHuman;

        // Current active environment
        private GameObject currentEnvironment;

        #endregion

        #region - Integers -

        // Index of the current active environment
        private int currentEnvironmentIndex;

        // Current state of the Directional Light of the current active environment
        private int directionalLightStateInt;

        // Index of the current active human
        private int currentHumanIndex;

        // Total number of environments
        private int numberOfEnvironments;

        // Total number of Humans
        private int numberOfHumans;

        // Number of available Animator Controllers
        private int numberOfAnimations;

        // Number of the current Animator Controller of the current active human
        private int currentAnimatorNumber = 1;

        // Frame number of the current animation of the current active human model
        private int animationFrameNumber = 1;

        #endregion

        #region - Floats -

        // Degrees that the camera has rotated for (0-360)
        private float rotatedDegrees = 0f;

        // Distance of the camera from the current active human on the z-axis
        private float zCameraDistance = 1.0f;

        #endregion

        #region - Other -

        // Rotation of the Directional Light of the current active environment
        private Quaternion currentDirectionalLightRotation;

        // State of the Directional Light of the current active environment
        private string directionalLightState;

        // List containing the names of all available Animator Controllers
        private List<string> animatorControllers = new List<string>();

        // Animator Controller of the current active human
        private string currentAnimator;

        #endregion

        #endregion

        #region - Methods -

        /// <summary>
        /// Called on the first iteration of the Simulation Scenario
        /// </summary>
        protected override void OnScenarioStart()
        {
            for (int i = 0; i < environmentParent.transform.childCount; i++)
                listOfEnvironments.Add(environmentParent.transform.GetChild(i).gameObject);
            numberOfEnvironments = listOfEnvironments.Count;

            for (int i = 0; i < humanParent.transform.childCount; i++)
                listOfHumans.Add(humanParent.transform.GetChild(i).gameObject);
            numberOfHumans = listOfHumans.Count;

            var info = new DirectoryInfo("Assets/Resources/" + animatorDirectory);
            var fileInfo = info.GetFiles();
            foreach (var file in fileInfo)
                if (!file.Name.Contains("meta"))
                    animatorControllers.Add(file.Name);
            numberOfAnimations = animatorControllers.Count;

            if (listOfHumans[0].name.Contains("Baby"))
            {
                int i = 0;
                while (!animatorControllers[i].Contains("Baby"))
                    i++;
                currentAnimatorNumber = i + 1;
            }

            currentAnimator = listOfHumans[0].GetComponent<Animator>().runtimeAnimatorController.name;
        }

        /// <summary>
        /// Called on each iteration of the Simulation Scenario
        /// </summary>
        protected override void OnIterationStart()
        {
            // Finding the current active environment
            for (int i = 0; i < numberOfEnvironments; i++)
            {
                currentEnvironment = listOfEnvironments[i];
                if (currentEnvironment.activeInHierarchy)
                {
                    currentEnvironmentIndex = i + 1;
                    break;
                }
                else if (i == numberOfEnvironments - 1)
                    collision.collided = true;
            }

            // Finding the current active human and the state of the current Directional Light
            for (int i = 0; i < numberOfHumans; i++)
            {
                GameObject tempHuman = listOfHumans[i];
                if (tempHuman.activeInHierarchy && tempHuman != currentHuman)
                {
                    currentHumanIndex = i + 1;
                    currentHuman = tempHuman;
                    directionalLightState = "1";
                    directionalLightStateInt = 1;
                    currentDirectionalLightRotation = currentEnvironment.transform.Find("Directional Light").gameObject.transform.rotation;
                    break;
                }
            }

            // Finding the state of the current Directional Light
            if (!currentEnvironment.transform.Find("Directional Light").gameObject.activeInHierarchy && !directionalLightState.Equals("off")) {
                directionalLightState = "off";
                directionalLightStateInt++;
            }
            else if (currentEnvironment.transform.Find("Directional Light").gameObject.transform.rotation != currentDirectionalLightRotation)
            {
                currentDirectionalLightRotation = currentEnvironment.transform.Find("Directional Light").gameObject.transform.rotation;
                directionalLightState = (int.Parse(directionalLightState) + 1).ToString();
                directionalLightStateInt++;
            }

            if (enableAnimations) {
                // Finding the number of the current animation of the current active human
                for (int i = 0; i < numberOfAnimations; i++)
                {
                    string temp = currentHuman.GetComponent<Animator>().runtimeAnimatorController.name;
                    if (temp != currentAnimator && temp == animatorControllers[i].Split('.')[0])
                    {
                        animationFrameNumber = 0;
                        currentAnimator = temp;
                        currentAnimatorNumber = i + 1;
                        break;
                    }
                }
            }

            // Distance of the camera from the person object on the z-axis as a string
            string zCameraDistancePrefix = zCameraDistance.ToString();

            // If the distance of the camera from the person object on the z-axis is an integer, a .0 string is added in order to turn it into a decimal
            if (zCameraDistance % 1 == 0)
                zCameraDistancePrefix += ".0";

            // Renaming the RGB Images
            if (enableAnimations)         
                imageName.imageName = currentEnvironmentIndex + "_" + currentHumanIndex + "_" + directionalLightStateInt + "_"
                + currentAnimatorNumber + "_" + animationFrameNumber + "_" + zCameraDistancePrefix + "_" + (rotatedDegrees % 360).ToString();
            else
                imageName.imageName = currentEnvironmentIndex + "_" + currentHumanIndex + "_" + directionalLightStateInt + "_"
                + zCameraDistancePrefix + "_" + (rotatedDegrees % 360).ToString();

            if (rotatedDegrees == (360 - rotationDegrees))
            {
                rotatedDegrees = 0;
                if (zCameraDistance != finalCameraDistance)
                    zCameraDistance += 0.5f;
                else
                {
                    animationFrameNumber++;
                    zCameraDistance = 1.0f;
                }
            }
            else
                rotatedDegrees += rotationDegrees;
        }

        #endregion
    }
}
