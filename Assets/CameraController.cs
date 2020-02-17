using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject boidController;
    public BoidController boidControlScript;

    public float rotationDampening = 10;

    public float fovConstant = 1500;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        boidControlScript = boidController.GetComponent<BoidController>();

        cam = transform.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 angles = transform.rotation.eulerAngles;
        Quaternion from = Quaternion.Euler(angles.x, angles.y, 0);

        Vector3 targetPosition = boidControlScript.GetCenter();
        Vector3 dir = targetPosition - transform.position;
        
        Quaternion to = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.RotateTowards(from, to, rotationDampening * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, targetPosition);

        cam.fieldOfView = fovConstant / distance;
    }
}
