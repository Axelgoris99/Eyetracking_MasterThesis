using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RayCastingSelector : MonoBehaviour
{
    [SerializeField] private bool useMouse;
	private static RayCastingSelector _instance;
    public Vector3 ray;
    public static RayCastingSelector Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (useMouse)
        {
            ray = Input.mousePosition;
        }
        else
        {
            Debug.Log("Need to implement Eye Tracking");
        }
    }
}
