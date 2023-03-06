using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Vector3 cubePos;

    public GameObject camPlane;
    private int layerToInteractWith;
    private GameObject selectedObject;
    // Start is called before the first frame update
    void Start()
    {
        layerToInteractWith = 6;

    }

    // Update is called once per frame
    void Update()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << layerToInteractWith;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        // layerMask = ~layerMask;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Object is translated along a plane
        if (selectedObject)
        {
            //Debug.Log("Selected");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                selectedObject.transform.position = hit.point;
            }
        }
        // If we click, we cast a ray and if the ray hits something from the right layer, the object is selected
        if (Input.GetButtonDown("Fire1"))
        {
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
        // When we stop clicking, the object is deselected
        if (Input.GetButtonUp("Fire1"))
        {
            //Debug.Log("Up");
            selectedObject = null;
            layerToInteractWith = 6;
        }

        // For debugging purpose
        // Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);       
    }
}

