using ST.SimModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.GameModel
{
    public class PromptModels
    {
        public delegate PromptControllerResult ProphetAction(ActionArgs Args);
        public delegate TextPrompt PromptView(ActionArgs args, ActionResult data);

        public class PromptControllerResult
        {
            public TextPrompt TextPrompt;
            public ActionResult ActionResult;
            public ActionArgs ActionArgs;
        }

        public class PromptBase
        {
            public Prophet Prophet;

            public PromptBase(Prophet prophet)
            {
                Prophet = prophet;
            }
        }

        public class TextPrompt : PromptBase
        {
            public string Text;
            public List<TextOption> Options;

            public bool TakeTypedInput;
            public ProphetAction TypedInputAction;

            public TextPrompt(Prophet prophet) : base(prophet) { }

            public ProphetAction Action;
        }

        public class TextOption : PromptBase
        {
            public string Text;

            public int RequireFollowers;
            public int RequireDivinity;
            public int RequireDonations;

            public bool HideIfRequirementNotMet = true;

            public ProphetAction Action;

            public TextOption(Prophet prophet) : base(prophet) { }
        }

        public class ActionArgs
        {
            public PromptControllers controller;

            public Random random;
            public Prophet playerProphet;

            public List<Prophet> proselytizingProphets;

            public Prophet proselytizingProphet;
            public Base Learned;

            public Religion WritingRenouncedReligion;
            public Tenant WritingTenant;
            public bool WritingExperience;

            public string TypedText;

            public List<Person> Followers;

            public List<Religion> DebugAllReligions;
            public List<Person> DebugAllPeople;

            public void Reset()
            {
                proselytizingProphets = new List<Prophet>();
                proselytizingProphet = null;
                Learned = null;

                WritingRenouncedReligion = null;
                WritingTenant = null;
                WritingExperience = false;

                TypedText = "";
            }
        }
    }
}
