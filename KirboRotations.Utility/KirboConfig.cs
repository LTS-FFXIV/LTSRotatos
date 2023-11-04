using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

namespace KirboRotations.Utility;

internal static class KirboConfig
{
	public static readonly TimeSpan CheckSpan = TimeSpan.FromSeconds(2.5);

	public const int HP_RECORD_TIME = 240;

	public static Queue<(DateTime time, SortedList<uint, float> hpRatios)> RecordedHP { get; } = new Queue<(DateTime, SortedList<uint, float>)>(241);


	public static GameObject? CurrentTarget => KirboService.TargetManager.Target;

	public static PlayerCharacter? LocalPlayer => KirboService.ClientState.LocalPlayer;
}
