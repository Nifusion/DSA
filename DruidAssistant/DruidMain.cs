using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Deployment.Application;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;
using System.Deployment;

namespace DruidAssistant
{
    public partial class DruidMain : Form
    {
        public bool flag1;
        public Summon currentST;

        public DruidMain()
        {
            InitializeComponent();
            lvAugments.Columns[lvAugments.Columns.Count - 1].Width = -2;

            nudCasterLevel.Value = Properties.Settings.Default.CasterLevel;
            tbSummonsXML.Text = Properties.Settings.Default.SummonsXML;
            tbSpellsXML.Text = Properties.Settings.Default.SpellsXML;

            //verify up to date with random ass changes you've managed to mess up
            //1: SummonTemplate renamed to Summon

            string fullFile = "";
            using (StreamReader sr = new StreamReader(tbSummonsXML.Text))
            {
                fullFile = sr.ReadToEnd();
            }

            fullFile = fullFile.Replace("SummonTemplate", "Summon");

            using (StreamWriter sw = new StreamWriter(tbSummonsXML.Text))
            {
                sw.Write(fullFile);
            }

            //future shit
            //string defaultConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.xml");
            //
            //string defaultSpells= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");
            //string defaultSummons = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml");
            //string defaultWildShapes = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WildShapes.xml");
            //string defaultCharacters = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Characters.xml");
            //
            //DSAConfigs dsacs = new DSAConfigs();
            //if (!File.Exists(defaultConfig))
            //{
            //    if (File.Exists(defaultSpells))
            //    {
            //        dsacs.Add(new DSAConfig(DSAFiles.Spells, defaultSpells));
            //    }
            //
            //    if (File.Exists(defaultSummons))
            //    {
            //        dsacs.Add(new DSAConfig(DSAFiles.Summons, defaultSummons));
            //    }
            //
            //    if (File.Exists(defaultWildShapes))
            //    {
            //        dsacs.Add(new DSAConfig(DSAFiles.WildShapes, defaultWildShapes));
            //    }
            //
            //    if (File.Exists(defaultCharacters))
            //    {
            //        dsacs.Add(new DSAConfig(DSAFiles.CharacterSheets, defaultCharacters));
            //    }
            //
            //    FileParser.DedicateConfigsToXML(defaultConfig, dsacs);
            //}
            //else
            //{
            //    dsacs = FileParser.GetConfigsFromXML(defaultConfig);
            //}
        }

        private void DruidMain_Load(object sender, EventArgs e)
        {
            RefreshSummonPage();
            RefreshSpellPage();
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.Text = string.Format("Druid Summoning Assistant v{0}",
                    ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4));
            }
        }

        private void ListDirectory(TreeView treeView, string path)
        {
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            directoryNode.Tag = directoryInfo.FullName;
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            }
            foreach (var file in directoryInfo.GetFiles())
            {
                TreeNode tn = new TreeNode(file.Name);
                tn.Tag = file.FullName;
                directoryNode.Nodes.Add(tn);
            }
            return directoryNode;
        }


        private SummonToForm AugmentTemplate(SummonToForm st)
        {
            st.PCCasterLevel = Convert.ToInt32(nudCasterLevel.Value);
            var list = lvAugments.CheckedItems.Cast<object>().Select(x => x.ToString()).ToList();
            if (list.Any(x => x.Contains("Feat: Spell Focus (Conjuration)")))
            {
                st = Feats.SpellFocusConjuration(st);
            }
            if (list.Any(x => x.Contains("Feat: Augment Summoning")))
            {
                st = Feats.AugmentSummoning(st);
            }
            if (list.Any(x => x.Contains("Feat: Greenbound Summoning")))
            {
                st = Feats.GreenboundSummoning.Augment(st);
            }
            if (list.Any(x => x.Contains("Item: Obad-Hai's Green Man")))
            {
                st = Items.ObadHaisGreenMan(st);
            }

            st.Rounds = st.PCCasterLevel;
            return st;
        }

        public int GetSizeMod(Size size)
        {
            switch (size)
            {
                case DruidAssistant.Size.Fine:
                    return 8;
                case DruidAssistant.Size.Diminuitive:
                    return 4;
                case DruidAssistant.Size.Tiny:
                    return 2;
                case DruidAssistant.Size.Small:
                    return 1;
                case DruidAssistant.Size.Medium:
                    return 0;
                case DruidAssistant.Size.Large:
                    return -1;
                case DruidAssistant.Size.Huge:
                    return -2;
                case DruidAssistant.Size.Gargantuan:
                    return -4;
                case DruidAssistant.Size.Colossal:
                    return -8;
                default:
                    return 0;
            }
        }

        public static int GetGrappleSizeMod(Size size)
        {
            switch (size)
            {
                case DruidAssistant.Size.Fine:
                    return -16;
                case DruidAssistant.Size.Diminuitive:
                    return -12;
                case DruidAssistant.Size.Tiny:
                    return -8;
                case DruidAssistant.Size.Small:
                    return -4;
                case DruidAssistant.Size.Medium:
                    return 0;
                case DruidAssistant.Size.Large:
                    return 4;
                case DruidAssistant.Size.Huge:
                    return 8;
                case DruidAssistant.Size.Gargantuan:
                    return 12;
                case DruidAssistant.Size.Colossal:
                    return 16;
                default:
                    return 0;
            }
        }

        private void TvTemplates_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                ListDirectory(tvTemplates, ((string[])e.Data.GetData(DataFormats.FileDrop))[0]);

            }
        }

        private void TvTemplates_DragEnter(object sender, DragEventArgs e)
        {
            DragDropEffects effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (Directory.Exists(path))
                    effects = DragDropEffects.Copy;
            }

            e.Effect = effects;
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (tvTemplates.SelectedNode != null)
            {
                if (tvTemplates.SelectedNode.Tag is Summon)
                {
                    SummonToForm st = new SummonToForm((Summon)tvTemplates.SelectedNode.Tag);
                    st = AugmentTemplate(st);
                    SummonedCreature sc = new SummonedCreature(st);
                    sc.Show();
                }
            }
            ClearSummonPage();
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            if (tvTemplates.SelectedNode != null)
            {
                if (tvTemplates.SelectedNode.Nodes.Count == 0)
                {
                    LoadPreview((Summon)tvTemplates.SelectedNode.Tag);
                }
            }
        }

        private void RefreshSpellPage()
        {
            string xmlPath = tbSpellsXML.Text;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");;

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tvSpells.Nodes.Clear();
            tvFavoritedSpells.Nodes.Clear();

            TreeNode tn0 = tvSpells.Nodes.Add("Level 0");
            TreeNode tn1 = tvSpells.Nodes.Add("Level 1");
            TreeNode tn2 = tvSpells.Nodes.Add("Level 2");
            TreeNode tn3 = tvSpells.Nodes.Add("Level 3");
            TreeNode tn4 = tvSpells.Nodes.Add("Level 4");
            TreeNode tn5 = tvSpells.Nodes.Add("Level 5");
            TreeNode tn6 = tvSpells.Nodes.Add("Level 6");
            TreeNode tn7 = tvSpells.Nodes.Add("Level 7");
            TreeNode tn8 = tvSpells.Nodes.Add("Level 8");
            TreeNode tn9 = tvSpells.Nodes.Add("Level 9");

            TreeNode tnf0 = tvFavoritedSpells.Nodes.Add("Level 0");
            TreeNode tnf1 = tvFavoritedSpells.Nodes.Add("Level 1");
            TreeNode tnf2 = tvFavoritedSpells.Nodes.Add("Level 2");
            TreeNode tnf3 = tvFavoritedSpells.Nodes.Add("Level 3");
            TreeNode tnf4 = tvFavoritedSpells.Nodes.Add("Level 4");
            TreeNode tnf5 = tvFavoritedSpells.Nodes.Add("Level 5");
            TreeNode tnf6 = tvFavoritedSpells.Nodes.Add("Level 6");
            TreeNode tnf7 = tvFavoritedSpells.Nodes.Add("Level 7");
            TreeNode tnf8 = tvFavoritedSpells.Nodes.Add("Level 8");
            TreeNode tnf9 = tvFavoritedSpells.Nodes.Add("Level 9");

            Spells spells = Spells.Retrieve(xmlPath);

            for (int i = 0; i < spells.Count; i++)
            {
                TreeNode thisSpell = new TreeNode(string.Format("{0} ({1})", spells[i].Name, spells[i].SourceBook));
                thisSpell.Tag = spells[i];

                TreeNode thisfSpell = new TreeNode(string.Format("{0} ({1})", spells[i].Name, spells[i].SourceBook));
                thisfSpell.Tag = spells[i];

                if (spells[i].Level == "0") { tn0.Nodes.Add(thisSpell); }
                if (spells[i].Level == "1") { tn1.Nodes.Add(thisSpell); }
                if (spells[i].Level == "2") { tn2.Nodes.Add(thisSpell); }
                if (spells[i].Level == "3") { tn3.Nodes.Add(thisSpell); }
                if (spells[i].Level == "4") { tn4.Nodes.Add(thisSpell); }
                if (spells[i].Level == "5") { tn5.Nodes.Add(thisSpell); }
                if (spells[i].Level == "6") { tn6.Nodes.Add(thisSpell); }
                if (spells[i].Level == "7") { tn7.Nodes.Add(thisSpell); }
                if (spells[i].Level == "8") { tn8.Nodes.Add(thisSpell); }
                if (spells[i].Level == "9") { tn9.Nodes.Add(thisSpell); }

                if (spells[i].Level == "0" && spells[i].Favorited) { tnf0.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "1" && spells[i].Favorited) { tnf1.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "2" && spells[i].Favorited) { tnf2.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "3" && spells[i].Favorited) { tnf3.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "4" && spells[i].Favorited) { tnf4.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "5" && spells[i].Favorited) { tnf5.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "6" && spells[i].Favorited) { tnf6.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "7" && spells[i].Favorited) { tnf7.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "8" && spells[i].Favorited) { tnf8.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "9" && spells[i].Favorited) { tnf9.Nodes.Add(thisfSpell); }
            }
            tvSpells.Sort();
            tvFavoritedSpells.Sort();
        }

        private void RefreshSpellPage(bool[] expanders)
        {
            string xmlPath = tbSpellsXML.Text;//string.IsNullOrEmpty(tbOverrideSpellXML.Text) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml") : tbOverrideSpellXML.Text;

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tvSpells.Nodes.Clear();
            tvFavoritedSpells.Nodes.Clear();

            TreeNode tn0 = tvSpells.Nodes.Add("Level 0");
            TreeNode tn1 = tvSpells.Nodes.Add("Level 1");
            TreeNode tn2 = tvSpells.Nodes.Add("Level 2");
            TreeNode tn3 = tvSpells.Nodes.Add("Level 3");
            TreeNode tn4 = tvSpells.Nodes.Add("Level 4");
            TreeNode tn5 = tvSpells.Nodes.Add("Level 5");
            TreeNode tn6 = tvSpells.Nodes.Add("Level 6");
            TreeNode tn7 = tvSpells.Nodes.Add("Level 7");
            TreeNode tn8 = tvSpells.Nodes.Add("Level 8");
            TreeNode tn9 = tvSpells.Nodes.Add("Level 9");

            TreeNode tnf0 = tvFavoritedSpells.Nodes.Add("Level 0");
            TreeNode tnf1 = tvFavoritedSpells.Nodes.Add("Level 1");
            TreeNode tnf2 = tvFavoritedSpells.Nodes.Add("Level 2");
            TreeNode tnf3 = tvFavoritedSpells.Nodes.Add("Level 3");
            TreeNode tnf4 = tvFavoritedSpells.Nodes.Add("Level 4");
            TreeNode tnf5 = tvFavoritedSpells.Nodes.Add("Level 5");
            TreeNode tnf6 = tvFavoritedSpells.Nodes.Add("Level 6");
            TreeNode tnf7 = tvFavoritedSpells.Nodes.Add("Level 7");
            TreeNode tnf8 = tvFavoritedSpells.Nodes.Add("Level 8");
            TreeNode tnf9 = tvFavoritedSpells.Nodes.Add("Level 9");

            Spells spells = Spells.Retrieve(xmlPath);

            for (int i = 0; i < spells.Count; i++)
            {
                TreeNode thisSpell = new TreeNode(string.Format("{0} ({1})", spells[i].Name, spells[i].SourceBook));
                thisSpell.Tag = spells[i];

                TreeNode thisfSpell = new TreeNode(string.Format("{0} ({1})", spells[i].Name, spells[i].SourceBook));
                thisfSpell.Tag = spells[i];

                if (spells[i].Level == "0") { tn0.Nodes.Add(thisSpell); }
                if (spells[i].Level == "1") { tn1.Nodes.Add(thisSpell); }
                if (spells[i].Level == "2") { tn2.Nodes.Add(thisSpell); }
                if (spells[i].Level == "3") { tn3.Nodes.Add(thisSpell); }
                if (spells[i].Level == "4") { tn4.Nodes.Add(thisSpell); }
                if (spells[i].Level == "5") { tn5.Nodes.Add(thisSpell); }
                if (spells[i].Level == "6") { tn6.Nodes.Add(thisSpell); }
                if (spells[i].Level == "7") { tn7.Nodes.Add(thisSpell); }
                if (spells[i].Level == "8") { tn8.Nodes.Add(thisSpell); }
                if (spells[i].Level == "9") { tn9.Nodes.Add(thisSpell); }

                if (spells[i].Level == "0" && spells[i].Favorited) { tnf0.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "1" && spells[i].Favorited) { tnf1.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "2" && spells[i].Favorited) { tnf2.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "3" && spells[i].Favorited) { tnf3.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "4" && spells[i].Favorited) { tnf4.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "5" && spells[i].Favorited) { tnf5.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "6" && spells[i].Favorited) { tnf6.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "7" && spells[i].Favorited) { tnf7.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "8" && spells[i].Favorited) { tnf8.Nodes.Add(thisfSpell); }
                if (spells[i].Level == "9" && spells[i].Favorited) { tnf9.Nodes.Add(thisfSpell); }

            }
            tvSpells.Sort();
            tvFavoritedSpells.Sort();

            if (expanders.Length == 20)
            {
                if (expanders[0]) { tn0.Expand(); }
                if (expanders[1]) { tn1.Expand(); }
                if (expanders[2]) { tn2.Expand(); }
                if (expanders[3]) { tn3.Expand(); }
                if (expanders[4]) { tn4.Expand(); }
                if (expanders[5]) { tn5.Expand(); }
                if (expanders[6]) { tn6.Expand(); }
                if (expanders[7]) { tn7.Expand(); }
                if (expanders[8]) { tn8.Expand(); }
                if (expanders[9]) { tn9.Expand(); }

                if (expanders[10]) { tnf0.Expand(); }
                if (expanders[11]) { tnf1.Expand(); }
                if (expanders[12]) { tnf2.Expand(); }
                if (expanders[13]) { tnf3.Expand(); }
                if (expanders[14]) { tnf4.Expand(); }
                if (expanders[15]) { tnf5.Expand(); }
                if (expanders[16]) { tnf6.Expand(); }
                if (expanders[17]) { tnf7.Expand(); }
                if (expanders[18]) { tnf8.Expand(); }
                if (expanders[19]) { tnf9.Expand(); }
            }
        }

        private void RefreshSummonPage()
        {
            string xmlPath = tbSummonsXML.Text;//string.IsNullOrEmpty(tbOverrideSummonXML.Text) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml") : tbOverrideSummonXML.Text;

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Summons summons = Summons.Retrieve(xmlPath);

            tvTemplates.Nodes.Clear();

            TreeNode tn1 = tvTemplates.Nodes.Add("Summon Nature's Ally 1");
            TreeNode tn2 = tvTemplates.Nodes.Add("Summon Nature's Ally 2");
            TreeNode tn3 = tvTemplates.Nodes.Add("Summon Nature's Ally 3");
            TreeNode tn4 = tvTemplates.Nodes.Add("Summon Nature's Ally 4");
            TreeNode tn5 = tvTemplates.Nodes.Add("Summon Nature's Ally 5");
            TreeNode tn6 = tvTemplates.Nodes.Add("Summon Nature's Ally 6");
            TreeNode tn7 = tvTemplates.Nodes.Add("Summon Nature's Ally 7");
            TreeNode tn8 = tvTemplates.Nodes.Add("Summon Nature's Ally 8");
            TreeNode tn9 = tvTemplates.Nodes.Add("Summon Nature's Ally 9");

            for (int i = 0; i < summons.Count; i++)
            {
                TreeNode thisSummon = new TreeNode(summons[i].Name);
                thisSummon.Tag = summons[i];
                if (summons[i].SummonSpell == "Summon Nature's Ally 1") { tn1.Nodes.Add(thisSummon); }
                if (summons[i].SummonSpell == "Summon Nature's Ally 2") { tn2.Nodes.Add(thisSummon); }
                if (summons[i].SummonSpell == "Summon Nature's Ally 3") { tn3.Nodes.Add(thisSummon); }
                if (summons[i].SummonSpell == "Summon Nature's Ally 4") { tn4.Nodes.Add(thisSummon); }
                if (summons[i].SummonSpell == "Summon Nature's Ally 5") { tn5.Nodes.Add(thisSummon); }
                if (summons[i].SummonSpell == "Summon Nature's Ally 6") { tn6.Nodes.Add(thisSummon); }
                if (summons[i].SummonSpell == "Summon Nature's Ally 7") { tn7.Nodes.Add(thisSummon); }
                if (summons[i].SummonSpell == "Summon Nature's Ally 8") { tn8.Nodes.Add(thisSummon); }
                if (summons[i].SummonSpell == "Summon Nature's Ally 9") { tn9.Nodes.Add(thisSummon); }
            }
            tvTemplates.Sort();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshSpellPage(GetSpellNodesExpansion());
        }

        private bool[] GetSpellNodesExpansion()
        {
            bool[] expander = new bool[20];

            expander[0] = tvSpells.Nodes[0].IsExpanded;
            expander[1] = tvSpells.Nodes[1].IsExpanded;
            expander[2] = tvSpells.Nodes[2].IsExpanded;
            expander[3] = tvSpells.Nodes[3].IsExpanded;
            expander[4] = tvSpells.Nodes[4].IsExpanded;
            expander[5] = tvSpells.Nodes[5].IsExpanded;
            expander[6] = tvSpells.Nodes[6].IsExpanded;
            expander[7] = tvSpells.Nodes[7].IsExpanded;
            expander[8] = tvSpells.Nodes[8].IsExpanded;
            expander[9] = tvSpells.Nodes[9].IsExpanded;

            expander[10] = tvFavoritedSpells.Nodes[0].IsExpanded;
            expander[11] = tvFavoritedSpells.Nodes[1].IsExpanded;
            expander[12] = tvFavoritedSpells.Nodes[2].IsExpanded;
            expander[13] = tvFavoritedSpells.Nodes[3].IsExpanded;
            expander[14] = tvFavoritedSpells.Nodes[4].IsExpanded;
            expander[15] = tvFavoritedSpells.Nodes[5].IsExpanded;
            expander[16] = tvFavoritedSpells.Nodes[6].IsExpanded;
            expander[17] = tvFavoritedSpells.Nodes[7].IsExpanded;
            expander[18] = tvFavoritedSpells.Nodes[8].IsExpanded;
            expander[19] = tvFavoritedSpells.Nodes[9].IsExpanded;

            return expander;
        }

        private void LoadSpellPage(Spell spell)
        {
            sittingSpellID = spell.Index;

            chkFavoriteSpell.Checked = spell.Favorited;
            tbSpellName.Text = spell.Name;
            tbSpellClass.Text = spell.Class;
            tbSpellLevel.Text = spell.Level;
            tbSpellDomain.Text = spell.Domain;
            tbSpellComponents.Text = spell.Components;
            tbSpellCastingTime.Text = spell.Casting;
            tbSpellRange.Text = spell.Range;
            tbSpellTarget.Text = spell.Target;
            tbSpellEffect.Text = spell.Effect;
            tbSpellDuration.Text = spell.Duration;
            tbSpellSaving.Text = spell.SavingThrow;
            tbSpellResistance.Text = spell.SpellResistance;
            tbSource.Text = spell.SourceBook;
            rtbSpellDescription.Text = spell.Description;
            rtbPersonalNotes.Text = spell.PersonalNotes;

        }

        private void ClearSpellPage()
        {
            sittingSpellID = 0;

            chkFavoriteSpell.Checked = false;
            tbSpellCastingTime.Clear();
            tbSpellClass.Clear();
            tbSpellComponents.Clear();
            tbSpellDomain.Clear();
            tbSpellDuration.Clear();
            tbSpellEffect.Clear();
            tbSpellLevel.Clear();
            tbSpellName.Clear();
            tbSpellRange.Clear();
            tbSpellSaving.Clear();
            tbSpellResistance.Clear();
            tbSpellTarget.Clear();
            tbSource.Clear();
            rtbSpellDescription.Clear();
            rtbPersonalNotes.Clear();
        }

        private void BtnRefreshSummons_Click(object sender, EventArgs e)
        {
            RefreshSummonPage();
        }

        private bool IsSummonExists(List<Summon> summons, Summon summon, out int indexMatch)
        {
            for (int i = 0; i < summons.Count; i++)
            {
                if (summons[i].Name.ToUpper() == summon.Name.ToUpper() && summons[i].SummonSpell.ToUpper() == summon.SummonSpell.ToUpper())
                {
                    indexMatch = i;
                    return true;
                }
            }
            indexMatch = -1;
            return false;
        }

        private bool FindSummon(List<Summon> summons, int summonIndex, out int indexMatch)
        {
            for (int i = 0; i < summons.Count; i++)
            {
                if (summons[i].Index == summonIndex)
                {
                    indexMatch = i;
                    return true;
                }
            }
            indexMatch = -1;
            return false;
        }

        private bool IsSpellExists(List<Spell> spells, Spell spell, out int indexMatch)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                if (spells[i].Name.ToUpper() == spell.Name.ToUpper() && spells[i].Level.ToUpper() == spell.Level.ToUpper())
                {
                    indexMatch = i;
                    return true;
                }
            }
            indexMatch = -1;
            return false;
        }

        private bool FindSpell(List<Spell> spells, int spellIndex, out int matchIndex)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                if (spells[i].Index == spellIndex)
                {
                    matchIndex = i;
                    return true;
                }
            }
            matchIndex = -1;
            return false;
        }

        private void TvTemplates_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (tvTemplates.SelectedNode != null)
            {
                if (tvTemplates.SelectedNode.Nodes.Count == 0)
                {
                    try
                    {
                        LoadPreview((Summon)tvTemplates.SelectedNode.Tag);
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void LoadPreview(Summon st)
        {
            sittingSummonIndex = st.Index;
            cbSummonSpell.SelectedItem = st.SummonSpell;
            cbSummonSpell.Text = st.SummonSpell;
            tbName.Text = st.Name;
            nudSTR.Value = st.Abilities.Str;
            nudDEX.Value = st.Abilities.Dex;
            nudCON.Value = st.Abilities.Con;
            nudINT.Value = st.Abilities.Int;
            nudWIS.Value = st.Abilities.Wis;
            nudCHA.Value = st.Abilities.Cha;

            nudFort.Value = st.Saves.Fort;
            nudRef.Value = st.Saves.Ref;
            nudWill.Value = st.Saves.Will;

            nudLevel.Value = st.Level;
            nudHD.Value = st.HitDie;
            nudNatural.Value = st.NaturalArmor;
            nudBAB.Value = st.BAB;
            nudGrapple.Value = st.Grapple;

            cbSize.SelectedItem = st.Size.ToString();
            tbType.Text = st.Type;
            tbEnvironment.Text = st.Environment;
            tbSpace.Text = st.Space;
            tbReach.Text = st.Reach;
            tbMovement.Text = st.Movement;

            rtbSkills.Text = ListToSeparated(st.Skills, ",");
            tbFeats.Text = ListToSeparated(st.Feats, ",");
            tbSpecAtks.Text = ListToSeparated(st.SpecAtks, ",");
            tbSpecQual.Text = ListToSeparated(st.SpecQual, ",");
            rtbCombat.Text = ListToSeparated(st.Notes, "\r");

            rtbAttacks.Text = ListToSeparated(st.Attacks, "\r");
            rtbFullAttacks.Text = ListToSeparated(st.FullAttacks, "\r");
        }

        private void BtnTryParseSpell_Click(object sender, EventArgs e)
        {
            ClearSpellPage();
            Spell tryParseSpell = new Spell(FileParser.GetSpellFromText(rtbSpellText.Text));
            tbSpellName.Text = tryParseSpell.Name;
            tbSpellClass.Text = tryParseSpell.Class;
            tbSpellLevel.Text = tryParseSpell.Level;
            tbSpellDomain.Text = tryParseSpell.Domain;
            tbSpellComponents.Text = tryParseSpell.Components;
            tbSpellCastingTime.Text = tryParseSpell.Casting;
            tbSpellRange.Text = tryParseSpell.Range;
            tbSpellTarget.Text = tryParseSpell.Target;
            tbSpellEffect.Text = tryParseSpell.Effect;
            tbSpellDuration.Text = tryParseSpell.Duration;
            tbSpellSaving.Text = tryParseSpell.SavingThrow;
            tbSpellResistance.Text = tryParseSpell.SpellResistance;
            chkFavoriteSpell.Checked = false;
            tbSource.Text = "";
            rtbSpellDescription.Text = tryParseSpell.Description;
        }

        private void BtnTryParseSummon_Click(object sender, EventArgs e)
        {
            ClearSummonPage();
            Summon st = FileParser.GetSummonTemplateFromText(rtbSummonText.Text);
            LoadPreview(st);
        }

        private string ListToSeparated(List<FullAttack> stringList, string sep)
        {
            string builder = "";
            for (int i = 0; i < stringList.Count; i++)
            {
                if (i == 0)
                {
                    builder = stringList[i].ToString();
                }
                else
                {
                    builder += sep + stringList[i].ToString();
                }
            }
            return builder;
        }

        private string ListToSeparated(List<Attack> stringList, string sep)
        {
            string builder = "";
            for (int i = 0; i < stringList.Count; i++)
            {
                if (i == 0)
                {
                    builder = stringList[i].ToString();
                }
                else
                {
                    builder += sep + stringList[i].ToString();
                }
            }
            return builder;
        }

        private string ListToSeparated(List<string> stringList, string sep)
        {
            string builder = "";
            for (int i = 0; i < stringList.Count; i++)
            {
                if (i == 0)
                {
                    builder = stringList[i];
                }
                else
                {
                    builder += sep + stringList[i];
                }
            }
            return builder;
        }

        private void TvTemplates_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                string xmlPath = tbSummonsXML.Text;//Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml");

                if (!File.Exists(xmlPath))
                {
                    MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (tvTemplates.SelectedNode != null)
                {
                    if (tvTemplates.SelectedNode.Tag is Summon)
                    {
                        if (MessageBox.Show(string.Format("Are you sure you want to delete {0} from this list?", ((Summon)tvTemplates.SelectedNode.Tag).Name), "Delete Summon Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
                        {
                            Summons summons = Summons.Retrieve(xmlPath);

                            if (IsSummonExists(summons, (Summon)tvTemplates.SelectedNode.Tag, out int indexMatch))
                            {
                                summons.RemoveAt(indexMatch);
                            }

                            summons.Save(xmlPath);
                            RefreshSummonPage();
                        }
                    }
                }
            }
        }

        private void BtnDev_Click(object sender, EventArgs e)
        {
            return;

            Spells spells = Spells.Retrieve(tbSpellsXML.Text);

            for (int i = 0; i < spells.Count; i++)
            {
                spells[i].Index = i + 1;
            }

            spells.Save(tbSpellsXML.Text);

            Summons summonTs = Summons.Retrieve(tbSummonsXML.Text);

            for (int i = 0; i < summonTs.Count; i++)
            {
                summonTs[i].Index = i + 1;
            }

            summonTs.Save(tbSummonsXML.Text);

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);
                DirectoryInfo[] folders = di.GetDirectories();
                for (int i = 0; i < folders.Length; i++)
                {
                    DirectoryInfo thisSummon = folders[i];
                    FileInfo[] files = thisSummon.GetFiles();
                    for (int j = 0; j < files.Length; j++)
                    {
                        Summon st = FileParser.GetSummonTemplateFromFile(files[j].FullName);
                        st.SummonSpell = thisSummon.Name;

                        Summons summons = Summons.Retrieve(tbSummonsXML.Text);
                        if (IsSummonExists(summons, st, out int removal))
                        {
                            if (MessageBox.Show("Would you like to overwrite?", "Summon already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                summons.RemoveAt(removal);
                                summons.Add(st);
                            }
                        }
                        else
                        {
                            summons.Add(st);
                        }

                        summons.Save(tbSummonsXML.Text);
                    }
                }
            }
        }

        private void FavoriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xmlPath = tbSpellsXML.Text; //string.IsNullOrEmpty(tbOverrideSpellXML.Text) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml") : tbOverrideSpellXML.Text;

            Spells spells = Spells.Retrieve(xmlPath);
            Spell thisSpell = (Spell)((TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).SourceControl).SelectedNode.Tag;  //((TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent().tvSpells).SelectedNode.Tag;

            if (IsSpellExists(spells, thisSpell, out int index))
            {
                if (spells[index].Favorited) { spells[index].Favorited = false; }
                else if (!spells[index].Favorited) { spells[index].Favorited = true; }
                else { spells[index].Favorited = true; }

                spells.Save(xmlPath);
            }

            RefreshSpellPage(GetSpellNodesExpansion());
        }

        int sittingSpellID = 0;
        int sittingSummonIndex = 0;

        private void ClearSummonPage()
        {
            sittingSummonIndex = 0;
            cbSummonSpell.Text = "";

            rtbSummonText.Clear();
            tbName.Clear();
            nudSTR.Value = 0;
            nudDEX.Value = 0;
            nudCON.Value = 0;
            nudINT.Value = 0;
            nudWIS.Value = 0;
            nudCHA.Value = 0;

            nudFort.Value = 0;
            nudRef.Value = 0;
            nudWill.Value = 0;

            nudLevel.Value = 0;
            nudHD.Value = 0;
            nudNatural.Value = 0;
            nudBAB.Value = 0;
            nudGrapple.Value = 0;

            cbSize.ResetText();
            tbType.Clear();
            tbEnvironment.Clear();
            tbSpace.Clear();
            tbReach.Clear();
            tbMovement.Clear();
            rtbSkills.Clear();
            tbFeats.Clear();
            tbSpecAtks.Clear();
            tbSpecQual.Clear();
            rtbCombat.Clear();
            rtbAttacks.Clear();
            rtbFullAttacks.Clear();
        }

        private void NudCasterLevel_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CasterLevel = nudCasterLevel.Value;
            Properties.Settings.Default.Save();
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearSummonPage();
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xmlPath = tbSummonsXML.Text;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml");

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tbName.Text == "")
            {
                MessageBox.Show("Summon Name must be filled out", "Summon Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cbSummonSpell.Text == "")
            {
                MessageBox.Show("Summon Spell/Category must be specified", "Summon Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Summon addSummon = new Summon();
            addSummon.SummonSpell = cbSummonSpell.Text;
            addSummon.Name = tbName.Text;
            addSummon.Abilities = new Abilities((int)nudSTR.Value, (int)nudDEX.Value, (int)nudCON.Value, (int)nudINT.Value, (int)nudWIS.Value, (int)nudCHA.Value);
            addSummon.Saves = new Saves((int)nudFort.Value, (int)nudRef.Value, (int)nudWill.Value);

            addSummon.Level = (int)nudLevel.Value;
            addSummon.HitDie = (int)nudHD.Value;
            addSummon.NaturalArmor = (int)nudNatural.Value;
            addSummon.BAB = (int)nudBAB.Value;
            addSummon.Grapple = (int)nudGrapple.Value;

            if (cbSize.SelectedItem.ToString() == "Diminuitive") { addSummon.Size = DruidAssistant.Size.Diminuitive; }
            else if (cbSize.SelectedItem.ToString() == "Fine") { addSummon.Size = DruidAssistant.Size.Fine; }
            else if (cbSize.SelectedItem.ToString() == "Tiny") { addSummon.Size = DruidAssistant.Size.Tiny; }
            else if (cbSize.SelectedItem.ToString() == "Small") { addSummon.Size = DruidAssistant.Size.Small; }
            else if (cbSize.SelectedItem.ToString() == "Medium") { addSummon.Size = DruidAssistant.Size.Medium; }
            else if (cbSize.SelectedItem.ToString() == "Large") { addSummon.Size = DruidAssistant.Size.Large; }
            else if (cbSize.SelectedItem.ToString() == "Huge") { addSummon.Size = DruidAssistant.Size.Huge; }
            else if (cbSize.SelectedItem.ToString() == "Gargantuan") { addSummon.Size = DruidAssistant.Size.Gargantuan; }
            else if (cbSize.SelectedItem.ToString() == "Colossal") { addSummon.Size = DruidAssistant.Size.Colossal; }
            else { addSummon.Size = DruidAssistant.Size.Medium; }

            addSummon.Type = tbType.Text;
            addSummon.Environment = tbEnvironment.Text;
            addSummon.Space = tbSpace.Text;
            addSummon.Reach = tbReach.Text;
            addSummon.Movement = tbMovement.Text;

            addSummon.Skills = rtbSkills.Text.Split(',').ToList();
            addSummon.Feats = tbFeats.Text.Split(',').ToList();
            addSummon.SpecAtks = tbSpecAtks.Text.Split(',').ToList();
            addSummon.SpecQual = tbSpecQual.Text.Split(',').ToList();
            addSummon.Notes = rtbCombat.Text.Split('\r').ToList();

            addSummon.Attacks = FileParser.GetAttacks(rtbAttacks.Text);
            addSummon.FullAttacks = FileParser.GetFullAttacks(rtbFullAttacks.Text);

            Summons summons = Summons.Retrieve(xmlPath);

            if (IsSummonExists(summons, addSummon, out int indexMatch))
            {
                if (MessageBox.Show("Summon of the same name and level already exists\rWould you like to add a duplicate?", "Spell already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    addSummon.Index = summons.Max(x => x.Index) + 1;
                    summons.Add(addSummon);
                    ClearSummonPage();
                }
            }
            else
            {
                addSummon.Index = summons.Max(x => x.Index) + 1;
                summons.Add(addSummon);
                ClearSummonPage();
            }

            summons.Save(xmlPath);
            RefreshSummonPage();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xmlPath = tbSummonsXML.Text;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml");

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (sittingSummonIndex == 0)
            {
                MessageBox.Show("Summon does not exist.", "Summon Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (tbName.Text == "")
                {
                    MessageBox.Show("Summon Name must be filled out", "Summon Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (cbSummonSpell.Text == "")
                {
                    MessageBox.Show("Summon Spell/Category must be specified", "Summon Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Summon updateSummon = new Summon();
                updateSummon.Index = sittingSummonIndex;
                updateSummon.SummonSpell = cbSummonSpell.Text;
                updateSummon.Name = tbName.Text;
                updateSummon.Abilities = new Abilities((int)nudSTR.Value, (int)nudDEX.Value, (int)nudCON.Value, (int)nudINT.Value, (int)nudWIS.Value, (int)nudCHA.Value);
                updateSummon.Saves = new Saves((int)nudFort.Value, (int)nudRef.Value, (int)nudWill.Value);

                updateSummon.Level = (int)nudLevel.Value;
                updateSummon.HitDie = (int)nudHD.Value;
                updateSummon.NaturalArmor = (int)nudNatural.Value;
                updateSummon.BAB = (int)nudBAB.Value;
                updateSummon.Grapple = (int)nudGrapple.Value;

                if (cbSize.SelectedItem.ToString() == "Diminuitive") { updateSummon.Size = DruidAssistant.Size.Diminuitive; }
                else if (cbSize.SelectedItem.ToString() == "Fine") { updateSummon.Size = DruidAssistant.Size.Fine; }
                else if (cbSize.SelectedItem.ToString() == "Tiny") { updateSummon.Size = DruidAssistant.Size.Tiny; }
                else if (cbSize.SelectedItem.ToString() == "Small") { updateSummon.Size = DruidAssistant.Size.Small; }
                else if (cbSize.SelectedItem.ToString() == "Medium") { updateSummon.Size = DruidAssistant.Size.Medium; }
                else if (cbSize.SelectedItem.ToString() == "Large") { updateSummon.Size = DruidAssistant.Size.Large; }
                else if (cbSize.SelectedItem.ToString() == "Huge") { updateSummon.Size = DruidAssistant.Size.Huge; }
                else if (cbSize.SelectedItem.ToString() == "Gargantuan") { updateSummon.Size = DruidAssistant.Size.Gargantuan; }
                else if (cbSize.SelectedItem.ToString() == "Colossal") { updateSummon.Size = DruidAssistant.Size.Colossal; }
                else { updateSummon.Size = DruidAssistant.Size.Medium; }

                updateSummon.Type = tbType.Text;
                updateSummon.Environment = tbEnvironment.Text;
                updateSummon.Space = tbSpace.Text;
                updateSummon.Reach = tbReach.Text;
                updateSummon.Movement = tbMovement.Text;

                updateSummon.Skills = rtbSkills.Text.Split(',').ToList();
                updateSummon.Feats = tbFeats.Text.Split(',').ToList();
                updateSummon.SpecAtks = tbSpecAtks.Text.Split(',').ToList();
                updateSummon.SpecQual = tbSpecQual.Text.Split(',').ToList();
                updateSummon.Notes = rtbCombat.Text.Split('\r').ToList();

                updateSummon.Attacks = FileParser.GetAttacks(rtbAttacks.Text);
                updateSummon.FullAttacks = FileParser.GetFullAttacks(rtbFullAttacks.Text);

                Summons summons = Summons.Retrieve(xmlPath);

                if (FindSummon(summons, sittingSummonIndex, out int indexMatch))
                {
                    summons[indexMatch] = updateSummon;
                }
                else
                {
                    MessageBox.Show("Summon cannot be found.", "Summon Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                summons.Save(xmlPath);
                RefreshSummonPage();
            }
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xmlPath = tbSummonsXML.Text;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml");

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (sittingSummonIndex == 0)
            {
                MessageBox.Show("Summon does not exist.", "Summon Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                Summons summons = Summons.Retrieve(xmlPath);

                if (FindSummon(summons, sittingSummonIndex, out int indexMatch))
                {
                    summons.RemoveAt(indexMatch);
                }
                else
                {
                    MessageBox.Show("Summon cannot be found.", "Summon Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                ClearSummonPage();
                summons.Save(xmlPath);
                summons.Save(xmlPath);
                RefreshSummonPage();
            }
        }

        private void TvSpells_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            (sender as TreeView).SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right)
            {
                if ((sender as TreeView).SelectedNode.Tag is Spell)
                {
                    Spell thisSpell = (Spell)(sender as TreeView).SelectedNode.Tag;
                    if (thisSpell.Favorited)
                    {
                        favoriteToolStripMenuItem.Text = "Unfavorite";
                    }
                    else
                    {
                        favoriteToolStripMenuItem.Text = "Favorite";
                    }
                    cmsSpells.Show(Cursor.Position);
                }
            }
        }

        private void SpellsPageTV_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if ((sender as TreeView).SelectedNode != null)
            {
                if ((sender as TreeView).SelectedNode.GetNodeCount(false) == 0)
                {
                    if ((sender as TreeView).SelectedNode.Tag is Spell)
                    {
                        Spell selectedSpell = (sender as TreeView).SelectedNode.Tag as Spell;
                        LoadSpellPage(selectedSpell);
                    }
                }
            }
        }

        private void AddSpellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xmlPath = tbSpellsXML.Text;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tbSpellName.Text == "")
            {
                MessageBox.Show("Spell Name must be filled out", "Spell Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(tbSpellLevel.Text, "[0-9]{1}"))
            {
                MessageBox.Show("Spell Level must be a number 0 - 9", "Spell Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Spell addSpell = new Spell();
            addSpell.Favorited = chkFavoriteSpell.Checked;
            addSpell.Name = tbSpellName.Text;
            addSpell.Class = tbSpellClass.Text;
            addSpell.Level = tbSpellLevel.Text;
            addSpell.Domain = tbSpellDomain.Text;
            addSpell.Components = tbSpellComponents.Text;
            addSpell.Casting = tbSpellCastingTime.Text;
            addSpell.Range = tbSpellRange.Text;
            addSpell.Target = tbSpellTarget.Text;
            addSpell.Effect = tbSpellEffect.Text;
            addSpell.Duration = tbSpellDuration.Text;
            addSpell.SavingThrow = tbSpellSaving.Text;
            addSpell.SpellResistance = tbSpellResistance.Text;
            addSpell.SourceBook = tbSource.Text;
            addSpell.Description = rtbSpellDescription.Text;
            addSpell.PersonalNotes = rtbPersonalNotes.Text;

            Spells spells = Spells.Retrieve(xmlPath);

            if (IsSpellExists(spells, addSpell, out int indexMatch))
            {
                if (MessageBox.Show("Spell of the same name and level already exists\rWould you like to add a duplicate?", "Spell already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    addSpell.Index = spells.Max(x => x.Index) + 1;
                    spells.Add(addSpell);
                    ClearSpellPage();
                }
            }
            else
            {
                addSpell.Index = spells.Max(x => x.Index) + 1;
                spells.Add(addSpell);
                ClearSpellPage();
            }
            rtbSpellText.Clear();
            spells.Save(xmlPath);
            RefreshSpellPage(GetSpellNodesExpansion());
        }

        private void SaveSpellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xmlPath = tbSpellsXML.Text;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (sittingSpellID == 0)
            {
                MessageBox.Show("Spell does not exist.", "Spell Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (tbSpellName.Text == "")
                {
                    MessageBox.Show("Spell Name must be filled out", "Spell Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!Regex.IsMatch(tbSpellLevel.Text, "[0-9]{1}"))
                {
                    MessageBox.Show("Spell Level must be a number 0 - 9", "Spell Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Spell updateSpell = new Spell();
                updateSpell.Index = sittingSpellID;
                updateSpell.Favorited = chkFavoriteSpell.Checked;
                updateSpell.Name = tbSpellName.Text;
                updateSpell.Class = tbSpellClass.Text;
                updateSpell.Level = tbSpellLevel.Text;
                updateSpell.Domain = tbSpellDomain.Text;
                updateSpell.Components = tbSpellComponents.Text;
                updateSpell.Casting = tbSpellCastingTime.Text;
                updateSpell.Range = tbSpellRange.Text;
                updateSpell.Target = tbSpellTarget.Text;
                updateSpell.Effect = tbSpellEffect.Text;
                updateSpell.Duration = tbSpellDuration.Text;
                updateSpell.SavingThrow = tbSpellSaving.Text;
                updateSpell.SpellResistance = tbSpellResistance.Text;
                updateSpell.SourceBook = tbSource.Text;
                updateSpell.Description = rtbSpellDescription.Text;
                updateSpell.PersonalNotes = rtbPersonalNotes.Text;

                Spells spells = Spells.Retrieve(xmlPath);

                if (FindSpell(spells, sittingSpellID, out int indexMatch))
                {
                    spells[indexMatch] = updateSpell;
                }
                else
                {
                    MessageBox.Show("Spell cannot be found.", "Spell Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                spells.Save(xmlPath);
                RefreshSpellPage(GetSpellNodesExpansion());
            }
        }

        private void DeleteSpellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xmlPath = tbSpellsXML.Text;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");

            if (MessageBox.Show("Are you sure you want to delete this spell?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (sittingSpellID == 0)
            {
                MessageBox.Show("Spell does not exist.", "Spell Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {

                Spells spells = Spells.Retrieve(xmlPath);

                if (FindSpell(spells, sittingSpellID, out int indexMatch))
                {
                    spells.RemoveAt(indexMatch);
                }
                else
                {
                    MessageBox.Show("Spell cannot be found.", "Spell Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                ClearSpellPage();
                spells.Save(xmlPath);

                RefreshSpellPage(GetSpellNodesExpansion());
            }
        }

        private void ClearSpellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearSpellPage();
            rtbSpellText.Clear();
        }

        private void XMLFiles_TextChanged(object sender, EventArgs e)
        {
            if (sender == tbSummonsXML)
            {
                Properties.Settings.Default.SummonsXML = tbSummonsXML.Text;
            }
            if (sender == tbSpellsXML)
            {
                Properties.Settings.Default.SpellsXML = tbSpellsXML.Text;
            }
        }

        private void CreateNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox tbSender = (TextBox)((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).SourceControl;
            if (tbSender == tbSummonsXML)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "XML Files (*.xml)| *.xml";
                    sfd.Title = "New Summon XML File";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        Summons s = new Summons();
                        s.Save(sfd.FileName);
                    }
                }
            }
            if (tbSender == tbSpellsXML)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "XML Files (*.xml)| *.xml";
                    sfd.Title = "New Spell XML File";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        Spells s = new Spells();
                        s.Save(sfd.FileName);
                    }
                }
            }
        }

        private void ChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox tbSender = (TextBox)((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).SourceControl;
            if (tbSender == tbSummonsXML)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Multiselect = false;
                    ofd.Filter = "XML Files (*.xml)| *.xml";
                    ofd.CheckFileExists = true;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Summons s = Summons.Retrieve(ofd.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("File not valid\rError: {0}", ex.Message), "File Select Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        tbSummonsXML.Text = ofd.FileName;
                    }
                }
            }
            if (tbSender == tbSpellsXML)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Multiselect = false;
                    ofd.Filter = "XML Files (*.xml)| *.xml";
                    ofd.CheckFileExists = true;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Spells s = Spells.Retrieve(ofd.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("File not valid\rError: {0}", ex.Message), "File Select Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        tbSpellsXML.Text = ofd.FileName;
                    }
                }
            }
        }

        private void ChkSpells_CheckedChanged(object sender, EventArgs e)
        {
            tlpSpells.RowStyles[1].SizeType = chkShowAllSpells.Checked ? SizeType.Percent : SizeType.Absolute;
            tlpSpells.RowStyles[1].Height = chkShowAllSpells.Checked ? 50f : 0f;
            chkShowAllSpells.BackColor = chkShowAllSpells.Checked ? Color.FromArgb(192, 255, 192) : Color.FromArgb(255, 192, 192);
            tvSpells.Visible = chkShowAllSpells.Checked;

            tlpSpells.RowStyles[3].SizeType = chkShowFavoriteSpells.Checked ? SizeType.Percent : SizeType.Absolute;
            tlpSpells.RowStyles[3].Height = chkShowFavoriteSpells.Checked ? 50f : 0f;
            chkShowFavoriteSpells.BackColor = chkShowFavoriteSpells.Checked ? Color.FromArgb(192, 255, 192) : Color.FromArgb(255, 192, 192);
            tvFavoritedSpells.Visible = chkShowFavoriteSpells.Checked;
        }

        private void DecomposeLineBreaksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = (RichTextBox)((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).SourceControl;
            rtb.Text = rtb.Text.Replace("\n ", " ").Replace("\n", " ").Replace("\r ", " ").Replace("\r", " ").Replace("  ", " ").Replace("  ", "");
        }
    }
}