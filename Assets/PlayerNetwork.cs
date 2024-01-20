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
    public enum TriggerFlags
    {
        NONE = 0,
        ATTACK = 1,
        HIT = 2,
        DEATH = 4
    }
    [Networked] public TriggerFlags anim_triggers { get; set; }
    
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
        
        animator.SetFloat("horizontalMovement", anim_movementInput.x);
        animator.SetFloat("verticalMovement", anim_movementInput.z);
        animator.SetBool("isMoving", anim_isMoving);

    }

    public override void Render()
    {
        if ((anim_triggers & TriggerFlags.ATTACK) != 0)
        {
            animator.SetTrigger("attack");
            anim_triggers = anim_triggers & ~TriggerFlags.ATTACK;
        }
        if ((anim_triggers & TriggerFlags.HIT) != 0)
        {
            animator.SetTrigger("hit");
            anim_triggers = anim_triggers & ~TriggerFlags.HIT;
        }
        if ((anim_triggers & TriggerFlags.DEATH) != 0)
        {
            animator.SetTrigger("death");
            anim_triggers = anim_triggers & ~TriggerFlags.DEATH;
        }
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
                anim_triggers |= TriggerFlags.ATTACK;
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
            anim_triggers |= TriggerFlags.HIT;
            health -= dmg;
            clientHealth.TakeDamage(dmg);
        }
    }
}