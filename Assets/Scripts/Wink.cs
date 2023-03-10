//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using ViveSR.anipal.Eye;
using TMPro;
public class Wink : MonoBehaviour
{
    //In case you want to display values for the blink/wink interaction
    //public TextMeshProUGUI left;
    //public TextMeshProUGUI right;

    //Send Events
    public delegate void LeftWink();
    public static event LeftWink onLeftWink;
    public delegate void RightWink();
    public static event RightWink onRightWink;


    public bool NeededToGetData = true;
    private Dictionary<EyeShape_v2, float> EyeWeightings = new Dictionary<EyeShape_v2, float>();
    private static EyeData_v2 eyeData = new EyeData_v2();
    private bool eye_callback_registered = false;

    private void Start()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
            SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

        if (NeededToGetData)
        {
            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
            {
                SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = true;
            }
            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }
            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false)
                SRanipal_Eye_API.GetEyeData_v2(ref eyeData);

            bool isLeftEyeActive = false;
            bool isRightEyeActive = false;
            if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
            {
                isLeftEyeActive = eyeData.no_user;
                isRightEyeActive = eyeData.no_user;
            }
            else if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT)
            {
                isLeftEyeActive = true;
                isRightEyeActive = true;
            }

            if (isLeftEyeActive || isRightEyeActive)
            {
                if (eye_callback_registered == true)
                {
                    SRanipal_Eye_v2.GetEyeWeightings(out EyeWeightings, eyeData); 
                }
                else
                    SRanipal_Eye_v2.GetEyeWeightings(out EyeWeightings);
                //left.text = EyeWeightings[EyeShape_v2.Eye_Left_Blink].ToString();
                //right.text = EyeWeightings[EyeShape_v2.Eye_Right_Blink].ToString();

                // If we wink only one eye, not when you naturally blink
                if(EyeWeightings[EyeShape_v2.Eye_Left_Blink] > 0.8 && EyeWeightings[EyeShape_v2.Eye_Right_Blink] < 0.8)
                {
                    onLeftWink();
                }
                if (EyeWeightings[EyeShape_v2.Eye_Right_Blink] > 0.8 && EyeWeightings[EyeShape_v2.Eye_Left_Blink] < 0.8)
                {
                    onRightWink();
                }
            }
            else
            {
                for (int i = 0; i < (int)EyeShape_v2.Max; ++i)
                {
                    bool isBlink = ((EyeShape_v2)i == EyeShape_v2.Eye_Left_Blink || (EyeShape_v2)i == EyeShape_v2.Eye_Right_Blink);
                    EyeWeightings[(EyeShape_v2)i] = isBlink ? 1 : 0;
                }
            }
        }
    }
    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
        eyeData = eye_data;
    }
}
