using System;
using System.Collections.Generic;
using System.Text;

namespace UltraRandomizer.HarmonyPatches
{
	public class WeaponCheat : ICheat
	{
		private static WeaponCheat _lastInstance;

		private bool active;

		public string LongName => "Weapon Randomizer";

		public string Identifier => "wr";

		public string ButtonEnabledOverride { get; }

		public string ButtonDisabledOverride { get; }

		public string Icon => null;

		public bool IsActive => active;

		public bool DefaultState { get; }

		public StatePersistenceMode PersistenceMode => StatePersistenceMode.Persistent;

		public void Enable()
		{
			active = true;
			_lastInstance = this;

			IsCheatActive.Instance.WeaponEnabled = true;
		}

		public void Disable()
		{
			active = false;

			IsCheatActive.Instance.WeaponEnabled = false;
		}

		public void Update() { }

		// Im using a function here because its easier to call using a transpiler
		public static bool GetActive()
		{
			if (_lastInstance != null)
			{
				return _lastInstance.active;
			}
			return false;
		}

	}
}
