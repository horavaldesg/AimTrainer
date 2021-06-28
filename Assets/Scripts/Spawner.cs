using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spawner : MonoBehaviour
{
    
    public GameObject target;
    [SerializeField] int targetCount;
    int targetCt;
    private void Awake()
    {
        
        targetCt = targetCount;
        Spawn();
    }
    private void Update()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        int i = 0;
       
        if(i >= targets.Length)
        {
            //Spawn();
        }
        //Debug.Log(targets[i]);
        //    if (targets == null)
        //    {
        //        Spawn();
        //    }


    }
    public void Spawn()
    {
        GameController.shotCount = 0;
        ShotCount.shotsHit = 0;
        for (int i = 0; i < targetCt; i++)
        {
            Instantiate(target, new Vector3(Random.Range(-10, 10), Random.Range(3, 10), 12), Quaternion.identity);
        }
        //Debug.Log("Spawn");
    }
    // Update is called once per frame
   
}
