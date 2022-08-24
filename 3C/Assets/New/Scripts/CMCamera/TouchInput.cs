using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

public class TouchInput : MonoBehaviour
{
    public CinemachineBrain cinemachineBrain;
    public CinemachineFreeLook cinemachineFreeLook;
    public float value;
    public float valueMul = 30;
    public Transform followTgt;
    public CmSwitch cmSwitch;
    // Start is called before the first frame update
    public PlayerInput playerInput;
    
    private Vector2 m_LeftMouseDownPos = Vector3.zero;
    private bool m_ClickDown;
    private float m_ClickTime;

    private void Start()
    {
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.DrawRay(followTgt.transform.position, followTgt.transform.forward * 20, Color.green);

        if (playerInput.LeftMouseInput && !m_ClickDown)
        {
            if (Time.time - m_ClickTime < 0.3f)
            {
                StartUpdateFwdTurn();
            }

            m_ClickTime = Time.time;
        }


        UpdateFwdTurn();

        if (updateFwdTurn) 
            return;
        UpdateNormalRot();
    }

    private void UpdateNormalRot()
    {
        if (playerInput.LeftMouseInput && !m_ClickDown)
        {
            m_ClickDown = true;
            value = 0;
            m_LeftMouseDownPos = playerInput.MouseMoveInput;
        }

        if (m_ClickDown && playerInput.MouseMoveInput.sqrMagnitude > 0.01f)
        {
            Vector2 delta = playerInput.MouseMoveInput - m_LeftMouseDownPos;
            if (delta.magnitude > 1)
            {
                if (cmSwitch && cmSwitch.isFreeCamera == false)
                {
                    cmSwitch.SwitchCamToTree(true);
                }
            }

            //Debug.LogError( delta );
            if (delta.x > 0)
            {
                value = -1;
                //right
            }
            else if (delta.x < 0)
            {
                value = 1;
                //left
            }
            else if (delta.x == 0)
            {
                value = 0;
            }

            m_LeftMouseDownPos = Input.mousePosition;
        }

        if (m_ClickDown && !playerInput.LeftMouseInput)
        {
            value = 0;
            m_LeftMouseDownPos = Input.mousePosition;
            m_ClickDown = false;
        }

        if (cinemachineBrain.ActiveVirtualCamera.Equals(cinemachineFreeLook) && cinemachineBrain.ActiveBlend == null)
        {
            cinemachineFreeLook.m_XAxis.m_InputAxisValue = value * valueMul;
        }
    }

    private void StartUpdateFwdTurn()
    {
        EndStartUpdateFwdTurn();
        if (Camera.main is not null)
        {
            Transform transform1 = followTgt.transform;
            testFacvl = Vector3.Angle(Camera.main.transform.forward, transform1.forward);

            Vector3 position = transform1.position;
            Vector3 followRight = transform1.right;
            Vector3 left = position + followRight * -5;
            Vector3 right = position + followRight * 5;

            Vector3 cameraPos = Camera.main.transform.position;
            float disL = Vector3.Distance(cameraPos, left);
            float disR = Vector3.Distance(cameraPos, right);

            if (disL < disR)
            {
                fwdRotMul = 1;
            }
            else
            {
                fwdRotMul = -1;
            }
        }

        //Debug.LogError(testFacvl + "   " + m_FwdRotMul);

        updateFwdTurn = true;
    }

    private void EndStartUpdateFwdTurn()
    {
        updateFwdTurn = false;
    }

    public int fwdRotMul;
    public bool updateFwdTurn;
    private float testFacvl = 120;
    public void UpdateFwdTurn()
    {
        if (updateFwdTurn == false)
            return;

        testFacvl = Vector3.SignedAngle(Camera.main.transform.forward, followTgt.transform.forward, Vector3.up);
        testFacvl = Mathf.Abs(testFacvl);
        if (testFacvl < 180)
        {
            cinemachineFreeLook.m_XAxis.m_InputAxisValue = 30 * fwdRotMul;
            if (testFacvl < 60)
            {
                cinemachineFreeLook.m_XAxis.m_InputAxisValue = 0.2f * fwdRotMul;
            }

            if (testFacvl < 20)
            {
                EndStartUpdateFwdTurn();
                cinemachineFreeLook.m_XAxis.m_InputAxisValue = 0;
            }
        }
    }


}
