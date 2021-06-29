using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MovingTarget : MonoBehaviour
{
    public float health;
    public RectTransform rect;
    private void Awake()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        health = 150;
    }

    // Update is called once per frame
    void Update()
    {
        rect.localScale = new Vector3(health/150, rect.localScale.y, rect.localScale.z);
        //Debug.Log(health);
        if(health <= 0)
        {
            
            Destroy(this.gameObject);
        }
    }
}
