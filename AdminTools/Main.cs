using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;
using UnityEngine;
using HarmonyLib;
using Utils;
using FacilityManagement.Patches;

namespace AdminTools
{
	public class Main : Plugin<Config>
	{
		public override string Author { get; } = "Exiled-Team";
		public override string Name { get; } = "Admin Tools";
		public override string Prefix { get; } = "AT";
        public override Version Version { get; } = new(7, 1, 0);

        public override Version RequiredExiledVersion { get; } = new(8, 5, 0);
		public string HarmonyID = "AdminTools-" + DateTime.Now;
        public EventHandlers EventHandlers;
		public static System.Random NumGen = new();
		public static List<Jailed> JailedPlayers = new();
		public static HashSet<Player> PryGateHubs = new();
		public static Dictionary<Player, List<GameObject>> BchHubs = new();
		public static Dictionary<Player, List<GameObject>> DumHubs = new();
		public static List<Player> IK = new();
        public static List<Player> BreakDoors { get; } = new();
        public static float HealthGain = 5;
		public static float HealthInterval = 1;
		public string OverwatchFilePath;
		public string HiddenTagsFilePath;
		public static bool RestartOnEnd = false;
		public static HashSet<Player> RoundStartMutes = new();
		public static Harmony harmony;
		public override void OnEnabled()
		{
			try
			{
				string path = Path.Combine(Paths.Config, "AdminTools");
				string overwatchFileName = Path.Combine(path, "AdminTools-Overwatch.txt");
				string hiddenTagFileName = Path.Combine(path, "AdminTools-HiddenTags.txt");

				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				if (!File.Exists(overwatchFileName))
					File.Create(overwatchFileName).Close();

				if (!File.Exists(hiddenTagFileName))
					File.Create(hiddenTagFileName).Close();

				OverwatchFilePath = overwatchFileName;
				HiddenTagsFilePath = hiddenTagFileName;
            }
            catch (Exception e)
            {
                Log.Error($"Loading error: {e}");
            }

            EventHandlers = new EventHandlers(this);
            harmony = new(HarmonyID);

            if (Config.qofieopf)
			{
                harmony.Patch(AccessTools.Method(typeof(RAUtils), nameof(RAUtils.ProcessPlayerIdOrNamesList)), new(AccessTools.Method(typeof(CustomRAUtilsAddon), nameof(CustomRAUtilsAddon.Prefix))));
            }

            Handlers.Player.Verified += EventHandlers.OnPlayerVerified;
            Handlers.Server.RoundEnded += EventHandlers.OnRoundEnd;
            Handlers.Player.TriggeringTesla += EventHandlers.OnTriggerTesla;
            Handlers.Player.ChangingRole += EventHandlers.OnSetClass;
            Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            Handlers.Player.InteractingDoor += EventHandlers.OnDoorOpen;
            Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
            Handlers.Player.Destroying += EventHandlers.OnPlayerDestroyed;
            Handlers.Player.InteractingDoor += EventHandlers.OnPlayerInteractingDoor;
		}

		public override void OnDisabled()
		{
			harmony?.UnpatchAll(HarmonyID);

            Handlers.Player.InteractingDoor -= EventHandlers.OnDoorOpen;
			Handlers.Player.Verified -= EventHandlers.OnPlayerVerified;
			Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnd;
			Handlers.Player.TriggeringTesla -= EventHandlers.OnTriggerTesla;
			Handlers.Player.ChangingRole -= EventHandlers.OnSetClass;
			Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
			Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
			Handlers.Player.Destroying -= EventHandlers.OnPlayerDestroyed;
			EventHandlers = null;
			NumGen = null;
		}
	}
}