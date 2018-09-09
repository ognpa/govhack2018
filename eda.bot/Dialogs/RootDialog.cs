using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eda.bot.eda;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace eda.bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            //// Calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            //// Return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            if (activity.Text == "ato")
            {
                var str = await EdaClient.GetAtoDataTypesAsync("ato");
                await context.PostAsync($"You sent {activity.Text}.");
                await context.PostAsync($"response ----- {str}"); 
            }
            else if (activity.Text == "search")
            {
                var srchClient = new SearchDatasets();

                var searchResults = await srchClient.GetAzSearchResultsAsync("Energy");



                // use attachment list 
                var responseMessage = context.MakeMessage(); // show have preperties set properly for conversation and recepient
                responseMessage.AttachmentLayout = AttachmentLayoutTypes.List;
                responseMessage.Attachments = new List<Attachment>();

                var i = 1;
                var item = searchResults.Results[0];
                {
                    var files = item.Document["files"] as string[]; 
                    string url = (string)files[0];
                   
                    CardAction cardButton = new CardAction()
                    {
                        Title = $"{new UriBuilder(url).Uri.AbsoluteUri}",
                        Type = "openUrl", 
                        Value = new UriBuilder(url).Uri.AbsoluteUri
                    };


                    List<CardAction> cardActions = new List<CardAction>();
                    cardActions.Add(cardButton);
                    HeroCard hCard = new HeroCard()
                    {
                        Title = $"{i++}: {item.Document["title"]}",
                        
                        Text = $"{item.Document["description"]}", 
                        //Text = $"Text: {v.name}",
                        Images = null,
                        Buttons = cardActions

                    };

                    Attachment cardAttachment = hCard.ToAttachment();
                    responseMessage.Attachments.Add(cardAttachment);
                }

                await context.PostAsync(responseMessage);
            }
            else if(activity.Text == "graph")
            {
                var resp = await EdaClient.GetPlotAsync("ato");


                var responseMessage = context.MakeMessage();
                responseMessage.Attachments.Add(new Attachment()
                {
                    ContentUrl = resp[0],
                    ContentType = "image/png",
                    Name = "ato.png"
                });


                await context.PostAsync(responseMessage);
            }
            else
            {

            }

            context.Wait(MessageReceivedAsync);
        }
    }
}