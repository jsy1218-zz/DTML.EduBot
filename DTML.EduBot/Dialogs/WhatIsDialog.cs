﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using DTML.EduBot.Constants;

namespace DTML.EduBot.Dialogs
{
    [LuisModel("31511772-4f1c-4590-87a8-0d6b8a7707a1", "a88bd2b022e34d5db56a73eb2bd33726")]
    [Serializable]
    public partial class WhatIsDialog : LuisDialog<object>
    {
        private const string BotName = "Professor Edword";

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, this is not something I know. Ask me something else !");
        }

        [LuisIntent("Age")]
        public async Task Age(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type == BotEntities.Age))
            {
                await context.PostAsync($"I am quite young. Just couple month old. But I am already a Professor. How about that !");
            }
        }

        [LuisIntent("WhatIs")]
        public async Task HandleBotName(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type == BotEntities.Name))
            {
                await context.PostAsync($"My name is {BotName}");
            }
            else
            if (result.Entities.Any(e => e.Type == BotEntities.Time))
            {
                await context.PostAsync($"It's always morning in the Botland, so I never need to sleep");
            }
            else
            if (result.Entities.Any(e => e.Type == BotEntities.Date))
            {
                var date = DateTime.Now.ToLongDateString();
                await context.PostAsync($"Oh, that's eary. It is {date}");
            }
            else
            {
                await context.PostAsync("Sorry, I didn't understand that");
            }
        }
    }
}