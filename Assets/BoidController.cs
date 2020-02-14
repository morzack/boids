using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public int boidCount = 50;
    public GameObject boidPrefab;
    public float boidScatteringRadius = 2; // we'll uniformly scatter boids in a shere of this size around the controller
    public float boidVelocityModifier = 3;

    public GameObject environmentParent;

    // Start is called before the first frame update
    void Start()
    {
        // All we want to do is spawn in a bunch of boids so that they interact with each other
        for (int i=0; i<boidCount; i++) {
            Vector3 position = Random.insideUnitSphere * boidScatteringRadius;

            Vector3 eulerRotation = Random.insideUnitSphere;
            eulerRotation *= 360;
            Quaternion rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, eulerRotation.z);
            
            GameObject createdBoid = Instantiate(boidPrefab, position, rotation);
            createdBoid.transform.parent = transform;
            createdBoid.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere * boidVelocityModifier;
            createdBoid.GetComponent<BoidBehavior>().environmentParent = environmentParent.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
