using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Collections.Concurrent;

namespace CSSharpChatAntiflood
{
    public static class ConcurrentDictionaryExtensions
    {
        /// <summary>
        /// Rebuilds ConcurrentDictionary to reduce internal memory usage
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize the memory overhead
        /// once it is known that no new elements will be added.
        /// 
        /// The capacity reduction to the minimum is not guaranteed.
        /// ConcurrentDictionary has a built-in capacity mechanism that
        /// may still allocate some default minimum capacity for performance reasons
        ///
        /// To allocate minimum size storage array, execute the following statements:
        ///
        /// dictionary.Clear();
        /// dictionary = dictionary.ReduceMemory();
        /// </remarks>
        public static ConcurrentDictionary<TKey, TValue> ReduceMemory<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> source)
        where TKey : notnull
        {
            return new ConcurrentDictionary<TKey, TValue>(source);
        }
    }

    public class CSSharpChatAntiflood : BasePlugin
    {
        public override string ModuleName => "CS# Chat Antiflood";
        public override string ModuleVersion => "2.0.0";
        public override string ModuleAuthor => "HKS 27D";
        public override string ModuleDescription => "";

        ConcurrentDictionary<int, bool> PlayersImmunity = [];
        ConcurrentDictionary<int, long> PlayersTimestamp = [];
        ConcurrentDictionary<int, byte> PlayersMessages = [];

        // Modify based on your preferences
        // Make sure you set MaxMessages value >= 3
        const byte MaxMessages = 3;
        const long MinSecondsBetweenMessages = 5;
        const string ChatAntifloodTAG = "Modders";

        public override void Load(bool hotReload)
        {
            AddCommandListener("say", ChatListener, HookMode.Pre);
            AddCommandListener("say_team", ChatListener, HookMode.Pre);

            RegisterEventHandler((EventPlayerConnectFull @event, GameEventInfo info) =>
            {
                CCSPlayerController Player = @event.Userid!;

                if (Player == null || !Player.IsValid || Player.IsBot || Player.IsHLTV)
                    return HookResult.Continue;

                if (!Player.UserId.HasValue)
                    return HookResult.Continue;

                if (AdminManager.PlayerHasPermissions(Player, "@css/root"))
                {
                    PlayersImmunity[Player.UserId.Value] = true;
                    return HookResult.Continue;
                }

                PlayersTimestamp[Player.UserId.Value] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                PlayersMessages[Player.UserId.Value] = 0;

                return HookResult.Continue;
            }, HookMode.Post);

            RegisterEventHandler((EventPlayerDisconnect @event, GameEventInfo info) =>
            {
                CCSPlayerController Player = @event.Userid!;

                if (Player == null || !Player.IsValid || Player.IsBot || Player.IsHLTV)
                    return HookResult.Continue;

                if (!Player.UserId.HasValue)
                    return HookResult.Continue;

                PlayersImmunity.TryRemove(Player.UserId.Value, out _);
                PlayersTimestamp.TryRemove(Player.UserId.Value, out _);
                PlayersMessages.TryRemove(Player.UserId.Value, out _);

                return HookResult.Continue;
            }, HookMode.Pre);

            RegisterListener<Listeners.OnMapStart>((mapName) =>
            {
                PlayersImmunity.Clear();
                PlayersImmunity = PlayersImmunity.ReduceMemory();
                PlayersTimestamp.Clear();
                PlayersTimestamp = PlayersTimestamp.ReduceMemory();
                PlayersMessages.Clear();
                PlayersMessages = PlayersMessages.ReduceMemory();

                AddUserIDValues();
            });
        }

        private void AddUserIDValues()
        {
            List<CCSPlayerController> TotalPlayers = Utilities.GetPlayers();
            foreach (CCSPlayerController Player in TotalPlayers)
            {
                if (Player != null && Player.IsValid && !Player.IsBot && !Player.IsHLTV && Player.Connected == PlayerConnectedState.PlayerConnected && Player.UserId.HasValue)
                {
                    if (AdminManager.PlayerHasPermissions(Player, "@css/root"))
                    {
                        PlayersImmunity[Player.UserId.Value] = true;
                        continue;
                    }

                    PlayersTimestamp[Player.UserId.Value] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    PlayersMessages[Player.UserId.Value] = 0;
                }
            }
        }

        private HookResult ChatListener(CCSPlayerController? Player, CommandInfo info)
        {
            if (Player != null && Player.IsValid && Player.Connected == PlayerConnectedState.PlayerConnected && Player.UserId.HasValue)
            {
                if (PlayersImmunity.ContainsKey(Player.UserId.Value))
                    return HookResult.Continue;

                if (PlayersTimestamp.TryGetValue(Player.UserId.Value, out long TimestampValue) && (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - TimestampValue > MinSecondsBetweenMessages))
                {
                    PlayersTimestamp[Player.UserId.Value] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    PlayersMessages[Player.UserId.Value] = 0;

                    return HookResult.Continue;
                }

                PlayersTimestamp[Player.UserId.Value] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (PlayersMessages.TryGetValue(Player.UserId.Value, out byte MessagesNumber) && MessagesNumber <= MaxMessages)
                {
                    PlayersMessages[Player.UserId.Value] = ++MessagesNumber;
                    return HookResult.Continue;
                }

                Player.PrintToChat($" {ChatColors.LightRed}{ChatAntifloodTAG} {ChatColors.Default}- Stop {ChatColors.LightRed}spamming {ChatColors.Default}the chat!");
                return HookResult.Handled;
            }
            return HookResult.Continue;
        }
    }
};
