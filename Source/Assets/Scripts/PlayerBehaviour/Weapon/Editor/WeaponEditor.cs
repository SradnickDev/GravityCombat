using PlayerBehaviour.Weapon.Type;
using UnityEditor;
using UnityEngine;

namespace PlayerBehaviour.Weapon.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Weapon))]
	public class WeaponEditor : UnityEditor.Editor
	{
		private Weapon m_weapon = null;
		private SerializedObject m_target = null;
		private UnityEditor.Editor m_editor = null;

		private Vector2 m_refMinMaxSlider = new Vector2(0, 0);

		public override bool HasPreviewGUI()
		{
			return true;
		}

		private void OnEnable()
		{
			m_weapon = (Weapon) target;
			m_target = new SerializedObject(m_weapon);
		}

		public override void OnInspectorGUI()
		{
			m_target.Update();

			var weaponNameProp = m_target.FindProperty("WeaponName");

			var weaponTypeProp = m_target.FindProperty("WeaponType");
			var triggerTypeProp = m_target.FindProperty("TriggerType");
			var modelProp = m_target.FindProperty("Model");
			var positionOffsetProp = m_target.FindProperty("PositionOffset");
			var projectileProp = m_target.FindProperty("Projectile");

			var muzzleFlashProp = m_target.FindProperty("MuzzleFlash");
			var chargeEffectProp = m_target.FindProperty("ChargeEffect");
			var muzzleFlashOffsetProp = m_target.FindProperty("MuzzleFlashOffset");
			var shootAudioProp = m_target.FindProperty("ShootAudio");
			var chargeAudioProp = m_target.FindProperty("ChargeAudio");
			var trailEffectProp = m_target.FindProperty("TrailEffect");
			var hitEffectProp = m_target.FindProperty("HitEffect");

			var hitMaskProp = m_target.FindProperty("HitMask");
			var shakeProfileProp = m_target.FindProperty("ShakeProfile");
			var damageProp = m_target.FindProperty("Damage");

			var ticRateProp = m_target.FindProperty("TicRate");
			var ticDamageProp = m_target.FindProperty("TicDamage");

			var fadeOutProp = m_target.FindProperty("FadeOutTime");
			var loadingTimeProp = m_target.FindProperty("ChargeTime");
			var rayThickProp = m_target.FindProperty("RayThickness");

			var distanceProp = m_target.FindProperty("MaxLength");
			var reflectProp = m_target.FindProperty("ReflectCount");
			var fireRateProp = m_target.FindProperty("FireRate");
			var ammoClipProp = m_target.FindProperty("AmmoClip");
			var reloadTimeProp = m_target.FindProperty("ReloadTime");
			var recoilProp = m_target.FindProperty("Recoil");
			var recoilDurationProp = m_target.FindProperty("RecoilResetDuration");
			var pushBackProp = m_target.FindProperty("PushBack");
			var spreadProp = m_target.FindProperty("Spread");
			var bulletCount = m_target.FindProperty("BulletCount");

			var upgradeProp = m_target.FindProperty("Upgrade");


			var weaponType = (WeaponType) weaponTypeProp.enumValueIndex;

			EditorGUILayout.LabelField("Weapon", EditorStyles.boldLabel);

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(weaponNameProp);
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Model", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(weaponTypeProp);
			EditorGUILayout.PropertyField(triggerTypeProp);
			EditorGUILayout.PropertyField(modelProp);
			EditorGUILayout.PropertyField(positionOffsetProp);

			if (weaponType == WeaponType.Pistol || weaponType == WeaponType.ShotGun)
			{
				EditorGUILayout.PropertyField(projectileProp);
			}


			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("VFX/SFX", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(muzzleFlashProp);
			if (weaponType == WeaponType.Sniper)
			{
				EditorGUILayout.PropertyField(chargeEffectProp);
			}

			EditorGUILayout.PropertyField(muzzleFlashOffsetProp);
			EditorGUILayout.PropertyField(shootAudioProp);
			if (weaponType == WeaponType.Sniper)
			{
				EditorGUILayout.PropertyField(chargeAudioProp);
			}

			if (weaponType == WeaponType.AssaultRifle || weaponType == WeaponType.Laser ||
				weaponType == WeaponType.Sniper)
			{
				EditorGUILayout.PropertyField(trailEffectProp);
				EditorGUILayout.PropertyField(hitEffectProp);
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(hitMaskProp);
			EditorGUILayout.PropertyField(shakeProfileProp);
			EditorGUILayout.PropertyField(damageProp);

			if (weaponType == WeaponType.Sniper)
			{
				EditorGUILayout.PropertyField(loadingTimeProp);
				EditorGUILayout.PropertyField(fadeOutProp);
				EditorGUILayout.PropertyField(ticRateProp);
				EditorGUILayout.PropertyField(ticDamageProp);
				EditorGUILayout.PropertyField(rayThickProp);
				EditorGUILayout.PropertyField(distanceProp);
			}

			if (weaponType == WeaponType.Laser)
			{
				EditorGUILayout.PropertyField(distanceProp);
				EditorGUILayout.PropertyField(reflectProp);
			}

			EditorGUILayout.PropertyField(fireRateProp);
			EditorGUILayout.PropertyField(ammoClipProp);
			EditorGUILayout.PropertyField(reloadTimeProp);


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(recoilProp);

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(recoilDurationProp);

			EditorGUILayout.PropertyField(pushBackProp);

			if (weaponType == WeaponType.ShotGun)
			{
				EditorGUILayout.PropertyField(spreadProp);
				EditorGUILayout.PropertyField(bulletCount);
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Upgrade", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(upgradeProp);
			EditorGUILayout.EndVertical();

			m_target.ApplyModifiedProperties();
			EditorUtility.SetDirty(m_weapon);
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (m_weapon.Model)
			{
				if (m_editor == null)
					m_editor = UnityEditor.Editor.CreateEditor(m_weapon.Model);
				m_editor.OnPreviewGUI(r, background);
			}
		}
	}
}