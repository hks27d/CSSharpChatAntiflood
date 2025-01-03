using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpChatAntiflood
{
    public class CSSharpChatAntiflood : BasePlugin
    {
        public override string ModuleName => "CS# Chat Antiflood";
        public override string ModuleVersion => "1.0.0";
        public override string ModuleAuthor => "HKS 27D";
        public override string ModuleDescription => "";

#pragma warning disable IDE0044 // Add readonly modifier
        Dictionary<int, int> PlayerWarnings = [];
        Dictionary<int, bool> PlayerRootStatus = [];
#pragma warning restore IDE0044 // Add readonly modifier

        readonly float ResetWarningsInterval = 5.0f;
        readonly int MaxMessages = 3;

        public override void Load(bool hotReload)
        {
            AddCommandListener("say", ChatAntifloodListener, HookMode.Pre);
            AddCommandListener("say_team", ChatAntifloodListener, HookMode.Pre);

            RegisterEventHandler((EventPlayerConnectFull @event, GameEventInfo info) =>
            {
                CCSPlayerController Player = @event.Userid!;

                if (Player == null || !Player.IsValid || Player.IsBot || Player.IsHLTV)
                    return HookResult.Continue;

                if (!Player.UserId.HasValue)
                    return HookResult.Continue;

                var PlayerUserId = Player.UserId.Value;
                if (AdminManager.PlayerHasPermissions(Player, "@css/root"))
                {
                    PlayerRootStatus.TryAdd(PlayerUserId, true);
                    return HookResult.Continue;
                }

                PlayerRootStatus.TryAdd(PlayerUserId, false);
                PlayerWarnings.TryAdd(PlayerUserId, 0);

                return HookResult.Continue;
            }, HookMode.Post);

            RegisterEventHandler((EventPlayerDisconnect @event, GameEventInfo info) =>
            {
                CCSPlayerController Player = @event.Userid!;

                if (Player == null || !Player.IsValid || Player.IsBot || Player.IsHLTV)
                    return HookResult.Continue;

                if (!Player.UserId.HasValue)
                    return HookResult.Continue;

                var PlayerUserId = Player.UserId.Value;
                PlayerRootStatus.Remove(PlayerUserId);
                PlayerWarnings.Remove(PlayerUserId);

                return HookResult.Continue;
            }, HookMode.Pre);

            RegisterListener<Listeners.OnMapStart>((mapName) =>
            {
                AddTimer(1.0f, () =>
                {
                    PlayerRootStatus.Clear();
                    PlayerWarnings.Clear();
                    ReaddUserIDValues();
                });
            });
            AddTimer(ResetWarningsInterval, ChatAntiflood_TimerCallback, TimerFlags.REPEAT);
        }

        private void ReaddUserIDValues()
        {
            List<CCSPlayerController> TotalPlayers = Utilities.GetPlayers();
            foreach (CCSPlayerController Player in TotalPlayers)
            {
                if (Player != null && Player.IsValid && Player.Connected == PlayerConnectedState.PlayerConnected && Player.UserId.HasValue)
                {
                    var PlayerUserId = Player.UserId.Value;
                    if (AdminManager.PlayerHasPermissions(Player, "@css/root"))
                        PlayerRootStatus.TryAdd(PlayerUserId, true);
                    else
                    {
                        PlayerRootStatus.TryAdd(PlayerUserId, false);
                        PlayerWarnings.TryAdd(PlayerUserId, 0);
                    }
                }
            }
        }

        private void ChatAntiflood_TimerCallback()
        {
            List<CCSPlayerController> players = Utilities.GetPlayers();
            foreach (CCSPlayerController Player in players)
            {
                if (Player != null && Player.IsValid && Player.Connected == PlayerConnectedState.PlayerConnected && !Player.IsBot && Player.UserId.HasValue)
                {
                    var PlayerUserId = Player.UserId.Value;
                    if (PlayerRootStatus.TryGetValue(PlayerUserId, out bool value) && value == false)
                        PlayerWarnings[PlayerUserId] = 0;
                }
            }
        }

        private HookResult ChatAntifloodListener(CCSPlayerController? Player, CommandInfo info)
        {
            if (Player != null && Player.IsValid && Player.Connected == PlayerConnectedState.PlayerConnected && Player.UserId.HasValue)
            {
                var PlayerUserId = Player.UserId.Value;
                if (PlayerRootStatus.TryGetValue(PlayerUserId, out bool StatusValue) && StatusValue == true)
                    return HookResult.Continue;

                ++PlayerWarnings[PlayerUserId];
                if (PlayerWarnings.TryGetValue(PlayerUserId, out int WarningsValue) && WarningsValue > MaxMessages)
                {
                    Player.PrintToChat($" {ChatColors.LightRed}Modders {ChatColors.Default}- Stop {ChatColors.LightRed}spamming {ChatColors.Default}the chat!");
                    return HookResult.Handled;
                }
            }

            return HookResult.Continue;
        }
    }
};
