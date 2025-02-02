﻿using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;

namespace fr34kyn01535.GlobalBan
{
    public class CommandUnban : IRocketCommand
    {
        public string Help => "Unbanns a player";

        public string Name => "unban";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public List<string> Permissions => new List<string>() {"globalban.unban"};

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            var name = GlobalBan.Instance.Database.UnbanPlayer(command[0]);
            if (!SteamBlacklist.unban(new CSteamID(name.Id)) && string.IsNullOrEmpty(name.Name))
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            else
            {
                UnturnedChat.Say("The player " + name.Name + " was unbanned");
            }
        }
    }
}