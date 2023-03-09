using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dwell : MonoBehaviour
{
    public delegate void DwellTime();
    public static event DwellTime onDwell;

    int layerMask = 1;
    private int layerToInteractWith;
    [SerializeField] float timerRef = 2.0f;
    float timer = 2.0f;
    GameObject selectedObject;
    bool grabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        Grab.onGrab += grabbedObject;
        Grab.onRelease += releasedObject;

        layerToInteractWith = 6;
        layerMask = 1 << layerToInteractWith;
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
            if (selectedObject == null)
            {
                selectedObject = hit.transform.gameObject;
                timer = timerRef;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
        if(timer < 0.0f)
        {
            onDwell();
            
        }
    }
    void grabbedObject()
    {
        grabbed = true;
    }
    void releasedObject()
    {
        grabbed = false;
        timer = timerRef;
    }
}
