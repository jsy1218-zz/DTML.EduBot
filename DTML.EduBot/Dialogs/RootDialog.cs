using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Bot.Connector;

namespace DTML.EduBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Common;
    using DTML.EduBot.Qna;

    [LuisModel("31511772-4f1c-4590-87a8-0d6b8a7707a1", "a88bd2b022e34d5db56a73eb2bd33726")]
    [QnaModel(hostUri: "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/", subscriptionKey: "d24b3b5df8b541cabfab6d4b12646ca0", modelId: "34aeee3d-51f6-42d4-bcb3-b8e8c1c1b88e")]
    [Serializable]
    public partial class RootDialog : QnaLuisDialog<object>
    {
        private const string LearnVocabularies = "Learn Vocabularies";
        private const string LearnConversationBasics = "Learn Conversation Basics";

        private static readonly IEnumerable<string> Choices = new ReadOnlyCollection<string>
            (new List<String> {
                LearnVocabularies,
                LearnConversationBasics});

        public override async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            string friendlyUserName = context.Activity.From.Name;

            PromptDialog.Choice(
                context,
                this.AfterChoiceSelected,
                Choices,
                "Hello Dear " + friendlyUserName + ",\n As an avid English language learner, which area would you like to learn today?",
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: Choices.Count());
        }

        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                switch (selection)
                {
                    case LearnVocabularies:
                        await context.PostAsync("This functionality is not yet implemented! Type anything to go back to the option(s).");
                        await this.StartAsync(context);
                        break;

                    case LearnConversationBasics:
                        await context.PostAsync("Great! Please type in anything to confirm you are ready.");
                        context.Call(new ConversationBasicsDialog(), this.AfterLearnConversationBasics);
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task AfterLearnConversationBasics(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                switch (selection)
                {
                    case "success":
                        await context.PostAsync("Congratz! You made it! Type anything if you would like to try another question.");
                        await this.StartAsync(context);
                        break;

                    case "failed":
                        await context.PostAsync("It's ok, there is always next time :)");
                        await this.StartAsync(context);
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task HandleUnrecognizedIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(BotPersonality.BotResponseUnrecognizedIntent);
        }

       [LuisIntent("Gibberish")]
        public async Task HandleGibberish(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(BotPersonality.BotResponseToGibberish);
        }

       
    }
}