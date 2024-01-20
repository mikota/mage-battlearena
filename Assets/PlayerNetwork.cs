using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    public PlayerController playerController;
    [SerializeField] private Health clientHealth;
    public PlayerRef playerRef;
    [SerializeField] private GameObject projectileFirstPrefab;
    [SerializeField] private GameObject projectileSecondPrefab;
    [SerializeField] private Animator animator;
    [Networked] public float health { get; set; }
    [Networked] public Vector3 lookPoint { get; set; }
    [Networked] public PlayerController.Ability ability { get; set; }
    [Networked] public Vector3 anim_movementInput { get; set; }
    [Networked] public bool anim_isMoving { get; set; }
    private const byte TRIGGER_ATTACK = 1;
    private const byte TRIGGER_HIT = 2;
    private const byte TRIGGER_DEATH = 3;
    [Networked] public byte anim_triggers { get; set; }
    
    //float _speed = 5.0f;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        //_cc.MaxSpeed = _speed;
    }

    public void Update()
    {
        clientHealth.SetCurrentHealth(health);
        playerController.SetLookPoint(lookPoint);
    }

    public override void Render()
    {
        animator.SetFloat("Horizontal", anim_movementInput.x);
        animator.SetFloat("Vertical", anim_movementInput.z);
        animator.SetBool("isMoving", anim_isMoving);
        if ((anim_triggers & TRIGGER_ATTACK) != 0)
        {
            animator.SetTrigger("attack");
        }
        anim_triggers = 0;
    }

    public override void FixedUpdateNetwork()
    {
        Vector3 relativePosition;
        Quaternion rotation;
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
            lookPoint = data.lookAt;
            playerController.SetLookPoint(data.lookAt);

            anim_movementInput = data.direction;
            anim_isMoving = data.direction.magnitude > 0.1f;
            anim_triggers = 0;

            if (data.buttons.IsSet(NetworkInputData.BUTTON_NOABILITY))
            {
                ability = PlayerController.Ability.None;
            } else
            {
                if (data.buttons.IsSet(NetworkInputData.BUTTON_FIRSTABILITY))
                {
                    ability = PlayerController.Ability.First;
                } else if (data.buttons.IsSet(NetworkInputData.BUTTON_SECONDABILITY))
                {
                    ability = PlayerController.Ability.Second;
                }
            }

            if (data.buttons.IsSet(NetworkInputData.BUTTON_ATTACK) && ability != PlayerController.Ability.None)
            {
                anim_triggers |= TRIGGER_ATTACK;
                GameObject prefab = null;
                if (ability == PlayerController.Ability.First)
                {
                    prefab = projectileFirstPrefab;
                } else
                {
                    prefab = projectileSecondPrefab;
                }
                var projectile = Runner.Spawn(prefab, transform.position + transform.forward, transform.rotation);
                relativePosition = lookPoint - transform.position;
                rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
                transform.localEulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
                projectile.GetComponent<Projectile>().Initialize(playerController.transform.forward);
            }
            //_cc.SimpleMove(_speed*data.direction*Runner.DeltaTime);
        }
        relativePosition = lookPoint - transform.position;
        rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        transform.localEulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
    }

    public void TakeDamage(float dmg)
    {
        if (playerController != null)
        {
            health -= dmg;
            clientHealth.TakeDamage(dmg);
        }
    }
}