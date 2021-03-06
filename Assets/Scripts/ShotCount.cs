using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShotCount : MonoBehaviour
{
    TextMeshProUGUI text;
    public static float shotsHit;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float accuracy = shotsHit / GameController.shotCount;
        text.SetText(GameController.shotCount.ToString("Shots: ##") + "\n" + shotsHit.ToString("Shots Hit: ##") + "\n" + accuracy.ToString("Accuracy: ##%"));
    }
}
