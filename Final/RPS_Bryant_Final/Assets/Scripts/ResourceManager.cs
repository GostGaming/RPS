using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // Like food in other games
    // how many units you have
    public float hands;

    // how many units you can have
    public float maxHands = 25;

    // Like gold in other games
    // how much buying power you have
    public float surplusHands;

    
    void Start()
    {
        surplusHands = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseHands() {
        surplusHands++;
    }
}
