using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        //public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
        //    ConfigurationManager.AppSettings["LuisAppId"],
        //    ConfigurationManager.AppSettings["LuisAPIKey"],
        //    domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        //{
        //}

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Greeting" with the name of your newly created intent in the following handler
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Cancel")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }
        float loanAmount = 0,tenure;
        [LuisIntent("Rate")]
        public async Task RateIntent(IDialogContext context, LuisResult result)
        {
            
            EntityRecommendation erLoanAMount=null;
            EntityRecommendation erTenure = null;
            result.TryFindEntity("loanAmount", out erLoanAMount);
            result.TryFindEntity("tenure", out erTenure);
            if (result.Entities.Count==1 && erLoanAMount != null && erLoanAMount.Type=="loanAmount")
            {
               
                loanAmount = float.Parse(erLoanAMount.Entity);
                await context.PostAsync($"Please tell us your tenure");
            }
            else if(result.Entities.Count == 1 && erTenure != null && erTenure.Type == "tenure")
            {

                tenure = float.Parse(erTenure.Entity);
                var installment = CalculateLoanInstallment(loanAmount, tenure);
                await context.PostAsync($"We calculated your monthly rate , it is "+installment.ToString());
            }

            //foreach (EntityRecommendation er in result.Entities)
            //{
            //    if (er != null)
            //    {
            //        object amount = 0;
            //        er.Resolution.TryGetValue("value", out amount);
            //    }
            //}



            //await context.PostAsync($"Please tell us your loan amount and tenure as comma separated");
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            // PromptDialog.Confirm(context, AfterResetAsync, "Are you sure", promptStyle: PromptStyle.Keyboard);
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");

        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {

                await context.PostAsync($"You said okay");

            }
            else
            {
                await context.PostAsync("try again");
            }

        }

        private float CalculateLoanInstallment(float loanAmount,float tenure)
        {
           
            return (loanAmount + (loanAmount * 5 / 100) / (tenure * 12));

        }
    }
}