using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;

public class PosePerAnimationRandomizer : Randomizer
{

    #region - Serialize Fields -

    // Parent Game Object containing all the human models
    [Tooltip("Parent Game Object containing all the human models")]
    [SerializeField] public GameObject humanParent;

    // Iterations required for the animation to move to its next frame
    [Tooltip("The iterations required for the animation to move to its next frame.")]
    [SerializeField] private int iterationsPerPose;

    // Boolean controlling whether animations will be turned on or off
    [Tooltip("Controls whether animations will be turned on or off")]
    [SerializeField] private bool enableAnimations = false;

    #endregion

    #region - Non SerializeFields -

    // Total number of Humans
    private int numberOfHumans;

    // Number of iterations for the current pose
    private long currentPoseIterations = 2;

    #endregion

    #region - Methods -

    /// <summary>
    /// Called on the first iteration of the Simulation Scenario
    /// </summary>
    protected override void OnScenarioStart()
    {
        numberOfHumans = humanParent.transform.childCount;
    }

    /// <summary>
    /// Called on each iteration of the Simulation Scenario
    /// </summary>
    protected override void OnIterationStart()
    {
        for (int i = 0; i < numberOfHumans; i++)
        {
            GameObject currentHuman = humanParent.transform.GetChild(i).gameObject;
            if (currentHuman.activeInHierarchy)
            {
                if (enableAnimations)
                {
                    currentHuman.GetComponent<Animator>().speed = Convert.ToSingle(currentPoseIterations % iterationsPerPose == 0);
                    currentPoseIterations++;
                }
                else
                    currentHuman.GetComponent<Animator>().speed = 0;
            }
        }
    }

    #endregion
}
