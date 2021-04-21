const Discord = require("discord.js");
const fs = require("fs");
const say = require("say");

const dadJokes = fs.readFileSync("./dadjokes.txt","utf-8").split("\n");

var lastIndex = -1;
module.exports = {
    name: "dadtts",
	description: "Fortæller en far joke, men i en voice kanal!",
	useParams: "far",
    useExample: "[prefix]far",
    useDesc: "Denne kommando vil få FarBot til at fortælle dig en far joke!",

    /**
     * @param {Discord.Client} bot
     * @param {Discord.Message} message
     * @param {Array<string>} args 
     */
    execute(bot,message,args)
    {
        var user = message.guild.members.cache.get(message.author.id);

        var index = Math.floor(Math.random()*dadJokes.length);

        while(index == lastIndex)
        {
            index = Math.floor(Math.random()*dadJokes.length);
        }

        if(user.voice.channel)
        {
            user.voice.channel.join().then(connection =>
                {   
                    dadJokes[index] = dadJokes[index].toLowerCase();
                    dadJokes[index] = dadJokes[index].replace("å","aa")
                    dadJokes[index] = dadJokes[index].replace("ø","oe")
                    dadJokes[index] = dadJokes[index].replace("æ","ae")
                    dadJokes[index] = dadJokes[index].replace("–","")
                    say.export(dadJokes[index],"Microsoft Helle","1","test.wav");
                    console.log("Telling this joke: " +  dadJokes[index])
                    setTimeout(() => {
                        connection.play("test.wav")
                        var inter = setInterval(() => {
                           if(connection.speaking.bitfield == 0) 
                           {
                            if(fs.existsSync("test.wav"))
                            {
                                fs.unlinkSync("test.wav")
                            }
                            connection.disconnect();
                            clearInterval(inter);
                           }
                        }, 100);
                        
                    }, 1000);
                });
        }
        else
        {
            message.reply("Not in a voice channel!")
        }
    }
}