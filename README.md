![VN logo](https://raw.githubusercontent.com/Cyber28/VillagerNames/main/icon.png)

# VillagerNames

Rename your villagers!

## Usage

- Hover any villager card and press N

![Demo pic](https://cdn.discordapp.com/attachments/985852934212444170/1058471373451825233/image.png)

![Demo pic](https://cdn.discordapp.com/attachments/985852934212444170/1058471373758021692/image.png)

## Installation (manual)

You need the [pre-configured BepInEx pack](https://stacklands.thunderstore.io/package/BepInEx/BepInExPack_Stacklands) for this mod. If you installed BepInEx from a different place, you will need to edit the config files.

Extract the contents of this folder to `Stacklands/BepInEx/plugins/VillagerNames`. The final folder layout should look like this:
```
plugins/
  VillagerNames/
    VillagerNames.dll
    icon.png
    manifest.json
    README.md
```

## Known bugs

- Keybinds still work while typing in the text field (this is a vanilla bug, not caused by the mod)
- Renamed babies don't keep the name when they grow into villagers

## Troubleshooting

This mod affects your save files minimally, it adds an ExtraData property to renamed villagers, which are easy to modify manually if something goes wrong

---

Special thanks to [lopidav](https://github.com/lopidav) for figuring out custom text inputs in modals before me and helping me fix some weird issues! 💜

If you have any questions or feedback, join us in the developer-supported [Stacklands Modding Discord server](https://discord.gg/j3FjwZVyWh)

This project was made with love for the Stacklands community 💖💜
