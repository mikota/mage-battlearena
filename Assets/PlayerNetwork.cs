using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    public PlayerController playerController;
    [SerializeField] private GameObject projectileFirstPrefab;

    //float _speed = 5.0f;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        //_cc.MaxSpeed = _speed;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
            playerController.SetLookPoint(data.lookAt);
            if (data.buttons.IsSet(NetworkInputData.BUTTON_ATTACK))
            {
                var projectile = Runner.Spawn(projectileFirstPrefab, transform.position + transform.forward, transform.rotation);
                projectile.GetComponent<Projectile>().Initialize(playerController.transform.forward);
            }
            //_cc.SimpleMove(_speed*data.direction*Runner.DeltaTime);
        }
    }
}