using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Grab : MonoBehaviour
{
    [SerializeField] private Camera cam;

    // How to select the pointed object
    public GameObject camPlane;
    private int layerToInteractWith;
    int layerMask = 1;
    private GameObject selectedObject;
    // rotation of the object
    public bool rotationMode = false;
    public enum rotation
    {
        positive,
        negative,
    }
    public rotation positiveRotation;
    int rotationDirection = 1;
    float rotationValue = 1;
    Vector3 rotationAxis = Vector3.right;

    // Event when the object is grabbed or released
    public delegate void ObjectGrabbed();
    public static event ObjectGrabbed onGrab;

    public delegate void ObjectReleased();
    public static event ObjectReleased onRelease;

    // Start is called before the first frame update
    void Start()
    {
        // First Rotation to calculate 
        CalculateRotation();
        rotationValue = rotationDirection * 500.0f * Time.deltaTime;

        // Layer for the raycasting
        layerToInteractWith = 6;

        //Events From the other inputs
        WordRecognizer.onGrab += RaycastAgainstInteractable;
        WordRecognizer.onRelease += ReleaseInteractable;
        WordRecognizer.onRotationAxisChanged += UpdateRotationAxis;
        WordRecognizer.onRotationDirectionChanged += UpdateRotationDirection;
        WordRecognizer.onRotationEnabled += ActivateRotationMode;
        WordRecognizer.onTranslationEnabled += ActivateTranslationMode;
        
        Wink.onLeftWink += HandleWink;
        Wink.onRightWink += HandleWink;
        Dwell.onDwell += HandleDwell;
        DualGaze.onSelected += GrabInteractable;
       
        // explicit
        if (cam == null)
        {
            cam = Camera.main;
        }
    }
    private void OnDisable()
    {
        // Events Unregistration
        WordRecognizer.onGrab -= RaycastAgainstInteractable;
        WordRecognizer.onRelease -= ReleaseInteractable;
        WordRecognizer.onRotationAxisChanged -= UpdateRotationAxis;
        WordRecognizer.onRotationDirectionChanged -= UpdateRotationDirection;
        WordRecognizer.onRotationEnabled -= ActivateRotationMode;
        WordRecognizer.onTranslationEnabled -= ActivateTranslationMode;
        
        Wink.onLeftWink -= HandleWink;
        Wink.onRightWink -= HandleWink;
        Dwell.onDwell -= HandleDwell;
        DualGaze.onSelected -= GrabInteractable;
    }
    // Update is called once per frame
    void Update()
    {
        // Bit shift the index of the layer to get a bit mask
        layerMask = 1 << layerToInteractWith;

        // Object is translated along a plane
        if (selectedObject != null)
        {
            // If we're in translation mode
            if (!rotationMode)
            {
                Ray ray = RayCastingSelector.Instance.ray;
                RaycastHit hit;
                //Debug.Log("Selected");
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    selectedObject.transform.position = hit.point;
                }
            }
            // If we're in rotation
            if (rotationMode)
            {
                // TO DO ? Head Tracking ?
            }
            RotationUsingKeyboard();
           
        }
        
        // Move the plane along the depth direction
        if (Input.GetKey(KeyCode.A))
        {
            camPlane.transform.localPosition += new Vector3(0, 0, 0.01f); 
        }
        if (Input.GetKey(KeyCode.E) && camPlane.transform.localPosition.z > 0.08f)
        {
            camPlane.transform.localPosition += new Vector3(0, 0, -0.01f);
        }
        
        
        // Throw a ray or release object
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastAgainstInteractable();
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            ReleaseInteractable();
        }

        // If we click, we cast a ray and if the ray hits something from the right layer, the object is selected
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastAgainstInteractable();
        }

        // When we stop clicking, the object is deselected
        if (Input.GetButtonUp("Fire1"))
        {
            ReleaseInteractable();
        }

        // For debugging purpose
        // Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);       
    }

    /// <summary>
    /// We throw a ray against the object from the interactable layer. If we touch something, we grab it
    /// </summary>
    void RaycastAgainstInteractable()
    {
        Ray ray = RayCastingSelector.Instance.ray;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            GrabInteractable(hit.transform);
        }
    }
    /// <summary>
    /// Grab the object that was hit. By selecting it, it now moves with our selected ray/method.
    /// </summary>
    /// <param name="objectHit">The object that we want to move along the plane.</param>
    void GrabInteractable(Transform objectHit)
    {
        // Move the plane on which the object will move
        camPlane.transform.position = objectHit.transform.position;
        // Do something with the object that was hit by the raycast.
        selectedObject = objectHit.gameObject;
        // if the object is one of the hands, we don't want to update their position, so we switch the Grabbed boolean on, stopping them from updating
        if (selectedObject.TryGetComponent<CopyPositionHands>(out CopyPositionHands comp))
        {
            comp.Grabbed = true;
        }
        // We want to raycast against the plane to move the object, we don't want selection to happen anymore so we change the layer to interact with
        layerToInteractWith = 7;
        // Throw an event to do stuff in other scripts
        onGrab();
    }

    /// <summary>
    /// We release the selected object by setting it to null
    /// </summary>
    void ReleaseInteractable()
    {
        if (selectedObject != null)
        {
            // Reset the Grabbed boolean if needed
            if (selectedObject.TryGetComponent<CopyPositionHands>(out CopyPositionHands comp))
            {
                comp.Grabbed = false;
            }
            selectedObject = null;
        }
        // Back in selection mode so we change the layer again
        layerToInteractWith = 6;
        // Event to release
        onRelease();
    }

    /// <summary>
    /// Handle Wink event. If we have an object selected, we release it, else, we raycast to grab.
    /// </summary>
    void HandleWink()
    {
        if (selectedObject == null)
        {
            RaycastAgainstInteractable();
        }
        else
        {
            ReleaseInteractable();
        }
    }

    // If we dwell, we select the object
    void HandleDwell(GameObject selection)
    {
        if (selectedObject == null)
        {
           GrabInteractable(selection.transform);
        }
    }

    /// <summary>
    /// We change the rotation axis to be X,Y,Z
    /// </summary>
    /// <param name="newAxis">the axis around which the object will rotate</param>
    void UpdateRotationAxis(Vector3 newAxis)
    {
        rotationAxis = newAxis;
    }

    /// <summary>
    /// Update the direction of the rotation : positive or negative direction
    /// </summary>
    void UpdateRotationDirection()
    {
        switch (positiveRotation)
        {
            case rotation.positive:
                positiveRotation = rotation.negative;
                CalculateRotation();
                break;
            case rotation.negative:
                positiveRotation = rotation.positive;
                CalculateRotation();
                break;
                
        }
    }
    /// <summary>
    /// Interpolate {0,1} to {-1,1} for the rotation enum to be usable directly
    /// </summary>
    void CalculateRotation()
    {
        rotationDirection = (int)positiveRotation * 2 - 1;
    }

    /// <summary>
    /// Some inputs to rotate the object with the keyboard and check how it works
    /// </summary>
    void RotationUsingKeyboard()
    {
        CalculateRotation();
        rotationValue = rotationDirection * 500.0f * Time.deltaTime;
        if (Input.GetKey(KeyCode.X))
        {
            rotationAxis = Vector3.right;
            selectedObject.transform.Rotate(rotationAxis * rotationValue, Space.Self);
        }
        if (Input.GetKey(KeyCode.Y))
        {
            rotationAxis = Vector3.up;
            selectedObject.transform.Rotate(rotationAxis * rotationValue, Space.Self);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            rotationAxis = Vector3.forward;
            selectedObject.transform.Rotate(rotationAxis * rotationValue, Space.Self);
        }
        if (Input.GetKey(KeyCode.R))
        {
            selectedObject.transform.Rotate(rotationAxis * rotationValue, Space.Self);
        }
    }
    
    /// <summary>
    /// Switch between rotation and translation
    /// </summary>
    void ActivateRotationMode()
    {
        rotationMode = !rotationMode;
    }
    /// <summary>
    /// Activate translation and translation only
    /// </summary>
    void ActivateTranslationMode()
    {
        rotationMode = false;
    }
}

