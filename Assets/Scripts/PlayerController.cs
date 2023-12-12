using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //// PlayerController components
    private Vector3 _movementInput;
    private Vector3 _lookPoint;
    private float _horizontalInput;
    private float _verticalInput;

    //// Player Components
    private Player _player;

    //// Projectile components
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _projectileSpawnPoint;
    private float _nextFireTime = 0f;

    void Start() {
        _movementInput = Vector3.zero;
        _lookPoint = Vector3.zero;
        _player = GetComponent<Player>();
    }

    void Update() {
        HandleMovementInput();
        HandleMouseInput();

        HandleMovementAnimation();
    }

    void FixedUpdate() {
        MoveCharacter();
        RotateCharacter();
    }

    void HandleMovementInput() {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        _movementInput = new Vector3(_horizontalInput, 0f, _verticalInput);

        _player.animator.SetFloat("horizontalMovement", _horizontalInput);
        _player.animator.SetFloat("verticalMovement", _verticalInput);
    }
    void HandleMouseInput() {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(cameraRay, out float rayDistance)) {
            _lookPoint = cameraRay.GetPoint(rayDistance);
        }

        if (Time.time >= _nextFireTime && Input.GetButtonDown("Fire1") && (_projectilePrefab != null && _projectileSpawnPoint != null)) {
            _player.animator.SetTrigger("attack");
            _nextFireTime = Time.time + _player.fireCooldown;
            Invoke("FireProjectile", 0.25f);
        }
    }

    void HandleMovementAnimation() {
        float horizontalMovement = 0f;
        float verticalMovement = 0f;

        if (_movementInput != Vector3.zero) {
            _player.animator.SetBool("isMoving", true);

            float angle = Vector3.SignedAngle(_movementInput, _lookPoint - transform.position, Vector3.up);
            horizontalMovement = -Mathf.Sin(Mathf.Deg2Rad * angle);
            verticalMovement = Mathf.Cos(Mathf.Deg2Rad * angle);
        } else {
            _player.animator.SetBool("isMoving", false);
        }

        _player.animator.SetFloat("horizontalMovement", horizontalMovement);
        _player.animator.SetFloat("verticalMovement", verticalMovement);
    }

    void MoveCharacter() {
        transform.Translate(_movementInput * _player.moveSpeed * Time.fixedDeltaTime, Space.World);
    }
    void RotateCharacter() {
        transform.LookAt(_lookPoint);
    }

    void FireProjectile() {
        GameObject projectileObject = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile != null) {
            projectile.Initialize(transform.forward * _player.projectileSpeed, _player.projectileDamage);
        } else {
            Debug.LogError("ProjectilePrefab is missing a Projectile component.");
        }
    }
}
