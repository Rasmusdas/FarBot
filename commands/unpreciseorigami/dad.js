const fs = require("fs");

const dadJokes = fs.readFileSync("./dadjokes.txt","utf-8").split("\n");

var lastIndex = -1;
module.exports = {
    name: "dad",
	description: "Fortæller en far joke!",
	useParams: "far",
    useExample: "[prefix]far",
    useDesc: "Denne kommando vil få FarBot til at fortælle dig en far joke!",
    async execute(bot,message,args)
    {
        var index = Math.floor(Math.random()*dadJokes.length);

        while(index == lastIndex)
        {
            index = Math.floor(Math.random()*dadJokes.length);
        }

        message.reply(dadJokes[index]);
    }
}