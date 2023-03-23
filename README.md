# Dead By Daylight Resource Pack Manager
This is a C# Windows Presentation Foundation (WPF) application created to manage Resource Packs for Dead by Daylight. It's designed to be both simple and highly customisable, allowing for per-category pack selection.
![image](https://user-images.githubusercontent.com/28228244/227016512-76c48197-559f-4a7d-9bd4-62f5dcb2f107.png)

## The Program
### Installing
Head over to the [Latest Releases](https://github.com/Charzard4261/DBD-ResourcePackManager/releases/latest) page and download & run the .msi, and the program will be installed with a shortcut created on your desktop.
It's the same process to update, too. Download the latest installer, and it'll replace the old version with the new one.

### How does it work?
The program automatically downloads a list of available packs from the [Packs Repository](https://github.com/Charzard4261/DBD-ResourcePackManager-Packs) and uses this to present you with packs to browse. You can search and filter to find a pack you like, then simply click download!

Packs you download are saved to your system, but not immediately applied to the game. Instead, head over to the customise tab to tune exactly how you want your game to look, whether that's a pack applied across the board or unique packs for each category!

A live preview is shown alongside these controls, so you're not left guessing whether you'd like the other variant more until you boot the game! These images and information are downloaded from the [Resources Repository](https://github.com/Charzard4261/DBD-ResourcePackManager-Resources), so when new characters come out you don't have to lift a finger!

### What can I customise?
You can apply packs across broad selections, such as one for all portraits, perks, addons etc, or pick a different pack for each category and side. Specifically, packs can be set for:
- Everything
- All Portraits
- All Perks
- All Addons
- All Offerings
- All Emblems
- All Status Effects
- Misc UI

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

It works on a layer system, meaning anything set below will overwrite whatever you set above. If you want to run a pack for everything but use a different one for killer portraits and yet another for item addons, you can do it easily!

A preview of the Icons is shown alongside these controls, allowing you to immediately see how everything looks when put together.

Once you're happy with how your pack looks, simply hit Install and it'll copy it all to your game (WARNING: This deletes your `<game path>\DeadByDaylight\Content\UI\Icons` folder, removing any previously installed packs) (you can change the game's install path in Settings).

### What's different from the Icon Toolbox?
It's not as pretty, but we're hoping that its flexibility makes up for it!

For the regular user, observed functionality is mostly the same, although you now have to click an extra button or two if all you want is just one pack. For people who want more in-depth customisation, it has never been easier!

On the other hand, packs need tot be hosted by their creators. This means the program has no costs, at the downside of potentially having packs break. The list of packs as well as the list of Survivor and Killer information are stored in two other companion repositories, and are automatically downloaded by the program when needed. This means you don't have to do anything when newer chapters are released.

Once this program's functionality is complete, only the two companion repositories need updating as images are downloaded and saved to your system. Since they are made to be simple, handing the project over does not mean transferring much manual labour.

## Uninstalling
To uninstall, use Windows' "Add or Remove Programs" tool. After uninstalling, there are some leftover files in the `%LocalAppData%\DBD-ResourcePackManager\` folder, so please remove them to get your storage back!
Uninstalling the program does not remove any packs you have installed to the game.

## Submitting a Pack
Follow the instructions in the [Pack Repository](https://github.com/Charzard4261/DBD-ResourcePackManager-Packs) to upload your pack. It may take a day to get added as it has to be manually checked.

## Contributing
If there's something you want to add, please do! Fork this repository and submit a pull request when you're happy with what you've changed. I apologise in advance for the spaghetti!

## Additional Info
- Icon made by Sherboffy using [a Folder icon created by kumakamu - Flaticon](https://www.flaticon.com/free-icons/folder)
- A big thank you to the DBD Wiki Team, who have been keeping all of the DBD asset images up-to-date
