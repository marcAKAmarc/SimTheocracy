using ST.SimModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ST.GameModel.PromptModels;

namespace ST.GameModel
{

    public class PromptControllers
    {
        public Game Game;

        public PromptControllers(Game game)
        {
            Game = game;
        }

        public PromptControllerResult NewDay(ActionArgs args)
        {
            return GoTo(
                 PromptViews.NewDay(args),
                 args);
        }

        public PromptControllerResult GoToHolyMountain(ActionArgs args)
        {
            var result = args.playerProphet.GoToHolyMountain(args.random);
            if (args.playerProphet.ActionResult.DivinationChange != 0)
                return GoTo(PromptViews.HolyMountainSuccess(args, result), args);
            else
                return GoTo(PromptViews.HolyMountainFailure(args), args);
        }

        public PromptControllerResult GoToPlaza(ActionArgs args)
        {
            return GoTo(PromptViews.PlazaProvinceOptions(args), args);
        }

        public PromptControllerResult GoToPlazaWithDest(ActionArgs args, Province province)
        {
            args.playerProphet.GoToPlaza(province);
            args.proselytizingProphets = Game.Prophets.Where(x => x.State == ProphetState.Reciting && x.CurrentLocation.Id == args.playerProphet.CurrentLocation.Id).ToList();
            return GoTo(PromptViews.PlazaArriveAtDestination(args), args);
        }

        public PromptControllerResult ListenToProphet(ActionArgs args, Prophet prophet)
        {
            if (prophet.State != ProphetState.Reciting)
                throw new Exception("Can not listen to prophet that is not reciting!");

            var learned = args.playerProphet.ListenToProphet(prophet, args.random);
            args.proselytizingProphet = prophet;
            args.Learned = learned;
            return GoTo(PromptViews.PlazaListenToProfit(args, prophet), args);
        }

        public PromptControllerResult Proselytize(ActionArgs args)
        {
            var attacked = args.playerProphet.CheckAttacked(args.random);
            if (attacked)
            {

            }
            args.playerProphet.Proselytize(args.random);
            return GoTo(PromptViews.PlazaProsetylize(args), args);
        }
        /*public static PromptControllerResult PlazaObserve(ActionArgs args)
        {

            return GoTo(PromptViews)
        }*/

        public static PromptControllerResult GoTo( TextPrompt textPrompt, ActionArgs actionArgs)
        {
            return new PromptControllerResult() { ActionResult = actionArgs.playerProphet.ActionResult, TextPrompt = textPrompt, ActionArgs = actionArgs };
        }
    }


}
