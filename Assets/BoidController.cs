using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public int boidCount = 50;
    public GameObject boidPrefab;
    public float boidScatteringRadius = 2; // we'll uniformly scatter boids in a shere of this size around the controller
    public float boidVelocityModifier = 3;

    public float individualityDeviation = 1f; // deviation of individuality vector
    public bool usingIndividuality = false;

    public bool racing = false;
    private int currentRaceTarget = 0;
    public GameObject raceControllerRoot;
    private float lastMinDistance = 0;
    public float minRaceDistance = 1;
    private bool updateRace = false;

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
            
            float individuality = Random.value*individualityDeviation;

            GameObject createdBoid = Instantiate(boidPrefab, position+transform.position, rotation);
            createdBoid.transform.parent = transform;
            createdBoid.GetComponent<Rigidbody>().velocity = (Random.insideUnitSphere+velocityInitial) * boidVelocityModifier;

            BoidBehavior behaviorComponent = createdBoid.GetComponent<BoidBehavior>();
            behaviorComponent.environmentParent = environmentParent.transform;
            behaviorComponent.raceGoalParent = raceControllerRoot.transform;
            behaviorComponent.individuality = usingIndividuality?individuality:1;
            behaviorComponent.racing = racing;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
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

        Vector3 raceVector = new Vector3(0, 0, 0);

        if (racing) {
            if (updateRace) {
                if (lastMinDistance < minRaceDistance) {
                    // increment race counter logic
                    currentRaceTarget += 1;
                    if (currentRaceTarget >= raceControllerRoot.transform.childCount) {
                        currentRaceTarget = 0;
                    }
                    lastMinDistance = 999;
                }
                updateRace = false;
            }
            else {
                updateRace = true;
            }
            for (int i=0; i<raceControllerRoot.transform.childCount; i++) {
                RaceController controller = raceControllerRoot.transform.GetChild(i).GetComponent<RaceController>();
                int identifier = controller.raceNumber;
                if (identifier == currentRaceTarget) {
                    raceVector = controller.transform.position;
                    controller.active = true;
                } else {
                    controller.active = false;
                }
            }
        }

        float currentRaceDistances = 999f;
        for (int i=0; i<transform.childCount; i++) {
            BoidBehavior b = transform.GetChild(i).GetComponent<BoidBehavior>();
            if (changed) {
                b.rule1 = rule1;
                b.rule2 = rule2;
                b.rule3 = rule3;
            }
            b.raceTarget = raceVector;
            if (currentRaceDistances > b.raceDistance) {
                currentRaceDistances = b.raceDistance;
            }
        }
        lastMinDistance = currentRaceDistances;
    }

    public Vector3 GetCenter() {
        Vector3 center = new Vector3(0, 0, 0);
        for (int i=0; i<transform.childCount; i++) {
            center += transform.GetChild(i).transform.position;
        }
        center /= transform.childCount;
        return center;
    }
}
