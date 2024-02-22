﻿using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands
{
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Roles;
    using PlayerRoles;
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class FakeSpawnCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "fakespawn";

        public string[] Aliases { get; } = new[] { "fakesync" };

        public string Description { get; } = "Sets everyone or a user to be invisible";

        public string[] Usage { get; } = new string[] { "%player%", "%role%" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.ghost"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: fakespawn ((player id / name) or (all / *)) (RoleTypeId)";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out RoleTypeId roletype))
            {
                response = $"Invalid value for RoleTypeId: {arguments.At(1)}\n{string.Join(", ", Enum.GetNames(typeof(RoleTypeId)))}.";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }
            foreach (Player player in players)
                player.ChangeAppearance(roletype);

            response = $"The followed player have been change to {roletype}:\n{Extensions.LogPlayers(players)}";
            return false;
        }
    }
}
