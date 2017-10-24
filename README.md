# CSL-Coop - Cities: Skylines modification
This project is originally forked from Gurkenschreck/Gurkenplayer. Special thanks goes to Mario Giugno who initialy created this mod.

I am trying to build a coop multiplayer mode for Cities: Skyline.
If you want to help me, then do it. :)

---

<b>Already done:</b>
* Setting up server and client socket (using Lidgren.Networking.dll (MIT license), thanks Lidgren!).
* It should synchronize money, area demand (commercial, residental, industrial), bought tiles.

<b>ToDo:</b>
* Create an UI on map start.
* Synchronize more stuff.
* (And bug fixes)

<b>Current procedure (not final):</b></br>
* Start the same map.
* On level load, the UI will be displayed and time will be stopped.
* In the displayed UI, the server can be set up or you can connect to a server.
* Once the server lobby is full, the host can start the game.

I do not know if the simulation speed should be changeable. I have to overthink some parts of the gameplay experience.

# How to test...

You can clone the repository and compile it, it should not throw any errors. 
To run the mod, you have to copy the provided Lidgren.Network.dll and SkylinesOverwatch.dll in the Managed_dlls folder into the same folder as the CSLCoop.dll.
Copy the CSLCoop.dll and the Lidgren.Network.dll and SkylinesOverwatch.dll and place it into the following path:

<b>%localappdata%\Colossal Order\Cities_Skylines\Addons\Mods\CSLCoop</b>

Now you can test the mod. :)

References:
Overwatch: https://github.com/arislancrescent/CS-SkylinesOverwatch
Lidgren Network Library: https://github.com/lidgren/lidgren-network-gen3
