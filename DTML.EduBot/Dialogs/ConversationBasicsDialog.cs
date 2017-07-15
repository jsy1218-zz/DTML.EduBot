using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using Autofac.Core;
using DTML.EduBot.Common;
using DTML.EduBot.Qna;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DTML.EduBot.Dialogs
{
    [QnaModel(hostUri: "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/", subscriptionKey: "d24b3b5df8b541cabfab6d4b12646ca0", modelId: "34aeee3d-51f6-42d4-bcb3-b8e8c1c1b88e")]
    [Serializable]
    public class ConversationBasicsDialog : IDialog<string>
    {
        private readonly IQnaService qnaService;
        private readonly int NUMBER_OF_CHOICES = 4;
        private string correctAnswer;

        public ConversationBasicsDialog()
        {
            using (var scope = WebApiConfig.Container.BeginLifetimeScope())
            {
                qnaService = WebApiConfig.Container.Resolve<IQnaService>();
            }
        }
    
        private string CorrectAnswer
        {
            get { return correctAnswer; }
            set { correctAnswer = value;  }
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            string friendlyUserName = context.Activity.From.Name;

            string botGreetingsQuestion = BotPersonality.GetRandomGreeting();

            IList<string> incorrectAnswers = PopulateIncorrectAnswers();

            await PopulateCorrectAnswer(botGreetingsQuestion, new CancellationToken());

            IList<string> allChoices = new List<string>(incorrectAnswers);
            allChoices.Add(this.CorrectAnswer);

            await context.PostAsync("Hi " + friendlyUserName + ", We will start with basic conversation starters.Given below sentence, please choose the most appropriate response.");

            PromptDialog.Choice(
                context,
                this.AfterChoiceSelected,
                allChoices,
                botGreetingsQuestion,
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: NUMBER_OF_CHOICES);
        }

        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string selection = await result;

                if (this.CorrectAnswer.Equals(selection))
                {
                    await context.PostAsync("You made it right! The correct answer is: " + this.CorrectAnswer);
                    context.Done("success");
                }
                else
                {
                    await context.PostAsync("Sorry the correct answer is: " + this.CorrectAnswer + " :(. Please type anything to show another greeting question.");
                    await this.StartAsync(context);
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task PopulateCorrectAnswer(string botGreetingsQuestion, CancellationToken token)
        {
            IQnaResult qnaResult = await qnaService.QueryAsync(botGreetingsQuestion, token).ConfigureAwait(false);

            if ("No good match found in the KB".Equals(qnaResult.Answer))
            {
                qnaResult.Answer = BotPersonality.GetRandomGenericResponse();
            }

            this.CorrectAnswer = qnaResult.Answer;
        }

        private IList<string> PopulateIncorrectAnswers()
        {
            IList<string> incorrectAnswers = new List<string>(NUMBER_OF_CHOICES - 1);

            for (int current = 1; current < NUMBER_OF_CHOICES; current++)
            {
                string nextAnswer = BotPersonality.BuildAcquaintance();
                if (!incorrectAnswers.Contains(nextAnswer))
                {
                    incorrectAnswers.Add(nextAnswer);
                }
            }

            return incorrectAnswers;
        }
    }
}