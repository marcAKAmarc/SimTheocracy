using ST.SimModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        
        
        public void DayActivity()
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

                if (currentPrompt.Options != null && currentPrompt.Options.Any())
                    ProcessOptionResponse();
                else if (currentPrompt.TakeTypedInput)
                    ProcessTextResponse();
                else
                    ProcessAction();
            }
        }

        public void EndOfDayReport()
        {
            var result = PromptControllers.EndOfDayReport(ActionArgs);
            ActionArgs = result.ActionArgs;
            DisplayView(
                PromptViews.EndOfDayReport(ActionArgs)
            );
            if (currentPrompt.Options != null && currentPrompt.Options.Any())
                ProcessOptionResponse();
            else if (currentPrompt.TakeTypedInput)
                ProcessTextResponse();
            else
                ProcessAction();
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

        public void ProcessAction()
        {
            if(FlowType == FlowType.Human)
                Console.ReadLine();
            if (FlowType == FlowType.NPC_Observable)
                Thread.Sleep(4000);

            PromptControllerResult result = null;
            if (currentPrompt.Action != null)
                result = currentPrompt.Action(ActionArgs);
            else
                result = null;

            if (result == null)
                currentPrompt = null;
            else
                currentPrompt = result.TextPrompt;
        }

        public void ProcessTextResponse()
        {
            string response;
            if(FlowType == FlowType.Human)
            {
                response = ProcessTextResponseHuman();
            }
            else
            {
                if (FlowType == FlowType.NPC_Observable)
                    Thread.Sleep(4000);
                response = ProcessTextResponseNPC();
            }

            ActionArgs.TypedText = response;

            PromptControllerResult result;

            if (currentPrompt.TypedInputAction == null)
                result = null;
            else
                result = currentPrompt.TypedInputAction(ActionArgs);

            if (result == null)
                currentPrompt = null;
            else
                currentPrompt = result.TextPrompt;
        }
        public void ProcessOptionResponse()
        {
            int response;

            if (FlowType == FlowType.Human)
                response = ProcessOptionResponseHuman();
            else
            {
                if (FlowType == FlowType.NPC_Observable)
                    Thread.Sleep(4000);
                response = ProcessOptionResponseNpc(Game.Random);
            }
            PromptControllerResult result;
            if (currentPrompt.Options?.Count >= 1 && currentPrompt.Options[response].Action != null)
            {
                result = currentPrompt.Options[response].Action(ActionArgs);
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

        public int ProcessOptionResponseHuman()
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

        public string ProcessTextResponseHuman()
        {
            return Console.ReadLine();
        }

        public string ProcessTextResponseNPC()
        {
            return "Toilet Data";
        }

        public int ProcessOptionResponseNpc(Random random)
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
            Console.WriteLine(WordWrap(value, 100));

            
        }

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        public static string WordWrap(string text, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return text;

            // Parse each line of text
            for (pos = 0; pos < text.Length; pos = next)
            {
                // Find end of line
                int eol = text.IndexOf(Environment.NewLine, pos);
                if (eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + Environment.NewLine.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;
                        if (len > width)
                            len = BreakLine(text, pos, width);
                        sb.Append(text, pos, len);
                        sb.Append(Environment.NewLine);

                        // Trim whitespace following break
                        pos += len;
                        while (pos < eol && Char.IsWhiteSpace(text[pos]))
                            pos++;
                    } while (eol > pos);
                }
                else sb.Append(Environment.NewLine); // Empty line
            }
            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        private static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;

            // If no whitespace found, break at maximum length
            if (i < 0)
                return max;

            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;
        }

    }
}
