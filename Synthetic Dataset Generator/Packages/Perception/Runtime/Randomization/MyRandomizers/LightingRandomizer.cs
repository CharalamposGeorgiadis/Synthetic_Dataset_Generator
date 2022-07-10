using System.Collections.Generic;
using UnityEngine.Perception.Randomization.Randomizers;
using System.IO;

namespace UnityEngine.Perception.Randomization.Scenarios
{
    /// <summary>
    /// Lighting Randomizer
    /// </summary>
    public class LightingRandomizer : Randomizer
    {
        #region - SerializeFields -

        // Parent Game Object containing all the environments
        [Tooltip("Parent Game Object containing all the environments")]
        [SerializeField] private GameObject environmentParent;

        // Parent Game Object containing all the human models
        [Tooltip("Parent Game Object containing all the human models")]
        [SerializeField] private GameObject humanParent;

        // Number of Directional Light position changes for each environment
        [Tooltip("Number of Directional Light position changes for each environment")]
        [SerializeField] private int numberOfDirectionalLightChanges;

        // Number of degrees for each Directional Light rotation for each axis
        [Tooltip("Number of degrees for each Directional Light rotation for each axis")]
        [SerializeField] private Vector3 directionalLightRotationDegrees;

        // Directory containing all the available Animation Controllers
        [Tooltip("Directory containing all the available Animation Controllers")]
        [SerializeField] private string animatorDirectory;

        // Boolean controlling whether animations will be turned on or off
        [Tooltip("Controls whether animations will be turned on or off")]
        [SerializeField] private bool enableAnimations = false;

        // Iterations required for the Directional Light to move to its next state. Only used if animations are turned off.
        [Tooltip("Iterations required for the Directional Light to move to its next state. Only used if animations are turned off.")]
        [SerializeField] private int iterationsPerLightChange;

        #endregion

        #region - Non SerializeFields -

        #region - GameObjects -
        // List containing all environments
        private List<GameObject> listOfEnvironments = new List<GameObject>();

        // Current active environment
        private GameObject currentEnvironment;

        // List containing all the human models
        private List<GameObject> listOfHumans = new List<GameObject>();

        // Current active human
        private GameObject currentHuman;

        // Directional Light of the current active environment
        private GameObject currentDirectionalLight;

        #endregion

        #region - Integers -

        // Total number of environments
        private int numberOfEnvironments;

        // Total number of Humans
        private int numberOfHumans;

        // Number of position changes that have occured for the current active Directional Light
        private int currentDirectionalLightChanges = 1;

        // Current iteration count for the current Directional Light state. Only used when animations are turned off.
        private int currentIterations = 1;

        #endregion

        #region - Strings -

        // Name of the first Animator Controller of the current active human
        private string firstAnimator;

        // Name of the final Animator Controller of the current active human
        private string finalAnimator;

        #endregion

        #region - Other -
        // Default rotation of the current active environment's Directional Light
        private Quaternion originalDirectionalLightRotation;

        // Simulation State object
        private GroundTruth.SimulationState simulationState;

        // Boolean containing whether the current active human is playing their final available animation
        private bool isOnFinalAnimation;

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
            currentEnvironment = listOfEnvironments[0];

            currentDirectionalLight = currentEnvironment.transform.Find("Directional Light").gameObject;
            originalDirectionalLightRotation = currentDirectionalLight.transform.rotation;

            foreach (ReflectionProbe r in currentEnvironment.GetComponentsInChildren<ReflectionProbe>())
                r.RenderProbe();

            simulationState = GroundTruth.DatasetCapture.SimulationState;

            if (enableAnimations)
            {
                for (int i = 0; i < humanParent.transform.childCount; i++)
                    listOfHumans.Add(humanParent.transform.GetChild(i).gameObject);
                currentHuman = listOfHumans[0];
                numberOfHumans = listOfHumans.Count;
                var info = new DirectoryInfo("Assets/Resources/" + animatorDirectory);
                var fileInfo = info.GetFiles();
                firstAnimator = currentHuman.GetComponent<Animator>().runtimeAnimatorController.name;
                if (currentHuman.name.Contains("Baby"))
                {
                    foreach (var file in fileInfo)
                        if (file.Name.Contains("Baby") && !file.Name.Contains(".meta"))
                            finalAnimator = file.Name.Split('.')[0];
                }
                else
                    // If the final aniμαtor controller in the directory is not suitable for adult humans the next line should be adjusted
                    finalAnimator = fileInfo[fileInfo.Length - 2].Name.Split('.')[0];
            }
        }

        /// <summary>
        /// Called on each iteration of the Simulation Scenario
        /// </summary>
        protected override void OnIterationStart()
        {
            // Finding the current active environment and its Directional Light
            for (int i = 0; i < numberOfEnvironments; i++)
            {
                GameObject tempEnvironment = listOfEnvironments[i];
                if (tempEnvironment.activeInHierarchy && tempEnvironment != currentEnvironment)
                {
                    currentEnvironment = tempEnvironment;
                    currentDirectionalLight = currentEnvironment.transform.Find("Directional Light").gameObject;
                    originalDirectionalLightRotation = currentDirectionalLight.transform.rotation;
                    break;
                }
            }

            // Finding the current active human
            for (int i = 0; i < numberOfHumans; i++)
            {
                GameObject tempHuman = listOfHumans[i];
                if (tempHuman.activeInHierarchy && tempHuman != currentHuman)
                {
                    currentHuman = tempHuman;
                    if (enableAnimations)
                    {
                        firstAnimator = currentHuman.GetComponent<Animator>().runtimeAnimatorController.name;
                        var info = new DirectoryInfo("Assets/Resources/" + animatorDirectory);
                        var fileInfo = info.GetFiles();
                        if (currentHuman.name.Contains("Baby"))
                        {
                            foreach (var file in fileInfo)
                                if (file.Name.Contains("Baby") && !file.Name.Contains(".meta"))
                                    finalAnimator = file.Name.Split('.')[0];
                        }
                        else
                            // If the final aniamtor controller in the directory is not suitable for adult humans the next line should be adjusted
                            finalAnimator = fileInfo[fileInfo.Length - 2].Name.Split('.')[0];
                    }
                }
            }

            bool newCapture = false;

            if (enableAnimations)
            {
                // Finding whether the current active human is on their final animation
                if (currentHuman.GetComponent<Animator>().runtimeAnimatorController.name == finalAnimator)
                    isOnFinalAnimation = true;

                // If the current active human's final animation ended the Directional Light state is changed
                if (isOnFinalAnimation && currentHuman.GetComponent<Animator>().runtimeAnimatorController.name == firstAnimator)
                {
                    isOnFinalAnimation = false;
                    newCapture = true;
                }
            }
            else
            {
                if (currentIterations == iterationsPerLightChange)
                {
                    currentIterations = 1;
                    newCapture = true;
                }
                else
                    currentIterations++;
            }

            if (newCapture)
            {
                if (!enableAnimations)
                    simulationState.k_MinPendingCapturesBeforeWrite = 1;

                // Deactivating the Directional Light on its final change
                if (currentDirectionalLightChanges == numberOfDirectionalLightChanges - 1)
                {
                    currentDirectionalLight.SetActive(false);
                    currentDirectionalLightChanges++;
                }
                // Reactivating the Directional Light and rotating it back to its original position
                else if (currentDirectionalLightChanges == numberOfDirectionalLightChanges)
                {
                    currentDirectionalLight.SetActive(true);
                    currentDirectionalLight.transform.rotation = originalDirectionalLightRotation;
                    currentDirectionalLightChanges = 1;
                }
                // Rotating the Directional Light
                else
                {
                    currentDirectionalLight.transform.rotation = Quaternion.Euler(
                        currentDirectionalLight.transform.rotation.eulerAngles.x + directionalLightRotationDegrees.x,
                        currentDirectionalLight.transform.rotation.eulerAngles.y + directionalLightRotationDegrees.y,
                        currentDirectionalLight.transform.rotation.eulerAngles.z + directionalLightRotationDegrees.z);
                    currentDirectionalLightChanges++;
                }

                // Re-rendering the current environment's Reflection Probe
                foreach (ReflectionProbe r in currentEnvironment.GetComponentsInChildren<ReflectionProbe>())
                    r.RenderProbe();
            }
            else
                if (!enableAnimations)
                    simulationState.k_MinPendingCapturesBeforeWrite++;

        }      
        #endregion
    }
}
