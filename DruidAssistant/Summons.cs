using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace DruidAssistant
{
    public class Summons : List<Summon>
    {
        public static Summons Retrieve(string xmlPath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Summons));

            using (Stream inputStream = File.OpenRead(xmlPath))
            {
                return (Summons)xs.Deserialize(inputStream);
            }
        }
    }

    public class SummonToForm
    {
        public int Index { get; set; }
        public string SummonSpell { get; set; }

        public int PCCasterLevel { get; set; }
        public int Rounds { get; set; }

        public string Name { get; set; }

        public Abilities Abilities { get; set; }
        public Abilities BaseAbilities { get; set; }

        public Size Size { get; set; }
        public string Type { get; set; }

        public string Environment { get; set; }

        public int Level { get; set; }
        public int HitDie { get; set; }
        public int CurrentHP { get; set; }

        public int Initiative { get; set; }

        public string Movement { get; set; }

        public int NaturalArmor { get; set; }

        public int BAB { get; set; }

        public int MiscGrapple { get; set; }
        public int MiscMelee { get; set; }
        public int MiscRange { get; set; }

        public List<Attack> Attacks { get; set; } = new List<Attack>();
        public List<FullAttack> FullAttacks { get; set; } = new List<FullAttack>();

        public List<SkillMod> Skills { get; set; } = new List<SkillMod>();
        public List<string> Feats { get; set; } = new List<string>();
        public List<string> SpecAtks { get; set; } = new List<string>();
        public List<string> SpecQual { get; set; } = new List<string>();
        public List<string> Notes { get; set; } = new List<string>();

        public string Space { get; set; }
        public string Reach { get; set; }

        public Saves Saves { get; set; }
        public int MiscFort { get; set; }
        public int MiscRef { get; set; }
        public int MiscWill { get; set; }

        public SummonToForm()
        {

        }

        public SummonToForm(Summon s)
        {
            Index = s.Index;
            SummonSpell = string.Copy(s.SummonSpell);

            Name = s.Name;
            Size = s.Size;
            Type = s.Type;
            Abilities = new Abilities();
            Abilities.Str = s.Abilities.Str;
            Abilities.Dex = s.Abilities.Dex;
            Abilities.Con = s.Abilities.Con;
            Abilities.Int = s.Abilities.Int;
            Abilities.Wis = s.Abilities.Wis;
            Abilities.Cha = s.Abilities.Cha;

            BaseAbilities = new Abilities();
            BaseAbilities.Str = s.Abilities.Str;
            BaseAbilities.Dex = s.Abilities.Dex;
            BaseAbilities.Con = s.Abilities.Con;
            BaseAbilities.Int = s.Abilities.Int;
            BaseAbilities.Wis = s.Abilities.Wis;
            BaseAbilities.Cha = s.Abilities.Cha;

            NaturalArmor = s.NaturalArmor;
            Saves = new Saves();
            Saves.Fort = s.Saves.Fort;
            Saves.Ref = s.Saves.Ref;
            Saves.Will = s.Saves.Will;

            Saves.Fort -= s.Abilities.ConMod;
            Saves.Ref -= s.Abilities.DexMod;
            Saves.Will -= s.Abilities.WisMod;

            Environment = s.Environment;

            Movement = s.Movement;

            Space = s.Space;
            Reach = s.Reach;

            Attacks = new List<Attack>(s.Attacks);
            FullAttacks = new List<FullAttack>(s.FullAttacks);

            Skills = FileParser.GetSkillMods(s.Skills);

            for (int i = 0; i < s.Skills.Count; i++)
            {
                Skills[i] = FileParser.CorrectSkillModBackward(Skills[i], BaseAbilities);
            }

            Feats = new List<string>(s.Feats);
            SpecAtks = new List<string>(s.SpecAtks);
            SpecQual = new List<string>(s.SpecQual);
            Notes = new List<string>(s.Notes);
            BAB = s.BAB;
            MiscGrapple = s.Grapple - s.BAB - s.Abilities.StrMod - DruidMain.GetGrappleSizeMod(s.Size);
            HitDie = s.HitDie;
            Level = s.Level;
        }
    }

    public class Summon
    {
        public int Index { get; set; }

        public string SummonSpell { get; set; }

        public string Name { get; set; }

        public Abilities Abilities { get; set; }

        public Size Size { get; set; }
        public string Type { get; set; }

        public string Environment { get; set; }

        public int Level { get; set; }
        public int HitDie { get; set; }

        public string Movement { get; set; }

        public int NaturalArmor { get; set; }

        public int BAB { get; set; }
        public int Grapple { get; set; }

        public List<Attack> Attacks { get; set; }
        public List<FullAttack> FullAttacks { get; set; }

        public List<string> Skills = new List<string>();
        public List<string> Feats = new List<string>();
        public List<string> SpecAtks = new List<string>();
        public List<string> SpecQual = new List<string>();
        public List<string> Notes = new List<string>();

        public string Space { get; set; }
        public string Reach { get; set; }

        public Saves Saves { get; set; }
    }

    public enum Size
    {
        Fine,
        Diminuitive,
        Tiny,
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan,
        Colossal
    }

    public class Abilities
    {
        public int Str { get; set; }
        public int StrMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Str));
            }
        }
        public int Dex { get; set; }
        public int DexMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Dex));
            }
        }
        public int Con { get; set; }
        public int ConMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Con));
            }
        }
        public int Int { get; set; }
        public int IntMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Int));
            }
        }
        public int Wis { get; set; }
        public int WisMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Wis));
            }
        }
        public int Cha { get; set; }
        public int ChaMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Cha));
            }
        }

        public Abilities()
        {

        }

        public Abilities(int s, int d, int co, int i, int w, int ch)
        {
            Str = s;
            Dex = d;
            Con = co;
            Int = i;
            Wis = w;
            Cha = ch;
        }

        private static decimal ScoreToMod(decimal input)
        {
            return Math.Floor((input - 10) / 2);
        }
    }

    public class Attack
    {
        public int Count { get; set; }
        public string Name { get; set; }
        public string Extra { get; set; }

        public Attack()
        {

        }

        public Attack(int c, string n, string e)
        {
            Count = c;
            Name = n;
            Extra = e;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} ({2})", Count, Name, Extra);
        }
    }

    public class FullAttack : List<Attack>
    {
        public FullAttack()
        {

        }

        public override string ToString()
        {
            string builder = "";
            for (int i = 0; i < this.Count; i++)
            {
                if (i == 0)
                {
                    builder = this[i].ToString();
                }
                else
                {
                    builder += " and " + this[i].ToString();
                }
            }
            return builder;
        }
    }

    public class Saves
    {
        public int Fort { get; set; }
        public int Ref { get; set; }
        public int Will { get; set; }

        public Saves()
        {

        }
        public Saves(int f, int r, int w)
        {
            Fort = f;
            Ref = r;
            Will = w;
        }
    }

    public class SkillMod
    {
        public Skill Skill { get; set; }
        public string Addition { get; set; }
        public int Bonus { get; set; }
        public int NewBonus { get; set; }
        public AlternateAbilityMods AlternateAbility { get; set; } = new AlternateAbilityMods();
        public AbilityMod DefMod { get; set; }
        //public AbilityMod? ModStr { get; set; }
        //public AbilityMod? ModDex { get; set; }
        //public AbilityMod? ModCon { get; set; }
        //public AbilityMod? ModInt { get; set; }
        //public AbilityMod? ModWis { get; set; }
        //public AbilityMod? ModCha { get; set; }
        public bool Asterisk { get; set; }

        public SkillMod()
        {

        }

        //public SkillMod(Skill skill, string addition, int bonus, AbilityMod defmod, AbilityMod? modstr, AbilityMod? moddex, AbilityMod? modcon, AbilityMod? modint, AbilityMod? modwis, AbilityMod? modcha, bool ast)
        //{
        //    Skill = skill;
        //    Addition = addition;
        //    Bonus = bonus;
        //    DefMod = defmod;
        //    ModStr = modstr;
        //    ModDex = moddex;
        //    ModCon = modcon;
        //    ModInt = modint;
        //    ModWis = modwis;
        //    ModCha = modcha;
        //    Asterisk = ast;
        //}

        public SkillMod(Skill skill, string addition, int bonus, AlternateAbilityMods alternates, bool ast)
        {
            Skill = skill;
            Addition = addition;
            Bonus = bonus;
            DefMod = DefaultSkillToAbilityMod[skill];
            AlternateAbility.STR = alternates.STR;
            AlternateAbility.DEX = alternates.DEX;
            AlternateAbility.CON = alternates.CON;
            AlternateAbility.INT = alternates.INT;
            AlternateAbility.WIS = alternates.WIS;
            AlternateAbility.CHA = alternates.CHA;
            Asterisk = ast;
        }

        public override string ToString()
        {
            return string.Format("{0}{1} {2}{3} {4}", NewBonus < 0 ? "-" : "+", Math.Abs(NewBonus), Skill, Asterisk ? "*" : "", Addition == "" ? "" : Addition);
        }

        public readonly static Dictionary<Skill, AbilityMod> DefaultSkillToAbilityMod = new Dictionary<Skill, AbilityMod>()
        {
            {Skill.Appraise,          AbilityMod.Int },
            {Skill.Balance,           AbilityMod.Dex },
            {Skill.Bluff,             AbilityMod.Cha },
            {Skill.Climb,             AbilityMod.Str },
            {Skill.Concentration,     AbilityMod.Con },
            {Skill.Craft,             AbilityMod.Int },
            {Skill.DecipherScript,    AbilityMod.Int },
            {Skill.Diplomacy,         AbilityMod.Cha },
            {Skill.DisableDevice,     AbilityMod.Int },
            {Skill.Disguise,          AbilityMod.Cha },
            {Skill.EscapeArtist,      AbilityMod.Dex },
            {Skill.Forgery,           AbilityMod.Int },
            {Skill.GatherInformation, AbilityMod.Cha },
            {Skill.HandleAnimal,      AbilityMod.Cha },
            {Skill.Heal,              AbilityMod.Wis },
            {Skill.Hide,              AbilityMod.Dex },
            {Skill.Intimidate,        AbilityMod.Cha },
            {Skill.Jump,              AbilityMod.Str },
            {Skill.Knowledge,         AbilityMod.Int },
            {Skill.Listen,            AbilityMod.Wis },
            {Skill.MoveSilently,      AbilityMod.Dex },
            {Skill.OpenLock,          AbilityMod.Dex },
            {Skill.Perform,           AbilityMod.Cha },
            {Skill.Profession,        AbilityMod.Wis },
            {Skill.Ride,              AbilityMod.Dex },
            {Skill.Search,            AbilityMod.Int },
            {Skill.SenseMotive,       AbilityMod.Wis },
            {Skill.SleightofHand,     AbilityMod.Dex },
            {Skill.Spellcraft,        AbilityMod.Int },
            {Skill.Spot,              AbilityMod.Wis },
            {Skill.Survival,          AbilityMod.Wis },
            {Skill.Swim,              AbilityMod.Str },
            {Skill.Tumble,            AbilityMod.Dex },
            {Skill.UseMagicDevice,AbilityMod.Cha },
            {Skill.UseRope, AbilityMod.Dex }
        };
    }

    public enum Skill
    {
        Appraise,
        Balance,
        Bluff,
        Climb,
        Concentration,
        ControlShape,
        Craft,
        DecipherScript,
        Diplomacy,
        DisableDevice,
        Disguise,
        EscapeArtist,
        Forgery,
        GatherInformation,
        HandleAnimal,
        Heal,
        Hide,
        Intimidate,
        Jump,
        Knowledge,
        Listen,
        MoveSilently,
        OpenLock,
        Perform,
        Profession,
        Ride,
        Search,
        SenseMotive,
        SleightofHand,
        SpeakLanguage,
        Spellcraft,
        Spot,
        Survival,
        Swim,
        Tumble,
        UseMagicDevice,
        UseRope
    }

    public enum AbilityMod
    {
        Str,
        Dex,
        Con,
        Int,
        Wis,
        Cha
    }

    public class AlternateAbilityMods
    {
        public AbilityMod? STR { get; set; }
        public AbilityMod? DEX { get; set; }
        public AbilityMod? CON { get; set; }
        public AbilityMod? INT { get; set; }
        public AbilityMod? WIS { get; set; }
        public AbilityMod? CHA { get; set; }
    }
}

/*
AC AND Bonus ATK
Fine: +8
Diminutive: +4
Tiny: +2
Small: +1
Medium: 0
Large: –1
Huge: –2
Gargantuan: –4
Colossal: –8
 */
