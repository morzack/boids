using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// most of the description about how the boids behave can be found here
// https://www.cs.toronto.edu/~dt/siggraph97-course/cwr87/

// Collision Avoidance: avoid collisions with nearby flockmates
// Velocity Matching: attempt to match velocity with nearby flockmates
// Flock Centering: attempt to stay close to nearby flockmates 

public class BoidBehavior : MonoBehaviour
{
    public float visibilityRadius = 3;

    public float movementAmount = 100;

    public float distanceKeeping = 1;
    public float matchingVelocityModifier = 8;

    public float timeScale = 10000;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        this.rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<Transform> boidBuddies = GetBoidsInRadius(visibilityRadius);
        Vector3 velocity = this.rb.velocity;

        // Rule 1
        velocity += Rule1(boidBuddies) / movementAmount / timeScale;

        // Rule 2
        velocity += Rule2(boidBuddies) / timeScale;

        // Rule 3
        velocity += Rule3(boidBuddies) / matchingVelocityModifier / timeScale;

        rb.velocity = velocity;
    }

    Vector3 Rule1(List<Transform> boidBuddies) {
        Vector3 centroid = GetCentroid(boidBuddies);
        Vector3 deltaCentroid = centroid - transform.position;
        return deltaCentroid;
    }

    Vector3 Rule2(List<Transform> boidBuddies) {
        Vector3 vel = new Vector3(0, 0, 0);
        foreach (Transform t in boidBuddies) {
            float distance = Vector3.Distance(transform.position, t.position);
            if (distance < distanceKeeping) {
                vel = vel - (transform.position - t.position);
            }
        }
        return vel;
    }

    Vector3 Rule3(List<Transform> boidBuddies) {
        Vector3 vel = new Vector3(0, 0, 0);
        foreach (Transform t in boidBuddies) {
            Vector3 otherVelocity = t.gameObject.GetComponent<Rigidbody>().velocity;
            vel += otherVelocity;
        }
        vel -= rb.velocity;
        vel /= boidBuddies.Count;
        return vel;
    }

    List<Transform> GetBoidsInRadius(float radius) {
        List<Transform> boidList = new List<Transform>();

        Transform parentBoid = transform.parent;
        for (int i=0; i<parentBoid.childCount; i++) {
            Transform child = parentBoid.GetChild(i);
            float distance = Vector3.Distance(transform.position, child.position);
            if (distance <= radius && !transform.Equals(child)) {
                boidList.Add(child);
            }
        }

        return boidList;
    }

    Vector3 GetCentroid(List<Transform> transforms) {
        Vector3 centroid = transform.position;
        foreach (Transform otherTransform in transforms) {
            centroid += otherTransform.position;
        }
        centroid /= transforms.Count;
        return centroid;
    }

    Vector3 GetEuler(Vector3 from, Vector3 to) {
        Vector3 distanceVector = to-from;   
        Quaternion lookVector = Quaternion.LookRotation(distanceVector, transform.up);
        return lookVector.eulerAngles;
    }

    void OnDrawGizmosSelected()
    {
        // draw how far the boid can see when selected for testing
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visibilityRadius);

        // get center of boids and draw it
        List<Transform> boidBuddies = GetBoidsInRadius(visibilityRadius);
        Vector3 centerBoids = GetCentroid(boidBuddies);
        Gizmos.DrawSphere(centerBoids, .2f);
    }
}
