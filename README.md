# server-obranu

Main Server for the Obranu/Napad Project

### What works

Making accounts via a basic WebSocket Client
"Logging" in with a WebSocket Client (Logging in does not serve any purpose yet)

### Files and Folders

* *TEMP/* equivalent of user/cache on JET/AE this is where all the cached json files go, it is deleted every time the server starts
* *LOGS/* This is where log files go format goes like this  {UnixTimeSeconds}.log
* **SRCS/**  Contains all the source files
* *USER/* this is where the Accounts.json file and user profiles are stored
* DATA/ equivalent of user/mods and db/ on JET/AE this is where all the item's information and the likes are stored, all of the valid mods are compiled into the respective files in TEMP/
* *CONF/* Contains configuration files for the server, currently only contains server.json which has settings for ip, port and log count,
* ~~LIBS/~~ Wanted to implement a folder where all .dll files would be stored like Newtonsoft.Json.dll but i could never get both CSC and the compiled executable to recognize the folder

folders in  *italics* dont need to present for the server to work as the server will automaticly generate them
**bold** folders are not present in pre-compiled versions are only for compilingfolders in  *italics* dont need to present for

### References

* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* [websocket-sharp-obranu](https://github.com/pixelkingliam/websocket-sharp-obranu)

#### Compiling and running it

***I've only tried to compile and run it on Arch Linux x86-64 using mono 6.12.0 but it should work on other Operating Systems like Windows 10/11 and other Linux distros***

###### Compile

`$ msbuild`

###### Execute

a simple `$ mono server.exe` should suffice, on some ports like 80 you need to run it with root privileges

running it like any other programs on windows 10/11 should also work