using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerNetwork : NetworkBehaviour
{
  private NetworkCharacterController _cc;

  private void Awake()
  {
    _cc = GetComponent<NetworkCharacterController>();
  }

  public override void FixedUpdateNetwork()
  {
    if (GetInput(out NetworkInputData data))
    {
      data.direction.Normalize();
      _cc.Move(50*data.direction*Runner.DeltaTime);
    }
  }
}