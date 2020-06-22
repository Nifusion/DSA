using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DruidAssistant
{
    public class Feats
    {
        public static SummonToForm SpellFocusConjuration(SummonToForm st)
        {
            st.PCCasterLevel++;
            return st;
        }

        public static SummonToForm AugmentSummoning(SummonToForm st)
        {
            st.Abilities.Str += 4;
            st.Abilities.Con += 4;
            return st;
        }

        public class GreenboundSummoning
        {
            public static SummonToForm Augment(SummonToForm st)
            {
                st.Type = string.Format("Plant (Augmented {0})", st.Type);
                st.HitDie = 8;

                st.NaturalArmor += 6;

                st.Attacks.Add(new Attack(1, "GB Slam", GreenboundSlamDamage(st.Size).ToString()));

                st.SpecAtks.Add("Entangle");
                st.SpecAtks.Add("Pass without Trace");
                st.SpecAtks.Add("Speak with Plants");
                st.SpecAtks.Add("Wall of Thorns (1/day)");

                st.SpecQual.Add("DR 10/magic and slashing.");
                st.SpecQual.Add("Fast Healing (Ex): +3/round");
                st.SpecQual.Add("Natural weapons → magic weapons");
                st.SpecQual.Add("Grapple Bonus (Ex): +4");
                st.MiscGrapple += 4;
                st.SpecQual.Add("Resist Cold and Electricity (Ex): +10");
                st.SpecQual.Add("Tremorsense");

                st.Abilities.Str += 6;
                st.Abilities.Dex += 2;
                st.Abilities.Con += 4;
                st.Abilities.Cha += 4;

                st.Environment = "Any forests";

                int hideIndex = st.Skills.FindIndex(x => x.Skill == Skill.Hide);
                int msIndex = st.Skills.FindIndex(x => x.Skill == Skill.MoveSilently);

                if (hideIndex > -1) { st.Skills[hideIndex].Asterisk = true; }
                if (msIndex > -1) { st.Skills[msIndex].Asterisk = true; }

                st.Notes.Add("Hide +16 (RB) (forest)");
                st.Notes.Add("Move Silently +16 (RB) (forest)");
                st.Notes.Add("Natural weapons treated as magic weapons for overcoming dmg reduction.");
                st.Notes.Add("Tremorsense(EX): Auto sense anything w/in 60' in contact with the ground.");
                return st;
            }

            public static string GreenboundSlamDamage(Size size)
            {
                return new[] { "1", "1d2", "1d3", "1d4", "1d6", "1d8", "2d6", "2d8", "4d6" }[Enum.GetValues(typeof(Size)).Cast<Size>().ToList().IndexOf(size)];
            }
        }
    }

    public class Items
    {
        public static SummonToForm ObadHaisGreenMan(SummonToForm st)
        {
            st.PCCasterLevel += 2;
            return st;
        }
    }
}
