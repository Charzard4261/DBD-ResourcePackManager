# Dead By Daylight Resource Pack Manager
This is a C# Windows Presentation Foundation (WPF) application created to manage Resource Packs for Dead by Daylight. It's designed to be both simple and highly customisable, allowing for per-category pack selection.

## The Progran
### Installing
Head over to the [Latest Releases](https://github.com/Charzard4261/DBD-ResourcePackManager/releases/latest) page and download the latest zip. Extract both files and run the .msi, and the program will be installed and a will be shortcut created on your desktop.
It's the same process to update, too. Download the latest installer, and it'll replace the old version with the new one.

### How does it work?
The program automatically downloads a list of available packs from the [Packs Repository](https://github.com/Charzard4261/DBD-ResourcePackManager-Packs) and uses this to present you with packs to browse. You can search and filter to find a pack you like, then simply click download!
Packs you download are saved to your system, but not immediately applied to the game. Instead, head over to the customimse tab to tune exactly how you want your game to look, whether that's a pack applied across the board or unique packs for each category!
A live preview is shown alongside these controls, so you're not left guessing whether you'd like the other variant more until you boot the game!

### What can I customise?
You can apply packs across broad selections, such as one for all portraits, perks, addons etc, or pick a different pack for each category and side. Specifically packs can be set for:
- Everything
- All Portraits
- All Perks
- All Addons
- All Offerings
- All Emblems
- All StatusEffects
- MiscUI

On the Survivor's side:
- Everything in Survivors
- Survivor Portraits
- Survivor Perks
- Items
- (Coming Soon) Item Addons
- (Coming Soon) Survivor Offerings
- (Coming Soon) Survivor Emblems

And on the Killer's side:
- Everything in Killer
- Killer Portraits
- Killer Perks
- Killer Powers
- (Coming Soon) Killer Addons
- (Coming Soon) Killer Offerings
- (Coming Soon) Killer Emblems

It works on a layer system, meaning anything set below will overwrite whatever you set above. If you want to run a pack for everything, but use another's killer portraits and yet another's item addons, you can do it easily!
A preview of the Icons is shown alongside these controls, allowing you to immediately see how everything looks when put together.
Once you're happy with how your pack looks, simply hit Install and it'll copy it all to your game (WARNING: This deletes your `<game path>\DeadByDaylight\Content\UI\Icons` folder, removing any previously installed packs) (you can change the game's install path in Settings).

### What's different from the Icon Toolbox?
It's not as pretty, but we're hoping that it's flexibility makes up for it! For the regular user, observed functionality is mostly the same, although you now have to click an extra button or two if all you want is just one pack.
On the upside, packs are hosted by their creators, meaning it has no operational cost. The list of packs as well as the list of Survivor and Killer information are stored in two other companion repositories, and are automatically downloaded by the program when needed, meaning no effort on your part is needed when newer chapters are released.
Once this program's functionality is complete, only those two repositories need updating as images are downloaded and saved to your system, and since they are made to be simple handing the project over does not mean transferring substantial manual labour or cost.

## Uninstalling
To uninstall, use Windows' "Add or Remove Programs" tool. After uninstalling, there are some leftover files in the `%LocalAppData%\DBD-ResourcePackManager\` folder, so please remove them to get your storage back!
Uninstalling the program does not remove any packs you have installed to the game.

## Contributing
If there's something you want to add, please do! Fork this repository and submit a pull request when you're happy with what you've changed. I apologise in advance for the spaghetti!
