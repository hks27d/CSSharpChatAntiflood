[![Discord](https://img.shields.io/discord/1323042994437357599?style=for-the-badge)](https://discord.com/invite/zsmUzthPXx)

# 📌 CSSharp Chat Antiflood
Stop chat flooding from players.

## 🌐 Description
Just a basic plugin to stop chat flooding. Root has immunity by default. Make sure you alter the code in case you need specific changes.

## 📗 Dependecies
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

## 📄 Code changes
If you just want to modify general settings of the plugin:
- `readonly float ResetWarningsInterval = 5.0f;` - timer interval
- `readonly int MaxMessages = 3;` - max messages per each interval

## 📄 Installation
1. Get an IDE & Code Editor (Visual Studio, Rider or others).
2. Edit and debug if you need to.
3. Build the code.
4. Add the files (`.dll`, `.pdb`, `.deps.json`) in their own directory.
5. Rename directory. Use the same name.
6. Upload directory in `/game/csgo/addons/counterstrikesharp/plugins`

## 🤝 Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## ⭐ Show your support
Give a ⭐ if this project helped you.

## 📝 License
MIT based on outlined exemption from CounterStrikeSharp.
