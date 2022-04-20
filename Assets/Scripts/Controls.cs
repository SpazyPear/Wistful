using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Controls : MonoBehaviour
{
    public InputDevice RightController;
    public InputDevice LeftController;
    public bool interactDown;
    public bool jumpDown;

    // Start is called before the first frame update
    void Awake()
    {
        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, inputDevices);
        RightController = inputDevices[0];
        inputDevices.Clear();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, inputDevices);
        LeftController = inputDevices[0];
    }

    void Update()
    {
        RightController.TryGetFeatureValue(CommonUsages.triggerButton, out jumpDown);
        RightController.TryGetFeatureValue(CommonUsages.primaryButton, out interactDown);
    }


}
