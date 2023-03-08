using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using ViveSR.anipal.Eye;
public class RayCastingSelector : MonoBehaviour
{
    [SerializeField] private bool useMouse;
	private static RayCastingSelector _instance;
    [SerializeField] private SRanipal_GazeRaySample_v2 gazeRay;

    public Ray ray;
    public Camera cam;


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
        if (useMouse)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            ray = new Ray(cam.transform.position, gazeRay.GazeDirectionCombined);
        }
    }
}
