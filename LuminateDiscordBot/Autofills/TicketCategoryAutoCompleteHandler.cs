using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace LuminateDiscordBot.Autofills
{
    public class TicketAutoCompleteLoader : AutocompleteHandler
    {
        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autoCompletInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            List<AutocompleteResult> results = new List<AutocompleteResult>();

            using (var scope = services.CreateScope())
            {
                using (var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>())
                {
                    List<Models.Database.TicketCategory> tickets = dataContext.TicketCategories.ToList();

                    foreach (var ticket in tickets)
                    {
                        string lookupcontext = JsonSerializer.Serialize(ticket);
                        if (!String.IsNullOrEmpty(autoCompletInteraction.Data.Current.Value.ToString()))
                        {
                            if (lookupcontext.ToLower().Contains(autoCompletInteraction.Data.Current.Value.ToString()!.ToLower())) { results.Add(new AutocompleteResult( $"{ticket.TicketDataName} | Autoresponse: {ticket.AutoResponseEnabled}", ticket.CategoryId)); }
                        }
                        else
                        {
                            results.Add(new AutocompleteResult($"{ticket.TicketDataName} | Autoresponse: {ticket.AutoResponseEnabled}", ticket.CategoryId));
                        }
                    }
                    results = results.Take(24).ToList();
                    results.Add(new AutocompleteResult($"[+] Add new", Constants.TICKET_CATEGORY_AUTOCOMPLETE_ADD_KEY));
                }
            }

            return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
        }
    }

}