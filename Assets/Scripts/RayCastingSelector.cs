using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using ViveSR.anipal.Eye;
public class RayCastingSelector : MonoBehaviour
{
    #region variables and members
    private static RayCastingSelector _instance;
    
    // Rendering
    [SerializeField] private GazeRay gazeRay;
    [SerializeField] private LineRenderer GazeRayRenderer;
    [SerializeField] private float lenghtRay = 25.0f;
    public Ray ray;
    public Camera cam;

    // Ray Input Selector Part 
    public enum RayInputModality // your custom enumeration
    {
       Mouse, EyeTracking, HeadTracking
    };
    public RayInputModality dropDown;
    private int nbInputInEnum;
    #endregion
    #region Singleton
    public static RayCastingSelector Instance
    {
        get
        {
            return _instance;
        }
    }

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        nbInputInEnum = System.Enum.GetValues(typeof(RayInputModality)).Length;
        //if(XRGeneralSettings.Instance.Manager.activeLoader != null)
        //{
        //    useMouse = false;
        //}
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        if(cam == null)
        {
            cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (dropDown)
        {
            case RayInputModality.EyeTracking:
                //ray = new Ray(cam.transform.position + cam.transform.up * adjustRayOrigin, gazeRay.GazeDirectionCombined);
                ray = new Ray(cam.transform.position, gazeRay.GazeDirectionCombined);
                break;
            case RayInputModality.Mouse:
                ray = cam.ScreenPointToRay(Input.mousePosition);
                break;
            case RayInputModality.HeadTracking:
                ray = new Ray(cam.transform.position, cam.transform.forward);
                break;
        }
        GazeRayRenderer.SetPosition(0, cam.transform.position );
        GazeRayRenderer.SetPosition(1, cam.transform.position+ ray.direction * lenghtRay);

        // Space Bar to change input modality
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(dropDown);
            dropDown = (RayInputModality)((int)(dropDown+1) % nbInputInEnum);
        }
    }
}
