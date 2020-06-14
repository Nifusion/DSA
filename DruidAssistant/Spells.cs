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
    public class Spells : List<Spell>
    {
        public static Spells Retrieve(string xmlPath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Spells));

            using (Stream inputStream = File.OpenRead(xmlPath))
            {
                return (Spells)xs.Deserialize(inputStream);
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
