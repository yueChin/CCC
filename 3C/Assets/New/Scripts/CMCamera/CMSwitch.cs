using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor.MemoryProfiler;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CMSwitch : MonoBehaviour
{   
    public CinemachineFreeLook CMFreeLook;
    public CinemachineVirtualCamera CMTarget;
    public CinemachineTargetGroup group;
    public Transform Target;
    public Transform[] Targets;
    public bool isFreeCamera = true;
    public float farDis = 10;
    public float nearDis = 3;
    public PlayerInput playerInput;
    
    public CinemachineImpulseSource ImpulseSource;
    public CinemachineBrain CinemachineBrain;
    
    private void Awake()
    {   
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        SwitchCamToTree(true);
    }

    public void DoShake( float value)
    {   
       Invoke( "DoShakeIntel",value );
    }

    void DoShakeIntel()
    {   
        ImpulseSource.GenerateImpulse();
    }

    public void SretFOV( bool ret,float value )
    {   
        if (CMTarget)
        {
            if (ret == false)
            {
                CMTarget.m_Lens.FieldOfView = 40;
            }
            else
            {   
                CMTarget.m_Lens.FieldOfView = value;
            }
        }
    }

    public float moveCheckDuration = 1.5f;
    float m_MoveCheckTime;

    void CheckMovingState()
    {
        if(!isFreeCamera)
            return;
        
        if (playerInput.WASDHold )
        {
            m_MoveCheckTime += Time.deltaTime;
        }

        if (m_MoveCheckTime > this.moveCheckDuration)
        {
            SwitchCamToTree(true);
            //Debug.LogError("m_moveCheckTimem_moveCheckTimem_moveCheckTime");
        }
    }

    // Update is called once per frame
    void Update()
    {         
        CheckMovingState();

        if (playerInput.QInput && isFreeCamera)
        {
            SwitchCamToTree(false);
        }
        
        if (!isFreeCamera)
        {   
            if (CMFreeLook != null)
            {
                Transform freeLk = CMFreeLook.transform;
                Transform target = CMTarget.transform;
                freeLk.position = target.position;
                freeLk.rotation = target.rotation;
            }
            
            if (Target != null)
            {   
                float dis = Vector3.Distance(transform.position, Target.position);
                if (dis > farDis || dis < nearDis)
                {
                    isFreeCamera = true;
                    m_MoveCheckTime = 0f;
                    //SwitchCamToTree(true);
                }

                if (dis < nearDis)
                {   
                    //SetNUll();
                }   
            }   
        }
        else if (isFreeCamera )
        {   

        }
    }

    private bool m_Flag = false;
    [ContextMenu( "SwitchCamToTree" )]
    void Switch()
    {   
        SwitchCamToTree(m_Flag);
        m_Flag = !m_Flag;
    }

    //public float switchInterval = 1f;
    //float lastTime;
     public void SwitchCamToTree( bool isFree )
    {      
        this.isFreeCamera = isFree;
        if (isFree)
        {
            if(CMFreeLook != null)
                CMFreeLook.Priority = 10;
            if(CMTarget != null)
                CMTarget.Priority = 8;
        }
        else
        {
            if (CMFreeLook != null)
                CMFreeLook.Priority = 8;
            if (CMTarget != null)
                CMTarget.Priority = 10;
        }
    }


    Transform GetT()
    {   
        Transform tgt = null;
        float value = farDis;
        foreach (Transform t in Targets)
        {   
            float dis = Vector3.Distance(transform.position, t.position);
            if (dis < value)
            {   
                value = dis;
                tgt = t;
            }   
        }
        return tgt;
    }
}
