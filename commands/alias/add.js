module.exports = {
    name: "alias-add",
    description: "Adds an alias to a certain command",
    useParams: "alias-add <command> <alias>",
    useExample: "[prefix]alias-add ban eject",
    useDesc: "This would add an alias to the ban command, so that you can call it with [prefix]eject",
    userPermissions: ["MANAGE_CHANNELS"],

    async execute(bot, message, args) {
        const commandName = args[0];
        const aliasName = args[1];

        if (!commandName || !aliasName) 
            return message.reply("You're either missing the command name or the alias name in your command call.");

        if (bot.aliases.get(aliasName))
            return message.reply("There's already an alias by this name, either remove it first or use another name for the alias.");

        if (bot.commands.get(aliasName))
            return message.reply("There's already a command with this name, so you can't make an alias named this.");

        const command = bot.commands.get(commandName);

        if (command) {
            bot.aliases.set(aliasName, commandName);
            message.reply(`Alias "${aliasName}" added for ${commandName}!`);
        } else {
            message.reply("I can't find the command you're trying to create an alias for.");
        }
    }
};