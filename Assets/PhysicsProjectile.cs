using UnityEngine;
using Fusion;
using Fusion.UnityPhysics;

namespace Projectiles.NetworkObjectExample
{
	// PhysicsProjectile is a NetworkObject spawned in a scene. It uses NetworkRigidbody to synchronize
	// its position and rotation constantly to all clients. This is inefficient and should really be used only
	// for special scenarios (e.g. large rolling projectile that needs to use Rigidbody) or when simplicity is the key.
	// See Projectiles Advanced how even grenades can be done without spawning separate NetworkObjects.
	// For a simple projectile example jump directly to the Example 3.
	[RequireComponent(typeof(NetworkRigidbody3D))]
	public class PhysicsProjectile : NetworkBehaviour
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private float _initialImpulse = 100f;
		[SerializeField]
		private float _lifeTime = 4f;
		[SerializeField]
		private GameObject _visualsRoot;
		[SerializeField]
		private GameObject _hitEffect;
		[SerializeField]
		private float _lifeTimeAfterHit = 2f;

		[Networked]
		private TickTimer _lifeCooldown { get; set; }
		[Networked]
		private NetworkBool _isDestroyed { get; set; }

		private bool _isDestroyedRender;

		private NetworkRigidbody3D _rigidbody;
		private Collider _collider;

		// PUBLIC METHODS

		public void Fire(Vector3 position, Quaternion rotation)
		{
			// TODO: Is teleport still necessary?
			_rigidbody.Teleport(position, rotation);

			_rigidbody.Rigidbody.isKinematic = false;
			_rigidbody.Rigidbody.AddForce(transform.forward * _initialImpulse, ForceMode.Impulse);

			// Set cooldown after which the projectile should be despawned
			if (_lifeTime > 0f)
			{
				_lifeCooldown = TickTimer.CreateFromSeconds(Runner, _lifeTime);
			}
		}

		// NetworkBehaviour INTERFACE

		public override void FixedUpdateNetwork()
		{
			_collider.enabled = _isDestroyed == false;

			if (_lifeCooldown.IsRunning == true && _lifeCooldown.Expired(Runner) == true)
			{
				Runner.Despawn(Object);
			}
		}

		public override void Render()
		{
			if (_isDestroyed == true && _isDestroyedRender == false)
			{
				_isDestroyedRender = true;
				ShowDestroyEffect();
			}
		}

		// MONOBEHAVIOUR

		protected void Awake()
		{
			_rigidbody = GetComponent<NetworkRigidbody3D>();
			_collider = GetComponentInChildren<Collider>();

			_collider.enabled = false;

			if (_hitEffect != null)
			{
				_hitEffect.SetActive(false);
			}
		}

		protected void OnCollisionEnter(Collision collision)
		{
			if (collision.rigidbody != null && Object != null)
			{
				ProcessHit();
			}
		}

		// PRIVATE METHODS

		private void ProcessHit()
		{
			// Save destroyed flag so hit effects can be shown on other clients as well
			_isDestroyed = true;

			_lifeCooldown = TickTimer.CreateFromSeconds(Runner, _lifeTimeAfterHit);

			// Stop the movement
			_rigidbody.Rigidbody.isKinematic = true;
			_collider.enabled = false;
		}

		private void ShowDestroyEffect()
		{
			if (_hitEffect != null)
			{
				_hitEffect.SetActive(true);
			}

			// Hide projectile visual
			if (_visualsRoot != null)
			{
				_visualsRoot.SetActive(false);
			}
		}
	}
}
