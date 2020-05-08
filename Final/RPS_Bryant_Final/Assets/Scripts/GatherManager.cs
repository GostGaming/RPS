using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherManager : ObjectInfo
{
    // how long it takes to gather 1 hand
    public static float GATHER_TIME = 1f;
    // how many turns can be gathered from a node
    public float availableTurns = 100f;
    
    public int gatherers;

    void Start()
    {
        objName = "Gather Node";
        objDescription = "Collect turns to create more \"hands.\"";
        StartCoroutine(GatherTick());
        gatherers = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (availableTurns <= 0) {
            Destroy(gameObject);
        }
    }

    public void GatherHand() {
        if (gatherers > 0) {
            availableTurns -= gatherers;
        }
    }

    IEnumerator GatherTick() {
        while (true) {
            yield return new WaitForSeconds(GATHER_TIME);
            GatherHand();
        }
    }

}
