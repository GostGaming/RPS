using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class GroundCircle : MonoBehaviour {
    
    public int segments = 50;
    public float xradius = 1;
    public float zradius = 1;
    LineRenderer line;
    public Color color = Color.green;

    private float lineWidth = 0.1f;

    void Start ()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;
        line.startColor = color;
        line.endColor = color;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material.color = color;
        line.positionCount = (segments + 1);
        line.useWorldSpace = false;
        CreatePoints ();
    }

    void CreatePoints ()
    {
        float x = 0;
        float y = gameObject.transform.position.y - 0.1f;
        float z = 0;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos (Mathf.Deg2Rad * angle) * zradius;

            line.SetPosition(i,new Vector3(x,0,z));

            angle += (360f / segments);
        }
    }
}