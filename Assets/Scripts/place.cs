using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class place : MonoBehaviour
{
    public GameObject tree1;
    public GameObject tree2;
    public GameObject tree3;
    public GameObject tree4;
    public GameObject tree5;

    public int range;
    // Start is called before the first frame update
    void Start() {
        RaycastHit hit;
        GameObject tree = new GameObject();
        Vector3 height = new Vector3(0, 4, 0);
        for (int x = 0; x < 1000; x++){
            transform.position = new Vector3(Random.Range(-range, range), 50, Random.Range(-range, range));
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 50, 1)){
                switch(Random.Range(1,6)){
                    case 1:
                        tree = PrefabUtility.InstantiatePrefab(tree1, hit.point + height) as GameObject;
                        break;
                    case 2:
                        tree = PrefabUtility.InstantiatePrefab(tree2, hit.point + height) as GameObject;
                        break;
                    case 3:
                        tree = PrefabUtility.InstantiatePrefab(tree3, hit.point + height) as GameObject;
                        break;
                    case 4:
                        tree = PrefabUtility.InstantiatePrefab(tree4, hit.point + height) as GameObject;
                        break;
                    case 5:
                        tree = PrefabUtility.InstantiatePrefab(tree5, hit.point + height) as GameObject;
                        break;
                }
                
            }
        }
    }
}
