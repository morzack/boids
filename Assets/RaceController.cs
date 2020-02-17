using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
    public int raceNumber;

    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        if (active) {
            Gizmos.DrawSphere(transform.position, 1);
        }
    }
}
