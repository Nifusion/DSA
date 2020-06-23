using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.IO;

namespace DruidAssistant
{
    public static class SpellExtensions
    {
        public static void Save(this Spells spells, string xmlPath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Spells));

            using (StreamWriter outputStream = new StreamWriter(xmlPath, false))
            {
                xs.Serialize(outputStream, spells);
            }
        }
    }
    public class Spells : List<Spell>
    {
        public static Spells Retrieve(string xmlPath)
        {
            using (Stream inputStream = File.OpenRead(xmlPath))
            {
                return (Spells)new XmlSerializer(typeof(Spells)).Deserialize(inputStream);
            }
        }
    }
    public class Spell
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Level { get; set; }
        public string Domain { get; set; }
        public string Components { get; set; }
        public string Casting { get; set; }
        public string Range { get; set; }
        public string Target { get; set; }
        public string Effect { get; set; }
        public string Duration { get; set; }
        public string SavingThrow { get; set; }
        public string SpellResistance { get; set; }
        public string Description { get; set; }
        public string PersonalNotes { get; set; }
        public string SourceBook { get; set; }
        public bool Favorited { get; set; }

        public Spell()
        {

        }

        public Spell(string[] spellArray)
        {
            Level = spellArray[0];
            Class = spellArray[1];
            Name = spellArray[2];
            Domain = spellArray[3];
            Components = spellArray[4];
            Casting = spellArray[5];
            Range = spellArray[6];
            Target = spellArray[7];
            Effect = spellArray[8];
            Duration = spellArray[9];
            SavingThrow = spellArray[10];
            SpellResistance = spellArray[11];
            Description = spellArray[12];
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
                    if (line == " " || line == "")
                    {
                        description += "\r\r";
                    }
                    else
                    {
                        description += line + " ";
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
            spellArray[12] = description.TrimStart(' ').TrimStart('\t').Replace("  ", " ");

            return spellArray;
        }

        public static string[] GetSpellLevel(string text)
        {
            Regex drd = new Regex("drd [0-9]");
            Regex druid = new Regex("druid [0-9]");
            if (drd.IsMatch(text.ToLower()))
            {
                string m = drd.Match(text.ToLower()).Value;
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
    }


}

/*
 * spellArray[0] = spellLevel;
                spellArray[1] = name;
                spellArray[2] = domain;
                spellArray[3] = components;
                spellArray[4] = castingTime;
                spellArray[5] = range;
                spellArray[6] = target;
                spellArray[7] = effect;
                spellArray[8] = duration;
                spellArray[9] = savingThrow;
                spellArray[10] = spellResistance;
                spellArray[11] = description;
 * */
