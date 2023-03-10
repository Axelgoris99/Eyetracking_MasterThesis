using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualGaze : MonoBehaviour
{
  
    public delegate void SelectionDualGaze(Transform selection);
    public static event SelectionDualGaze onSelected;

    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private float distFlag;
    // Think about the layers to setup in unity !!
    int layerMask = 1;
    int layerMaskFlag = 8;
    private int layerToInteractWith;
    GameObject selectedObject;
    GameObject flag;
    bool grabbed = false;
    [SerializeField] float timerRef = 2.0f;
    float timer = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        Grab.onGrab += grabbedObject;
        Grab.onRelease += releasedObject;

        layerToInteractWith = 6;
        layerMask = 1 << layerToInteractWith;
        layerMaskFlag = 1 << 8;

    }
    private void OnDisable()
    {
        Grab.onGrab -= grabbedObject;
        Grab.onRelease -= releasedObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbed) { return; }
        Ray ray = RayCastingSelector.Instance.ray;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            timer = timerRef;
            if (selectedObject == null)
            {
                selectedObject = hit.transform.gameObject;
                Vector3 hitPoint = hit.point;
                // In case the flag is still up 
                if(flag != null) { Destroy(flag); }
                flag = Instantiate(objectToSpawn);
                flag.transform.position = hitPoint + hit.normal.normalized * flag.GetComponent<BoxCollider>().size.z * distFlag;
                flag.SetActive(true);
            }
            if(hit.transform.gameObject != selectedObject && hit.transform.gameObject != flag)
            {
                // We could change it here but we can just wait for one more frame, doesn't change much
                selectedObject = null;
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
