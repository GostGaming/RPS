using UnityEngine;

public class SelectOnClick : MonoBehaviour {
    public GameObject selectedObject = null;
    public GameObject highlightedObject = null;

    private Ray ray;
    private RaycastHit hitData;
    Behaviour halo;
    private void Update() {

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitData, 1000)) {
            highlightedObject = hitData.transform.gameObject;

            if (Input.GetMouseButtonDown(0)) {
                HaloOff(selectedObject);
                selectedObject = highlightedObject;
                HaloOn(selectedObject);
                
            }
        } else {
            highlightedObject = null;
            if (Input.GetMouseButtonDown(0)) {
                HaloOff(selectedObject);
                selectedObject = null;
            }
        }
    }
    private void HaloOn(GameObject obj) {
        if (obj) {
            halo = (Behaviour) obj.GetComponent("Halo");
            if (halo)
                halo.enabled = true;
        }
    }
    private void HaloOff(GameObject obj) {
        if (obj) {
            halo = (Behaviour) obj.GetComponent("Halo");
            if (halo)
                halo.enabled = false;
        }
    }
}