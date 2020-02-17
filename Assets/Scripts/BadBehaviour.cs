using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// most of the description about how the boids behave can be found here
// https://www.cs.toronto.edu/~dt/siggraph97-course/cwr87/

// Collision Avoidance: avoid collisions with nearby flockmates
// Velocity Matching: attempt to match velocity with nearby flockmates
// Flock Centering: attempt to stay close to nearby flockmates 

public class BadBehaviour : MonoBehaviour
{
    public float visibilityRadius = 3;

    public float aggressiveness = 1;

    public float distanceKeepingTerrainModifier = 1;

    public float timeScale = 10000;

    private Rigidbody rb;

    public Transform environmentParent;

    private List<Vector3> terrainPointsGizmo;

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
        List<Transform> boidPrey = GetPreyInRadius(visibilityRadius);

        List<Vector3> terrainPoints = GetEnvironmentInRadius(visibilityRadius);
        this.terrainPointsGizmo = terrainPoints;

        Vector3 velocity = new Vector3(0, 0, 0);

        // Avoid terrain
        velocity += AvoidTerrainCollision(terrainPoints) / timeScale;

        velocity = velocity + (((GetPrey().position - transform.position) * aggressiveness) / timeScale);

        this.rb.velocity += velocity;

        transform.eulerAngles = this.rb.velocity;
    }

    Transform GetPrey(){
        List<Transform> boids = GetPreyInRadius(visibilityRadius);
        Transform nearest = boids.OrderBy(t => (transform.position - t.position).sqrMagnitude).First();
        return nearest;
    } 
    Vector3 AvoidTerrainCollision(List<Vector3> terrainCollisions) {
        Vector3 vel = new Vector3(0, 0, 0);
        foreach (Vector3 v in terrainCollisions) {
            float distance = Vector3.Distance(transform.position, v);
            vel -= (v - transform.position) * (distanceKeepingTerrainModifier/(distance!=0?distance:0.00001f));
        }
        return vel;
    }

    List<Transform> GetPreyInRadius(float radius) {
        List<Transform> boidList = new List<Transform>();
        Collider[] boids = Physics.OverlapSphere(transform.position, radius, 1 << 8);

        if (boids.Length > 0){
            foreach (Collider b in boids){
               boidList.Add(b.transform); 
            }
        }
        else{
            boidList.Add(transform);
        }
        return boidList;
    }

    List<Vector3> GetEnvironmentInRadius(float radius) {
        List<Vector3> environmentList = new List<Vector3>();
        for (int i=0; i<environmentParent.childCount; i++) {
            Transform child = environmentParent.GetChild(i);
            Collider otherCollider = child.GetComponent<Collider>();
            Vector3 collisionPoint = otherCollider.ClosestPoint(transform.position); 
            if (Vector3.Distance(transform.position, collisionPoint) < radius) {
                environmentList.Add(collisionPoint);
            }
        }
        return environmentList;
    }

    void OnDrawGizmosSelected()
    {
        // draw how far the boid can see when selected for testing
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visibilityRadius);

        // get nearest prey
        Vector3 centerBoids = GetPrey().position;
        Gizmos.DrawSphere(centerBoids, .2f);

        // draw collisions with terrain
        foreach (Vector3 v in this.terrainPointsGizmo) {
            Gizmos.DrawSphere(v, .1f);
        }
    }

    void OnTriggerEnter(Collider c){
        if (c.gameObject.layer == 8){
            Debug.Log("Eaten!");
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.PlayOneShot((AudioClip)Resources.Load("eat"));
            Destroy(c.gameObject);
        }
    }
}
