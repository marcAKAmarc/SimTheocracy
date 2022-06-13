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

        public PromptControllerResult Review(ActionArgs args)
        {
            return GoTo(PromptViews.ReviewData(args), args);
        }
        public PromptControllerResult ReviewDebugReligion(ActionArgs args)
        {
            args.DebugAllReligions = Game.religions;
            args.DebugAllPeople = Game.Provinces.SelectMany(x => x.People).ToList();
            return GoTo(PromptViews.DebugReviewReligion(args), args);
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

        public PromptControllerResult WriteScriptureOptions(ActionArgs args)
        {
            return GoTo(PromptViews.WriteScriptureOptions(args), args);
        }

        public PromptControllerResult WriteScriptureText(ActionArgs args)
        {
            return GoTo(PromptViews.WriteScriptureText(args), args);
        }

        public PromptControllerResult WriteScriptureChooseExperience(ActionArgs args)
        {
            args.WritingExperience = true;
            return GoTo(PromptViews.WriteScriptureText(args), args);
        }
        public PromptControllerResult WriteScriptureChooseReligion(ActionArgs args, PercievedReligion religion)
        {
            args.WritingRenouncedReligion = religion.Religion;
            return GoTo(PromptViews.WriteScriptureText(args), args);
        }
        public PromptControllerResult WriteScriptureChooseTenant(ActionArgs args, Tenant tenant)
        {
            args.WritingTenant = tenant;
            return GoTo(PromptViews.WriteScriptureText(args), args);
        }
        public PromptControllerResult WriteScriptureExperience(ActionArgs args)
        {
            args.playerProphet.WriteScripture(args.random, new List<Tenant>(), new List<Religion>(), 1, args.TypedText);
            return GoTo(PromptViews.ScriptureWritten(args), args);
        }
        public PromptControllerResult WriteScriptureRenounce(ActionArgs args)
        {
            args.playerProphet.WriteScripture(args.random, new List<Tenant>(), new List<Religion>() { args.WritingRenouncedReligion }, -1, args.TypedText);
            return GoTo(PromptViews.ScriptureWritten(args), args);
        }
        public PromptControllerResult WriteScriptureTenant(ActionArgs args)
        {
            args.playerProphet.WriteScripture(args.random, new List<Tenant>() { args.WritingTenant }, new List<Religion>(), 3, args.TypedText);
            return GoTo(PromptViews.ScriptureWritten(args), args);
        }

        public static PromptControllerResult GoTo( TextPrompt textPrompt, ActionArgs actionArgs)
        {
            return new PromptControllerResult() { ActionResult = actionArgs.playerProphet.ActionResult, TextPrompt = textPrompt, ActionArgs = actionArgs };
        }
    }


}
