using System.Collections.Generic;
using UnityEngine.Perception.Randomization.Randomizers;

namespace UnityEngine.Perception.Randomization.Scenarios
{
    /// <summary>
    /// Environment Randomizer
    /// </summary>
    public class EnvironmentRandomizer : Randomizer
    {
        #region - SerializeFields -

        // Parent Game Object containing all the environments
        [Tooltip("Parent Game Object containing all the environments")]
        [SerializeField] private GameObject environmentParent;

        // Parent Game Object containing all the human models
        [Tooltip("Parent Game Object containing all the human models")]
        [SerializeField] private GameObject humanParent;

        // This Simulation Scenario
        [Tooltip("This Simulation Scenario")]
        [SerializeField] private GameObject simulationScenario;

        #endregion

        #region - Non SerializeFields -

        #region - GameObjects -

        // List containing all the human models
        private List<GameObject> listOfHumans = new List<GameObject>();

        // List containing all the environments
        private List<GameObject> listOfEnvironments = new List<GameObject>();


        // Final human in the list of humans
        private GameObject finalHuman;

        // First human in the list of humans
        private GameObject firstHuman;

        #endregion

        #region - Integers -

        // Total number of environments
        private int numberOfEnvironments;

        // The index of the current environment on the list of environments
        private int currentEnvironemntIndex = 0;

        #endregion

        #region - Other -
        // Boolean containing whether the first human has been activated again or not
        private bool wasActive = false;

        // Fixed Length Scenario Component
        private FixedLengthScenario fixedLengthScenario;

        #endregion

        #endregion

        #region - Methods -

        /// <summary>
        /// Called on each iteration of the Simulation Scenario
        /// </summary>
        protected override void OnScenarioStart()
        {
            int numberOfHumans = humanParent.transform.childCount;
            for (int i = 0; i < numberOfHumans; i++)
                listOfHumans.Add(humanParent.transform.GetChild(i).gameObject);
            finalHuman = listOfHumans[numberOfHumans - 1];
            firstHuman = listOfHumans[0];

            for (int i = 0; i < environmentParent.transform.childCount; i++)
                listOfEnvironments.Add(environmentParent.transform.GetChild(i).gameObject);
            numberOfEnvironments = listOfEnvironments.Count;

            foreach (ReflectionProbe r in listOfEnvironments[0].GetComponentsInChildren<ReflectionProbe>())
                r.RenderProbe();

            fixedLengthScenario = simulationScenario.GetComponent<FixedLengthScenario>();
        }

        /// <summary>
        /// Called on each iteration of the Simulation Scenario
        /// </summary>
        protected override void OnIterationStart()
        {
            // If there is an active environment the total iterations of the Simulation Scenario are increased by 1
            fixedLengthScenario.constants.totalIterations++;

            if (finalHuman.activeInHierarchy)
                wasActive = finalHuman.activeInHierarchy;
            // Changing to the next environment once the first human has been activated again
            if (wasActive && firstHuman.activeInHierarchy)
            {
                wasActive = false;
                listOfEnvironments[currentEnvironemntIndex].SetActive(false);
                currentEnvironemntIndex++;
                if (currentEnvironemntIndex != numberOfEnvironments)
                {
                    listOfEnvironments[currentEnvironemntIndex].SetActive(true);
                    // Rendering the Reflection Probe of the new environment
                    foreach (ReflectionProbe r in listOfEnvironments[currentEnvironemntIndex].GetComponentsInChildren<ReflectionProbe>())
                        r.RenderProbe(); ;
                }
                else
                {
                    // If there is no active environment the total iterations of the Simulation Scenario are decreased by 1 and the Scenario ends
                    fixedLengthScenario.constants.totalIterations--;
                }
            }
        }

        #endregion
    }
}
