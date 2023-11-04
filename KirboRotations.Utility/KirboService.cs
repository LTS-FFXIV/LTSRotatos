using System;
using System.IO;
using System.Reflection;
using Dalamud.Configuration;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace KirboRotations.Utility;

internal class KirboService
{
	[PluginService]
	internal static BuddyList BuddyList { get; private set; } = null;


	[PluginService]
	internal static ChatGui ChatGui { get; private set; } = null;


	internal static bool ClassLocked { get; set; } = true;


	[PluginService]
	internal static ClientState ClientState { get; private set; } = null;


	[PluginService]
	internal static CommandManager CommandManager { get; private set; } = null;


	[PluginService]
	internal static Condition Condition { get; private set; } = null;


	internal static PluginConfigurations Configuration { get; set; } = null;


	[PluginService]
	internal static DataManager DataManager { get; private set; } = null;


	[PluginService]
	internal static Framework Framework { get; private set; } = null;


	[PluginService]
	internal static GameGui GameGui { get; private set; } = null;


	[PluginService]
	internal static DalamudPluginInterface Interface { get; private set; } = null;


	[PluginService]
	internal static JobGauges JobGauges { get; private set; } = null;


	[PluginService]
	internal static ObjectTable ObjectTable { get; private set; } = null;


	public static string PluginFolder
	{
		get
		{
			string codeBase = Assembly.GetExecutingAssembly().Location;
			UriBuilder uri = new UriBuilder(codeBase);
			string path = Uri.UnescapeDataString(uri.Path);
			return Path.GetDirectoryName(path);
		}
	}

	[PluginService]
	internal static PartyList PartyList { get; private set; } = null;


	[PluginService]
	internal static SigScanner SigScanner { get; private set; } = null;


	[PluginService]
	internal static TargetManager TargetManager { get; private set; } = null;

}
