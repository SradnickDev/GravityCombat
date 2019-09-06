using Photon.Pun;
using PlayerBehaviour.Weapon.Model;
using UnityEngine;

namespace PlayerBehaviour.Weapon.Type
{
	public class ShotGun : WeaponBase
	{
		public override void Shoot()
		{
			if (Time.time > CooldownCollapsed && !IsReloading && !WeaponCollision.IsColliding())
			{
				PhotonView.RPC("ShootEffectRpc", RpcTarget.All, WeaponSocket.transform.forward);

				PushBack();
				ReduceAmmo();
				CameraShake.StartShake(Weapon.ShakeProfile);
				CooldownCollapsed = Time.time + Weapon.FireRate;
			}
		}

		[PunRPC]
		protected override void ShootEffectRpc(Vector3 vec3)
		{
			var dir = vec3;
			OnFirstShot();
			CreateProjectiles(dir);
			Recoil();
			PlayMuzzleFlash();
			PlayAudio();
		}

		/// <summary>
		/// Instantiate projectiles in specific angles with a bit of randomness between each angle.
		/// </summary>
		/// <param name="dir"></param>
		private void CreateProjectiles(Vector3 dir)
		{
			var angle = -(Weapon.Spread / Weapon.BulletCount) * Weapon.BulletCount / 2;
			for (var i = 0; i < Weapon.BulletCount; i++)
			{
				var gbj = Instantiate(Weapon.Projectile, MuzzleFlash.transform.position, Quaternion.identity);
				var projectile = gbj.GetComponent<ProjectileModel>();

				var prevAngle = angle;
				angle += Weapon.Spread / (float) Weapon.BulletCount;
				var rndAngle = Random.Range(prevAngle, angle);

				var direction = Quaternion.Euler(0, 0, rndAngle) * dir;

				projectile.Init(direction, PhotonNetwork.LocalPlayer, gameObject, Weapon.Damage);
			}
		}
	}
}