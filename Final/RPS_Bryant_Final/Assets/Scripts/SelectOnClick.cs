using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Ignore error, something with visual studio
using UnityEngine.UI;

public class SelectOnClick : MonoBehaviour {
    
//    public GameObject selectedObject = null;
    public List<GameObject> selectedUnits;
    
    public GameObject highlightedObject = null;
    public bool hasPrimary;
    public GameObject primaryObject;
    public RectTransform selectionBox;

    private Vector2 startPos;

    private Ray ray;
    private RaycastHit hitData;
    LineRenderer circle;
    private ObjectInfo selectedObjectInfo;

    

    private void Update() {

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitData, 100)) {
            
            if (hitData.collider.tag == "Ground") {
                highlightedObject = null;
            }
            else {
                highlightedObject = hitData.transform.gameObject;
            }
            if (Input.GetMouseButtonDown(0)) {
                
                startPos = Input.mousePosition;
            }
            // Mouse released
            if (Input.GetMouseButtonUp(0)) {
                UnitCheck();
            }
            // mouse held
            if (Input.GetMouseButton(0)){
                UpdateBox(Input.mousePosition);
            }
        } else {
            highlightedObject = null;
        }
    }

    private void UnitCheck() {
        selectionBox.gameObject.SetActive(false);
        selectedUnits = new List<GameObject>();
        // bottom left position
        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        // top right position
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);
        GameObject[] units = CameraController.GetUnits();
        GameObject[] structures = CameraController.GetStructures();
        // make sure everything is deselected first
        // I know we iterate through at worst 4 times, need to find better way to deselect everything
        foreach (GameObject unit in units) {
            ObjectInfo unitInfo = unit.GetComponent<ObjectInfo>();
            unitInfo.isSelected = false;
        }
        foreach (GameObject str in structures) {
            ObjectInfo unitInfo = str.GetComponent<ObjectInfo>();
            unitInfo.isSelected = false;
        }
        foreach (GameObject unit in units) {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
            ObjectInfo unitInfo = unit.GetComponent<ObjectInfo>();

            // make sure everything is deselected first
            unitInfo.isSelected = false;

            if(screenPos.x > min.x && screenPos.x < max.x && 
                screenPos.y > min.y && screenPos.y < max.y) {
                    selectedUnits.Add(unit);
                    unitInfo.isSelected = true;
            }
            else if (hitData.collider.gameObject == unit) {
                selectedUnits.Add(unit);
                unitInfo.isSelected = true;
            }
        }
        if (selectedUnits.Count == 0) {
            foreach (GameObject str in structures) {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(str.transform.position);
                ObjectInfo strInfo = str.GetComponent<ObjectInfo>();
                // make sure everything is deselected first
                strInfo.isSelected = false;

                if(screenPos.x > min.x && screenPos.x < max.x && 
                    screenPos.y > min.y && screenPos.y < max.y) {
                        selectedUnits.Add(str);
                        strInfo.isSelected = true;
                }
                else if (hitData.collider.gameObject == str) {
                    selectedUnits.Add(str);
                    strInfo.isSelected = true;
                }
            }
        }
    }
    
    private void UpdateBox(Vector2 curMousePos) {
        if(!selectionBox.gameObject.activeInHierarchy) {
            selectionBox.gameObject.SetActive(true);
        }
        float width = curMousePos.x - startPos.x;
        float height = curMousePos.y - startPos.y;
        
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width),Mathf.Abs(height));
        selectionBox.anchoredPosition = startPos + 
                                        new Vector2(width / 2, height / 2);

    }

}