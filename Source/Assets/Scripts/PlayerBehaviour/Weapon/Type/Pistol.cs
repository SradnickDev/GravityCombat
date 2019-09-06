using Photon.Pun;
using PlayerBehaviour.Weapon.Model;
using UnityEngine;

namespace PlayerBehaviour.Weapon.Type
{
	public class Pistol : WeaponBase
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
			OnFirstShot();
			CreateProjectile(vec3);
			Recoil();
			PlayMuzzleFlash();
			PlayAudio();
		}

		private void CreateProjectile(Vector3 dir)
		{
			var gbj = Instantiate(Weapon.Projectile, MuzzleFlash.transform.position, Quaternion.identity);
			var projectile = gbj.GetComponent<ProjectileModel>();
			projectile.Init(dir, PhotonNetwork.LocalPlayer, gameObject, Weapon.Damage);
		}
	}
}