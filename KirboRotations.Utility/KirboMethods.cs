using System;
using System.Threading;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using RotationSolver.Basic.Helpers;

namespace KirboRotations.Utility;

internal static class KirboMethods
{
	internal static PlayerCharacter? LocalPlayer => KirboService.ClientState.LocalPlayer;

	internal static GameObject? CurrentTarget { get; private set; } = KirboService.TargetManager.Target;


	public static bool Flag { get; internal set; } = false;


	public static TimeSpan CheckSpan { get; private set; }

	internal static void ResetDebugFlag()
	{
		if (Flag)
		{
			ResetBoolAfterDelay();
			PluginLog.Debug($"Resetting Flag > {Flag}", Array.Empty<object>());
		}
	}

	internal static void ResetBoolAfterDelay()
	{
		Thread.Sleep(300);
		Flag = false;
		PluginLog.Debug($"flag is {Flag}", Array.Empty<object>());
	}

	internal static float PlayerHealthPercentageHp()
	{
		return (float)((Character)LocalPlayer).CurrentHp / (float)((Character)LocalPlayer).MaxHp * 100f;
	}

	internal static float GetTargetHPPercent(GameObject? OurTarget = null)
	{
		if (OurTarget == null)
		{
			OurTarget = CurrentTarget;
			if (OurTarget == null)
			{
				return 0f;
			}
		}
		BattleChara chara = (BattleChara)(object)((OurTarget is BattleChara) ? OurTarget : null);
		return (chara == null) ? 0f : ((float)((Character)chara).CurrentHp / (float)((Character)chara).MaxHp * 100f);
	}

	internal static float EnemyHealthMaxHp()
	{
		if (CurrentTarget == null)
		{
			return 0f;
		}
		GameObject? currentTarget = CurrentTarget;
		BattleChara chara = (BattleChara)(object)((currentTarget is BattleChara) ? currentTarget : null);
		if (chara == null)
		{
			return 0f;
		}
		return ((Character)chara).MaxHp;
	}

	internal static float EnemyHealthCurrentHp()
	{
		if (CurrentTarget == null)
		{
			return 0f;
		}
		GameObject? currentTarget = CurrentTarget;
		BattleChara chara = (BattleChara)(object)((currentTarget is BattleChara) ? currentTarget : null);
		if (chara == null)
		{
			return 0f;
		}
		return ((Character)chara).CurrentHp;
	}

	internal static bool HasFriendlyTarget(GameObject? OurTarget = null)
	{
		if (OurTarget == null)
		{
			OurTarget = CurrentTarget;
			if (OurTarget == null)
			{
				return false;
			}
		}
		return false;
	}

	internal static bool HasBattleTarget()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		GameObject currentTarget = CurrentTarget;
		BattleNpc val = (BattleNpc)(object)((currentTarget is BattleNpc) ? currentTarget : null);
		if (val != null)
		{
			BattleNpcSubKind battleNpcKind = val.BattleNpcKind;
			if ((int)battleNpcKind == 1 || (int)battleNpcKind == 5)
			{
				return true;
			}
		}
		return false;
	}

	internal static float GetDeadTime(this BattleChara b, bool wholeTime = false)
	{
		if ((GameObject)(object)b == (GameObject)null)
		{
			return float.NaN;
		}
		uint objectId = ((GameObject)b).ObjectId;
		DateTime startTime = DateTime.MinValue;
		float thatTimeRatio = 0f;
		foreach (var (time, hpRatios) in KirboConfig.RecordedHP)
		{
			if (hpRatios.TryGetValue(objectId, out var ratio) && ratio != 1f)
			{
				startTime = time;
				thatTimeRatio = ratio;
				break;
			}
		}
		TimeSpan timespan = DateTime.Now - startTime;
		if (startTime == DateTime.MinValue || timespan < CheckSpan)
		{
			return float.NaN;
		}
		float ratioNow = ObjectHelper.GetHealthRatio(b);
		float ratioReduce = thatTimeRatio - ratioNow;
		if (ratioReduce <= 0f)
		{
			return float.NaN;
		}
		return (float)timespan.TotalSeconds / ratioReduce * (wholeTime ? 1f : ratioNow);
	}

	internal static bool Boss()
	{
		return false;
	}

	internal static bool TrashMob()
	{
		return false;
	}
}
