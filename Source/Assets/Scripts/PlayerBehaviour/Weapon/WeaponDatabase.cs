using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace PlayerBehaviour.Weapon
{
	[CreateAssetMenu(menuName = "Weapon/Database")]
	public class WeaponDatabase : ScriptableObject
	{
		[SerializeField, ReorderableList] private List<Weapon> Weapons = new List<Weapon>();

		public Weapon GetWeapon(int index)
		{
			return Weapons[index];
		}

		public int GetIndex(Weapon weapon)
		{
			return Weapons.IndexOf(weapon);
		}
	}
}