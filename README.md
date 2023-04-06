# ProjectEdit
Project Edit is a game idea where you can create levels and mini games in every platform and cross play with all platform. With ability to write scripts in the Lua programming language to add extra functionallity, Though it's not required by any mean.

The game won't be limited by only a few objects and sounds you can choose from, you can create and import any assets that you wish to use. Ofcourse to that there will be some restrictions to avoid copy rights.

All the levels will be uploaded to a database that the game can access. So uploading a game will be as simple as pressing the upload button, and the same goes to downloading. The API is written in PHP and the database used is MySQL for simplicity.
The players will have a simple way to get new levels or when they make a level to play with their friends, because everything will be handled within the game and no extra complexity will be added.

The scripting system uses Lua as mentioned earlier with the MoonSharp library that allows interperting Lua code from C#. The game will use unities Entity Component System to handle all the logic inside of the game to give the best performence in low end/mobile devices. From the scripting API you will be able to access other entities and change their properties including the ones written in C#. There are some plans to make the scripting system able to access assets, though that's hard because of the limitations in some platforems and the Moon Sharp library.

The editor where the creators will spend most of their times will be made sure that it's easy to use and also usable in all platforms so that it can give the best experiance to all users.

Some levels will be released within the game to showcase what the editor can do in the simplest way and in the most advanced way. All of the assets used in these games will be provided in the editor so creators can make their own version of the games.
The game will have continuos updates and with evey update there will be a level(s) that show case what's new and all the assets added in the level will ofcourse be included in the editor by default.

The game is still in a very early stage of development but a lot of the main systems are done and currently working on the multiplater system.
