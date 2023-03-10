using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class DualGaze : MonoBehaviour
{
  
    public delegate void SelectionDualGaze(Transform selection);
    public static event SelectionDualGaze onSelected;

    [SerializeField] private GameObject objectToSpawn;
    public GameObject sphere;
    // Think about the layers to setup in unity !!
    int layerMask = 1;
    int layerMaskFlag = 8;
    int layerMaskPlane = 7;
    private int layerToInteractWith;
    GameObject selectedObject;
    GameObject flag;
    bool grabbed = false;
    [SerializeField] float timerRef = 2.0f;
    float timer = 2.0f;
    public GameObject camPlane;
    Ray ray;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        Grab.onGrab += grabbedObject;
        Grab.onRelease += releasedObject;

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
        // suspend until the end
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Debug.Log("test");
        BoxCollider bounds = selectedObject.GetComponent<BoxCollider>();

        Vector3[] corners = new Vector3[8];
        corners[0] = bounds.center + new Vector3(bounds.size.x / 2, bounds.size.y / 2, bounds.size.z / 2);
        corners[1] = bounds.center + new Vector3(bounds.size.x / 2, bounds.size.y / 2, -bounds.size.z / 2);
        corners[2] = bounds.center + new Vector3(bounds.size.x / 2, -bounds.size.y / 2, bounds.size.z / 2);
        corners[3] = bounds.center + new Vector3(bounds.size.x / 2, -bounds.size.y / 2, -bounds.size.z / 2);
        corners[4] = bounds.center + new Vector3(-bounds.size.x / 2, bounds.size.y / 2, bounds.size.z / 2);
        corners[5] = bounds.center + new Vector3(-bounds.size.x / 2, bounds.size.y / 2, -bounds.size.z / 2);
        corners[6] = bounds.center + new Vector3(-bounds.size.x / 2, -bounds.size.y / 2, bounds.size.z / 2);
        corners[7] = bounds.center + new Vector3(-bounds.size.x / 2, -bounds.size.y / 2, -bounds.size.z / 2);

        Vector3[] worldPosition = new Vector3[8];
        for (int i = 0; i < corners.Length; i++)
        {
            worldPosition[i] = selectedObject.transform.TransformPoint(corners[i]);
        }
        for (int i = 0; i < corners.Length; i++)
        {
            RaycastHit hitPlane;
            Ray rayVertex = new Ray(ray.origin, worldPosition[i] - ray.origin);
            if (Physics.Raycast(rayVertex, out hitPlane, Mathf.Infinity, layerMaskPlane))
            {
                flag = Instantiate(sphere);
                flag.transform.position = hitPlane.point;
                flag.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ray = RayCastingSelector.Instance.ray;
        if (grabbed) { return; }
        
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            timer = timerRef;
            if (selectedObject != hit.transform.gameObject)
            {
                selectedObject = hit.transform.gameObject;
                camPlane.transform.position = selectedObject.transform.position;
                StartCoroutine(WaitForEndOfFrameSoThatThePlaneHasTimeToMove());
            }
        }
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskFlag))
        {
            if (hit.transform.gameObject == flag)
            {
                onSelected(selectedObject.transform);
                Destroy(flag);
            }
        }
        timer -= Time.deltaTime;
        if(timer < 0.0f)
        {
            selectedObject = null;
            Destroy(flag);
        }
    }
    void grabbedObject()
    {
        grabbed = true;
    }
    void releasedObject()
    {
        grabbed = false;
        selectedObject = null;
    }
}
