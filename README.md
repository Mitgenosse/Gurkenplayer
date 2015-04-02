# Gurkenplayer - Cities: Skylines modification
Welcome to my first modification. The name of the mod will likely change.

I am trying to build a coop multiplayer mode for Cities: Skyline.
If you want to help me, then do it. :)

<p>Already done:</p>
-Setting up server and client socket (using Lidgren.Networking.dll (MIT license), thanks Lidgren!).
-It should synchronize money, area demand (commercial, residental, industrial), bought tiles.

ToDo:
-Create an UI on map start.
-Synchronize more stuff.
(-And bug fixes)

Current procedure (not final):
-Start the same map.
-On level load, the UI will be displayed and time will be stopped.
-In the displayed UI, the server can be set up or you can connect to a server.
-Once the server lobby is full, the host can start the game.

I do not know if the simulation speed should be changeable. I have to overthink some parts of the gameplay experience.

This source is going to be licensed under the MIT License when it is usable.
