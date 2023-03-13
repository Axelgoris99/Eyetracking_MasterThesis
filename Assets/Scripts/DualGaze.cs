using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class DualGaze : MonoBehaviour
{
    // Events
    public delegate void SelectionDualGaze(Transform selection);
    public static event SelectionDualGaze onSelected;

    // the objects to spawn
    [SerializeField] private GameObject objectToSpawn;
    public GameObject sphere;
    // Think about the layers to setup in unity !!
    int layerMask = 1;
    int layerMaskFlag = 8;
    int layerMaskPlane = 7;
    private int layerToInteractWith;

    // The selected object and the flag object
    GameObject selectedObject;
    GameObject flag;
    bool grabbed = false;
    // Timer
    [SerializeField] float timerRef = 2.0f;
    float timer = 2.0f;
    public GameObject camPlane;
    // Ray
    Ray ray;
    RaycastHit hit;
    Vector3 hitPoint;
    // Start is called before the first frame update
    void Start()
    {
        Grab.onGrab += grabbedObject;
        Grab.onRelease += releasedObject;

        // Layer preparation
        layerToInteractWith = 6;
        layerMask = 1 << layerToInteractWith;
        layerMaskFlag = 1 << 8;
        layerMaskPlane = 1 << 7;     

    }
    private void OnDisable()
    {
        Grab.onGrab -= grabbedObject;
        Grab.onRelease -= releasedObject;
    }

    IEnumerator WaitForEndOfFrameSoThatThePlaneHasTimeToMove()
    {
        // suspend until the end of some frames, otherwise the plane does not have time to move... ?
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        BoxCollider bounds = selectedObject.GetComponent<BoxCollider>();

        // Get the position of the cube vertex in local space
        Vector3[] corners = new Vector3[8];
        corners[0] = bounds.center + new Vector3(bounds.size.x / 2, bounds.size.y / 2, bounds.size.z / 2);
        corners[1] = bounds.center + new Vector3(bounds.size.x / 2, bounds.size.y / 2, -bounds.size.z / 2);
        corners[2] = bounds.center + new Vector3(bounds.size.x / 2, -bounds.size.y / 2, bounds.size.z / 2);
        corners[3] = bounds.center + new Vector3(bounds.size.x / 2, -bounds.size.y / 2, -bounds.size.z / 2);
        corners[4] = bounds.center + new Vector3(-bounds.size.x / 2, bounds.size.y / 2, bounds.size.z / 2);
        corners[5] = bounds.center + new Vector3(-bounds.size.x / 2, bounds.size.y / 2, -bounds.size.z / 2);
        corners[6] = bounds.center + new Vector3(-bounds.size.x / 2, -bounds.size.y / 2, bounds.size.z / 2);
        corners[7] = bounds.center + new Vector3(-bounds.size.x / 2, -bounds.size.y / 2, -bounds.size.z / 2);

        // Get the vertex of the cube in world position
        Vector3[] worldPosition = new Vector3[8];
        for (int i = 0; i < corners.Length; i++)
        {
            worldPosition[i] = selectedObject.transform.TransformPoint(corners[i]);
        }
        
        // Recreate the view of the camera on the plane - projection
        List<Vector3> hitPositionPlane = new List<Vector3>();
        for (int i = 0; i < corners.Length; i++)
        {
            RaycastHit hitPlane;
            Ray rayVertex = new Ray(ray.origin, worldPosition[i] - ray.origin);
            if (Physics.Raycast(rayVertex, out hitPlane, Mathf.Infinity, layerMaskPlane))
            {
                hitPositionPlane.Add(hitPlane.point);
            }
        }
        
        // Return the convex hull of the points
        List<Vector3> convexHull = JarvisMarchAlgorithm.GetConvexHull(hitPositionPlane);

        // Find the point of intersection on the plane for the first hit point
        RaycastHit hitTargetPlane;
        Ray rayTargetVertex = new Ray(ray.origin, hitPoint - ray.origin);
        Vector3 planeIntersection = new Vector3();
        if (Physics.Raycast(rayTargetVertex, out hitTargetPlane, Mathf.Infinity, layerMaskPlane))
        {
            planeIntersection = hitTargetPlane.point;
        }

        // Find the closest point on the hull in respect to the first hit point
        float distance = Mathf.Infinity;
        int minIndex = 0;
        for(int i = 0; i < convexHull.Count; i++)
        {
            float dist = Vector3.Distance(convexHull[i], planeIntersection);
            if (dist < distance) {
                minIndex = i;
                distance = dist;
                Debug.Log("Distance " + distance + "  index" + i);
            };
        }

        // The point to spawn the object is convexHull[minIndex]
        if(flag != null) { Destroy(flag); }
        flag = Instantiate(objectToSpawn);
        flag.transform.position = convexHull[minIndex];
        flag.SetActive(true);
        
        // We need to offset it a little bit, otherwise if you come passing by one of the convex hull point, you automatically select it!
        Vector3 directionFromMiddleToConvexHullPoint = flag.transform.position - selectedObject.transform.position;
        //flag.transform.position = flag.transform.position + directionFromMiddleToConvexHullPoint.normalized*flag.GetComponent<Bounds>().size.x/2;        
        flag.transform.position = flag.transform.position + directionFromMiddleToConvexHullPoint * 0.1f;

    }

    // Update is called once per frame
    void Update()
    {
        // Ray casting from the singleton
        ray = RayCastingSelector.Instance.ray;
        if (grabbed) { return; }
        
        // Throw a ray against the interactable layer mask
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            //If we touch something, we reset the timer so that the flag stays as long as you're watching the object
            timer = timerRef;
            if (selectedObject != hit.transform.gameObject)
            {
                // the point that was hit and the selected object are set, the camera plane as well
                hitPoint = hit.point;
                selectedObject = hit.transform.gameObject;
                camPlane.transform.position = selectedObject.transform.position;
                // We launch the coroutine that will calculate the projection of the box Collider on the plane where we will place the flag
                StartCoroutine(WaitForEndOfFrameSoThatThePlaneHasTimeToMove());
            }
        }
        
        //Throw a ray against the flag layer mask
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskFlag))
        {   
            // if we touch the flag, we select the object and destroy the flag
            if (hit.transform.gameObject == flag)
            {
                onSelected(selectedObject.transform);
                Destroy(flag);
            }
        }
        // If the timer reaches zero, we reset the selection and destroy the flag
        timer -= Time.deltaTime;
        if(timer < 0.0f)
        {
            selectedObject = null;
            Destroy(flag);
        }
    }
    /// <summary>
    /// We already have an object
    /// </summary>
    void grabbedObject()
    {
        grabbed = true;
    }
    /// <summary>
    /// Release an object by saying that we don't grab it and deselect every object
    /// </summary>
    void releasedObject()
    {
        grabbed = false;
        selectedObject = null;
    }
}
