using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static GameObject[] unitList;
    public static GameObject[] structList;

    public float panSpeed = 0.3f;
    public float rotateSpeed = 80f;
    public float rotateAmount = 80f;
    public float zoomSpeed = 20f;
    private Quaternion rotation;

    private float panDetect = 30f;
    private float minHeight = 4f;
    private float maxHeight = 15f;

   

    // Start is called before the first frame update
    void Start()
    {
        RefreshUnits();
        RefreshStructures();
        rotation = Camera.main.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        RotateCamera();
        if (Input.GetKeyUp(KeyCode.Space)) {
            Camera.main.transform.rotation = rotation;
        }
    }

    public static GameObject[] GetUnits() {
        return unitList;
    }

    public static void RefreshUnits() {
        unitList = GameObject.FindGameObjectsWithTag("Unit");
    }

    public static void RefreshStructures() {
        structList = GameObject.FindGameObjectsWithTag("ProductionBuilding");
    }
    
    public static GameObject[] GetStructures() {
        return structList;
    }
    

    void MoveCamera() {
        float camX = Camera.main.transform.position.x;
        float camZ = Camera.main.transform.position.z;
        float camY = Camera.main.transform.position.y;

        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        
        // pan left
        if(Input.GetKey(KeyCode.A) || mouseX > 0 && mouseX < panDetect) {
            camX -= panSpeed;
        }
        // pan right
        else if(Input.GetKey(KeyCode.D) || mouseX < Screen.width && 
                                    mouseX > Screen.width - panDetect) {
            camX += panSpeed;
        }
        // pan up
        if(Input.GetKey(KeyCode.W) || mouseY < Screen.height && 
                                    mouseY > Screen.height - panDetect) {
            camZ += panSpeed;
        }
        // pan down
        else if(Input.GetKey(KeyCode.S) || mouseY > 0 && mouseY < panDetect) {
            camZ -= panSpeed;
        }

        // Zoom in
        camY -= Input.GetAxis("Mouse ScrollWheel") * panSpeed * zoomSpeed;
        camY = Mathf.Clamp(camY, minHeight, maxHeight);
        // Zoom out

        Vector3 newCamPosition = new Vector3(camX, camY, camZ);
        Camera.main.transform.position = newCamPosition;
    }

    void RotateCamera() {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        if (Input.GetMouseButton(2)) {
            destination.x -= Input.GetAxis("Mouse Y") * rotateAmount;
            destination.y += Input.GetAxis("Mouse X") * rotateAmount;
        }
        
        if (destination != origin) {
            Camera.main.transform.eulerAngles = 
                Vector3.MoveTowards(origin, destination, Time.deltaTime * rotateSpeed);
        }
    }
}
