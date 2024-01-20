using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //// PlayerController components
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float moveSpeedAiming = 5f;
    private Animator animator;
    private Vector3 movementInput;
    private Vector3 lookPoint;
    private float horizontalInput;
    private float verticalInput;
    private Rigidbody playerRigidbody;
    private bool gameOver = false;

    //// Projectile components
    [SerializeField] GameObject projectileFirstPrefab;
    [SerializeField] GameObject projectileSecondPrefab;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] private float abilityFirstCooldownTime;
    [SerializeField] private float abilitySecondCooldownTime;
    private float abilityFirstNextFireTime = 0f;
    private float abilitySecondNextFireTime = 0f;

    //// Ability components
    [SerializeField] private GameObject abilityRange;
    [SerializeField] private AbilityCooldown abilityFirstCooldown;
    [SerializeField] private AbilityCooldown abilitySecondCooldown;
    [SerializeField] private GameObject abilityFirstHolder;
    [SerializeField] private GameObject abilitySecondHolder;

    [SerializeField] private CharacterController movementController;
    private BasicSpawner networkSpawner;
    private Ability ability;
    public enum Ability
    {
        None,
        First,
        Second
    }

    public void SetLookPoint(Vector3 _lookPoint)
    {
        lookPoint = _lookPoint;
    }

    void Start() {
        ability = Ability.None;
        movementInput = Vector3.zero;
        lookPoint = Vector3.zero;
        animator = GetComponentInChildren<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        networkSpawner = FindObjectOfType<BasicSpawner>();
    }

    void Update() {
        if (gameOver) return; 
        //HandleMovementInput();
        HandleMouseInput();
      //  HandleMovementAnimation();
        HandleAbilityInput();
    }

    void FixedUpdate() {
        if (gameOver) return;
        //MoveCharacter();
        //RotateCharacter();
        RotateAbilityRange();
    }

    void HandleMovementInput() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        movementInput = new Vector3(horizontalInput, 0f, verticalInput);

        animator.SetFloat("horizontalMovement", horizontalInput);
        animator.SetFloat("verticalMovement", verticalInput);
    }
    void HandleMouseInput() {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(cameraRay, out float rayDistance)) {
         //   lookPoint = cameraRay.GetPoint(rayDistance);
            networkSpawner.ClientSetLookat(cameraRay.GetPoint(rayDistance)); 
        }
        return;
        if (ability == Ability.None) return;

        if (Input.GetButtonDown("Fire1")) {
            if (Time.time >= abilityFirstNextFireTime && ability == Ability.First)
            {
                animator.SetTrigger("attack");
                abilityFirstNextFireTime = Time.time + abilityFirstCooldownTime;
                abilityFirstCooldown.StartCooldown(abilityFirstCooldownTime);
            }
            else if (Time.time >= abilitySecondNextFireTime &&  ability == Ability.Second)
            {
                animator.SetTrigger("attack");
                abilitySecondNextFireTime = Time.time + abilitySecondCooldownTime;
                abilitySecondCooldown.StartCooldown(abilitySecondCooldownTime);
            }
        }
    }

    void HandleAbilityInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ability = Ability.None;
            abilityFirstHolder.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            abilitySecondHolder.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            ability = Ability.First;
            abilityFirstHolder.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            abilitySecondHolder.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ability = Ability.Second;
            abilityFirstHolder.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            abilitySecondHolder.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }

    void RotateAbilityRange()
    {
        if (ability == Ability.None) abilityRange.SetActive(false);
        else
        {
            abilityRange.SetActive(true);
            abilityRange.transform.LookAt(new Vector3(lookPoint.x, abilityRange.transform.position.y, lookPoint.z), Vector3.up);
        }
    }

    void HandleMovementAnimation() {
        float horizontalMovement = 0f;
        float verticalMovement = 0f;

        if (movementController.velocity.magnitude > 0.25f) {
            animator.SetBool("isMoving", true);

          //  float angle = Vector3.SignedAngle(movementInput, lookPoint - transform.position, Vector3.up);
            float angle = Vector3.SignedAngle(movementController.velocity, lookPoint - transform.position, Vector3.up);
            horizontalMovement = -Mathf.Sin(Mathf.Deg2Rad * angle);
            verticalMovement = Mathf.Cos(Mathf.Deg2Rad * angle);
        } else {
            animator.SetBool("isMoving", false);
        }

        animator.SetFloat("horizontalMovement", horizontalMovement);
        animator.SetFloat("verticalMovement", verticalMovement);
    }

    void MoveCharacter() {
        if (ability == Ability.None)
            playerRigidbody.MovePosition(transform.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        else
            playerRigidbody.MovePosition(transform.position + movementInput * moveSpeedAiming * Time.fixedDeltaTime);
    }
    void RotateCharacter() {
        transform.LookAt(lookPoint);
    }

    public void FireProjectile() {
        GameObject projectileObject;
        if (ability == Ability.First)
            projectileObject = Instantiate(projectileFirstPrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        
        else if (ability == Ability.Second)
            projectileObject = Instantiate(projectileSecondPrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        else return;

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile != null) {
            projectile.Initialize(transform.forward);
        } else {
            Debug.LogError("ProjectilePrefab is missing a Projectile component.");
        }
    }

    public void GameOver()
    {
        gameOver = true;
    }
}
