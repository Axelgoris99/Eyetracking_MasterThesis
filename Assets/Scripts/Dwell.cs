using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dwell : MonoBehaviour
{
    // Event
    public delegate void DwellTime(GameObject selection);
    public static event DwellTime onDwell;

    // Layers to raycats against
    int layerMask = 1;
    private int layerToInteractWith;
    
    // Timer for the dwell time
    [SerializeField] float timerRef = 2.0f;
    float timer = 2.0f;
    
    // Selected object and has it been grabbed already
    GameObject selectedObject;
    bool grabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        // React to events being fired on the Grab script
        Grab.onGrab += grabbedObject;
        Grab.onRelease += releasedObject;

        // Interactable layer to raycast against
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
        // If we have an object already, don't do anything
        if (grabbed) { return; }
        
        // Get the ray from the singleton
        Ray ray = RayCastingSelector.Instance.ray;
        RaycastHit hit;
        // Raycast against the interactable layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // If we don't have an object already or that it's a different one, select the one being hit
            if (selectedObject != hit.transform.gameObject)
            {
                selectedObject = hit.transform.gameObject;
                timer = timerRef;
            }
            // Else, just decrease the timer
            else
            {
                timer -= Time.deltaTime;
            }
        }
        // If we don't hit anything
        else
        {
            timer = timerRef;
            selectedObject = null;
        }
        // If the timer reaches zero, it means we stayed on the object so we can select it by throwing an event
        if(timer < 0.0f)
        {
            onDwell(selectedObject);
        }
    }
    
    /// <summary>
    /// the grabbed property set to true
    /// </summary>
    void grabbedObject()
    {
        grabbed = true;
    }
    /// <summary>
    /// Grabbed set to false and timer reset as well as the selected object
    /// </summary>
    void releasedObject()
    {
        grabbed = false;
        timer = timerRef;
        selectedObject = null;
    }
}
