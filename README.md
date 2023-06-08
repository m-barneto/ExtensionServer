# ExtensionServer
This project was used to add Discord Rich Presence to web applications that would ordinarily be unable to do so.

The Extension Server solution is a headless server that recieves local web requests. It uses that data to update the Discord Rich Presence status as well as any other modules attached, such as a discord bot.

The PandoraPresence solution is a chrome extension that I've written to scrape data off of pandora such as song title, album, artist, and duration/listening time. It sends that info over to the Extension Server in order to update the rich presence.

![pandorapresence](https://github.com/m-barneto/ExtensionServer/assets/4347791/6582cffb-da89-4f52-b587-dbc204e0aece)
