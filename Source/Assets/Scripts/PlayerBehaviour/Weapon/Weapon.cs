using PlayerBehaviour.Weapon.Type;
using UnityEngine;

namespace PlayerBehaviour.Weapon
{
	public enum TriggerType
	{
		OnTriggerPressed,
		OnTriggerHold
	}

	/// <summary>
	/// Weapon Template
	/// </summary>
	[CreateAssetMenu(menuName = "Weapon / new Base")]
	public class Weapon : ScriptableObject
	{
		public string WeaponName = "";
		public WeaponType WeaponType = WeaponType.Pistol;
		public TriggerType TriggerType = TriggerType.OnTriggerPressed;
		public GameObject Model = null;
		public Vector3 PositionOffset = new Vector3(0, 0, 0);
		public GameObject Projectile = null;

		public ParticleSystem MuzzleFlash = null;
		public ParticleSystem ChargeEffect = null;
		public Vector3 MuzzleFlashOffset = new Vector3(0, 0, 0);
		public ParticleSystem HitEffect = null;
		public GameObject TrailEffect = null;
		public AudioClip ShootAudio = null;
		public AudioClip ChargeAudio = null;

		public LayerMask HitMask = new LayerMask();
		public int ShakeProfile = 0;
		public float Damage = 0;
		public float TicRate = 0;
		public float TicDamage = 0;
		public float ChargeTime = 0.0f;
		public float FadeOutTime = 0.0f;
		public float RayThickness = 0.0f;
		public float MaxLength = 0;
		public int ReflectCount = 0;
		public float FireRate = 0.0f;
		public int AmmoClip = 0;
		public float ReloadTime = 0;
		public Vector2 Recoil = new Vector2(0, 0);
		public float RecoilResetDuration = 0.1f;
		public float PushBack = 0;
		[Range(-45, 45)] public float Spread = 10;
		[Range(0, 10)] public int BulletCount = 0;

		public Weapon Upgrade = null;
	}
}