using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte BUTTON_ATTACK = 1;
    public NetworkButtons buttons;
    public Vector3 direction;
    public Vector3 lookAt;
}
