using System;
using System.Collections.Generic;

public class NormalInptuState : FSMState<CMController>
{
    public NormalInptuState(int id, string name = "NormalController") : base(id, name)
    {
    }

    public override void Tick()
    {
        t.SetMovementInput(t.playerInput.WASDInput);
        t.SetJumpInput(t.playerInput.JumpInput);
        t.SetDashInput(t.playerInput.DashInput);
    }
}