﻿using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Unturned.Commands;
using SDG;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using Rocket.Core.Steam;

namespace fr34kyn01535.GlobalBan
{
    public class CommandBan : IRocketCommand
    {
        public string Help => "Banns a player";

        public string Name => "ban";

        public string Syntax => "<player> [reason] [duration]";

        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public List<string> Permissions => new List<string>() {"globalban.ban"};

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            try
            {
                if (command.Length == 0 || command.Length > 3)
                {
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                    return;
                }

                var isOnline = false;

                CSteamID steamid;
                string charactername = null;


                var otherPlayer = UnturnedPlayer.FromName(command[0]);
                var otherPlayerID = command.GetCSteamIDParameter(0);
                if (otherPlayer == null || otherPlayer.CSteamID.ToString() == "0" ||
                    caller != null && otherPlayer.CSteamID.ToString() == caller.Id)
                {
                    var player = GlobalBan.GetPlayer(command[0]);
                    if (player.Key.ToString() != "0")
                    {
                        steamid = player.Key;
                        charactername = player.Value;
                    }
                    else
                    {
                        if (otherPlayerID != null)
                        {
                            steamid = new CSteamID(otherPlayerID.Value);
                            var playerProfile = new Profile(otherPlayerID.Value);
                            charactername = playerProfile.SteamID;
                        }
                        else
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                            return;
                        }
                    }
                }
                else
                {
                    isOnline = true;
                    steamid = otherPlayer.CSteamID;
                    charactername = otherPlayer.CharacterName;
                }

                var adminName = "Console";
                if (caller != null) adminName = caller.ToString();

                if (command.Length == 3)
                {
                    var duration = 0;
                    if (int.TryParse(command[2], out duration))
                    {
                        GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), adminName, command[1],
                            duration);
                        UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername,
                            command[1]));
                        if (isOnline)
                            Provider.kick(steamid, command[1]);
                    }
                    else
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                        return;
                    }
                }
                else if (command.Length == 2)
                {
                    GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), adminName, command[1], 0);
                    UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername,
                        command[1]));
                    if (isOnline)
                        Provider.kick(steamid, command[1]);
                }
                else
                {
                    GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), adminName, "", 0);
                    UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public", charactername));
                    if (isOnline)
                        Provider.kick(steamid, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}