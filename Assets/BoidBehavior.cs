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

    public bool rule1 = true;
    public bool rule2 = true;
    public bool rule3 = true;

    public bool racing = false;
    public float raceModifier = 0;
    public Vector3 raceTarget;
    public float raceDistance = 0;

    public float individuality;

    public float movementAmount = 100;

    public float distanceKeeping = 1;
    public float distanceKeepingModifier = .5f;
    public float matchingVelocityModifier = 8;

    public float distanceKeepingTerrain = 1;
    public float distanceKeepingTerrainModifier = 1;

    public float timeScale = 10000;

    private Rigidbody rb;

    public Transform environmentParent;
    public Transform raceGoalParent;

    private List<Vector3> terrainPointsGizmo;

    public float maxMagnitude = 1;

    // Start is called before the first frame update
    void Start()
    {
        this.rb = gameObject.GetComponent<Rigidbody>();

        this.terrainPointsGizmo = new List<Vector3>();
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<Transform> boidBuddies = GetBoidsInRadius(visibilityRadius);

        List<Vector3> terrainPoints = GetEnvironmentInRadius(visibilityRadius);
        this.terrainPointsGizmo = terrainPoints;

        Vector3 velocity = new Vector3(0, 0, 0);

        // Rule 1
        if (rule1) {
            velocity += Rule1(boidBuddies) * movementAmount / timeScale;
        }

        // Rule 2
        if (rule2) {
            velocity += Rule2(boidBuddies) / timeScale;
        }

        // Rule 3
        if (rule3) {
            velocity += Rule3(boidBuddies) * matchingVelocityModifier / timeScale;
        }

        if (racing) {
            velocity += RacingRule(raceTarget) / timeScale;
        }

        // apply individuality
        velocity *= individuality;

        // Avoid terrain
        velocity += AvoidTerrainCollision(terrainPoints) / timeScale;

        this.rb.velocity += velocity;

        // ensure we don't go too fast
        if (this.rb.velocity.magnitude > maxMagnitude) {
            this.rb.velocity = this.rb.velocity.normalized * maxMagnitude;
        }

        transform.eulerAngles = this.rb.velocity;
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
            vel -= (t.position - transform.position) * distanceKeepingModifier / (distance/distanceKeeping);
        }
        return vel;
    }

    Vector3 Rule3(List<Transform> boidBuddies) {
        Vector3 vel = new Vector3(0, 0, 0);
        foreach (Transform t in boidBuddies) {
            Vector3 otherVelocity = t.gameObject.GetComponent<Rigidbody>().velocity;
            vel += otherVelocity;
        }
        if (boidBuddies.Count > 0) {
            vel /= boidBuddies.Count;
        }
        return (vel - this.rb.velocity);
    }

    Vector3 AvoidTerrainCollision(List<Vector3> terrainCollisions) {
        Vector3 vel = new Vector3(0, 0, 0);
        foreach (Vector3 v in terrainCollisions) {
            float distance = Vector3.Distance(transform.position, v);
            vel -= (v - transform.position) * (distanceKeepingTerrainModifier/(distance!=0?distance:0.00001f));
        }
        return vel;
    }

    Vector3 RacingRule(Vector3 raceTarget) {
        raceDistance = Vector3.Distance(transform.position, raceTarget);
        return (raceTarget - transform.position) * raceModifier / (raceDistance!=0?raceDistance:0.00000f);
    }

    List<Transform> GetBoidsInRadius(float radius) {
        List<Transform> boidList = new List<Transform>();

        Transform parentBoid = transform.parent;
        for (int i=0; i<parentBoid.childCount; i++) {
            Transform child = parentBoid.GetChild(i);
            float distance = Vector3.Distance(transform.position, child.position);
            if (distance <= radius && distance > 0) {
                boidList.Add(child);
            }
        }

        return boidList;
    }

    List<Vector3> GetObjectsInRadius(float radius, Transform parent) {
        List<Vector3> environmentList = new List<Vector3>();
        for (int i=0; i<parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            Collider otherCollider = child.GetComponent<Collider>();
            Vector3 collisionPoint = otherCollider.ClosestPoint(transform.position); 
            if (Vector3.Distance(transform.position, collisionPoint) < radius) {
                environmentList.Add(collisionPoint);
            }
        }
        return environmentList;
    }

    List<Vector3> GetEnvironmentInRadius(float radius) {
        List<Vector3> environmentObject = GetObjectsInRadius(radius, environmentParent);
        if (racing) {
            List<Vector3> raceGoals = GetObjectsInRadius(radius, raceGoalParent);
            environmentObject.AddRange(raceGoals);
        }
        return environmentObject;
    }

    Vector3 GetCentroid(List<Transform> transforms) {
        Vector3 centroid = new Vector3(0, 0, 0);
        foreach (Transform otherTransform in transforms) {
            centroid += otherTransform.position;
        }
        if (transforms.Count > 0) {
            centroid /= transforms.Count;
        } else {
            centroid = transform.position;
        }
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

        // draw collisions with terrain
        foreach (Vector3 v in this.terrainPointsGizmo) {
            Gizmos.DrawSphere(v, .1f);
        }
    }
}
