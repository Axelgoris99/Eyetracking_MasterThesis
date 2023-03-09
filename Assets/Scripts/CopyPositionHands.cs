using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPositionHands : MonoBehaviour
{
    [SerializeField] private Transform targetRef;
    private bool grabbed;
    public bool Grabbed { get { return grabbed; } set { grabbed = value; } }
    public void setPosition()
    {
        transform.position = targetRef.position;
    }
    private void Start()
    {
        grabbed = true;
    }
    private void Update()
    {
        if (grabbed) {}
        else setPosition();
    }

}
