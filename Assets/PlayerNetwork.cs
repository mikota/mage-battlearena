using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System.Runtime.InteropServices;

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
    private GameObject abilityHolder1;
    private GameObject abilityHolder2;
    private AbilityCooldown ability1Cooldown;
    private AbilityCooldown ability2Cooldown;
    [SerializeField] private float abilityFirstCooldownTime = 0.2f;
    [SerializeField] private float abilitySecondCooldownTime = 1.5f;
    private float ability1NextFireTime = 0f;
    private float ability2NextFireTime = 0f;
    [Networked] public bool isDead { get; set; }
    [Networked] public float respawnTime { get; set; }
    public int killCount { get; set; }
    [Networked] public int _killCount { get; set; }
    
    //float _speed = 5.0f;
    public void IncKillCount()
    {
        killCount++;
    }
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        //_cc.MaxSpeed = _speed;
    }

    public void Update()
    {
        clientHealth.SetCurrentHealth(health);
        clientHealth.SetKillCount(_killCount);
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
            animator.SetTrigger("getHit");
            anim_triggers = anim_triggers & ~TriggerFlags.HIT;
        }
        if ((anim_triggers & TriggerFlags.DEATH) != 0)
        {
            animator.SetTrigger("die");
            anim_triggers = anim_triggers & ~TriggerFlags.DEATH;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (isDead)
        {
            _cc.enabled = false;
            if (Time.time > respawnTime)
            {
                isDead = false;
                health = 1000;
                clientHealth.SetCurrentHealth(health);
                anim_triggers = TriggerFlags.NONE;
                animator.Rebind();
                animator.Update(0f);
                playerController.SetLookPoint(transform.position + transform.forward);
                Vector3 spawnPosition;
                GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("spawnpoint");

                if (spawnPoints.Length > 0)
                {
                    spawnPosition = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
                }
                else
                {
                    spawnPosition = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 1.10f, UnityEngine.Random.Range(-1.0f, 1.0f));
                }
                transform.position = spawnPosition;
            } else
            {
                return;
            }
        }
        _cc.enabled = true;
        _killCount = killCount;
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
                GameObject prefab = null;
                bool canAttack = true;
                if (ability == PlayerController.Ability.First)
                {
                    prefab = projectileFirstPrefab;
                    if (Time.time < ability1NextFireTime)
                    {
                        canAttack = false;
                    } else
                    {
                        ability1NextFireTime = Time.time + abilityFirstCooldownTime;
                        if (ability1Cooldown == null)
                        {
                            ability1Cooldown = GameObject.FindGameObjectWithTag("AbilityFirst")
                                .GetComponentInChildren<AbilityCooldown>();
                        }
                        ability1Cooldown.StartCooldown(abilityFirstCooldownTime);
                    }
                } else
                {
                    if (Time.time < ability2NextFireTime)
                    {
                        canAttack = false;
                    } else
                    {
                        ability2NextFireTime = Time.time + abilitySecondCooldownTime;
                        if (ability2Cooldown == null)
                        {
                            ability2Cooldown = GameObject.FindGameObjectWithTag("AbilitySecond")
                                .GetComponentInChildren<AbilityCooldown>();
                        }
                        ability2Cooldown.StartCooldown(abilitySecondCooldownTime);
                    }
                    prefab = projectileSecondPrefab;
                }
                if (canAttack) 
                {
                    anim_triggers |= TriggerFlags.ATTACK;
                    var projectile = Runner.Spawn(prefab, transform.position + transform.forward, transform.rotation);
                    relativePosition = lookPoint - transform.position;
                    rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
                    transform.localEulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
                    projectile.GetComponent<Projectile>().Initialize(playerController.transform.forward, this); 
                }
            }
            //_cc.SimpleMove(_speed*data.direction*Runner.DeltaTime);
        }
        relativePosition = lookPoint - transform.position;
        rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        transform.localEulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        //put back on ground if flying up
        if (Mathf.Abs(transform.position.y) > 2.5f)
        {
            transform.position = new Vector3(transform.position.x, 2.5f, transform.position.z);
        }
        //put in center of map if too far out
        if (Mathf.Abs(transform.position.x) > 21f || Mathf.Abs(transform.position.z) > 42f)
        {
            transform.position = new Vector3(0f, 2.5f, 0f);
        }
    }

    public bool TakeDamage(float dmg)
    {
        if (playerController != null)
        {
            anim_triggers |= TriggerFlags.HIT;
            health -= dmg;
            clientHealth.TakeDamage(dmg);
            if (health <= 0)
            {
                anim_triggers |= TriggerFlags.DEATH;
                isDead = true;
                respawnTime = Time.time + 1.5f;
                return true;
            }
        }
        return false;
    }
}