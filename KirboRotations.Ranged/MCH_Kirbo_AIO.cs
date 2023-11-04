using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using ImGuiNET;
using KirboRotations.Utility;
using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.Ranged;

[RotationDesc(/*Could not decode attribute arguments.*/)]
[SourceCode(Path = "main/KirboRotations/Ranged/MCH_Kirbo_AIO.cs")]
[LinkDescription("https://i.imgur.com/23r8kFK.png", "Early AA")]
[LinkDescription("https://i.imgur.com/vekKW2k.jpg", "Delayed Tools")]
[LinkDescription("https://i.imgur.com/bkLg5WS.png", "123 Tools")]
[LinkDescription("https://i.imgur.com/qdCOQKy.png", "Fast Wildfire")]
public class MCH_Kirbo_AIO : MCH_Base
{
	private bool display_headers = false;

	private bool display_rotationInfo = true;

	private bool display_combat = true;

	private bool display_action = true;

	public override string GameVersion => "6.48";

	public override string RotationName => "Kirbo's - All In One";

	public override string Description => "My work on MCH rotations found on the Balance. \nTested on a Level 90 MCH with 2.50gcd.\n\n:)";

	private bool InBurst { get; set; } = false;


	private bool IsDying { get; set; } = false;


	public override bool ShowStatus => true;

	private int Openerstep { get; set; } = 0;


	private bool OpenerHasFinished { get; set; } = false;


	private bool OpenerHasFailed { get; set; } = false;


	private bool OpenerActionsAvailable { get; set; } = false;


	private bool OpenerInProgress { get; set; } = false;


	private bool SafeToUseHypercharge { get; set; } = false;


	private bool SafeToUseWildfire { get; set; } = false;


	private bool WillhaveTool { get; set; } = false;


	protected override IRotationConfigSet CreateConfiguration()
	{
		return ((CustomRotation)this).CreateConfiguration().SetCombo("RotationSelection", 1, "Select which Rotation will be used. (Openers will only be followed at level 90)", new string[6] { "Early AA", "Delayed Tools", "123 Tools", "Fast Wildfire", "FFlogs Opener (Experimental)", "Test (Experimental)" }).SetBool("BatteryStuck", false, "Battery overcap protection\n(Will try and use Rook AutoTurret if Battery is at 100 and next skill increases Battery)")
			.SetBool("HeatStuck", false, "Heat overcap protection\n(Will try and use HyperCharge if Heat is at 100 and next skill increases Heat)")
			.SetBool("DumpSkills", true, "Dump Skills when Target is dying\n(Will try and spend remaining resources before boss dies)");
	}

	protected override IAction CountDownAction(float remainTime)
	{
		IAction val = default(IAction);
		if (OpenerActionsAvailable)
		{
			switch (((CustomRotation)this).Configs.GetCombo("RotationSelection"))
			{
			case 0:
			{
				if (remainTime <= ((IAction)MCH_Base.AirAnchor).AnimationLockTime && StatusHelper.HasStatus((BattleChara)(object)CustomRotation.Player, true, (StatusID[])(object)new StatusID[1] { (StatusID)851 }) && MCH_Base.AirAnchor.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
				{
					PluginLog.LogDebug("Early AA Opener Started", Array.Empty<object>());
					PluginLog.Debug("OpenerInProgress has been set to 'True'", Array.Empty<object>());
					OpenerInProgress = true;
					return (IAction)(object)MCH_Base.AirAnchor;
				}
				IAction act0 = default(IAction);
				if (remainTime <= ((IAction)CustomRotation.TinctureOfDexterity8).AnimationLockTime + ((IAction)MCH_Base.AirAnchor).AnimationLockTime && ((CustomRotation)this).UseBurstMedicine(ref act0, false))
				{
					return act0;
				}
				if (remainTime <= 5f && MCH_Base.Reassemble.CurrentCharges > 1)
				{
					return (IAction)(object)MCH_Base.Reassemble;
				}
				break;
			}
			case 1:
			{
				if (remainTime <= ((IAction)MCH_Base.SplitShot).AnimationLockTime && MCH_Base.SplitShot.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
				{
					OpenerInProgress = true;
					return (IAction)(object)MCH_Base.SplitShot;
				}
				IAction act1 = default(IAction);
				if ((double)remainTime <= (double)(((IAction)MCH_Base.SplitShot).AnimationLockTime + ((IAction)CustomRotation.TinctureOfDexterity8).AnimationLockTime) + 0.2 && ((CustomRotation)this).UseBurstMedicine(ref act1, false))
				{
					return act1;
				}
				break;
			}
			case 2:
				if ((double)remainTime <= 0.6 + (double)((IAction)MCH_Base.SplitShot).AnimationLockTime && MCH_Base.SplitShot.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
				{
					OpenerInProgress = true;
					return (IAction)(object)MCH_Base.SplitShot;
				}
				break;
			case 3:
			{
				if (remainTime <= ((IAction)MCH_Base.Drill).AnimationLockTime && StatusHelper.HasStatus((BattleChara)(object)CustomRotation.Player, true, (StatusID[])(object)new StatusID[1] { (StatusID)851 }) && MCH_Base.AirAnchor.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
				{
					OpenerInProgress = true;
					return (IAction)(object)MCH_Base.AirAnchor;
				}
				IAction act2 = default(IAction);
				if (remainTime <= ((IAction)CustomRotation.TinctureOfDexterity8).AnimationLockTime + ((IAction)MCH_Base.AirAnchor).AnimationLockTime && ((CustomRotation)this).UseBurstMedicine(ref act2, false))
				{
					return act2;
				}
				if (remainTime <= 5f && MCH_Base.Reassemble.CurrentCharges > 1)
				{
					return (IAction)(object)MCH_Base.Reassemble;
				}
				break;
			}
			case 4:
			{
				if (remainTime <= ((IAction)MCH_Base.AirAnchor).AnimationLockTime && StatusHelper.HasStatus((BattleChara)(object)CustomRotation.Player, true, (StatusID[])(object)new StatusID[1] { (StatusID)851 }) && MCH_Base.AirAnchor.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
				{
					OpenerInProgress = true;
					return (IAction)(object)MCH_Base.AirAnchor;
				}
				IAction act3 = default(IAction);
				if (remainTime <= ((IAction)CustomRotation.TinctureOfDexterity8).AnimationLockTime + ((IAction)MCH_Base.AirAnchor).AnimationLockTime && ((CustomRotation)this).UseBurstMedicine(ref act3, false))
				{
					return act3;
				}
				if (remainTime < 5f && MCH_Base.Reassemble.CurrentCharges > 1)
				{
					return (IAction)(object)MCH_Base.Reassemble;
				}
				break;
			}
			case 5:
			{
				if (remainTime <= ((IAction)MCH_Base.SplitShot).AnimationLockTime && MCH_Base.SplitShot.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
				{
					OpenerInProgress = true;
					return (IAction)(object)MCH_Base.SplitShot;
				}
				IAction act4 = default(IAction);
				if ((double)remainTime <= (double)(((IAction)MCH_Base.SplitShot).AnimationLockTime + ((IAction)CustomRotation.TinctureOfDexterity8).AnimationLockTime) + 0.2 && ((CustomRotation)this).UseBurstMedicine(ref act4, false))
				{
					return act4;
				}
				break;
			}
			}
		}
		if (((Character)CustomRotation.Player).Level < 90)
		{
			if (((IEnoughLevel)MCH_Base.AirAnchor).EnoughLevel && (double)remainTime <= 0.6 + (double)CustomRotation.CountDownAhead && MCH_Base.AirAnchor.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
			{
				return (IAction)(object)MCH_Base.AirAnchor;
			}
			if (!((IEnoughLevel)MCH_Base.AirAnchor).EnoughLevel && ((IEnoughLevel)MCH_Base.Drill).EnoughLevel && (double)remainTime <= 0.6 + (double)CustomRotation.CountDownAhead && MCH_Base.Drill.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
			{
				return (IAction)(object)MCH_Base.Drill;
			}
			if (!((IEnoughLevel)MCH_Base.AirAnchor).EnoughLevel && !((IEnoughLevel)MCH_Base.Drill).EnoughLevel && ((IEnoughLevel)MCH_Base.HotShot).EnoughLevel && (double)remainTime <= 0.6 + (double)CustomRotation.CountDownAhead && MCH_Base.HotShot.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
			{
				return (IAction)(object)MCH_Base.HotShot;
			}
			if (!((IEnoughLevel)MCH_Base.AirAnchor).EnoughLevel && !((IEnoughLevel)MCH_Base.Drill).EnoughLevel && !((IEnoughLevel)MCH_Base.HotShot).EnoughLevel && (double)remainTime <= 0.6 + (double)CustomRotation.CountDownAhead && MCH_Base.CleanShot.CanUse(ref val, (CanUseOption)0, (byte)0, (byte)0))
			{
				return (IAction)(object)MCH_Base.CleanShot;
			}
			if (remainTime < 5f && MCH_Base.Reassemble.CurrentCharges > 0)
			{
				return (IAction)(object)MCH_Base.Reassemble;
			}
		}
		return ((CustomRotation)this).CountDownAction(remainTime);
	}

	private bool Opener(out IAction act)
	{
		byte OverHeatStacks = StatusHelper.StatusStack((BattleChara)(object)CustomRotation.Player, true, (StatusID[])(object)new StatusID[1] { (StatusID)2688 });
		while (OpenerInProgress && (!OpenerHasFinished || !OpenerHasFailed))
		{
			if (CustomRotation.TimeSinceLastAction.TotalSeconds > 3.0 && !KirboMethods.Flag)
			{
				OpenerHasFailed = true;
				OpenerInProgress = false;
				Openerstep = 0;
				PluginLog.Warning("Opener Failed Reason: 'Time Since Last Action more then 3 seconds'", Array.Empty<object>());
				PluginLog.Debug("openerstep is now: {Openerstep}", Array.Empty<object>());
				PluginLog.Debug("opener is no longer in progress", Array.Empty<object>());
				KirboMethods.Flag = true;
			}
			if (((GameObject)CustomRotation.Player).IsDead && !KirboMethods.Flag)
			{
				OpenerHasFailed = true;
				OpenerInProgress = false;
				Openerstep = 0;
				PluginLog.Warning("Opener Failed Reason: 'You died'", Array.Empty<object>());
				PluginLog.Debug($"openerstep is now: {Openerstep}", Array.Empty<object>());
				PluginLog.Debug("opener is no longer in progress", Array.Empty<object>());
				KirboMethods.Flag = true;
			}
			switch (((CustomRotation)this).Configs.GetCombo("RotationSelection"))
			{
			case 0:
				switch (Openerstep)
				{
				case 0:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.AirAnchor }), MCH_Base.AirAnchor.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 1:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 2:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 3:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill }), MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 4:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.BarrelStabilizer }), MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 5:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SplitShot }), MCH_Base.SplitShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 6:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 7:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SlugShot }), MCH_Base.SlugShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 8:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 9:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 10:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.CleanShot }), MCH_Base.CleanShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 11:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Reassemble }), MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 12:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Wildfire }), MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)17, (byte)0, (byte)0));
				case 13:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.ChainSaw }), MCH_Base.ChainSaw.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 14:
					return OpenerStep(CustomRotation.IsLastAbility(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.RookAutoturret }), MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 15:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Hypercharge }), MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)51, (byte)0, (byte)0));
				case 16:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 4, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 17:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 18:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 3, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 19:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 20:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 2, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 21:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 22:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 1, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 23:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 24:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 0, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 25:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 26:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill }), MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 27:
					OpenerHasFinished = true;
					OpenerInProgress = false;
					PluginLog.Debug("Succesfully completed Opener: 'Early AA'", Array.Empty<object>());
					break;
				}
				break;
			case 1:
				switch (Openerstep)
				{
				case 0:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SplitShot }), MCH_Base.SplitShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 1:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 2:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 3:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill }), MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 4:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.BarrelStabilizer }), MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 5:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SlugShot }), MCH_Base.SlugShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 6:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 7:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.CleanShot }), MCH_Base.CleanShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 8:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Reassemble }), MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 9:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 10:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.AirAnchor }), MCH_Base.AirAnchor.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 11:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Reassemble }), MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 12:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Wildfire }), MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)17, (byte)0, (byte)0));
				case 13:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.ChainSaw }), MCH_Base.ChainSaw.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 14:
					return OpenerStep(CustomRotation.IsLastAbility(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.RookAutoturret }), MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 15:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Hypercharge }), MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)51, (byte)0, (byte)0));
				case 16:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 4, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 17:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 18:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 3, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 19:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 20:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 2, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 21:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 22:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 1, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 23:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 24:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 0, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 25:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 26:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill }), MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 27:
					OpenerHasFinished = true;
					OpenerInProgress = false;
					PluginLog.Debug("Succesfully completed Opener: 'Delayed Tools'", Array.Empty<object>());
					break;
				}
				break;
			case 2:
				switch (Openerstep)
				{
				case 0:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SplitShot }), MCH_Base.SplitShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 1:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 2:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 3:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SlugShot }), MCH_Base.SlugShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 4:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.BarrelStabilizer }), MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 5:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.CleanShot }), MCH_Base.CleanShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 6:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.AirAnchor }), MCH_Base.AirAnchor.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 7:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Reassemble }), MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 8:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 9:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill }), MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 10:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Reassemble }), MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 11:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Wildfire }), MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)17, (byte)0, (byte)0));
				case 12:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.ChainSaw }), MCH_Base.ChainSaw.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 13:
					return OpenerStep(CustomRotation.IsLastAbility(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.RookAutoturret }), MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 14:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Hypercharge }), MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)51, (byte)0, (byte)0));
				case 15:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 4, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 16:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 17:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 3, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 18:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 19:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 2, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 20:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 21:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 1, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 22:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 23:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 0, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 24:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 25:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SplitShot }), MCH_Base.SplitShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 26:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 27:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 28:
					OpenerHasFinished = true;
					OpenerInProgress = false;
					PluginLog.Debug("Succesfully completed Opener: '123 Tools'", Array.Empty<object>());
					break;
				}
				break;
			case 3:
				switch (Openerstep)
				{
				case 0:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill }), MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 1:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 2:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 3:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.AirAnchor }), MCH_Base.AirAnchor.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 4:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.BarrelStabilizer }), MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 5:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Wildfire }), MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)17, (byte)0, (byte)0));
				case 6:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.ChainSaw }), MCH_Base.ChainSaw.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 7:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 8:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Hypercharge }), MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)17, (byte)0, (byte)0));
				case 9:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 4, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 10:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 11:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 3, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 12:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 13:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 2, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 14:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 15:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 1, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 16:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 17:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 0, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 18:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 19:
					OpenerHasFinished = true;
					OpenerInProgress = false;
					PluginLog.Debug("Succesfully completed Opener: 'Fast Wildfire'", Array.Empty<object>());
					break;
				}
				break;
			case 4:
				switch (Openerstep)
				{
				case 0:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.AirAnchor }), MCH_Base.AirAnchor.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 1:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.BarrelStabilizer }), MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 2:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 3:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill }), MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 4:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Reassemble }), MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 5:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 6:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.ChainSaw }), MCH_Base.ChainSaw.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 7:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 8:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 9:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SplitShot }), MCH_Base.SplitShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 10:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 11:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 12:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SlugShot }), MCH_Base.SlugShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 13:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Wildfire }), MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)17, (byte)0, (byte)0));
				case 14:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.CleanShot }), MCH_Base.CleanShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 15:
					return OpenerStep(CustomRotation.IsLastAbility(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.RookAutoturret }), MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 16:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Hypercharge }), MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)17, (byte)0, (byte)0));
				case 17:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 4, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 18:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 3, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 19:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 20:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 2, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 21:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 22:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 1, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 23:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 24:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 0, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 25:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 26:
					OpenerHasFinished = true;
					OpenerInProgress = false;
					PluginLog.Debug("Succesfully completed Opener: 'FFLogs Opener'", Array.Empty<object>());
					break;
				}
				break;
			case 5:
				switch (Openerstep)
				{
				case 0:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SplitShot }), MCH_Base.SplitShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 1:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 2:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 3:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill }), MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 4:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.BarrelStabilizer }), MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 5:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.SlugShot }), MCH_Base.SlugShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 6:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 7:
					return OpenerStep(CustomRotation.IsLastGCD(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.CleanShot }), MCH_Base.CleanShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 8:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 9:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Reassemble }), MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 10:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.AirAnchor }), MCH_Base.AirAnchor.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 11:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Reassemble }), MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0));
				case 12:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Wildfire }), MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)17, (byte)0, (byte)0));
				case 13:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.ChainSaw }), MCH_Base.ChainSaw.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 14:
					return OpenerStep(CustomRotation.IsLastAbility(true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.RookAutoturret }), MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 15:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Hypercharge }), MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)16, (byte)0, (byte)0));
				case 16:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 4, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 17:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 18:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 3, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 19:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 20:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 2, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 21:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.GaussRound }), MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 22:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 1, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 23:
					return OpenerStep(CustomRotation.IsLastAbility(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Ricochet }), MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0));
				case 24:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.HeatBlast }) && OverHeatStacks == 0, MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 25:
					return OpenerStep(CustomRotation.IsLastGCD(false, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill }), MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0));
				case 26:
					OpenerHasFinished = true;
					OpenerInProgress = false;
					PluginLog.Debug("Succesfully completed Opener: 'Test'", Array.Empty<object>());
					break;
				}
				break;
			}
		}
		act = null;
		return false;
	}

	private bool OpenerStep(bool condition, bool result)
	{
		if (condition)
		{
			Openerstep++;
			return false;
		}
		return result;
	}

	protected override bool GeneralGCD(out IAction act)
	{
		BattleChara t = CustomRotation.Target;
		if (OpenerInProgress)
		{
			return Opener(out act);
		}
		if (!OpenerInProgress || OpenerHasFailed || OpenerHasFinished)
		{
			if (MCH_Base.AutoCrossbow.CanUse(ref act, (CanUseOption)1, (byte)3, (byte)0) && ObjectHelper.DistanceToPlayer((GameObject)(object)t) <= 12f)
			{
				return true;
			}
			if (MCH_Base.HeatBlast.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if ((double)ObjectHelper.GetHealthRatio(t) > 0.6 && MCH_Base.BioBlaster.CanUse(ref act, (CanUseOption)1, (byte)3, (byte)0) && ObjectHelper.DistanceToPlayer((GameObject)(object)t) <= 12f)
			{
				return true;
			}
			if (MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (MCH_Base.AirAnchor.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (!((IEnoughLevel)MCH_Base.AirAnchor).EnoughLevel && MCH_Base.HotShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (MCH_Base.ChainSaw.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (MCH_Base.SpreadShot.CanUse(ref act, (CanUseOption)1, (byte)3, (byte)0))
			{
				return true;
			}
			if (MCH_Base.CleanShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (MCH_Base.SlugShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (MCH_Base.SplitShot.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
		}
		act = null;
		return false;
	}

	private bool DelayedTools(IAction nextGCD, out IAction act)
	{
		if (OpenerInProgress)
		{
			return Opener(out act);
		}
		act = null;
		return false;
	}

	protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		BattleChara t = CustomRotation.Target;
		PlayerCharacter p = CustomRotation.Player;
		TerritoryContentType Content = CustomRotation.TerritoryContentType;
		bool Dungeon = (int)Content == 2;
		bool Roulette = (int)Content == 1;
		bool Deepdungeon = (int)Content == 21;
		bool VCDungeonFinder = (int)Content == 30;
		bool FATEs = (int)Content == 8;
		bool Eureka = (int)Content == 26;
		bool None = (int)Content == 0;
		bool Ultimate = (int)Content == 28;
		bool usetinc = ((CustomRotation)this).UseBurstMedicine(ref act, false);
		if ((!((IAction)CustomRotation.TinctureOfDexterity8).IsCoolingDown || !((IAction)CustomRotation.TinctureOfDexterity7).IsCoolingDown || !((IAction)CustomRotation.TinctureOfDexterity6).IsCoolingDown) && !StatusHelper.HasStatus((BattleChara)(object)CustomRotation.Player, false, (StatusID[])(object)new StatusID[1] { (StatusID)1946 }) && MCH_Base.Wildfire.WillHaveOneCharge(20f) && !InBurst && ((CustomRotation)this).UseBurstMedicine(ref act, true))
		{
			return true;
		}
		if (OpenerInProgress && !OpenerHasFailed && !OpenerHasFinished)
		{
			return Opener(out act);
		}
		if (((CustomRotation)this).Configs.GetBool("BatteryStuck") && !OpenerInProgress && MCH_Base.Battery == 100 && MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0) && (nextGCD == MCH_Base.ChainSaw || nextGCD == MCH_Base.AirAnchor || nextGCD == MCH_Base.CleanShot))
		{
			return true;
		}
		if (((CustomRotation)this).Configs.GetBool("HeatStuck") && !OpenerInProgress && MCH_Base.Heat == 100 && MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0) && (nextGCD == MCH_Base.SplitShot || nextGCD == MCH_Base.SlugShot || nextGCD == MCH_Base.CleanShot))
		{
			return true;
		}
		if (((CustomRotation)this).Configs.GetBool("DumpSkills") && IsDying && CustomRotation.IsTargetBoss)
		{
			if (!StatusHelper.HasStatus((BattleChara)(object)CustomRotation.Player, false, (StatusID[])(object)new StatusID[1] { (StatusID)851 }) && MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)2, (byte)0, (byte)0) && MCH_Base.Reassemble.CurrentCharges > 0 && (nextGCD == MCH_Base.ChainSaw || nextGCD == MCH_Base.AirAnchor || nextGCD == MCH_Base.Drill))
			{
				return true;
			}
			if (MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (MCH_Base.AirAnchor.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (MCH_Base.ChainSaw.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (MCH_Base.Drill.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0))
			{
				return true;
			}
			if (MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)1, (byte)0, (byte)0) && MCH_Base.Battery >= 50)
			{
				return true;
			}
			if (MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && !WillhaveTool && MCH_Base.Heat >= 50)
			{
				return true;
			}
			if ((double)ObjectHelper.GetHealthRatio(t) < 0.03 && nextGCD == MCH_Base.CleanShot && MCH_Base.Reassemble.CurrentCharges > 0 && MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)35, (byte)0, (byte)0))
			{
				return true;
			}
			if ((double)ObjectHelper.GetHealthRatio(t) < 0.03 && MCH_Base.RookAutoturret.ElapsedAfter(5f) && MCH_Base.QueenOverdrive.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
			{
				return true;
			}
			if ((double)ObjectHelper.GetHealthRatio(t) < 0.02 && (StatusHelper.HasStatus((BattleChara)(object)p, false, (StatusID[])(object)new StatusID[1] { (StatusID)1946 }) || InBurst) && MCH_Base.Wildfire.ElapsedAfter(5f) && MCH_Base.Detonator.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
			{
				return true;
			}
		}
		if ((!OpenerInProgress || OpenerHasFailed || OpenerHasFinished) && ((Character)CustomRotation.Player).Level >= 90)
		{
			switch (((CustomRotation)this).Configs.GetCombo("RotationSelection"))
			{
			case 0:
				if ((nextGCD == MCH_Base.ChainSaw || CustomRotation.IsLastGCD((ActionID[])(object)new ActionID[1] { (ActionID)25788 }) || !((IAction)MCH_Base.Wildfire).IsCoolingDown) && MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)16, (byte)0, (byte)0))
				{
					return true;
				}
				if (MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
				{
					return true;
				}
				if (MCH_Base.Reassemble.CurrentCharges > 0)
				{
					if (MCH_Base.Reassemble.CurrentCharges == 1)
					{
						if (nextGCD == MCH_Base.ChainSaw && MCH_Base.Wildfire.ElapsedAfter(65f) && MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)2, (byte)0, (byte)0))
						{
							return false;
						}
						if ((nextGCD == MCH_Base.ChainSaw || nextGCD == MCH_Base.Drill || nextGCD == MCH_Base.AirAnchor) && !MCH_Base.Wildfire.ElapsedAfter(65f) && MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)2, (byte)0, (byte)0))
						{
							return true;
						}
					}
					if (MCH_Base.Reassemble.CurrentCharges == 1 && MCH_Base.Reassemble.WillHaveOneCharge(55f) && (nextGCD == MCH_Base.ChainSaw || nextGCD == MCH_Base.Drill || nextGCD == MCH_Base.AirAnchor) && MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)2, (byte)0, (byte)0))
					{
						return true;
					}
				}
				if (CustomRotation.InCombat && ((GameObject)t).IsTargetable)
				{
					if (!MCH_Base.Wildfire.ElapsedAfter(60f) && MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
					{
						return true;
					}
					if ((MCH_Base.Wildfire.ElapsedAfter(60f) || !((IAction)MCH_Base.Wildfire).IsInCooldown) && ((MCH_Base.Battery >= 80 && (nextGCD == MCH_Base.AirAnchor || nextGCD == MCH_Base.ChainSaw)) || (MCH_Base.Battery == 100 && (nextGCD == MCH_Base.CleanShot || nextGCD == MCH_Base.AirAnchor || nextGCD == MCH_Base.ChainSaw))) && MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
					{
						return true;
					}
				}
				if (!WillhaveTool)
				{
					if (!MCH_Base.Wildfire.ElapsedAfter(80f) && MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
					{
						return true;
					}
					if ((MCH_Base.Wildfire.ElapsedAfter(80f) || !((IAction)MCH_Base.Wildfire).IsInCooldown) && MCH_Base.Heat == 100 && (nextGCD == MCH_Base.SplitShot || nextGCD == MCH_Base.SlugShot || nextGCD == MCH_Base.CleanShot) && MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
					{
						return true;
					}
				}
				break;
			case 1:
				if (MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)16, (byte)0, (byte)0) && nextGCD == MCH_Base.ChainSaw && MCH_Base.Heat >= 50)
				{
					return true;
				}
				if (MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0))
				{
					if (((IAction)MCH_Base.Wildfire).IsCoolingDown && CustomRotation.IsLastGCD((ActionID[])(object)new ActionID[1] { (ActionID)16498 }))
					{
						return true;
					}
					return true;
				}
				if (MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0) && !StatusHelper.HasStatus((BattleChara)(object)p, true, (StatusID[])(object)new StatusID[1] { (StatusID)851 }))
				{
					if (IActionHelper.IsTheSameTo(nextGCD, true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.ChainSaw }))
					{
						return true;
					}
					if ((IActionHelper.IsTheSameTo(nextGCD, true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.AirAnchor }) || IActionHelper.IsTheSameTo(nextGCD, true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.Drill })) && !MCH_Base.Wildfire.WillHaveOneCharge(55f))
					{
						return true;
					}
				}
				if (MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)16, (byte)0, (byte)0) && ((GameObject)t).IsTargetable && CustomRotation.InCombat)
				{
					if (CustomRotation.CombatElapsedLess(60f) && !CustomRotation.CombatElapsedLess(45f) && MCH_Base.Battery >= 50)
					{
						return true;
					}
					if (((IAction)MCH_Base.Wildfire).IsCoolingDown && MCH_Base.Wildfire.ElapsedAfter(105f) && MCH_Base.Battery == 100 && (nextGCD == MCH_Base.AirAnchor || nextGCD == MCH_Base.CleanShot))
					{
						return true;
					}
					if (MCH_Base.Battery >= 90 && !MCH_Base.Wildfire.ElapsedAfter(70f))
					{
						return true;
					}
					if (MCH_Base.Battery >= 80 && !MCH_Base.Wildfire.ElapsedAfter(77.5f) && CustomRotation.IsLastGCD((ActionID[])(object)new ActionID[1] { (ActionID)16500 }))
					{
						return true;
					}
				}
				if (MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && !WillhaveTool)
				{
					if (InBurst && CustomRotation.IsLastGCD((ActionID[])(object)new ActionID[1] { (ActionID)25788 }))
					{
						return true;
					}
					if (MCH_Base.Heat >= 100 && MCH_Base.Wildfire.WillHaveOneCharge(10f))
					{
						return true;
					}
					if (MCH_Base.Heat >= 90 && MCH_Base.Wildfire.WillHaveOneCharge(40f))
					{
						return true;
					}
					if (MCH_Base.Heat >= 50 && !MCH_Base.Wildfire.WillHaveOneCharge(30f))
					{
						return true;
					}
				}
				break;
			case 2:
				if (MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)18, (byte)0, (byte)0) && ((nextGCD == MCH_Base.ChainSaw && MCH_Base.Heat >= 50) || (CustomRotation.IsLastGCD((ActionID[])(object)new ActionID[1] { (ActionID)25788 }) && MCH_Base.Heat >= 50)))
				{
					return true;
				}
				if (MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
				{
					return true;
				}
				if (MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)2, (byte)0, (byte)0) && ((IAction)MCH_Base.AirAnchor).IsCoolingDown && nextGCD == MCH_Base.ChainSaw)
				{
					return true;
				}
				if (!MCH_Base.Wildfire.ElapsedAfter(70f) && MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)2, (byte)0, (byte)0) && (nextGCD == MCH_Base.Drill || nextGCD == MCH_Base.AirAnchor))
				{
					return true;
				}
				if (MCH_Base.Wildfire.ElapsedAfter(110f) && MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)2, (byte)0, (byte)0) && nextGCD == MCH_Base.ChainSaw)
				{
					return true;
				}
				if (((GameObject)t).IsTargetable && CustomRotation.InCombat && ((IEnoughLevel)MCH_Base.RookAutoturret).EnoughLevel && MCH_Base.Battery >= 50)
				{
					if (CustomRotation.CombatElapsedLess(61f) && !CustomRotation.CombatElapsedLess(31f) && MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
					{
						return true;
					}
					if (MCH_Base.Wildfire.ElapsedAfter(110f) && (MCH_Base.Battery == 100 || (MCH_Base.Battery >= 70 && nextGCD == MCH_Base.ChainSaw) || (MCH_Base.Battery >= 70 && nextGCD == MCH_Base.AirAnchor)) && MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
					{
						return true;
					}
					if (MCH_Base.Battery >= 80 && !MCH_Base.Wildfire.ElapsedAfter(70f) && MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
					{
						return true;
					}
				}
				if (MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && !WillhaveTool && !MCH_Base.Wildfire.ElapsedAfter(90f) && ((IAction)MCH_Base.Wildfire).IsCoolingDown)
				{
					return true;
				}
				break;
			case 3:
				if (MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)18, (byte)0, (byte)0) && (nextGCD == MCH_Base.ChainSaw || CustomRotation.IsLastGCD((ActionID[])(object)new ActionID[1] { (ActionID)25788 })))
				{
					return true;
				}
				if (MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
				{
					return true;
				}
				if (MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)2, (byte)0, (byte)0) && ((IAction)MCH_Base.AirAnchor).IsCoolingDown && (nextGCD == MCH_Base.ChainSaw || nextGCD == MCH_Base.AirAnchor))
				{
					return true;
				}
				if (((GameObject)t).IsTargetable && CustomRotation.InCombat && MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && MCH_Base.Battery >= 50)
				{
					if (!CustomRotation.CombatElapsedLess(31f) && CustomRotation.CombatElapsedLess(80f) && MCH_Base.Battery >= 60)
					{
						return true;
					}
					if (MCH_Base.Wildfire.ElapsedAfter(115f) && MCH_Base.Battery >= 50)
					{
						return true;
					}
					if (MCH_Base.Battery >= 50 && !MCH_Base.Wildfire.ElapsedAfter(80f))
					{
						return true;
					}
				}
				if (MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && !WillhaveTool && !MCH_Base.Wildfire.ElapsedAfter(90f) && ((IAction)MCH_Base.Wildfire).IsCoolingDown)
				{
					return true;
				}
				break;
			case 4:
				if (MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && ((GameObject)t).IsTargetable && CustomRotation.InCombat && MCH_Base.Battery >= 50)
				{
					return true;
				}
				if (MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)16, (byte)0, (byte)0) && CustomRotation.IsLastAbility((ActionID[])(object)new ActionID[1] { (ActionID)17209 }))
				{
					return true;
				}
				if (!WillhaveTool && MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
				{
					if (StatusHelper.HasStatus((BattleChara)(object)CustomRotation.Player, false, (StatusID[])(object)new StatusID[1] { (StatusID)1946 }) && MCH_Base.Heat >= 50)
					{
						return true;
					}
					if (MCH_Base.Heat < 100 && !InBurst && MCH_Base.Wildfire.WillHaveOneCharge(30f) && !MCH_Base.BarrelStabilizer.WillHaveOneCharge(25f))
					{
						return false;
					}
					if (InBurst && CustomRotation.IsLastGCD((ActionID[])(object)new ActionID[1] { (ActionID)25788 }))
					{
						return true;
					}
					if (MCH_Base.Heat >= 100 && MCH_Base.Wildfire.WillHaveOneCharge(10f))
					{
						return true;
					}
					if (MCH_Base.Heat >= 90 && MCH_Base.Wildfire.WillHaveOneCharge(40f))
					{
						return true;
					}
					if (MCH_Base.Heat >= 50 && !MCH_Base.Wildfire.WillHaveOneCharge(20f))
					{
						return true;
					}
				}
				if ((!((IAction)MCH_Base.BarrelStabilizer).IsCoolingDown && MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && MCH_Base.Wildfire.WillHaveOneCharge(10f)) || InBurst || MCH_Base.IsOverheated || StatusHelper.HasStatus((BattleChara)(object)p, false, (StatusID[])(object)new StatusID[1] { (StatusID)1946 }))
				{
					return true;
				}
				if (MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0) && !StatusHelper.HasStatus((BattleChara)(object)p, true, (StatusID[])(object)new StatusID[1] { (StatusID)851 }))
				{
					if (MCH_Base.Battery >= 90 && nextGCD == MCH_Base.ChainSaw && MCH_Base.Wildfire.WillHaveOneCharge(5f))
					{
						return false;
					}
					if (IActionHelper.IsTheSameTo(nextGCD, true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.ChainSaw }))
					{
						return true;
					}
					if (MCH_Base.Reassemble.CurrentCharges == 2 && (nextGCD == MCH_Base.Drill || nextGCD == MCH_Base.AirAnchor || nextGCD == MCH_Base.ChainSaw))
					{
						return true;
					}
				}
				break;
			case 5:
				if (MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && ((GameObject)t).IsTargetable && CustomRotation.InCombat)
				{
					if (CustomRotation.CombatElapsedLess(82f) && !CustomRotation.CombatElapsedLess(60f) && MCH_Base.Battery == 90)
					{
						return true;
					}
					if ((StatusHelper.HasStatus((BattleChara)(object)p, false, (StatusID[])(object)new StatusID[1] { (StatusID)1946 }) && MCH_Base.Battery >= 70 && CustomRotation.IsLastGCD((ActionID[])(object)new ActionID[1] { (ActionID)25788 })) || (MCH_Base.Wildfire.WillHaveOneCharge(5f) && MCH_Base.Battery >= 90 && nextGCD == MCH_Base.ChainSaw))
					{
						return true;
					}
					if ((MCH_Base.Battery == 80 || MCH_Base.Battery == 100) && !MCH_Base.Wildfire.WillHaveOneCharge(30f) && !CustomRotation.CombatElapsedLess(85f))
					{
						return true;
					}
				}
				if (MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)16, (byte)0, (byte)0))
				{
					if (nextGCD == MCH_Base.ChainSaw)
					{
						return true;
					}
					if (MCH_Base.Heat >= 50 && !MCH_Base.ChainSaw.WillHaveOneCharge(10f))
					{
						return true;
					}
				}
				if (!WillhaveTool && MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0))
				{
					if (StatusHelper.HasStatus((BattleChara)(object)CustomRotation.Player, false, (StatusID[])(object)new StatusID[1] { (StatusID)1946 }) && MCH_Base.Heat >= 50)
					{
						return true;
					}
					if (MCH_Base.Heat < 100 && !InBurst && MCH_Base.Wildfire.WillHaveOneCharge(30f) && !MCH_Base.BarrelStabilizer.WillHaveOneCharge(25f))
					{
						return false;
					}
					if (InBurst && CustomRotation.IsLastGCD((ActionID[])(object)new ActionID[1] { (ActionID)25788 }))
					{
						return true;
					}
					if (MCH_Base.Heat >= 100 && MCH_Base.Wildfire.WillHaveOneCharge(10f))
					{
						return true;
					}
					if (MCH_Base.Heat >= 90 && MCH_Base.Wildfire.WillHaveOneCharge(40f))
					{
						return true;
					}
					if (MCH_Base.Heat >= 50 && !MCH_Base.Wildfire.WillHaveOneCharge(20f))
					{
						return true;
					}
				}
				if (MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && (MCH_Base.Wildfire.WillHaveOneCharge(10f) || InBurst || MCH_Base.IsOverheated))
				{
					return true;
				}
				if (MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0) && !StatusHelper.HasStatus((BattleChara)(object)p, true, (StatusID[])(object)new StatusID[1] { (StatusID)851 }))
				{
					if (MCH_Base.Battery >= 90 && nextGCD == MCH_Base.ChainSaw && MCH_Base.Wildfire.WillHaveOneCharge(5f))
					{
						return false;
					}
					if (IActionHelper.IsTheSameTo(nextGCD, true, (IAction[])(object)new IAction[1] { (IAction)MCH_Base.ChainSaw }) && MCH_Base.Battery != 0)
					{
						return true;
					}
					if (MCH_Base.Reassemble.CurrentCharges == 2 && (nextGCD == MCH_Base.Drill || nextGCD == MCH_Base.AirAnchor || nextGCD == MCH_Base.ChainSaw))
					{
						return true;
					}
				}
				break;
			}
		}
		if (Deepdungeon || Eureka || Roulette || Dungeon || VCDungeonFinder || FATEs || ((Character)CustomRotation.Player).Level < 90)
		{
			if (MCH_Base.Wildfire.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && CustomRotation.IsTargetBoss && SafeToUseWildfire)
			{
				return true;
			}
			if (MCH_Base.Reassemble.CurrentCharges > 0 && MCH_Base.Reassemble.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0))
			{
				if (((IEnoughLevel)MCH_Base.ChainSaw).EnoughLevel && (nextGCD == MCH_Base.ChainSaw || nextGCD == MCH_Base.Drill || nextGCD == MCH_Base.AirAnchor))
				{
					return true;
				}
				if (!((IEnoughLevel)MCH_Base.Drill).EnoughLevel && nextGCD == MCH_Base.CleanShot)
				{
					return true;
				}
			}
			if (MCH_Base.BarrelStabilizer.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && ((GameObject)t).IsTargetable && CustomRotation.InCombat)
			{
				return true;
			}
			if (MCH_Base.Hypercharge.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && CustomRotation.InCombat && ((GameObject)t).IsTargetable)
			{
				if ((double)ObjectHelper.GetHealthRatio(t) > 0.25)
				{
					return true;
				}
				if (CustomRotation.IsTargetBoss)
				{
					return true;
				}
			}
			if (MCH_Base.RookAutoturret.CanUse(ref act, (CanUseOption)0, (byte)0, (byte)0) && ((GameObject)t).IsTargetable && CustomRotation.InCombat)
			{
				if (!CustomRotation.IsTargetBoss && CustomRotation.CombatElapsedLess(30f))
				{
					return true;
				}
				if (CustomRotation.IsTargetBoss)
				{
					return true;
				}
			}
		}
		act = null;
		return false;
	}

	protected override bool AttackAbility(out IAction act)
	{
		if (OpenerInProgress)
		{
			return Opener(out act);
		}
		if (MCH_Base.GaussRound.CurrentCharges >= MCH_Base.Ricochet.CurrentCharges)
		{
			if (MCH_Base.GaussRound.CanUse(ref act, (CanUseOption)3, (byte)0, (byte)0))
			{
				return true;
			}
		}
		else if (MCH_Base.Ricochet.CanUse(ref act, (CanUseOption)19, (byte)0, (byte)0))
		{
			return true;
		}
		act = null;
		return false;
	}

	protected override void UpdateInfo()
	{
		HandleOpenerAvailability();
		ToolKitCheck();
		StateOfOpener();
	}

	private void ToolKitCheck()
	{
		bool WillHaveDrill = MCH_Base.Drill.WillHaveOneCharge(5f);
		bool WillHaveAirAnchor = MCH_Base.AirAnchor.WillHaveOneCharge(5f);
		bool WillHaveChainSaw = MCH_Base.ChainSaw.WillHaveOneCharge(5f);
		if (((Character)CustomRotation.Player).Level >= 90)
		{
			WillhaveTool = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;
		}
		SafeToUseHypercharge = !WillhaveTool;
		InBurst = StatusHelper.HasStatus((BattleChara)(object)CustomRotation.Player, false, (StatusID[])(object)new StatusID[1] { (StatusID)1946 });
		IsDying = (double)ObjectHelper.GetHealthRatio(CustomRotation.Target) < 0.05;
	}

	public void StateOfOpener()
	{
		if (((GameObject)CustomRotation.Player).IsDead)
		{
			OpenerHasFailed = false;
			OpenerHasFinished = false;
			Openerstep = 0;
		}
		if (!CustomRotation.InCombat)
		{
			OpenerHasFailed = false;
			OpenerHasFinished = false;
			Openerstep = 0;
		}
	}

	public override void OnTerritoryChanged()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Invalid comparison between Unknown and I4
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Invalid comparison between Unknown and I4
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Invalid comparison between Unknown and I4
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Invalid comparison between Unknown and I4
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Invalid comparison between Unknown and I4
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Invalid comparison between Unknown and I4
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Invalid comparison between Unknown and I4
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Invalid comparison between Unknown and I4
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Invalid comparison between Unknown and I4
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Invalid comparison between Unknown and I4
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Invalid comparison between Unknown and I4
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		if ((int)CustomRotation.TerritoryContentType == 0)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 2)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 3)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 4)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 5)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 5 && CustomRotation.IsInHighEndDuty)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 7)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 8)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 9)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 10)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 13)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 21)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 26)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 28)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
		if ((int)CustomRotation.TerritoryContentType == 30)
		{
			PluginLog.Debug($"Current Content is {CustomRotation.TerritoryContentType}", Array.Empty<object>());
		}
	}

	public void HandleOpenerAvailability()
	{
		bool Lvl90 = ((Character)CustomRotation.Player).Level >= 90;
		bool HasChainSaw = !((IAction)MCH_Base.ChainSaw).IsCoolingDown;
		bool HasBarrelStabilizer = !((IAction)MCH_Base.BarrelStabilizer).IsCoolingDown;
		bool HasRicochet = MCH_Base.Ricochet.CurrentCharges == 3;
		bool HasWildfire = !((IAction)MCH_Base.Wildfire).IsCoolingDown;
		bool HasGaussRound = MCH_Base.GaussRound.CurrentCharges == 3;
		bool ReassembleOneCharge = MCH_Base.Reassemble.CurrentCharges >= 1;
		bool NoHeat = MCH_Base.Heat == 0;
		bool NoBattery = MCH_Base.Battery == 0;
		bool Openerstep0 = Openerstep == 0;
		OpenerActionsAvailable = ReassembleOneCharge && HasChainSaw && HasBarrelStabilizer && HasRicochet && HasWildfire && HasGaussRound && Lvl90 && NoBattery && NoHeat && Openerstep0;
	}

	public override void DisplayStatus()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		BattleChara t = CustomRotation.Target;
		PlayerCharacter p = CustomRotation.Player;
		float TargetHPP = ObjectHelper.GetHealthRatio(t);
		int partysize = CustomRotation.PartyMembers.Count();
		TerritoryContentType content = CustomRotation.TerritoryContentType;
		string Opener_Available = (OpenerActionsAvailable ? "Available" : "Unavailable");
		string CanUseTincture = (((IAction)CustomRotation.TinctureOfDexterity8).IsCoolingDown ? "False" : "True");
		Vector4 White = new Vector4(1f, 1f, 1f, 1f);
		Vector4 yellow = new Vector4(1f, 1f, 0f, 1f);
		Vector4 Red = new Vector4(1f, 0f, 0f, 1f);
		Vector4 Green = new Vector4(0f, 1f, 0f, 1f);
		Vector4 Blue = new Vector4(0f, 0.5882353f, 40f / 51f, 1f);
		Vector4 Purple = new Vector4(0.5882353f, 0f, 1f, 1f);
		Vector4 Orange = new Vector4(49f / 51f, 0.5921569f, 13f / 85f, 1f);
		ImGui.BeginChild("Menu", new Vector2(650f, 350f), false);
		ImGui.BeginTabBar("tabBar");
		if (ImGui.BeginTabItem("Rotation Info"))
		{
			if (display_rotationInfo && ImGui.BeginTable("RotationInfo", 2, (ImGuiTableFlags)1928))
			{
				ImGui.Indent();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Name of Rotation: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("The title of the rotation name");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, ((CustomRotation)this).RotationName ?? "");
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Current Job: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("Your current Job/class");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, ((CustomRotation)this).Name ?? "");
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Game Version: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("The game version the rotation is for.");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, ((CustomRotation)this).GameVersion ?? "");
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Ping/RTT: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("Shows your current ping to the servers.\n1 second of ping is equal to 1.000ms\nBelow 0.080ms = Green\nAbove 0.080ms = Yellow\nAbove 0.200ms = Red");
				}
				ImGui.TableNextColumn();
				if ((double)CustomRotation.Ping < 0.08)
				{
					ImGui.TextColored(Green, CustomRotation.Ping * 1000f + "ms");
				}
				else if ((double)CustomRotation.Ping > 0.08)
				{
					ImGui.TextColored(yellow, CustomRotation.Ping + " ms");
				}
				else if ((double)CustomRotation.Ping > 0.2)
				{
					ImGui.TextColored(Red, CustomRotation.Ping + " ms");
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Opener Selection: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("What opener is selected.");
				}
				switch (((CustomRotation)this).Configs.GetCombo("RotationSelection"))
				{
				case 0:
					ImGui.TableNextColumn();
					ImGui.TextColored(White, "Early AA");
					break;
				case 1:
					ImGui.TableNextColumn();
					ImGui.TextColored(White, "Delayed Tools");
					break;
				case 2:
					ImGui.TableNextColumn();
					ImGui.TextColored(White, "123 Tools");
					break;
				case 3:
					ImGui.TableNextColumn();
					ImGui.TextColored(White, "Fast Wildfire");
					break;
				case 4:
					ImGui.TableNextColumn();
					ImGui.TextColored(White, "FFlogs Opener");
					break;
				case 5:
					ImGui.TableNextColumn();
					ImGui.TextColored(White, "Test");
					break;
				}
				ImGui.Unindent();
				ImGui.EndTable();
			}
			ImGui.EndTabItem();
		}
		if (ImGui.BeginTabItem("Combat Info"))
		{
			if (display_combat && ImGui.BeginTable("CombatInfo", 2, (ImGuiTableFlags)67464))
			{
				ImGui.Indent();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Target name: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("Target's name");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, ((object)((GameObject)CustomRotation.Target).Name)?.ToString() ?? "");
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Target's Health: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("Target's Health Percentage based\n1 means 100%");
				}
				ImGui.TableNextColumn();
				if ((double)TargetHPP > 0.6)
				{
					ImGui.TextColored(Green, $"{TargetHPP * 100f}%%");
				}
				if ((double)TargetHPP < 0.6 && (double)TargetHPP > 0.05)
				{
					ImGui.TextColored(yellow, $"{TargetHPP * 100f}%%");
				}
				if ((double)TargetHPP < 0.05 && (double)TargetHPP > 0.03)
				{
					ImGui.TextColored(Orange, $"{TargetHPP * 100f}%%");
				}
				if ((double)TargetHPP < 0.03)
				{
					ImGui.TextColored(Red, $"{TargetHPP * 100f}%%");
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "IsInHighEndDuty:  ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("quick check for if current content is 'highend'.");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, CustomRotation.IsInHighEndDuty.ToString());
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "is boss?:  ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("quick check for if current content is 'highend'.");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, CustomRotation.IsTargetBoss.ToString());
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "is dying?:  ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("quick check for if current content is 'highend'.");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, CustomRotation.IsTargetDying.ToString());
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Current Conten: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("The current content type you're doing");
				}
				ImGui.TableNextColumn();
				if (CustomRotation.IsInHighEndDuty)
				{
					ImGui.TextColored(Red, "Is High End!");
				}
				else
				{
					ImGui.TextColored(White, ((object)(TerritoryContentType)(ref content)).ToString());
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Party Size: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("How many people are in your party right now.");
				}
				ImGui.TableNextColumn();
				if (partysize == 1)
				{
					ImGui.TextColored(Blue, "Solo");
				}
				else
				{
					ImGui.TextColored(White, partysize.ToString());
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Can use tincture: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("It's checking if the Grade 8 DEX tincture is off cooldown");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, CanUseTincture ?? "");
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Are we In Burst: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("This is set to Wildfire, If Wildfire buff is up, it means we're in burst!");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, InBurst.ToString() ?? "");
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Members In 2mins Burst");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("Shows how many people currently have a party buff up\nIf playing Solo this will be -1");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, CustomRotation.RatioOfMembersIn2minsBurst.ToString() ?? "");
				ImGui.TableNextRow();
				ImGui.Unindent();
				ImGui.EndTable();
			}
			ImGui.EndTabItem();
		}
		if (ImGui.BeginTabItem("Action Info"))
		{
			if (display_action && ImGui.BeginTable("ActionInfo", 2, (ImGuiTableFlags)67464))
			{
				if (display_headers)
				{
					ImGui.TableSetupColumn("Setting");
					ImGui.TableSetupColumn("Status");
					ImGui.TableHeadersRow();
				}
				ImGui.Indent();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Safe To Use Hypercharge: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("Is a method with various checks to determine\nwether or not Hypercharge can be used");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, SafeToUseHypercharge.ToString());
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "Time since last action: ");
				ImGui.TableNextColumn();
				if (CustomRotation.InCombat)
				{
					ImGui.TextColored(White, $"{(int)CustomRotation.TimeSinceLastAction.TotalSeconds}S");
				}
				else
				{
					ImGui.TextColored(White, "N/A");
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextColored(White, "OverHeat Stacks: ");
				if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
				{
					ImGui.SetTooltip("How many overheat stacks left");
				}
				ImGui.TableNextColumn();
				ImGui.TextColored(White, StatusHelper.StatusStack((BattleChara)(object)CustomRotation.Player, true, (StatusID[])(object)new StatusID[1] { (StatusID)2688 }).ToString());
				if (!CustomRotation.InCombat)
				{
					ImGui.TableNextColumn();
					ImGui.TextColored(White, "Opener Skills: ");
					if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
					{
						ImGui.SetTooltip("Check to see if the skills needed for the opener are available to use");
					}
					ImGui.TableNextColumn();
					if (OpenerActionsAvailable)
					{
						ImGui.TextColored(Green, Opener_Available);
					}
					else if (!OpenerActionsAvailable && !OpenerInProgress)
					{
						ImGui.TextColored(Red, Opener_Available);
					}
				}
				if (CustomRotation.InCombat)
				{
					if (!OpenerInProgress && OpenerHasFinished)
					{
						ImGui.TableNextColumn();
						ImGui.TextColored(White, "Opener Status: ");
						if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
						{
							ImGui.SetTooltip("If the opener finished or not");
						}
						ImGui.TableNextColumn();
						ImGui.TextColored(Green, "Opener Finished");
					}
					else if (OpenerInProgress && (!OpenerHasFinished || !OpenerHasFailed))
					{
						ImGui.TableNextColumn();
						ImGui.TextColored(White, "Opener Status: ");
						if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
						{
							ImGui.SetTooltip("If the opener finished or not");
						}
						ImGui.TableNextColumn();
						ImGui.TextColored(yellow, $"Opener in progress - Step: {Openerstep}");
					}
					else if (!OpenerInProgress && OpenerHasFailed)
					{
						ImGui.TableNextColumn();
						ImGui.TextColored(White, "Opener Status: ");
						if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
						{
							ImGui.SetTooltip("If the opener finished or not");
						}
						ImGui.TableNextColumn();
						ImGui.TextColored(Red, "Opener failed");
					}
				}
				ImGui.Unindent();
				ImGui.EndTable();
			}
			ImGui.EndTabItem();
		}
		ImGui.EndTabBar();
		ImGui.EndChild();
		ImGui.Separator();
		if (ImGui.Button("Reset"))
		{
			PluginLog.Debug($"InCombat = {CustomRotation.InCombat}", Array.Empty<object>());
			OpenerInProgress = false;
			PluginLog.Debug($"set 'OpenerInProgress' to {OpenerInProgress}", Array.Empty<object>());
			Openerstep = 0;
			PluginLog.Debug($"set 'Openerstep' to {Openerstep}", Array.Empty<object>());
			OpenerHasFailed = false;
			PluginLog.Debug($"set 'OpenerHasFailed' to {OpenerHasFailed}", Array.Empty<object>());
			OpenerHasFinished = false;
			PluginLog.Debug($"set 'OpenerHasFinished' to {OpenerHasFinished}", Array.Empty<object>());
		}
		if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
		{
			ImGui.SetTooltip("Use in case Opener gets stuck.");
		}
		ImGui.SameLine();
		ImGui.Checkbox("Rotation", ref display_rotationInfo);
		if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
		{
			ImGui.SetTooltip("Displays Rotation related info.");
		}
		ImGui.SameLine();
		ImGui.Checkbox("Combat", ref display_combat);
		if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
		{
			ImGui.SetTooltip("Displays Combat related info.");
		}
		ImGui.SameLine();
		ImGui.Checkbox("Action", ref display_action);
		if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
		{
			ImGui.SetTooltip("Displays Action related info.");
		}
		ImGui.SameLine();
		ImGui.Checkbox("Header", ref display_headers);
		if (ImGui.IsItemHovered((ImGuiHoveredFlags)0))
		{
			ImGui.SetTooltip("Test value\nJust displays\nSome headers\n.");
		}
	}
}
