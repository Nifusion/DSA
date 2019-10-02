using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace DruidAssistant
{
    public class FileParser
    {
        public static string GetTextFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }

        public static SummonTemplate GetSummonTemplateFromFile(string path)
        {
            SummonTemplate thisTemplate = new SummonTemplate();
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

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                int i = 0;
                bool combat = false;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace("–", "-");
                    i++;
                    if (!combat)
                    {
                        if (i == 1 || line.Contains("click to see monster")) { name = line; }
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

        public static SummonTemplate GetSummonTemplateFromText(string text)
        {
            SummonTemplate thisTemplate = new SummonTemplate();
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
            return input.First().ToString().ToUpper() + input.Substring(1).ToLower();
        }

        public static SkillMod CorrectSkillModBackward(SkillMod sm, Abilities a)
        {
            List<int> mods = new List<int>();

            if (sm.DefMod == AbilityMod.Str) { mods.Add(a.StrMod); }
            else if (sm.DefMod == AbilityMod.Dex) { mods.Add(a.DexMod); }
            else if (sm.DefMod == AbilityMod.Con) { mods.Add(a.ConMod); }
            else if (sm.DefMod == AbilityMod.Int) { mods.Add(a.IntMod); }
            else if (sm.DefMod == AbilityMod.Wis) { mods.Add(a.WisMod); }
            else if (sm.DefMod == AbilityMod.Cha) { mods.Add(a.ChaMod); }

            if (sm.ModStr.HasValue)
            {
                mods.Add(a.StrMod);
            }
            if (sm.ModDex.HasValue)
            {
                mods.Add(a.DexMod);
            }
            if (sm.ModCon.HasValue)
            {
                mods.Add(a.ConMod);
            }
            if (sm.ModInt.HasValue)
            {
                mods.Add(a.IntMod);
            }
            if (sm.ModWis.HasValue)
            {
                mods.Add(a.WisMod);
            }
            if (sm.ModCha.HasValue)
            {
                mods.Add(a.ChaMod);
            }

            sm.Bonus -= mods.Max();

            return sm;
        }

        public static SkillMod CorrectSkillModForward(SkillMod sm, Abilities a)
        {
            List<int> mods = new List<int>();

            if (sm.DefMod == AbilityMod.Str) { mods.Add(a.StrMod); }
            else if (sm.DefMod == AbilityMod.Dex) { mods.Add(a.DexMod); }
            else if (sm.DefMod == AbilityMod.Con) { mods.Add(a.ConMod); }
            else if (sm.DefMod == AbilityMod.Int) { mods.Add(a.IntMod); }
            else if (sm.DefMod == AbilityMod.Wis) { mods.Add(a.WisMod); }
            else if (sm.DefMod == AbilityMod.Cha) { mods.Add(a.ChaMod); }

            if (sm.ModStr.HasValue)
            {
                mods.Add(a.StrMod);
            }
            if (sm.ModDex.HasValue)
            {
                mods.Add(a.DexMod);
            }
            if (sm.ModCon.HasValue)
            {
                mods.Add(a.ConMod);
            }
            if (sm.ModInt.HasValue)
            {
                mods.Add(a.IntMod);
            }
            if (sm.ModWis.HasValue)
            {
                mods.Add(a.WisMod);
            }
            if (sm.ModCha.HasValue)
            {
                mods.Add(a.ChaMod);
            }

            sm.NewBonus = sm.Bonus + mods.Max();

            return sm;
        }

        public static string GetEnvironment(string env)
        {
            return env.Replace("Environment:", "").Replace("\t", "");
        }

        public static string GetSpeeds(string speed)
        {
            speed = speed.Replace("Speed:", "").Replace("\t", "");

            speed = Regex.Replace(speed, @"\(([^)]+)\)", "");

            string[] fix = speed.Split(',');
            string fixBlank = "";
            for (int i = 0; i < fix.Length; i++)
            {
                if (i == 0)
                {
                    fixBlank += fix[i].TrimStart(' ').TrimEnd(' ');
                }
                else
                {
                    fixBlank += " | " + fix[i].TrimStart(' ').TrimEnd(' ');
                }
            }

            return fixBlank;
        }

        public static string GetSpaceReach(string spacereach, out string reach)
        {
            string replaced = spacereach.Replace("Space/Reach:", "").Replace("\t", "");
            string[] splitReplaced = replaced.Split('/');
            reach = splitReplaced[1];
            return splitReplaced[0];
        }

        public static List<Attack> GetAttacks(string attack)
        {
            List<Attack> attacks = new List<Attack>();

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
                        weapon = findCount.Replace(weapon, "");
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

            int b = Convert.ToInt32(bab.Match(bg).Value.Replace(@"/", ""));
            grapple = Convert.ToInt32(gra.Match(bg).Value.Replace(@"/", ""));

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
            Regex level = new Regex("[0-9]{1,2}d");
            Regex hitdie = new Regex("d[0-9]{1,2}");

            count = Convert.ToInt32(level.Match(hd).Value.Replace("d", ""));

            return Convert.ToInt32(hitdie.Match(hd).Value.Replace("d", ""));
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

            int iFort = Convert.ToInt32(FortNum.Match(Fort.Match(saves).Value).Value);
            int iRef = Convert.ToInt32(RefNum.Match(Ref.Match(saves).Value).Value);
            int iWill = Convert.ToInt32(WillNum.Match(Will.Match(saves).Value).Value);

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
            AbilityMod? amStr = null;
            AbilityMod? amDex = null;
            AbilityMod? amCon = null;
            AbilityMod? amInt = null;
            AbilityMod? amWis = null;
            AbilityMod? amCha = null;

            string additional = Regex.Match(skill, @"\(([^)]+)\)").Value;
            int skillNumber = Convert.ToInt32(Regex.Match(skill, "[+-]?[0-9]+").Value);
            string otherHalf = Regex.Replace(skill, "[0-9]+", "");
            otherHalf = otherHalf.TrimStart(' ').TrimEnd(' ');
            bool ast = false;

            if (otherHalf != "")
            {
                if (otherHalf.Contains("*"))
                {
                    ast = true;
                }

                string[] otherMods = otherHalf.Split(' ');
                for (int i = 0; i < otherMods.Length; i++)
                {
                    if (otherMods[i].ToString().ToUpper() == "STR")
                    {
                        amStr = AbilityMod.Str;
                    }
                    if (otherMods[i].ToString().ToUpper() == "DEX")
                    {
                        amDex = AbilityMod.Dex;
                    }
                    if (otherMods[i].ToString().ToUpper() == "CON")
                    {
                        amCon = AbilityMod.Con;
                    }
                    if (otherMods[i].ToString().ToUpper() == "INT")
                    {
                        amInt = AbilityMod.Int;
                    }
                    if (otherMods[i].ToString().ToUpper() == "WIS")
                    {
                        amWis = AbilityMod.Wis;
                    }
                    if (otherMods[i].ToString().ToUpper() == "CHA")
                    {
                        amCha = AbilityMod.Cha;
                    }
                }
            }
            AbilityMod?[] values = new AbilityMod?[] { amStr, amDex, amCon, amInt, amWis, amCha };
            int counter = 0;
            AbilityMod? defHolder = null;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].HasValue)
                {
                    defHolder = values[i];
                    counter++;
                }
            }

            if (counter != 1)
            {
                defHolder = null;
            }

            if (skill.Contains("Appraise")) { return new SkillMod(Skill.Appraise, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Int : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Balance")) { return new SkillMod(Skill.Balance, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Dex : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Bluff")) { return new SkillMod(Skill.Bluff, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Cha : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Climb")) { return new SkillMod(Skill.Climb, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Str : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Concentration")) { return new SkillMod(Skill.Concentration, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Con : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Craft")) { return new SkillMod(Skill.Craft, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Int : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Decipher Script")) { return new SkillMod(Skill.DecipherScript, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Int : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Diplomacy")) { return new SkillMod(Skill.Diplomacy, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Cha : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Disable Device")) { return new SkillMod(Skill.DisableDevice, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Int : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Disguise")) { return new SkillMod(Skill.Disguise, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Cha : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Escape Artist")) { return new SkillMod(Skill.EscapeArtist, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Dex : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Forgery")) { return new SkillMod(Skill.Forgery, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Int : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Gather Information")) { return new SkillMod(Skill.GatherInformation, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Cha : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Handle Animal")) { return new SkillMod(Skill.HandleAnimal, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Cha : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Heal")) { return new SkillMod(Skill.Heal, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Wis : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Hide")) { return new SkillMod(Skill.Hide, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Dex : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Intimidate")) { return new SkillMod(Skill.Intimidate, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Cha : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Jump")) { return new SkillMod(Skill.Jump, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Str : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Knowledge")) { return new SkillMod(Skill.Knowledge, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Int : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Listen")) { return new SkillMod(Skill.Listen, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Wis : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Move Silently")) { return new SkillMod(Skill.MoveSilently, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Dex : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Open Lock")) { return new SkillMod(Skill.OpenLock, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Dex : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Perform")) { return new SkillMod(Skill.Perform, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Cha : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Profession")) { return new SkillMod(Skill.Profession, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Wis : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Ride")) { return new SkillMod(Skill.Ride, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Dex : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Search")) { return new SkillMod(Skill.Search, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Int : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Sense Motive")) { return new SkillMod(Skill.SenseMotive, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Wis : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Sleight of Hand")) { return new SkillMod(Skill.SleightofHand, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Dex : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Spellcraft")) { return new SkillMod(Skill.Spellcraft, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Int : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Spot")) { return new SkillMod(Skill.Spot, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Wis : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Survival")) { return new SkillMod(Skill.Survival, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Wis : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Swim")) { return new SkillMod(Skill.Swim, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Str : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Tumble")) { return new SkillMod(Skill.Tumble, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Dex : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Use Magic Device")) { return new SkillMod(Skill.UseMagicDevice, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Cha : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            if (skill.Contains("Use Rope")) { return new SkillMod(Skill.UseRope, additional, skillNumber, !defHolder.HasValue ? AbilityMod.Dex : defHolder.Value, (!defHolder.HasValue && amStr.HasValue) ? amStr.Value : (AbilityMod?)null, (!defHolder.HasValue && amDex.HasValue) ? amDex.Value : (AbilityMod?)null, (!defHolder.HasValue && amCon.HasValue) ? amCon.Value : (AbilityMod?)null, (!defHolder.HasValue && amInt.HasValue) ? amInt.Value : (AbilityMod?)null, (!defHolder.HasValue && amWis.HasValue) ? amWis.Value : (AbilityMod?)null, (!defHolder.HasValue && amCha.HasValue) ? amCha.Value : (AbilityMod?)null, ast); }
            return null;
        }

        public static string[] GetSpellFromText(string text)
        {
            string[] textArray = text.Replace("\r\n", "~").Replace("\n", "~").Replace("\r", "~").Split('~');

            bool desc = false;

            string[] spellArray = new string[13];

            string name = "";
            string domain = "";
            string spellLevel = "";
            string components = "";
            string castingTime = "";
            string range = "";
            string target = "";
            string effect = "";
            string duration = "";
            string savingThrow = "";
            string spellResistance = "";
            string description = "";

            for (int i = 0; i < textArray.Length; i++)
            {
                string line = textArray[i];


                if (i == 0)
                {
                    name = line;
                }
                if (i == 1)
                {
                    domain = line;
                }
                if (line.Contains("Level:"))
                {
                    spellLevel = line.Split(':')[1];
                }
                if (line.Contains("Components:"))
                {
                    components = line.Split(':')[1];
                }
                if (line.Contains("Casting Time:"))
                {
                    castingTime = line.Split(':')[1];
                }
                if (line.Contains("Range:"))
                {
                    range = line.Split(':')[1];
                }
                if (line.Contains("Target:") || line.Contains("Area:"))
                {
                    target = line.Split(':')[1];
                }
                if (line.Contains("Effect:"))
                {
                    effect = line.Split(':')[1];
                }
                if (line.Contains("Duration:"))
                {
                    duration = line.Split(':')[1];
                }
                if (line.Contains("Saving Throw:"))
                {
                    savingThrow = line.Split(':')[1];
                }
                if (line.Contains("Spell Resistance:"))
                {
                    spellResistance = line.Split(':')[1];
                }
                if (line.Contains("Description:") || (!line.Contains(":") && i > 3))
                {
                    line = line.Replace("Description:", "");
                    desc = true;
                }
                if (desc)
                {
                    line.Replace("\r", " ").Replace("\n", " ");
                    if (line == " "||line == "")
                    {
                        description += "\r\r";
                    }
                    else
                    {
                        description += line+" ";
                    }
                }
            }
            string[] classLevel = GetSpellLevel(spellLevel.TrimStart(' ').TrimStart('\t'));

            spellArray[0] = classLevel[1].TrimStart(' ').TrimStart('\t');
            spellArray[1] = classLevel[0].TrimStart(' ').TrimStart('\t');
            spellArray[2] = name.TrimStart(' ').TrimStart('\t');
            spellArray[3] = domain.TrimStart(' ').TrimStart('\t');
            spellArray[4] = components.TrimStart(' ').TrimStart('\t');
            spellArray[5] = castingTime.TrimStart(' ').TrimStart('\t');
            spellArray[6] = range.TrimStart(' ').TrimStart('\t');
            spellArray[7] = target.TrimStart(' ').TrimStart('\t');
            spellArray[8] = effect.TrimStart(' ').TrimStart('\t');
            spellArray[9] = duration.TrimStart(' ').TrimStart('\t');
            spellArray[10] = savingThrow.TrimStart(' ').TrimStart('\t');
            spellArray[11] = spellResistance.TrimStart(' ').TrimStart('\t');
            spellArray[12] = description.TrimStart(' ').TrimStart('\t').Replace("  "," ");

            return spellArray;
        }

        public static string[] GetSpellLevel(string text)
        {
            Regex drd = new Regex("Drd [0-9]");
            Regex druid = new Regex("druid [0-9]");
            if (drd.IsMatch(text))
            {
                string m = drd.Match(text).Value;
                string[] ma = m.Split(' ');
                ma[0] = "Druid";
                return ma;
            }
            else if (druid.IsMatch(text.ToLower()))
            {
                string m = druid.Match(text.ToLower()).Value;
                string[] ma = m.Split(' ');
                ma[0] = "Druid";
                return ma;
            }
            else
            {
                return new string[] { "Class Not Found", "0" };
            }
        }

        public static Spells GetSpellsFromXML(string xmlPath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Spells));

            using (Stream inputStream = File.OpenRead(xmlPath))
            {
                return (Spells)xs.Deserialize(inputStream);
            }
        }

        public static void DedicateSpellsToXML(string xmlPath, Spells spells)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Spells));

            using (StreamWriter outputStream = new StreamWriter(xmlPath, false))
            {
                xs.Serialize(outputStream, spells);
            }
        }

        public static SummonTemplates GetSummonsFromXML(string xmlPath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(SummonTemplates));

            using (Stream inputStream = File.OpenRead(xmlPath))
            {
                return (SummonTemplates)xs.Deserialize(inputStream);
            }
        }

        public static void DedicateSummonsToXML(string xmlPath, SummonTemplates spells)
        {
            XmlSerializer xs = new XmlSerializer(typeof(SummonTemplates));

            using (StreamWriter outputStream = new StreamWriter(xmlPath, false))
            {
                xs.Serialize(outputStream, spells);
            }
        }
    }
}