using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherManager : MonoBehaviour
{
    // how long it takes to gather 1 hand
    public float gatherTime = 5f;
    // how many hands can be gathered from a node
    public float availableHands = 100f;
    
    public int gatherers;

    void Start()
    {
        StartCoroutine(GatherTick());
        gatherers = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (availableHands <= 0) {
            Destroy(gameObject);
        }
    }

    public void GatherHand() {
        if (gatherers > 0) {
            availableHands -= gatherers;
        }
    }

    IEnumerator GatherTick() {
        while (true) {
            yield return new WaitForSeconds(1);
            GatherHand();
        }
    }

}
