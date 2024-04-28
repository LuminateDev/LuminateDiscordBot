using Discord;
using Discord.Interactions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuminateDiscordBot.Autofills
{
    internal class TicketCategoryAutoCompleteHandler
    {
        public class TicketAutoCompleteLoader : AutocompleteHandler
        {
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autoCompletInteraction, IParameterInfo parameter, IServiceProvider services)
            {
                List<AutocompleteResult> results = new List<AutocompleteResult>();

                List<Objects.TicketCategory> tickets = DBManager.GetTicketCategories();

                foreach (var ticket in tickets)
                {
                    string lookupcontext = $"{JsonSerializer.Serialize(ticket)}";
                    if (!String.IsNullOrEmpty(autoCompletInteraction.Data.Current.Value.ToString()))
                    {
                        if (lookupcontext.ToLower().Contains(autoCompletInteraction.Data.Current.Value.ToString().ToLower())) { results.Add(new AutocompleteResult(ticket.TicketTopic, ticket.TicketDataName)); }
                    }
                    else
                    {
                        results.Add(new AutocompleteResult(ticket.TicketTopic, ticket.TicketDataName));
                    }
                }

                return AutocompletionResult.FromSuccess(results.Take(25));
            }
        }
    }
}
