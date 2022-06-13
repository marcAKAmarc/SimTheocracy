using ST.SimModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ST.GameModel;

namespace ST.FrontEndConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var Game = new Game();
            Game.Setup();

            List<FlowEngine> PlayerEngines = new List<FlowEngine>();
            foreach(var prophet in Game.Prophets)
            {
                if(prophet.Id == Game.HumanPlayer.Id)
                {
                    PlayerEngines.Add(new FlowEngine(Game, prophet, FlowType.Human, new PromptControllers(Game)));
                }
                else
                {
                    PlayerEngines.Add(new FlowEngine(Game, prophet, FlowType.NPC_Observable, new PromptControllers(Game)));
                }
            }

            while (true)
            {
                PlayerEngines.Shuffle(Game.Random);
                foreach(var playerengine in PlayerEngines)
                {
                    playerengine.Step();
                }
                foreach(var playerengine in PlayerEngines)
                {
                    playerengine.Reset();
                }
            }
            
            
        }
    }

    public static class ListExt
    {
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
