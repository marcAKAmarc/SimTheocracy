using ST.SimModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ST.GameModel.PromptModels;

namespace ST.GameModel
{
    public enum FlowType { Human, NPC, NPC_Observable}
    public class FlowEngine
    {
        public Game Game;
        public PromptControllers PromptControllers;

        public TextPrompt currentPrompt;
        public ActionArgs ActionArgs;

        public Prophet Player; //can be both npc and human
        public FlowType FlowType; 
        public FlowEngine(Game game, Prophet player, FlowType flowType,  PromptControllers promptControllers)
        {
            Game = game;
            PromptControllers = promptControllers;
            Player = player;
            FlowType = flowType;
            ActionArgs = new ActionArgs() { playerProphet = Player, random = Game.Random, controller = PromptControllers };

        }

        public void Go()
        {
            while (true)
            {
                Step();
            }
        }

        public void Step()
        {
            if (currentPrompt == null)
            {
                var result = PromptControllers.NewDay(ActionArgs);
                ActionArgs = result.ActionArgs;
                DisplayView(
                    PromptViews.NewDay(ActionArgs)
                );
            }
            while (currentPrompt != null) {
                
                DisplayView(
                    currentPrompt
                );

                ProcessResponse();
            }
        }

        public void Reset()
        {
            ActionArgs.Reset();
            ActionArgs.playerProphet.GoHome();
        }

        public void DisplayView(TextPrompt prompt)
        {
            currentPrompt = prompt;

            if (FlowType == FlowType.NPC)
                return;

            Console.Clear();
            NiceWriter.WriteLine(prompt.Text);
            for (var i = 0; i < prompt.Options?.Count; i++)
            {
                NiceWriter.WriteLine(i + ":  " + prompt.Options[i].Text);
            }
        }

        public void ProcessResponse()
        {
            int response;
            if (FlowType == FlowType.Human)
                response = ProcessResponseHuman();
            else
                response = ProcessResponseNpc(Game.Random);

            PromptControllerResult result;
            if (currentPrompt.Options?.Count >= 1 && currentPrompt.Options[response].action != null)
            {
                result = currentPrompt.Options[response].action(ActionArgs);
            }
            else
            {
                result = null;//PromptControllers.NewDay(ActionArgs);
            }

            if (result == null)
                currentPrompt = null;
            else
                currentPrompt = result.TextPrompt;
        }

        public int ProcessResponseHuman()
        {
            int response = -1;
            string typed = Console.ReadLine();
            if (currentPrompt.Options?.Count >= 1)
            {
                var parsed = int.TryParse(typed, out response);
                while (true)
                {

                    if (parsed == true && response >= 0 && response < currentPrompt.Options.Count)
                        break;
                    NiceWriter.WriteLine("Invalid response.  Please retry.");
                    typed = Console.ReadLine();
                    parsed = int.TryParse(typed, out response);
                }
            }
            return response;
        }

        public int ProcessResponseNpc(Random random)
        {
            int response = -1;
            if(currentPrompt.Options?.Count >= 1)
                response = random.Next(currentPrompt.Options.Count);
            return 1;
            //return response;
        }
    }

    public static class NiceWriter 
    {
        

        public static void WriteLine(string value)
        {
            Console.WriteLine(WordWrap(value));

            
        }

        public static string WordWrap(string text)
        {
            String[] words = text.Split(' ');
            StringBuilder buffer = new StringBuilder();

            foreach (String word in words)
            {
                buffer.Append(word);

                if (buffer.Length >= 80)
                {
                    String line = buffer.ToString().Substring(0, buffer.Length - word.Length);
                    Console.WriteLine(line);
                    buffer.Clear();
                    buffer.Append(word);
                }

                buffer.Append(" ");

            }
            return buffer.ToString();
        }
    }
}
