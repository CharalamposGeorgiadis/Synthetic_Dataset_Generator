using UnityEngine;

/// <summary>
/// Camera Collision Detection
/// </summary>
public class CollisionDetection : MonoBehaviour
{
    // Boolean containg whether an object has collided with another object or not
    public bool collided = false;

    /// <summary>
    /// Called when a collision occurs
    /// </summary>
    /// <param name="other">Collider of the other object.</param>
    private void OnTriggerEnter(Collider other)
    {
        collided = true;
    }

    /// <summary>
    /// Called once per frame for each object that is colliding with the trigger object
    /// </summary>
    /// <param name="other">Collider of the other object.</param>
    private void OnTriggerStay(Collider other)
    {
        collided = true;
    }

    /// <summary>
    /// Called once all collisions have ended
    /// </summary>
    /// <param name="other">Collider of the other object.</param>
    private void OnTriggerExit(Collider other)
    {
        collided = false;
    }
}
