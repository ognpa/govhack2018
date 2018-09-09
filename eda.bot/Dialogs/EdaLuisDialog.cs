using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using eda.bot.eda;

namespace eda.bot.Dialogs
{

    [LuisModel(modelID: "", subscriptionKey: "")]
    [Serializable]
    public class EdaLuisDialog : LuisDialog<object>
    {
        static Dictionary<string, string> _SupportedDatasets = new Dictionary<string, string>()
        {
            {"ato", "ato" }
        }; 


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            //var userData =  await (context.Activity.GetStateClient() as StateClient).BotState.GetUserDataAsync(context.Activity.ChannelId, context.Activity.From.Id);
            string message = $"Greeting back to you '{context.Activity.From.Name}'.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("DescribeDataset")]
        public async Task DescribeDatasetIntent(IDialogContext context, LuisResult result)
        {


            EntityRecommendation dataSetEntity;
            string dstype;
            if (result.TryFindEntity("supporteddatasettype", out dataSetEntity))
            {
                System.Diagnostics.Debug.WriteLine($"entity {dataSetEntity}");
                dstype = dataSetEntity.Entity;
            }
            else
            {
                dstype = result.Query;
            }

            if (!_SupportedDatasets.Any(m => m.Key == dstype))
            {
                dstype = "ato";
                await context.PostAsync($"Which dataset?? anyways, here is the {dstype} one!");

            }



            var dtTypes = await EdaClient.GetAtoDataTypesAsync(dstype);
            await context.PostAsync($"You sent {result.Query}.");
            //await context.PostAsync($"response ----- {str}");

            var responseMessage = context.MakeMessage();
            int i = 1;
            foreach (var item in dtTypes)
            {
                HeroCard hCard = new HeroCard()
                {
                    Title = $"Column {i++}: {item.ColName}",
                    Subtitle = $"Type: {item.ColType}"
                };
                responseMessage.Attachments.Add(hCard.ToAttachment());
            }


            responseMessage.AttachmentLayout = AttachmentLayoutTypes.List;

            await context.PostAsync(responseMessage);


            context.Wait(this.MessageReceived);
        }


        [LuisIntent("FindDataset")]
        public async Task FindDatasetIntent(IDialogContext context, LuisResult result)
        {

            EntityRecommendation dataSetEntity;
            string query;
            if (result.TryFindEntity("datasettopic", out dataSetEntity))
            {
                System.Diagnostics.Debug.WriteLine($"Call Azure Search, pass {dataSetEntity}");
                query = dataSetEntity.Entity;
            }
            else
            {
                query = result.Query;
            }

            var srchClient = new SearchDatasets();
            var searchResults = await srchClient.GetAzSearchResultsAsync(query);

            // use attachment list 
            var responseMessage = context.MakeMessage(); // show have preperties set properly for conversation and recepient
            responseMessage.AttachmentLayout = AttachmentLayoutTypes.List;
            responseMessage.Attachments = new List<Attachment>();

            var i = 0;
            //var item = searchResults.Results[0];
            foreach (var item in searchResults.Results)
            {
                if (i > 5) { break; }


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
                    Subtitle = $"Score: {item.Score}", 
                    Text = $"{item.Document["description"]}",
                    //Text = $"Text: {v.name}",
                    Images = null,
                    Buttons = cardActions

                };

                Attachment cardAttachment = hCard.ToAttachment();
                responseMessage.Attachments.Add(cardAttachment);
                responseMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel; 
            }

            if (searchResults.Results.Count > 0)
            {
                await context.PostAsync(responseMessage);
            }
            else
            {
                await context.PostAsync($"I couldn't find any thing about  {query}. Try to rephrase your question!");
                await context.PostAsync($"... or maybe there is nothing about this topic! hmmm! :/ ");
            }
            context.Wait(this.MessageReceived);
        }


        [LuisIntent("ListDatasets")]
        public async Task ListDatasetsIntent(IDialogContext context, LuisResult result)
        {
            //var userData =  await (context.Activity.GetStateClient() as StateClient).BotState.GetUserDataAsync(context.Activity.ChannelId, context.Activity.From.Id);
            string message = $"We support the ATO and AIHW datasets at the momement, more will be added gradually.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("PlotDataset")]
        public async Task PlotDatasetIntent(IDialogContext context, LuisResult result)
        {

            EntityRecommendation dataSetEntity;
            string dstype;
            if (result.TryFindEntity("supporteddatasettype", out dataSetEntity))
            {
                System.Diagnostics.Debug.WriteLine($"entity {dataSetEntity}");
                dstype = dataSetEntity.Entity;
            }
            else
            {
                dstype = result.Query;
            }

            if (!_SupportedDatasets.Any(m => m.Key == dstype))
            {
                dstype = "ato";
                await context.PostAsync($"Which dataset?? anyways, here is the {dstype} one!");

            }

            var resp = await EdaClient.GetPlotAsync(dstype);
          
            int i = 1; 
            foreach (var item in resp)
            {
                var responseMessage = context.MakeMessage();
                responseMessage.Attachments.Add(new Attachment()
                {
                    ContentUrl = item,
                    ContentType = "image/png",
                    ThumbnailUrl = item, 
                    Name = $"{dstype}-plot{i++}.png"
                });
                responseMessage.AttachmentLayout = AttachmentLayoutTypes.List;
                await context.PostAsync(responseMessage);
            }


          

            context.Wait(this.MessageReceived);
        }




    }
}
