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
        public int Index;
        public string SummonSpell;

        public int PCCasterLevel;
        public int Rounds;

        public string Name;

        public Abilities Abilities;
        public Abilities BaseAbilities;

        public Size Size;
        public string Type;

        public string Environment;

        public int Level;
        public int HitDie;
        public int CurrentHP;

        public int Initiative;

        public string Movement;

        public int NaturalArmor;

        public int BAB;

        public int MiscGrapple;
        public int MiscMelee;
        public int MiscRange;

        public List<Attack> Attacks = new List<Attack>();
        public List<FullAttack> FullAttacks = new List<FullAttack>();

        public List<SkillMod> Skills = new List<SkillMod>();
        public List<string> Feats = new List<string>();
        public List<string> SpecAtks = new List<string>();
        public List<string> SpecQual = new List<string>();
        public List<string> Notes = new List<string>();

        public string Space;
        public string Reach;

        public Saves Saves;
        public int MiscFort;
        public int MiscRef;
        public int MiscWill;

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
        public int Index{ get; set; }

        public string SummonSpell{ get; set; }

        public string Name{ get; set; }

        public Abilities Abilities{ get; set; }

        public Size Size{ get; set; }
        public string Type{ get; set; }

        public string Environment{ get; set; }

        public int Level{ get; set; }
        public int HitDie{ get; set; }

        public string Movement{ get; set; }

        public int NaturalArmor{ get; set; }

        public int BAB{ get; set; }
        public int Grapple{ get; set; }

        public List<Attack> Attacks{ get; set; }
        public List<FullAttack> FullAttacks{ get; set; }

        public List<string> Skills = new List<string>()  ;
        public List<string> Feats = new List<string>()   ;
        public List<string> SpecAtks = new List<string>();
        public List<string> SpecQual = new List<string>();
        public List<string> Notes = new List<string>();

        public string Space{ get; set; }
        public string Reach{ get; set; }

        public Saves Saves{ get; set; }
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
        public int Str;
        public int StrMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Str));
            }
        }
        public int Dex;
        public int DexMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Dex));
            }
        }
        public int Con;
        public int ConMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Con));
            }
        }
        public int Int;
        public int IntMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Int));
            }
        }
        public int Wis;
        public int WisMod
        {
            get
            {
                return Convert.ToInt32(ScoreToMod(Wis));
            }
        }
        public int Cha;
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
        public int Count;
        public string Name;
        public string Extra;

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
        public int Fort;
        public int Ref;
        public int Will;

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
        public Skill Skill;
        public string Addition;
        public int Bonus;
        public int NewBonus;
        public AbilityMod DefMod;
        public AbilityMod? ModStr;
        public AbilityMod? ModDex;
        public AbilityMod? ModCon;
        public AbilityMod? ModInt;
        public AbilityMod? ModWis;
        public AbilityMod? ModCha;
        public bool Asterisk;

        public SkillMod()
        {

        }

        public SkillMod(Skill skill, string addition, int bonus, AbilityMod defmod, AbilityMod? modstr, AbilityMod? moddex, AbilityMod? modcon, AbilityMod? modint, AbilityMod? modwis, AbilityMod? modcha, bool ast)
        {
            Skill = skill;
            Addition = addition;
            Bonus = bonus;
            DefMod = defmod;
            ModStr = modstr;
            ModDex = moddex;
            ModCon = modcon;
            ModInt = modint;
            ModWis = modwis;
            ModCha = modcha;
            Asterisk = ast;
        }

        public override string ToString()
        {
            return string.Format("{0}{1} {2}{3} {4}", NewBonus < 0 ? "-" : "+", Math.Abs(NewBonus), Skill, Asterisk ? "*" : "", Addition == "" ? "" : Addition);
        }
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
