[![Discord](https://img.shields.io/discord/1323042994437357599?style=for-the-badge)](https://discord.com/invite/zsmUzthPXx)

# 📌 CSSharp Chat Antiflood
Mitigate Counter-Strike 2 chat flooding from players.

## 🌐 Description
An advanced high-performance plugin with built-in concurrency support to mitigate Counter-Strike 2 chat flooding. Root has immunity by default. Make sure you alter the code in case you need specific changes.

## 📗 Dependecies
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

## 🎁 Main characteristics
- .NET built-in ConcurrentDictionary
- Internal logic based on Unix Timestamps
- Prioritization of `const` data types usage
- Calls for `Clear()`, `ReduceMemory()` and `AddUserIDValues()` during `OnMapStart` event in case `EventPlayerDisconnect` is not triggered on map change and some of the players will not connect back to the server, leaving the data stored in dictionaries
- No high overhead data types, no overflow or underflow and no useless memory allocs
- Making 1 Dictionary lookup before alloc instead of multiple lookups
- Big O time complexity of `O(1)` on average or `O(n)` in the worst case

## 📁 Installation
1. Go to software releases section. Pick the latest.
2. Download `CSSharpChatAntiflood.zip`.
3. Extract the archive.
4. Make sure you have the needed files (`.dll`, `.pdb`, `.deps.json`) in the folder `CSSharpChatAntiflood`.
5. Upload that directory in `/game/csgo/addons/counterstrikesharp/plugins`.

## ⚡ Building
1. Get an IDE & Code Editor (Visual Studio, Rider or others).
2. Edit and debug if you need to.
3. Build the code.
4. Add the files (`.dll`, `.pdb`, `.deps.json`) in their own directory.
5. Rename directory. Use the same name.
6. Upload it with containing files in `/game/csgo/addons/counterstrikesharp/plugins`.

## 🔵 Code changes
If you want to change general settings of the plugin:
- `const byte MaxMessages = 3;` - Max messages before blocking
- `const long MinSecondsBetweenMessages = 5;` - Min messages between messages to avoid PlayersTimestamp changes
- `const string ChatAntifloodTAG = "Modders";` - Chat TAG for message

> [!CAUTION]
> - Make sure you set `MaxMessages` value >= `3`.
> - In case of refusal you may need to do a code refactoring.

## 🤝 Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## ⭐ Show your support
Give a ⭐ if this project helped you.

## 📝 License
MIT based on outlined exemption from CounterStrikeSharp.
