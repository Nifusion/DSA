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
    public static class IO
    {
        public static void Save(this Summons spells, string xmlPath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Summons));

            using (StreamWriter outputStream = new StreamWriter(xmlPath, false))
            {
                xs.Serialize(outputStream, spells);
            }
        }

        public static void Save(this Spells spells, string xmlPath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Spells));

            using (StreamWriter outputStream = new StreamWriter(xmlPath, false))
            {
                xs.Serialize(outputStream, spells);
            }
        }
    }
}
