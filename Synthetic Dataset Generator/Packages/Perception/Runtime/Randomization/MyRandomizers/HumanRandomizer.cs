using System.Collections.Generic;
using UnityEngine.Perception.Randomization.Randomizers;

namespace UnityEngine.Perception.Randomization.Scenarios
{
    /// <summary>
    /// Human Randomizer
    /// </summary>
    public class HumanRandomizer : Randomizer
    {
        #region - SerializeFields -

        // Parent Game Object containing all the environments
        [Tooltip("Parent Game Object containing all the environments")]
        [SerializeField] private GameObject environmentParent;

        // Parent Game Object containing all the human models
        [Tooltip("Parent Game Object containing all the human models")]
        [SerializeField] private GameObject humanParent;

        #endregion

        #region - Non SerializeFields

        #region - GameObjects -

        // List containing all the human models
        private List<GameObject> listOfHumans = new List<GameObject>();

        // List containing all the environments
        private List<GameObject> listOfEnvironments = new List<GameObject>();

        // Current active human
        private GameObject currentHuman;

        // Directional Light of the current active environment
        private GameObject currentDirectionalLight;

        // Current active environment
        private GameObject currentEnvironment;

        #endregion

        #region - Integers -

        // Total number of environments
        private int numberOfEnvironments;

        // Total number of Humans
        private int numberOfHumans;

        // The index of the current active human on the list of humans
        private int currentHumanIndex = 0;

        #endregion

        #region - Other -
        // Default rotation of the current active environment's Directional Light
        private Quaternion originalDirectionalLightRotation;

        // Boolean containing whether the first human should be activated again or not
        private bool humanSwitch = false;

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
            currentHuman = listOfHumans[0];
            numberOfHumans = listOfHumans.Count;
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
            
            if (currentDirectionalLight.transform.rotation != originalDirectionalLightRotation)
                humanSwitch = true;

            // If the directional light was at its final state in the previous Iteration and at its first state in this Iteration, change human
            if (humanSwitch && currentDirectionalLight.transform.rotation == originalDirectionalLightRotation)
            {
                humanSwitch = false;
                currentHuman.SetActive(false);
                currentHumanIndex++;
                if (currentHumanIndex == numberOfHumans)
                    currentHumanIndex = 0;
                currentHuman = listOfHumans[currentHumanIndex];
                currentHuman.SetActive(true);
            }
        }
        #endregion
    }
}
