using System;
using System.Collections.Generic;

public class NormalInptuState<T> : FSMState where T : Controller
{
    private CMController m_Controller;
    
    public NormalInptuState(int id, string name = null) : base(id, name)
    {
        StateName = "NormalController";
    }

    public void SetController(CMController controller)
    {
        m_Controller = controller;
    }
    
    public override void Tick()
    {
        m_Controller.SetMovementInput(m_Controller.playerInput.WASDInput);
        m_Controller.SetJumpInput(m_Controller.playerInput.JumpInput);
        m_Controller.SetDashInput(m_Controller.playerInput.DashInput);
    }

    public override void Destroy()
    {
        base.Destroy();
        m_Controller = null;
    }
}