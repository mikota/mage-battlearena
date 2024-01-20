using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte BUTTON_ATTACK = 1;
    public const byte BUTTON_NOABILITY = 2;
    public const byte BUTTON_FIRSTABILITY = 4;
    public const byte BUTTON_SECONDABILITY = 8;
    public NetworkButtons buttons;
    public Vector3 direction;
    public Vector3 lookAt;
}
