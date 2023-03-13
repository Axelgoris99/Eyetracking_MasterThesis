using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPositionHands : MonoBehaviour
{
    [SerializeField] private Transform targetRef;
    private bool grabbed;
    public bool Grabbed { get { return grabbed; } set { grabbed = value; } }
    
    /// <summary>
    /// Change the position of the object to that of its target
    /// </summary>
    public void setPosition()
    {
        transform.position = targetRef.position;
    }
    private void Start()
    {
        // We don't want the object to directly follow so we let it wait for a few seconds so that the hands get to the correct position
        grabbed = true;
        
    }
    /// <summary>
    /// Once the target is reached by the skeleton hand we make it follow the skeleton
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForTheTargetToBeAtItsPosition()
    {
        yield return new WaitForSeconds(1.0f);
        grabbed = false;
    }
    
    private void Update()
    {
        // If the hand is grabbed, don't do anything
        if (grabbed) {}
        else setPosition();
    }

}
