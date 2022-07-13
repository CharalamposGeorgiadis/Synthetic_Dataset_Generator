using System.Collections.Generic;
using UnityEngine.Perception.Randomization.Randomizers;
using System.IO;

namespace UnityEngine.Perception.Randomization.Scenarios
{
    /// <summary>
    /// Human Randomizer
    /// </summary>
    public class AnimationRandomizer : Randomizer
    {
        #region - SerializeFields -

        // Parent Game Object containing all the human models
        [Tooltip("Parent Game Object containing all the human models")]
        [SerializeField] private GameObject humanParent;

        // Directory containing all the available Animation Controllers
        [Tooltip("Directory containing all the available Animation Controllers")]
        [SerializeField] private string animatorDirectory;

        // Boolean controlling whether animations will be turned on or off
        [Tooltip("Controls whether animations will be turned on or off")]
        [SerializeField] private bool enableAnimations = false;

        #endregion

        #region - Non SerializeFields

        #region - GameObjects -
        // List containing all the human models
        private List<GameObject> listOfHumans = new List<GameObject>();

        // Current active human
        private GameObject currentHuman;

        #endregion

        #region - Integers -

        // Number of the current Animator of the current active human
        private int currentAnimationNumber = 0;

        // Number of available Animator Controllers
        private int numberOfAnimations;

        // Total number of humans
        private int numberOfHumans;

        #endregion

        #region - Other -

        // Animator component of the current active human
        private Animator currentAnimator;

        // List containing the names of all available Animator Controllers
        private List<string> animatorControllers = new List<string>();

        // Simulation State object
        private GroundTruth.SimulationState simulationState;

        #endregion

        #endregion

        #region - Methods -

        /// <summary>
        /// Called on the first iteration of the Simulation Scenario
        /// </summary>
        protected override void OnScenarioStart()
        {
            if (enableAnimations)
            {
                numberOfHumans = humanParent.transform.childCount;
                for (int i = 0; i < numberOfHumans; i++)
                    listOfHumans.Add(humanParent.transform.GetChild(i).gameObject);
                currentHuman = listOfHumans[0];

                currentAnimator = currentHuman.GetComponent<Animator>();
                var info = new DirectoryInfo("Assets/Resources/" + animatorDirectory);
                var fileInfo = info.GetFiles();
                foreach (var file in fileInfo)
                    if (!file.Name.Contains("meta"))
                        animatorControllers.Add(file.Name);
                numberOfAnimations = animatorControllers.Count;
                simulationState = GroundTruth.DatasetCapture.SimulationState;
            }
        }

        /// <summary>
        /// Called on each iteration of the Simulation Scenario
        /// </summary>
        protected override void OnIterationStart()
        {
            if (enableAnimations)
            {
                for (int i = 0; i < numberOfHumans; i++)
                {
                    GameObject tempHuman = listOfHumans[i];
                    if (tempHuman.activeInHierarchy && tempHuman != currentHuman)
                    {
                        currentHuman = tempHuman;
                        currentAnimator = currentHuman.GetComponent<Animator>();
                        currentAnimationNumber = 0;
                        if (currentHuman.name.Contains("Baby"))
                            while (!animatorControllers[currentAnimationNumber].Contains("Baby"))
                                currentAnimationNumber++;
                        else
                            while (animatorControllers[currentAnimationNumber].Contains("ONLY"))
                                currentAnimationNumber++;
                    }
                }

                if (currentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
                {
                    currentAnimationNumber++;
                    if (currentAnimationNumber == numberOfAnimations)
                        currentAnimationNumber = 0;
                    if (currentHuman.name.Contains("Baby"))
                        while (!animatorControllers[currentAnimationNumber].Contains("Baby"))
                        {
                            currentAnimationNumber++;
                            if (currentAnimationNumber == numberOfAnimations)
                                currentAnimationNumber = 0;
                        }
                    else
                    {
                        while (animatorControllers[currentAnimationNumber].Contains("ONLY"))
                        {
                            currentAnimationNumber++;
                            if (currentAnimationNumber == numberOfAnimations)
                                currentAnimationNumber = 0;
                        }
                    }
                    string path = animatorDirectory + animatorControllers[currentAnimationNumber].Split('.')[0];
                    currentHuman.GetComponent<Animator>().runtimeAnimatorController = Resources.Load(path) as RuntimeAnimatorController;
                    simulationState.k_MinPendingCapturesBeforeWrite = 1;
                }
                else
                    simulationState.k_MinPendingCapturesBeforeWrite++;
            }
        }

        #endregion
    }
}
