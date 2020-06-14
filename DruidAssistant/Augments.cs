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
                st.MiscGrapple = 4;
                st.SpecQual.Add("Resist Cold and Electricity (Ex): +10");
                st.SpecQual.Add("Tremorsense");

                st.Abilities.Str += 6;
                st.Abilities.Dex += 2;
                st.Abilities.Con += 4;
                st.Abilities.Cha += 4;

                st.Environment = "Any forests";

                for (int i = 0; i < st.Skills.Count; i++)
                {
                    if (st.Skills[i].Skill == Skill.Hide)
                    {
                        st.Skills[i].Asterisk = true;
                    }
                    if (st.Skills[i].Skill == Skill.MoveSilently)
                    {
                        st.Skills[i].Asterisk = true;
                    }
                }

                st.Notes.Add("Hide +16 (RB) (forest)");
                st.Notes.Add("Move Silently +16 (RB) (forest)");
                st.Notes.Add("Natural weapons treated as magic weapons for overcoming dmg reduction.");
                st.Notes.Add("Tremorsense(EX): Auto sense anything w/in 60' in contact with the ground.");
                return st;
            }

            public static string GreenboundSlamDamage(Size size)
            {
                switch (size)
                {
                    case Size.Fine:
                        return "1";
                    case Size.Diminuitive:
                        return "1d2";
                    case Size.Tiny:
                        return "1d3";
                    case Size.Small:
                        return "1d4";
                    case Size.Medium:
                        return "1d6";
                    case Size.Large:
                        return "1d8";
                    case Size.Huge:
                        return "2d6";
                    case Size.Gargantuan:
                        return "2d8";
                    case Size.Colossal:
                        return "4d6";
                    default:
                        return "Can't Calculate Damage";
                }
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
