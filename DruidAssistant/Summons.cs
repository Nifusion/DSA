using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace DruidAssistant
{
    public static class SummonExtensions
    {
        public static void Save(this Summons spells, string xmlPath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Summons));

            using (StreamWriter outputStream = new StreamWriter(xmlPath, false))
            {
                xs.Serialize(outputStream, spells);
            }
        }
    }

    public class Summons : List<Summon>
    {
        public static Summons Retrieve(string xmlPath)
        {
            using (Stream inputStream = File.OpenRead(xmlPath))
            {
                return (Summons)new XmlSerializer(typeof(Summons)).Deserialize(inputStream);
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
            Abilities = new Abilities()
            {
                Str = s.Abilities.Str,
                Dex = s.Abilities.Dex,
                Con = s.Abilities.Con,
                Int = s.Abilities.Int,
                Wis = s.Abilities.Wis,
                Cha = s.Abilities.Cha
            };

            BaseAbilities = new Abilities()
            {
                Str = s.Abilities.Str,
                Dex = s.Abilities.Dex,
                Con = s.Abilities.Con,
                Int = s.Abilities.Int,
                Wis = s.Abilities.Wis,
                Cha = s.Abilities.Cha
            };

            NaturalArmor = s.NaturalArmor;
            Feats = new List<string>(s.Feats);
            SpecAtks = new List<string>(s.SpecAtks);
            SpecQual = new List<string>(s.SpecQual);

            Saves = new Saves()
            {
                Fort = s.Saves.Fort,
                Ref = s.Saves.Ref,
                Will = s.Saves.Will
            };

            Saves.Fort -= s.Abilities.ConMod;
            Saves.Ref -= s.Abilities.DexMod;
            Saves.Will -= s.Abilities.WisMod;

            if (Feats.Any(x => x.ToUpper() == "IRON WILL")) { Saves.Will -= 2; MiscWill += 2; }

            Environment = s.Environment;

            Movement = s.Movement;

            Space = s.Space;
            Reach = s.Reach;

            Attacks = new List<Attack>(s.Attacks);
            FullAttacks = new List<FullAttack>(s.FullAttacks);

            Skills = Summon.GetSkillMods(s.Skills);

            for (int i = 0; i < s.Skills.Count; i++)
            {
                Skills[i] = Summon.CorrectSkillModBackward(Skills[i], BaseAbilities);
            }

            Notes = new List<string>(s.Notes);
            BAB = s.BAB;
            MiscGrapple = s.Grapple - s.BAB - s.Abilities.StrMod - Summon.GetGrappleSizeMod(s.Size);
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
        public List<string> Skills { get; set; } = new List<string>();
        public List<string> Feats { get; set; } = new List<string>();
        public List<string> SpecAtks { get; set; } = new List<string>();
        public List<string> SpecQual { get; set; } = new List<string>();
        public List<string> Notes { get; set; } = new List<string>();
        public string Space { get; set; }
        public string Reach { get; set; }
        public Saves Saves { get; set; }

        public static int GetSizeMod(Size size)
        {
            return new[] { 8, 4, 2, 1, 0, -1, -2, -4, -8 }[Enum.GetValues(typeof(Size)).Cast<Size>().ToList().IndexOf(size)];
        }

        public static int GetGrappleSizeMod(Size size)
        {
            return new[] { -16, -12, -8, -4, 0, 4, 8, 12, 16 }[Enum.GetValues(typeof(Size)).Cast<Size>().ToList().IndexOf(size)];
        }

        public static Summon GetSummonTemplateFromText(string text)
        {
            Summon thisTemplate = new Summon();
            string name = "";
            string sizetype = "";
            string hitdie = "";
            string speed = "";
            string armor = "";
            string babgrapple = "";
            string attacks = "";
            string fullattacks = "";
            string spacereach = "";
            string specatk = "";
            string specqual = "";
            string saves = "";
            string abilities = "";
            string skills = "";
            string feats = "";
            string environment = "";

            string[] textArray = text.Replace("\r\n", "~").Replace("\r", "~").Replace("\n", "~").Split('~');
            bool combat = false;
            for (int i = 0; i < textArray.Length; i++)
            {
                string line = textArray[i];
                line = line.Replace("–", "-");
                if (!combat)
                {
                    if (i == 0 || line.Contains("click to see monster")) { name = line; }
                    if (line.StartsWith("Size/Type")) { sizetype = line; }
                    if (line.StartsWith("Hit Dice")) { hitdie = line; }
                    if (line.StartsWith("Speed")) { speed = line; }
                    if (line.StartsWith("Armor Class")) { armor = line; }
                    if (line.StartsWith("Base Attack")) { babgrapple = line; }
                    if (line.StartsWith("Attack")) { attacks = line; }
                    if (line.StartsWith("Full Attack")) { fullattacks = line; }
                    if (line.StartsWith("Space")) { spacereach = line; }
                    if (line.StartsWith("Special Attacks")) { specatk = line; }
                    if (line.StartsWith("Special Qualities")) { specqual = line; }
                    if (line.StartsWith("Saves")) { saves = line; }
                    if (line.StartsWith("Abilities")) { abilities = line; }
                    if (line.StartsWith("Skills:")) { skills = line; }
                    if (line.StartsWith("Feats")) { feats = line; }
                    if (line.StartsWith("Environment")) { environment = line; }
                    if (line.ToUpper().StartsWith("COMBAT")) { combat = true; }
                }
                else
                {
                    thisTemplate.Notes.Add(line);
                }
            }

            thisTemplate.Name = InputToFirstCharUpper(name.Replace(" click to see monster", ""));
            thisTemplate.Size = GetSize(sizetype, out string type);
            thisTemplate.Type = type;
            thisTemplate.Abilities = GetAbilities(abilities);

            thisTemplate.NaturalArmor = GetNaturalArmor(armor);
            thisTemplate.Saves = GetSaves(saves);

            thisTemplate.Environment = GetEnvironment(environment);

            thisTemplate.Movement = GetSpeeds(speed);

            thisTemplate.Space = GetSpaceReach(spacereach, out string reach);
            thisTemplate.Reach = reach;

            thisTemplate.Attacks = GetAttacks(attacks);
            thisTemplate.FullAttacks = GetFullAttacks(fullattacks);

            thisTemplate.Skills = GetSkills(skills);

            thisTemplate.Feats = GetFeats(feats);
            thisTemplate.SpecAtks = GetSpecAtk(specatk);
            thisTemplate.SpecQual = GetSpecQual(specqual);
            thisTemplate.BAB = GetBABGrapple(babgrapple, out int grapple);
            thisTemplate.Grapple = grapple;
            thisTemplate.HitDie = GetHitDie(hitdie, out int level);
            thisTemplate.Level = level;

            return thisTemplate;
        }

        private static string InputToFirstCharUpper(string input)
        {
            return input != "" ? input.First().ToString().ToUpper() + input.Substring(1).ToLower() : "";
        }

        public static SkillMod CorrectSkillModBackward(SkillMod sm, Abilities a)
        {
            List<int> mods = new List<int>();

            if (sm.DefMod == AbilityMod.Str || sm.AlternateAbility.STR.HasValue) { mods.Add(a.StrMod); }
            else if (sm.DefMod == AbilityMod.Dex || sm.AlternateAbility.DEX.HasValue) { mods.Add(a.DexMod); }
            else if (sm.DefMod == AbilityMod.Con || sm.AlternateAbility.CON.HasValue) { mods.Add(a.ConMod); }
            else if (sm.DefMod == AbilityMod.Int || sm.AlternateAbility.INT.HasValue) { mods.Add(a.IntMod); }
            else if (sm.DefMod == AbilityMod.Wis || sm.AlternateAbility.WIS.HasValue) { mods.Add(a.WisMod); }
            else if (sm.DefMod == AbilityMod.Cha || sm.AlternateAbility.CHA.HasValue) { mods.Add(a.ChaMod); }

            sm.Bonus -= mods.Max();

            return sm;
        }

        public static SkillMod CorrectSkillModForward(SkillMod sm, Abilities a)
        {
            List<int> mods = new List<int>();

            if (sm.DefMod == AbilityMod.Str || sm.AlternateAbility.STR.HasValue) { mods.Add(a.StrMod); }
            else if (sm.DefMod == AbilityMod.Dex || sm.AlternateAbility.DEX.HasValue) { mods.Add(a.DexMod); }
            else if (sm.DefMod == AbilityMod.Con || sm.AlternateAbility.CON.HasValue) { mods.Add(a.ConMod); }
            else if (sm.DefMod == AbilityMod.Int || sm.AlternateAbility.INT.HasValue) { mods.Add(a.IntMod); }
            else if (sm.DefMod == AbilityMod.Wis || sm.AlternateAbility.WIS.HasValue) { mods.Add(a.WisMod); }
            else if (sm.DefMod == AbilityMod.Cha || sm.AlternateAbility.CHA.HasValue) { mods.Add(a.ChaMod); }

            sm.NewBonus = sm.Bonus + mods.Max();

            return sm;
        }

        public static string GetEnvironment(string env)
        {
            return env.Replace("Environment:", "").Replace("\t", "");
        }

        public static string GetSpeeds(string speed)
        {
            speed = Regex.Replace(speed.Replace("Speed:", "").Replace("\t", ""), @"\(([^)]+)\)", "");

            return string.Join(" | ", speed.Split(',').Select(x => x.TrimStart(' ').TrimEnd(' ')));
        }

        public static string GetSpaceReach(string spacereach, out string reach)
        {
            if (spacereach == "")
            {
                reach = "";
                return "";
            }

            string[] splitReplaced = spacereach.Replace("Space/Reach:", "").Replace("\t", "").Split('/');
            if (splitReplaced.Length != 2)
            {
                reach = "";
                return splitReplaced[0];
            }

            reach = splitReplaced[1];
            return splitReplaced[0];
        }

        public static List<Attack> GetAttacks(string attack)
        {
            List<Attack> attacks = new List<Attack>();

            if (attack == "")
            {
                return attacks;
            }

            string[] split = attack.Replace("Attack:", "").Replace("\t", "").Replace(" or ", "~").Replace("\n", "~").Split('~');
            for (int i = 0; i < split.Length; i++)
            {
                string[] splitByParentheses = Regex.Split(split[i], @"\(([^)]+)\)");
                List<string> tmp = new List<string>();
                for (int j = 0; j < splitByParentheses.Length; j++)
                {
                    if (splitByParentheses[j] != "")
                    {
                        tmp.Add(splitByParentheses[j].TrimStart(' ').TrimEnd(' '));
                    }
                }

                string weapon = Regex.Replace(tmp[0], "[+-][0-9]{1,2}", "");
                string inside = Regex.Replace(tmp[1], "[+-][0-9]{1,2}", "");
                int count = 1;
                Regex findCount = new Regex("[0-9]{1,2}");
                if (findCount.IsMatch(weapon))
                {
                    count = Convert.ToInt32(findCount.Match(weapon).Value);
                    weapon = findCount.Replace(weapon, "");
                }

                attacks.Add(new Attack(count, weapon.TrimStart(' ').TrimEnd(' ').Replace("  ", " "), inside));
            }

            return attacks;
        }

        public static List<FullAttack> GetFullAttacks(string fullAttack)
        {
            if (fullAttack == "")
            {
                return new List<FullAttack>();
            }
            List<FullAttack> fullAttacks = new List<FullAttack>();
            string[] splitor = fullAttack.Replace("Full Attack:", "").Replace("\t", "").Replace(" or ", "~").Replace("\n", "~").Split('~');

            for (int i = 0; i < splitor.Length; i++)
            {
                string[] splitand = splitor[i].Replace(" and ", "~").Split('~');
                FullAttack thisFull = new FullAttack();
                for (int j = 0; j < splitand.Length; j++)
                {
                    string[] splitparen = Regex.Split(splitand[j], @"\(([^)]+)\)");
                    List<string> tmp = new List<string>();
                    for (int k = 0; k < splitparen.Length; k++)
                    {
                        if (splitparen[k] != "")
                        {
                            tmp.Add(splitparen[k].TrimStart(' ').TrimEnd(' '));
                        }
                    }

                    string weapon = Regex.Replace(tmp[0], "[+-][0-9]{1,2}", "");
                    string inside = Regex.Replace(tmp[1], "[+-][0-9]{1,2}", "");
                    int count = 1;
                    Regex findCount = new Regex("[0-9]{1,2}");
                    if (findCount.IsMatch(weapon))
                    {
                        count = Convert.ToInt32(findCount.Match(weapon).Value);
                        weapon = findCount.Replace(weapon, "").TrimStart(' ');
                    }

                    thisFull.Add(new Attack(count, weapon.Replace("  ", " "), inside));
                }
                fullAttacks.Add(thisFull);
            }

            return fullAttacks;
        }

        public static List<SkillMod> GetSkillMods(List<string> skills)
        {
            List<SkillMod> s = new List<SkillMod>();
            for (int i = 0; i < skills.Count; i++)
            {
                SkillMod sm = GetSkill(skills[i]);
                if (skills[i] != null)
                {
                    s.Add(sm);
                }
            }
            return s;
        }

        public static List<string> GetSkills(string skills)
        {
            string tmp = skills.Replace("Skills:", "").Replace("\t", "");
            tmp = Regex.Replace(tmp, @",(?=[^()]*\))", " | ");
            string[] trimmers = tmp.Replace("  ", " ").Split(',');

            List<string> s = new List<string>();
            for (int i = 0; i < trimmers.Length; i++)
            {
                if (trimmers[i] != null)
                {
                    s.Add(trimmers[i].TrimStart(' ').TrimEnd(' '));
                }
            }
            return s;
        }

        public static List<string> GetSpecAtk(string spatk)
        {
            string tmp = spatk.Replace("Special Attacks:", "").Replace("\t", "");
            string[] trimmers = tmp.Split(',');

            List<string> sa = new List<string>();
            for (int i = 0; i < trimmers.Length; i++)
            {
                sa.Add(trimmers[i].Trim(' '));
            }
            return sa;
        }

        public static List<string> GetSpecQual(string spqual)
        {
            string tmp = spqual.Replace("Special Qualities:", "").Replace("\t", "");
            string[] trimmers = tmp.Split(',');

            List<string> sq = new List<string>();
            for (int i = 0; i < trimmers.Length; i++)
            {
                sq.Add(trimmers[i].Trim(' '));
            }
            return sq;
        }

        public static List<string> GetFeats(string feats)
        {
            string tmp = feats.Replace("Feats:", "").Replace("\t", "");
            string[] trimmers = tmp.Split(',');

            List<string> f = new List<string>();
            for (int i = 0; i < trimmers.Length; i++)
            {
                f.Add(trimmers[i].Trim(' '));
            }
            return f;
        }

        public static int GetBABGrapple(string bg, out int grapple)
        {
            Regex bab = new Regex(@"[+-]?[0-9]{1,2}\/");
            Regex gra = new Regex(@"\/[+-]?[0-9]{1,2}");

            string babString = bab.Match(bg).Value.Replace(@"/", "");
            string graString = gra.Match(bg).Value.Replace(@"/", "");

            babString = babString != "" ? babString : "0";
            graString = graString != "" ? graString : "0";

            int b = Convert.ToInt32(babString);
            grapple = Convert.ToInt32(graString);

            return b;
        }

        public static int GetNaturalArmor(string armor)
        {
            Regex natural = new Regex("[0-9]{1,2} natural");

            Regex number = new Regex("[0-9]{1,2}");

            if (!natural.IsMatch(armor))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(number.Match(natural.Match(armor).Value).Value);
            }
        }

        public static int GetHitDie(string hd, out int count)
        {
            string levelString = Regex.Match(hd, "[0-9]{1,2}d").Value.Replace("d", "");
            string hitdieString = Regex.Match(hd, "d[0-9]{1,2}").Value.Replace("d", "");

            levelString = levelString != "" ? levelString : "0";
            hitdieString = hitdieString != "" ? hitdieString : "0";

            count = Convert.ToInt32(levelString);

            return Convert.ToInt32(hitdieString);
        }

        public static Size GetSize(string size, out string type)
        {
            string firstword = size.Split(' ')[0];
            string trimfirstword = firstword.Replace("Size/Type:", "").Replace("\t", "");
            type = size.Replace(firstword + " ", "");

            Enum.TryParse(trimfirstword, out Size result);

            return result;
        }

        public static Saves GetSaves(string saves)
        {
            Regex Fort = new Regex("Fort [+-][0-9]{1,2}");
            Regex Ref = new Regex("Ref [+-][0-9]{1,2}");
            Regex Will = new Regex("Will [+-][0-9]{1,2}");

            Regex FortNum = new Regex("[+-][0-9]{1,2}");
            Regex RefNum = new Regex("[+-][0-9]{1,2}");
            Regex WillNum = new Regex("[+-][0-9]{1,2}");

            string FortString = Fort.Match(saves).Value;
            string RefString = Ref.Match(saves).Value;
            string WillString = Will.Match(saves).Value;

            string FortStringNum = FortNum.Match(FortString).Value;
            string RefStringNum = RefNum.Match(RefString).Value;
            string WillStringNum = WillNum.Match(WillString).Value;

            FortStringNum = FortStringNum != "" ? FortStringNum : "0";
            RefStringNum = RefStringNum != "" ? RefStringNum : "0";
            WillStringNum = WillStringNum != "" ? WillStringNum : "0";

            int iFort = Convert.ToInt32(FortStringNum);
            int iRef = Convert.ToInt32(RefStringNum);
            int iWill = Convert.ToInt32(WillStringNum);

            return new Saves(iFort, iRef, iWill);
        }

        public static Abilities GetAbilities(string abilities)
        {
            Regex Str = new Regex("Str [0-9]{1,2}");
            Regex Dex = new Regex("Dex [0-9]{1,2}");
            Regex Con = new Regex("Con [0-9]{1,2}");
            Regex Int = new Regex("Int [0-9]{1,2}");
            Regex Wis = new Regex("Wis [0-9]{1,2}");
            Regex Cha = new Regex("Cha [0-9]{1,2}");

            Regex StrNum = new Regex("[0-9]{1,2}");
            Regex DexNum = new Regex("[0-9]{1,2}");
            Regex ConNum = new Regex("[0-9]{1,2}");
            Regex IntNum = new Regex("[0-9]{1,2}");
            Regex WisNum = new Regex("[0-9]{1,2}");
            Regex ChaNum = new Regex("[0-9]{1,2}");

            var vStrNum = StrNum.Match(Str.Match(abilities).Value).Value;
            var vDexNum = DexNum.Match(Dex.Match(abilities).Value).Value;
            var vConNum = ConNum.Match(Con.Match(abilities).Value).Value;
            var vIntNum = IntNum.Match(Int.Match(abilities).Value).Value;
            var vWisNum = WisNum.Match(Wis.Match(abilities).Value).Value;
            var vChaNum = ChaNum.Match(Cha.Match(abilities).Value).Value;

            int iStr = Convert.ToInt32(vStrNum.ToString() == "" ? "0" : vStrNum);
            int iDex = Convert.ToInt32(vDexNum.ToString() == "" ? "0" : vDexNum);
            int iCon = Convert.ToInt32(vConNum.ToString() == "" ? "0" : vConNum);
            int iInt = Convert.ToInt32(vIntNum.ToString() == "" ? "0" : vIntNum);
            int iWis = Convert.ToInt32(vWisNum.ToString() == "" ? "0" : vWisNum);
            int iCha = Convert.ToInt32(vChaNum.ToString() == "" ? "0" : vChaNum);

            return new Abilities(iStr, iDex, iCon, iInt, iWis, iCha);
        }

        public static SkillMod GetSkill(string skill)
        {
            AlternateAbilityMods addlAbMods = new AlternateAbilityMods();

            string cleanSkill = skill.Replace("+", "ø").Replace("-", "ø").Replace("*", "ø").Replace("(", "ø").Split('ø')[0];
            string addlComments = Regex.Match(skill, @"\(([^)]+)\)").Value;
            int skillRanks = Convert.ToInt32(Regex.Match(skill, "[+-]?[0-9]+").Value);
            string addlMods = Regex.Replace(skill, "[0-9]+", "");
            addlMods = addlMods.TrimStart(' ').TrimEnd(' ');
            bool asteriskFound = false;

            if (addlMods != "")
            {
                if (addlMods.Contains("*"))
                {
                    asteriskFound = true;
                }

                List<string> otherMods = addlMods.Split(' ').ToList();

                addlAbMods.STR = otherMods.Any(x => x.ToUpper() == "STR") ? AbilityMod.Str : (AbilityMod?)null;
                addlAbMods.DEX = otherMods.Any(x => x.ToUpper() == "DEX") ? AbilityMod.Dex : (AbilityMod?)null;
                addlAbMods.CON = otherMods.Any(x => x.ToUpper() == "CON") ? AbilityMod.Con : (AbilityMod?)null;
                addlAbMods.INT = otherMods.Any(x => x.ToUpper() == "INT") ? AbilityMod.Int : (AbilityMod?)null;
                addlAbMods.WIS = otherMods.Any(x => x.ToUpper() == "WIS") ? AbilityMod.Wis : (AbilityMod?)null;
                addlAbMods.CHA = otherMods.Any(x => x.ToUpper() == "CHA") ? AbilityMod.Cha : (AbilityMod?)null;
            }

            List<Skill> availableSkills = Enum.GetValues(typeof(Skill)).Cast<Skill>().ToList();
            int foundIndex = availableSkills.FindIndex(x => x.ToString() == cleanSkill.Replace(" ", ""));

            if (foundIndex > -1)
            {
                return new SkillMod(availableSkills[foundIndex], addlComments, skillRanks, addlAbMods, asteriskFound);
            }
            else
            {

            }

            return null;
        }
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
            get { return Convert.ToInt32(ScoreToMod(Str)); }
        }
        public int Dex { get; set; }
        public int DexMod
        {
            get { return Convert.ToInt32(ScoreToMod(Dex)); }
        }
        public int Con { get; set; }
        public int ConMod
        {
            get { return Convert.ToInt32(ScoreToMod(Con)); }
        }
        public int Int { get; set; }
        public int IntMod
        {
            get { return Convert.ToInt32(ScoreToMod(Int)); }
        }
        public int Wis { get; set; }
        public int WisMod
        {
            get { return Convert.ToInt32(ScoreToMod(Wis)); }
        }
        public int Cha { get; set; }
        public int ChaMod
        {
            get { return Convert.ToInt32(ScoreToMod(Cha)); }
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
            return string.Join(" and ", this);
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
        public bool Asterisk { get; set; }

        public SkillMod()
        {

        }

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
            {Skill.UseMagicDevice,    AbilityMod.Cha },
            {Skill.UseRope,           AbilityMod.Dex }
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

    public class Augments
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

                    st.Attacks.Add(new Attack(1, "GB Slam", SlamDamage(st.Size).ToString()));

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

                public static string SlamDamage(Size size)
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
