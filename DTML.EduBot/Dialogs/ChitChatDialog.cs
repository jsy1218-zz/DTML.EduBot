﻿namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using DTML.EduBot.Common;
    using DTML.EduBot.UserData;
    using Attributes;

    [PreConfiguredLuisModel]
    [PreConfiguredQnaModel]
    [Serializable]
    public partial class ChitChatDialog : QnaLuisDialog<object>
    {
        private readonly LevelDialog _levelDialog;
        private readonly IUserDataRepository _iUserDataRepository;

        public ChitChatDialog(LevelDialog levelDialog, IUserDataRepository iUserDataRepository)
        {
            _levelDialog = levelDialog;
            _iUserDataRepository = iUserDataRepository;
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task HandleUnrecognizedIntent(IDialogContext context, LuisResult result)
        {
            string translatedBotResponse = await this.TranslateBotResponseAsync(context, BotPersonality.BotResponseUnrecognizedIntent);
            await context.PostAsync(translatedBotResponse);
        }
       
        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            try
            {
                string userInputText = (await item).Text;

                if (context.Activity != null && context.Activity.AsMessageActivity() != null)
                {
                    context.Activity.AsMessageActivity().Text = await MessageTranslator.TranslateTextAsync(userInputText);
                }

                await base.MessageReceived(context, item);
            }
            catch (Exception)
            {
                // Suppress potential exception from await for demo purpose
            }
        }

        protected virtual async Task<string> TranslateBotResponseAsync(IDialogContext context, string rawResponse)
        {
            string translatedBotResponse = string.Empty;
            UserData userData = _iUserDataRepository.GetUserData(context.Activity.From.Id);

            if (userData != null)
            {
                translatedBotResponse = await MessageTranslator.TranslateTextAsync(rawResponse,
                    userData.NativeLanguageIsoCode);
            }
            else
            {
                return rawResponse;
            }

            return translatedBotResponse;
        }
    }
}