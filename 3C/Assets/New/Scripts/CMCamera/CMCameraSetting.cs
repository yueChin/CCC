using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CMCameraSetting : MonoBehaviour
{   
    public Cinemachine.CinemachineBrain Brain;
    public CinemachineBlenderSettings BlenderSettings;
    
    public CinemachineImpulseSource CinemachineImpulseSource;
    public CinemachineFixedSignal CinemachineFixedSignal;
    
    // Start is called before the first frame update
    void Start()
    {   
        if (Brain != null && Brain.m_CustomBlends == null && BlenderSettings != null)
        {
            Brain.m_CustomBlends = BlenderSettings;
        }
        if (CinemachineImpulseSource != null && CinemachineFixedSignal != null)
        {
            CinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = CinemachineFixedSignal;
        }
    }   
}
