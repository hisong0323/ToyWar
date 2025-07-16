using Fusion;
using UnityEngine;
public struct NetworkInputData : INetworkInput
{
    public const byte SpawnButton = 1;

    public NetworkButtons Buttons;

    public Vector3 SpawnPosition;
}