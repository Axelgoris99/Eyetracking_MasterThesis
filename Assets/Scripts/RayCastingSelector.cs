using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using ViveSR.anipal.Eye;
public class RayCastingSelector : MonoBehaviour
{
	private static RayCastingSelector _instance;
    [SerializeField] private GazeRay gazeRay;
    [SerializeField] private LineRenderer GazeRayRenderer;
    [SerializeField] private float lenghtRay = 25.0f;

    public Ray ray;
    public Camera cam;
    public enum RayInputModality // your custom enumeration
    {
       Mouse, EyeTracking, HeadTracking
    };
    public RayInputModality dropDown;

    public static RayCastingSelector Instance
    {
        get
        {
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
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
    }
}
