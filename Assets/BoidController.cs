using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public int boidCount = 50;
    public GameObject boidPrefab;
    public float boidScatteringRadius = 2; // we'll uniformly scatter boids in a shere of this size around the controller
    public float boidVelocityModifier = 3;

    public bool rule1 = true;
    private bool prevRule1 = true;
    public bool rule2 = true;
    private bool prevRule2 = true;
    public bool rule3 = true;
    private bool prevRule3 = true;

    public GameObject environmentParent;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 velocityInitial = Random.insideUnitSphere;
        // All we want to do is spawn in a bunch of boids so that they interact with each other
        for (int i=0; i<boidCount; i++) {
            Vector3 position = Random.insideUnitSphere * boidScatteringRadius;

            Vector3 eulerRotation = Random.insideUnitSphere;
            eulerRotation *= 360;
            Quaternion rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, eulerRotation.z);
            
            GameObject createdBoid = Instantiate(boidPrefab, position+transform.position, rotation);
            createdBoid.transform.parent = transform;
            createdBoid.GetComponent<Rigidbody>().velocity = (Random.insideUnitSphere+velocityInitial) * boidVelocityModifier;
            createdBoid.GetComponent<BoidBehavior>().environmentParent = environmentParent.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool changed = false;
        if (rule1 != prevRule1) {
            prevRule1 = rule1;
            changed = true;
        }
        if (rule2 != prevRule2) {
            prevRule2 = rule2;
            changed = true;
        }
        if (rule3 != prevRule3) {
            prevRule3 = rule3;
            changed = true;
        }
        if (changed) {
            for (int i=0; i<transform.childCount; i++) {
                BoidBehavior b = transform.GetChild(i).GetComponent<BoidBehavior>();
                b.rule1 = rule1;
                b.rule2 = rule2;
                b.rule3 = rule3;
            }
        }
    }
}
