using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Bot.Sample.LuisBot
{
    //public enum BedSizeOptions
    //{
    //    King,
    //    Queen,
    //    Single,
    //    Double
    //}

    //public enum AmenitiesOptions
    //{
    //    Kitchen,
    //    ExtraTowels,
    //    GymAccess,
    //    Wifi
    //}

    public enum PersonalBankingOptions
    {
        Balance,
        Transactions
    }

    [Serializable]
    public class PersonalBanking
    {
        public PersonalBankingOptions? BankingOptions;
      

        public static IForm<PersonalBanking> BuildForm()
        {
            return new FormBuilder<PersonalBanking>()
                .Message($"Welcome to personal banking!{System.Environment.NewLine} What would you like to do: ")
                .Build();
        }
    }
}