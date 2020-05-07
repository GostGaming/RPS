using UnityEngine;

public class SelectOnClick : MonoBehaviour {
    public GameObject selectedObject = null;
    public GameObject highlightedObject = null;

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
                LeftClick();
            }
        } else {
            highlightedObject = null;
        }
    }

    private void LeftClick() {
        // Deselect any other objects
        CircleOff(selectedObject);
        
        if (highlightedObject != null && highlightedObject.tag == "Selectable") {
            selectedObject = highlightedObject;
            selectedObjectInfo = selectedObject.GetComponent<ObjectInfo>();
            if (selectedObjectInfo) selectedObjectInfo.isSelected = true;

            CircleOn(selectedObject);
        }
        else {
            selectedObject = null;
        }
    }

    private void CircleOn(GameObject obj) {
        if (obj) {
            circle = obj.GetComponent<LineRenderer>();
            if (circle)
                circle.enabled = true;
        }
    }
    private void CircleOff(GameObject obj) {
        if (obj) {
            circle = obj.GetComponent<LineRenderer>();
            if (circle)
                circle.enabled = false;
        }
    }
}