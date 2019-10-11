using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DruidAssistant
{
    public class DSAConfigs : List<DSAConfig>
    {

    }
    
    public class DSAConfig
    {
        public DSAFiles DSAFile { get; set; }
        public string DSAPath { get; set; }

        public DSAConfig()
        {

        }

        public DSAConfig(DSAFiles dsaf,string dsap)
        {
            DSAFile = dsaf;
            DSAPath = dsap;
        }
    }

    public enum DSAFiles
    {
        Spells,
        Summons,
        WildShapes,
        CharacterSheets
    }
}
