using ST.SimModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ST.GameModel.PromptModels;

namespace ST.GameModel
{
    public static class PromptViews
    {




        public static TextPrompt NewDay(ActionArgs args)
        {

            var root = new TextPrompt(args.playerProphet)
            {
                Text = "The sun rises.  The prophet " + args.playerProphet.Name + " ponders how to spend the day...",
                Options = new List<TextOption>() {

                    new TextOption(args.playerProphet){
                        Text = "Go to the Holy Mountain...",
                        action = args.controller.GoToHolyMountain
                    },

                    new TextOption(args.playerProphet){
                        Text = "Go to a Plaza...",
                        action = args.controller.GoToPlaza
                    }
                }
            };
            return root;
        }

        public static TextPrompt HolyMountainSuccess(ActionArgs args, Tenant recievedTenant)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = "The prophet " + args.playerProphet.Name +
                " sits atop the holy mountain and meditates through vivid visions.  Deeply changed, the prophet returns to " +
                args.playerProphet.Residence + ".  " + Environment.NewLine + Environment.NewLine + args.playerProphet.ActionResult.ToString() + Environment.NewLine + Environment.NewLine + (recievedTenant != null ? Environment.NewLine + "Recieved the following tenant from the divine:  " + recievedTenant.Ideal.Description : "")
            };
            return root;
        }



        public static TextPrompt HolyMountainFailure(ActionArgs args)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = "The prophet " + args.playerProphet.Name + " sits atop the holy mountain and meditates, though no vivid visions arrive.  Unchanged, the prophet returns to " + args.playerProphet.Residence + ".  " + args.playerProphet.ActionResult.ToString()
            };
            return root;
        }

        public static TextPrompt PlazaProvinceOptions(ActionArgs args)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = "The prophet ponders which plaza to travel?",
                Options = GeneratePlazaDestinationOptions(args)
            };
            return root;
        }

        private static List<TextOption> GeneratePlazaDestinationOptions(ActionArgs args)
        {
            var result = new List<TextOption>();
            foreach (var destination in args.playerProphet.KnownProvinces)
            {
                result.Add(
                    new TextOption(args.playerProphet) {
                        Text = destination.Name,
                        action = (a) => args.controller.GoToPlazaWithDest(args, destination)
                    }
                );
            }
            return result;
        }

        public static TextPrompt PlazaArriveAtDestination(ActionArgs args)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = args.playerProphet.CurrentLocation.People.Count > 30 ?
                            "The plaza of " + args.playerProphet.CurrentLocation.ToString() + " is dense with people."
                       :
                            args.playerProphet.CurrentLocation.People.Count > 20 ?
                                "The plaza of " + args.playerProphet.CurrentLocation.ToString() + " is bustling."
                            :
                                args.playerProphet.CurrentLocation.People.Count > 10 ?
                                    "The plaza of " + args.playerProphet.CurrentLocation.ToString() + " is somewhat busy."
                                :
                                    "The plaza of " + args.playerProphet.CurrentLocation.ToString() + " is calm, with few people.",

                Options = new List<TextOption>()
                {
                    new TextOption(args.playerProphet)
                    {
                        Text = "Observe the Plaza"
                    },
                    new TextOption(args.playerProphet)
                    {
                        Text = "Proselytize",
                        action = args.controller.Proselytize
                    },
                    new TextOption(args.playerProphet)
                    {
                        Text = "Visit Temple"
                    }
                }
            };

            if (args.proselytizingProphets.Count > 1)
            {
                root.Text += "The prophet " + args.playerProphet.ToString() + " notices a handful of people reciting scripture.";
            }
            else if (args.proselytizingProphets.Count == 1)
            {
                root.Text += "The prophet " + args.playerProphet.ToString() + " notices someone reciting scripture.";
            }
            foreach (var proph in args.proselytizingProphets)
            {
                root.Options.Add(
                    new TextOption(args.playerProphet)
                    {
                        Text = "Listen to " + proph.Name + " recite scripture",
                        action = (a) => args.controller.ListenToProphet(args, proph)
                    }
                );
            }

            return root;

        }

        public static TextPrompt PlazaListenToProfit(ActionArgs args, Prophet prophet)
        {
            var root = new TextPrompt(args.playerProphet) {
                Text = "The prophet " + args.playerProphet.Name + " stood quietly with the crowd surrounding " + prophet.Name + "."
            };

            if (args.Learned != null)
            {
                root.Text += args.playerProphet.Name + " learned the following: " + Environment.NewLine;
                root.Text += args.Learned.ToString();
            }
            else
            {
                root.Text += args.playerProphet.Name + " learned the nothing new.";
            }

            return root;
        }

        public static TextPrompt PlazaProsetylize(ActionArgs args)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = "You are preaching."
            };
            return root;
        }
    }
}
