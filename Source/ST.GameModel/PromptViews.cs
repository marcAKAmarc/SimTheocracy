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
                    },

                    new TextOption(args.playerProphet)
                    {
                        Text = "Review information...",
                        action = args.controller.Review
                    }
                }
            };


            if (args.playerProphet.Divination > 0 || args.playerProphet.KnownReligions.Count > 0 || args.playerProphet.RecievedTenants.Count > 0)
            {
                root.Options.Add(
                    new TextOption(args.playerProphet)
                    {
                        Text = "Write scripture...",
                        action = args.controller.WriteScriptureOptions
                    }
                );
            }
            return root;
        }

        public static TextPrompt ReviewData(ActionArgs args)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = "Prophet " + args.playerProphet.Name + " takes a moment to revisit the knowledge they have gathered...",
                Options = new List<TextOption>()
                {
                    new TextOption(args.playerProphet)
                    {
                        Text = "Debug - Religion Following Breakdown",
                        action = args.controller.ReviewDebugReligion
                    },
                    new TextOption(args.playerProphet)
                    {
                        Text = "About " + args.playerProphet.Name
                    },
                    new TextOption(args.playerProphet)
                    {
                        Text = "About " + args.playerProphet.Religion
                    }
                }
            };
            return root;
        }

        public static TextPrompt DebugReviewReligion(ActionArgs args)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = ""
            };

            foreach(var score in args.DebugAllReligions)
            {
                root.Text += score.Name + " - " + args.DebugAllPeople.Count(x => x.Religion.Id == score.Id) + " followers.";
                root.Text += Environment.NewLine;
            }
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
                root.Text += args.playerProphet.Name + " learned of the following: " + Environment.NewLine + Environment.NewLine;
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
                Text = "Prophet " + args.playerProphet.Name + " stands on a discarded crate while gathering the power to speak."
            };

            if(args.playerProphet.Religion.Scriptures.Count > 0)
            {
                var scriptureWords = args.playerProphet.Religion.Scriptures[args.random.Next(args.playerProphet.Religion.Scriptures.Count)].Text.Split(' ').ToList();
                var start = args.random.Next(scriptureWords.Count);
                var endInclusive = args.random.Next(start, scriptureWords.Count);
                root.Text += "  The prophet's voice carries a determined message: \"...";
                for(var i = start; i <= endInclusive; i++)
                {
                    root.Text += " " + scriptureWords[i];
                }
                root.Text += "...";

                root.Text += Environment.NewLine + "The prophet steps down, and surveys the crowd that still surrounds the discarded crate.  Have they been swayed?"
                    + Environment.NewLine + args.playerProphet.ActionResult.ToString();
            }
            else
            {
                root.Text += "..  ...but the prophet hasn't prepared any scripture.  " + args.playerProphet.Name + " steps down, feeling defeated.";
            }

            
            return root;
        }

        public static TextPrompt WriteScriptureOptions(ActionArgs args)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = "Prophet " + args.playerProphet.Name + " gathers paper and writing utensils and reflects on what will be written..."
            };
            root.Options = new List<TextOption>();
            if(args.playerProphet.Divination > 0)
            {
                root.Options.Add(
                    new TextOption(args.playerProphet)
                    {
                        Text = "Write of experiences on the holy mountain.",
                        action = args.controller.WriteScriptureChooseExperience
                    }
                );
            }
            foreach(var gatheredTenant in args.playerProphet.RecievedTenants)
            {
                root.Options.Add(
                    new TextOption(args.playerProphet)
                    {
                        Text = "Write of divine tenant:  " + gatheredTenant.Ideal.Description,
                        action = (a) => args.controller.WriteScriptureChooseTenant(args, gatheredTenant)
                    }
                );
            }
            foreach(var renouncable in args.playerProphet.KnownReligions)
            {
                root.Options.Add(
                    new TextOption(args.playerProphet)
                    {
                        Text = "Renounce the religion of " + renouncable.Religion.Name,
                        action = (a) => args.controller.WriteScriptureChooseReligion(args, renouncable)
                    }
                );
            }

            return root;
        }

        public static TextPrompt WriteScriptureText(ActionArgs args)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = "Prophet " + args.playerProphet.Name + " focuses on ",
                TakeTypedInput = true
            };
            if(args.WritingRenouncedReligion != null)
            {
                root.Text += args.WritingRenouncedReligion.ToString() + ".";
                root.TypedInputAction = args.controller.WriteScriptureRenounce;
            }
            if(args.WritingTenant != null)
            {
                root.Text += "writing a new tenant.  It must be " + args.WritingTenant.Level.ToString() + " that followers " + args.WritingTenant.Ideal.Description;
                root.TypedInputAction = args.controller.WriteScriptureTenant;
            }
            if(args.WritingExperience == true)
            {
                root.Text += "writing of divine experience.";
                root.TypedInputAction = args.controller.WriteScriptureExperience;
            }

            root.Text += "  Prophet " + args.playerProphet.Name + " writes the following words:  (Type scripture and press Enter) ";

            return root;
        }

        public static TextPrompt ScriptureWritten(ActionArgs args)
        {
            var root = new TextPrompt(args.playerProphet)
            {
                Text = "Prophet " + args.playerProphet.Name + " sets down their writing tool and feels new energy in the air." + Environment.NewLine + args.playerProphet.ActionResult.ToString()
            };

            return root;
        }

    }
}
