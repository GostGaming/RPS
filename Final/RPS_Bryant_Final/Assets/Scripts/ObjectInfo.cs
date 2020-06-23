using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInfo : MonoBehaviour
{
    public UnitTypes unitType;
    public float unitHealth;
    public float maxHealth;
    public string objName;
    public string objDescription;
    public bool isSelected;
    public LineRenderer circle;
    public Tasks task;

    public Image healthBar;
    public Canvas healthBarCanvas;
    
    private void Start() {

        if (maxHealth <= 0f) {
            maxHealth = 1f;
        }
    }
}