﻿using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using Utils;

namespace FacilityManagement.Patches
{
    public static class CustomRAUtilsAddon
    {
        public static bool Prefix(ref List<ReferenceHub> __result, ArraySegment<string> args, int startindex, out string[] newargs, bool keepEmptyEntries = false)
        {
            try
            {
                string text = RAUtils.FormatArguments(args, startindex);
                List<ReferenceHub> list = ListPool<ReferenceHub>.Shared.Rent();
                if (text.StartsWith("@", StringComparison.Ordinal))
                {

                    foreach (object obj in new Regex("@\"(.*?)\".|@[^\\s.]+\\.").Matches(text))
                    {
                        Match match = (Match)obj;
                        text = RAUtils.ReplaceFirst(text, match.Value, "");
                        string name = match.Value.Substring(1).Replace("\"", "").Replace(".", "");
                        List<ReferenceHub> list2 = (from ply in ReferenceHub.AllHubs
                                                    where ply.nicknameSync.MyNick.Equals(name)
                                                    select ply).ToList();
                        if (list2.Count == 1 && !list.Contains(list2[0]))
                        {
                            list.Add(list2[0]);
                        }
                    }
                    newargs = text.Split(new char[]
                    {
                        ' '
                    }, keepEmptyEntries ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
                    __result = list;
                }
                else
                {
                    if (args.At(startindex).Length > 0)
                    {
                        string[] array = args.At(startindex).Split('.');
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i] is "all" or "*")
                            {
                                list.AddRange(Player.List.Select(x => x.ReferenceHub));
                                break;
                            }
                            if (int.TryParse(array[i], out int playerId))
                            {
                                if (ReferenceHub.TryGetHub(playerId, out ReferenceHub item))
                                    list.Add(item);
                                break;
                            }
                            else if (Enum.TryParse(array[i], true, out Side side))
                            {
                                list.AddRange(Player.Get(side).Select(x => x.ReferenceHub));
                            }
                            else if (Enum.TryParse(array[i], true, out RoleTypeId roletype))
                            {
                                list.AddRange(Player.Get(roletype).Select(x => x.ReferenceHub));
                            }
                            else if (Enum.TryParse(array[i], true, out Team team1))
                            {
                                list.AddRange(Player.Get(team1).Select(x => x.ReferenceHub));
                            }
                            else if (Enum.TryParse(array[i], true, out SimplifyTeam simplifyTeam))
                            {
                                list.AddRange(Player.Get((Team)team1).Select(x => x.ReferenceHub));
                            }
                        }
                    }
                    newargs = (args.Count > 1) ? RAUtils.FormatArguments(args, startindex + 1).Split(new char[]
                    {
                        ' '
                    }, keepEmptyEntries ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries) : null;
                    __result = list;
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                newargs = null;
                __result = null;
                return false;
            }
            return false;
        }

        public enum SimplifyTeam
        {
            SCP,
            MTF,
            CI,
            RSC,
            CLD,
            RIP,
            TUT,
        }
    }
}