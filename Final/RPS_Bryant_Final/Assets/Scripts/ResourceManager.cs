using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // Like food in other games
    // how many units you have
    private int hands;

    // how many units you can have
    public static int maxHands = 25;

    // Like gold in other games
    // how much buying power you have
    // aka: use turns to buy hands
    public int turns;

    public int Hands { get => hands; set => hands = value; }

    void Start()
    {
        hands = 0;
        turns = 10;
    }
}
