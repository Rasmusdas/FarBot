
var path = require('path');
const Discord = require("discord.js");
const fs = require("fs");
const childProcess = require('child_process');
const EventEmitter = require("events");
const { Readable } = require("stream");
class DataFinished extends EventEmitter {}
const config = require(path.dirname(require.main.filename)+"\\config.json")

var powershellEmitter = null;
const dadJokes = fs.readFileSync("./dadjokes.txt","utf-8").split("\n");

var lastIndex = -1;
module.exports = {
    name: "fartts",
	description: "Fortæller en far joke, men i en voice kanal!",
	useParams: "far",
    useExample: "[prefix]far",
    useDesc: "Denne kommando vil få FarBot til at fortælle dig en far joke!",

    /**
     * @param {Discord.Client} bot
     * @param {Discord.Message} message
     * @param {Array<string>} arg
     */
    execute(bot,message,arg)
    {
        powershellEmitter = new DataFinished();
        var user = message.guild.members.cache.get(message.author.id);
        var index = Math.floor(Math.random()*dadJokes.length);
        while(index == lastIndex)
        {
            index = Math.floor(Math.random()*dadJokes.length);
        }
        var tempJoke = dadJokes[index];
        tempJoke = tempJoke.toLowerCase();
        tempJoke = tempJoke.split("å").join("aa")
        tempJoke = tempJoke.split("ø").join("oe")
        tempJoke = tempJoke.split("æ").join("ae")
        tempJoke = tempJoke.split("–").join("-")
        
        var event = powershellEmitter.on("data",(bytes) => 
        {
            if(user.voice.channel)
            {
                var channel = user.voice.channel;
                channel.join().then(connection => 
                    {
                        message.reply(dadJokes[index])
                        var buffer = Readable.from(Buffer.from(bytes));
                        var talking = connection.play(buffer)
                        talking.on("finish", ()=>
                        {
                            channel.leave();
                        })
                    })
            }
            else
            {
                message.reply("Not in a voice channel!")
                return;
            }
        })

        var vals = setupPowerShell(tempJoke);

        var args = vals[0];
        var pipedData = vals[1];

        var options = {"shell": true}
        
        runPowerShell(args,pipedData,options);



        

    }
}

function setupPowerShell(text)
{
    let args = []
    let pipedData = ''

    let psCommand = `Add-Type -AssemblyName System.speech;$speak = New-Object System.Speech.Synthesis.SpeechSynthesizer;`

    psCommand += `$OutStream = [System.IO.MemoryStream]::new(100);`
    psCommand += `$speak.SetOutputToWaveStream($OutStream);`
    psCommand += `$speak.SelectVoice(\\"` + config.voice + `\\");`;
    psCommand += `$speak.Speak([Console]::In.ReadToEnd());`
    psCommand += "$data = $OutStream.ToArray() -join '-';"
    psCommand += `Write-Output $data;`

    pipedData += text
    
    args.push(psCommand)

    return [args,pipedData];
}

function runPowerShell(args,pipedData,options)
{
    var child = childProcess.spawn("powershell", args, options)
        child.stdin.setEncoding('utf-8')
        child.stdin.setEncoding('utf-8')
        child.stdout.setEncoding('utf-8')
        if (pipedData) {
            child.stdin.end(pipedData)
        }
        let chunks = "";
        let bytes = [];
        child.stdout.on("data",function(data){
            chunks+=data;
        });
        child.stderr.on("data",function(data){
            console.log("Powershell Errors: " + data);
        });
        child.on("exit",function(){
            bytes = chunks.replace("\r","").replace("\n","").replace(" ","").split("-")
            powershellEmitter.emit("data", bytes);
        });
}

