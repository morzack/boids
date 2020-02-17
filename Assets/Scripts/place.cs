using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class place : MonoBehaviour
{
    public GameObject tree1;
    public GameObject tree2;
    public GameObject tree3;
    public GameObject tree4;
    public GameObject tree5;
    public float height;
    public Collider box;
    public float numTrees;
    // Start is called before the first frame update
    void Start() {
        RaycastHit hit;
        GameObject tree;
        Vector3 offset = new Vector3(0, 3, 0);
        Bounds bounds = box.bounds;
        for (int x = 0; x < numTrees; x++){
            transform.position =  new Vector3(Random.Range(bounds.min.x, bounds.max.x),height,Random.Range(bounds.min.z, bounds.max.z));
            if (Physics.Raycast(transform.position, Vector3.down, out hit, height)){
                switch(Random.Range(1,6)){
                    case 1:
                        tree = Instantiate(tree1, hit.point + offset, Quaternion.identity);
                        break;
                    case 2:
                        tree = Instantiate(tree2, hit.point + offset, Quaternion.identity);
                        break;
                    case 3:
                        tree = Instantiate(tree3, hit.point + offset, Quaternion.identity);
                        break;
                    case 4:
                        tree = Instantiate(tree4, hit.point + offset, Quaternion.identity);
                        break;
                    case 5:
                        tree = Instantiate(tree5, hit.point + offset, Quaternion.identity);
                        break;
                }
                
            }
        }
    }
}
