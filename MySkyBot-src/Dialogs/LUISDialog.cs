//using HotelBot.Models.FacebookModels;
using LuisBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Bot.Sample.LuisBot
{
    //[LuisModel("fe8f2316-c921-42e8-a8f1-a13ef047f26d", "15bf2048ffd34b87a1861040094af4fd")]
    [Serializable]
    public class LUISDialog: LuisDialog<PersonalBanking>
    {
        private readonly BuildFormDelegate<PersonalBanking> Banking;
        private readonly BuildFormDelegate<Loan> loanFormDelegate;
        private Loan loan;

        [field: NonSerialized()]
        protected Activity _message;

        public LUISDialog(BuildFormDelegate<PersonalBanking> personalBanking) : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
            this.Banking = personalBanking;
            loan = new Loan();
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            _message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry I don't know what you mean.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Done")]
        public async Task Done(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Thanks for contacting us, in case you want more detail please log into our website www.bank.com");
        }

        [LuisIntent("Appointment")]
        public async Task Appointment(IDialogContext context, LuisResult result)
        {
            EntityRecommendation erappointmentDate = null;
            EntityRecommendation erappointmentTime = null;
          

            result.TryFindEntity("appointmentDate", out erappointmentDate);
            result.TryFindEntity("appointmentTime", out erappointmentTime);

            if (result.Entities.Count == 0)
            {
                await context.PostAsync($"Thanks you for contacting us ,tell us date and time for appointment");
            }
            else if (result.Entities.Count == 2 && erappointmentDate != null && erappointmentDate.Type == "appointmentDate" && erappointmentTime != null && erappointmentTime.Type == "appointmentTime")
            {
                await context.PostAsync($"Your appointment has been fiixed on " + erappointmentDate.Entity + " at " + erappointmentTime.Entity);
                await context.PostAsync("Do want any other help?");
            }
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            context.Call(new GreetingDialog(), callback);
        }

        private async Task callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }

        [LuisIntent("Features")]
        public async Task ShowFeatures(IDialogContext context, LuisResult result)
        {
            var enrollmentForm = new FormDialog<PersonalBanking>(new PersonalBanking(), this.Banking, FormOptions.PromptInStart);
            context.Call<PersonalBanking>(enrollmentForm, callback);
        }

        [LuisIntent("ShowBalance")]
        public async Task ShowBalance(IDialogContext context, LuisResult result)
        {
            var enrollmentForm = new FormDialog<PersonalBanking>(new PersonalBanking(), this.Banking, FormOptions.PromptInStart);
            context.Call<PersonalBanking>(enrollmentForm, callback);
        }

        //float loanAmount = 0, tenure;
        [LuisIntent("Rate")]
        public async Task RateIntent(IDialogContext context, LuisResult result)
        {

            EntityRecommendation erLoanAMount = null;
            EntityRecommendation erTenure = null;
            result.TryFindEntity("loanAmount", out erLoanAMount);
            result.TryFindEntity("tenure", out erTenure);

            EntityRecommendation erappointmentDate = null;
            EntityRecommendation erappointmentTime = null;
            result.TryFindEntity("loanAmount", out erLoanAMount);
            result.TryFindEntity("tenure", out erTenure);

            result.TryFindEntity("appointmentDate", out erappointmentDate);
            result.TryFindEntity("appointmentTime", out erappointmentTime);


            if (result.Entities.Count == 1 && erLoanAMount != null && erLoanAMount.Type == "loanAmount")
            {
                this.loan.LoanAmount = float.Parse(erLoanAMount.Entity);
                await context.PostAsync($"Please tell us your tenure");
            }
            else if (result.Entities.Count == 1 && erTenure != null && erTenure.Type == "tenure" )
            {
                this.loan.Tenure = float.Parse(erTenure.Entity);
                this.loan.Installment = CalculateLoanInstallment(this.loan.LoanAmount, this.loan.Tenure);
                await context.PostAsync($"We calculated your monthly rate , it is " + this.loan.Installment.ToString());

                PromptDialog.Confirm(context, AfterResetAsync, "Would you like to set an appoinment with bank advisor?","", promptStyle: PromptStyle.Keyboard);
                
            }
            else if(result.Entities.Count == 2 && erTenure != null && erTenure.Type == "tenure" && erLoanAMount != null && erLoanAMount.Type == "loanAmount")
            {
                this.loan.LoanAmount = float.Parse(erLoanAMount.Entity);
                this.loan.Tenure = float.Parse(erTenure.Entity);
                this.loan.Installment = CalculateLoanInstallment(this.loan.LoanAmount, this.loan.Tenure);
                await context.PostAsync($"We calculated your monthly rate , it is " + this.loan.Installment.ToString());
            }

            if (result.Entities.Count == 2 && erappointmentDate != null && erappointmentDate.Type == "appointmentDate" && erappointmentTime != null && erappointmentTime.Type == "loanAmount")
            {
                await context.PostAsync($"Your appointment has been fiixed on "+ erappointmentDate.Entity+" at "+ erappointmentTime.Entity);
                await context.PostAsync("Do want any other help?");
            }

            //context.Wait(MessageReceived);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                await context.PostAsync($"Tell us date and time for appointment");
            }
            else
            {
                await context.PostAsync("Okay, do want any other help?");
            }
        }



        private float CalculateLoanInstallment(float loanAmount, float tenure)
        {

            return (loanAmount + (loanAmount * 5 / 100) / (tenure * 12));

        }

       
    }
}