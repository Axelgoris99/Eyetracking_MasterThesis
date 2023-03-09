using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Grab : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Vector3 cubePos;

    public GameObject camPlane;
    private int layerToInteractWith;
    int layerMask = 1;
    private GameObject selectedObject;
    // Start is called before the first frame update
    void Start()
    {
        layerToInteractWith = 6;
        WordRecognizer.onGrab += GrabInteractable;
        WordRecognizer.onRelease += ReleaseInteractable;
        if(cam == null)
        {
            cam = Camera.main;
        }
    }
    private void OnDisable()
    {
        WordRecognizer.onGrab -= GrabInteractable;
        WordRecognizer.onRelease -= ReleaseInteractable;
    }
    // Update is called once per frame
    void Update()
    {
        // Bit shift the index of the layer to get a bit mask
        layerMask = 1 << layerToInteractWith;

        // This would cast rays only against colliders in layerToInteractWith.
        // But instead we want to collide against everything except layer blabla. The ~ operator does this, it inverts a bitmask.
        // layerMask = ~layerMask;

        //Ray ray2 = RayCastingSelector.Instance.ray;
        //Debug.DrawRay(ray2.origin, ray2.direction, Color.yellow);

        // Object is translated along a plane
        if (selectedObject != null)
        {
            Ray ray = RayCastingSelector.Instance.ray;
            RaycastHit hit;
            //Debug.Log("Selected");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                selectedObject.transform.position = hit.point;
            }
        }
        // If we click, we cast a ray and if the ray hits something from the right layer, the object is selected
        if (Input.GetButtonDown("Fire1"))
        {
            GrabInteractable();
        }
        
        // When we stop clicking, the object is deselected
        if (Input.GetButtonUp("Fire1"))
        {
            ReleaseInteractable();
        }

        // For debugging purpose
        // Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);       
    }
    void GrabInteractable()
    {
        Ray ray = RayCastingSelector.Instance.ray;
        RaycastHit hit;
        //Debug.Log("Down");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Transform objectHit = hit.transform;
            camPlane.transform.position = objectHit.transform.position;

            cubePos = objectHit.position;
            // Do something with the object that was hit by the raycast.
            selectedObject = objectHit.gameObject;
            layerToInteractWith = 7;
        }
    }

    void ReleaseInteractable()
    {
        selectedObject = null;
        layerToInteractWith = 6;
    }
}

