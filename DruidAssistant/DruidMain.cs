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
        public SummonTemplate currentST;

        public DruidMain()
        {
            InitializeComponent();
        }

        private void DruidMain_Load(object sender, EventArgs e)
        {
            RefreshSummonPage();
            RefreshSpellPage(new bool[] { true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false });
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
            if (clbAugments.GetItemChecked(0))
            {
                st = Feats.AugmentSummoning(st);
            }
            if (clbAugments.GetItemChecked(1))
            {
                st = Feats.GreenboundSummoning.Augment(st);
            }
            if (clbAugments.GetItemChecked(2))
            {
                st = Items.ObadHaisGreenMan(st);
            }
            //if (clbAugments.GetItemChecked(3))
            //{
            //    currentST = SummonTemplate.AshboundSummoning(currentST);
            //}
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
        private static decimal ScoreToMod(decimal input)
        {
            return Math.Floor((input - 10) / 2);
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

        private void RtbTextInvestigate_TextChanged(object sender, EventArgs e)
        {
            UpdateRTBPreview();
        }

        private void UpdateRTBPreview()
        {

        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (tvTemplates.SelectedNode != null)
            {
                if (tvTemplates.SelectedNode.Tag is SummonTemplate)
                {
                    SummonToForm st = new SummonToForm((SummonTemplate)tvTemplates.SelectedNode.Tag);
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
                    LoadPreview((SummonTemplate)tvTemplates.SelectedNode.Tag);
                }
            }
        }

        private void RefreshSpellPage(bool[] expanders)
        {
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");

            if (tbOverrideSpellXML.Text != "")
            {
                xmlPath = tbOverrideSpellXML.Text;
            }

            tbOverrideSpellXML.Text = xmlPath;

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tvSpells.Nodes.Clear();

            TreeNode allSpells = tvSpells.Nodes.Add("All Spells");
            TreeNode favoriteSpells = tvSpells.Nodes.Add("Favorite Spells");

            TreeNode tn0 = allSpells.Nodes.Add("Level 0");
            TreeNode tn1 = allSpells.Nodes.Add("Level 1");
            TreeNode tn2 = allSpells.Nodes.Add("Level 2");
            TreeNode tn3 = allSpells.Nodes.Add("Level 3");
            TreeNode tn4 = allSpells.Nodes.Add("Level 4");
            TreeNode tn5 = allSpells.Nodes.Add("Level 5");
            TreeNode tn6 = allSpells.Nodes.Add("Level 6");
            TreeNode tn7 = allSpells.Nodes.Add("Level 7");
            TreeNode tn8 = allSpells.Nodes.Add("Level 8");
            TreeNode tn9 = allSpells.Nodes.Add("Level 9");

            TreeNode tnf0 = favoriteSpells.Nodes.Add("Level 0");
            TreeNode tnf1 = favoriteSpells.Nodes.Add("Level 1");
            TreeNode tnf2 = favoriteSpells.Nodes.Add("Level 2");
            TreeNode tnf3 = favoriteSpells.Nodes.Add("Level 3");
            TreeNode tnf4 = favoriteSpells.Nodes.Add("Level 4");
            TreeNode tnf5 = favoriteSpells.Nodes.Add("Level 5");
            TreeNode tnf6 = favoriteSpells.Nodes.Add("Level 6");
            TreeNode tnf7 = favoriteSpells.Nodes.Add("Level 7");
            TreeNode tnf8 = favoriteSpells.Nodes.Add("Level 8");
            TreeNode tnf9 = favoriteSpells.Nodes.Add("Level 9");

            Spells spells = FileParser.GetSpellsFromXML(xmlPath);

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

            if (expanders.Length == 22)
            {
                if (expanders[0]) { allSpells.Expand(); }
                if (expanders[1]) { tn0.Expand(); }
                if (expanders[2]) { tn1.Expand(); }
                if (expanders[3]) { tn2.Expand(); }
                if (expanders[4]) { tn3.Expand(); }
                if (expanders[5]) { tn4.Expand(); }
                if (expanders[6]) { tn5.Expand(); }
                if (expanders[7]) { tn6.Expand(); }
                if (expanders[8]) { tn7.Expand(); }
                if (expanders[9]) { tn8.Expand(); }
                if (expanders[10]) { tn9.Expand(); }

                if (expanders[11]) { favoriteSpells.Expand(); }
                if (expanders[12]) { tnf0.Expand(); }
                if (expanders[13]) { tnf1.Expand(); }
                if (expanders[14]) { tnf2.Expand(); }
                if (expanders[15]) { tnf3.Expand(); }
                if (expanders[16]) { tnf4.Expand(); }
                if (expanders[17]) { tnf5.Expand(); }
                if (expanders[18]) { tnf6.Expand(); }
                if (expanders[19]) { tnf7.Expand(); }
                if (expanders[20]) { tnf8.Expand(); }
                if (expanders[21]) { tnf9.Expand(); }
            }

            allSpells.EnsureVisible();

        }

        private void RefreshSummonPage()
        {
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml");

            if (tbOverrideSummonXML.Text != "")
            {
                xmlPath = tbOverrideSummonXML.Text;
            }

            tbOverrideSummonXML.Text = xmlPath;

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SummonTemplates summons = FileParser.GetSummonsFromXML(xmlPath);

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
            bool[] expander = new bool[22];

            expander[0] = tvSpells.Nodes[0].IsExpanded;
            expander[1] = tvSpells.Nodes[0].Nodes[0].IsExpanded;
            expander[2] = tvSpells.Nodes[0].Nodes[1].IsExpanded;
            expander[3] = tvSpells.Nodes[0].Nodes[2].IsExpanded;
            expander[4] = tvSpells.Nodes[0].Nodes[3].IsExpanded;
            expander[5] = tvSpells.Nodes[0].Nodes[4].IsExpanded;
            expander[6] = tvSpells.Nodes[0].Nodes[5].IsExpanded;
            expander[7] = tvSpells.Nodes[0].Nodes[6].IsExpanded;
            expander[8] = tvSpells.Nodes[0].Nodes[7].IsExpanded;
            expander[9] = tvSpells.Nodes[0].Nodes[8].IsExpanded;
            expander[10] = tvSpells.Nodes[0].Nodes[9].IsExpanded;

            expander[11] = tvSpells.Nodes[1].IsExpanded;
            expander[12] = tvSpells.Nodes[1].Nodes[0].IsExpanded;
            expander[13] = tvSpells.Nodes[1].Nodes[1].IsExpanded;
            expander[14] = tvSpells.Nodes[1].Nodes[2].IsExpanded;
            expander[15] = tvSpells.Nodes[1].Nodes[3].IsExpanded;
            expander[16] = tvSpells.Nodes[1].Nodes[4].IsExpanded;
            expander[17] = tvSpells.Nodes[1].Nodes[5].IsExpanded;
            expander[18] = tvSpells.Nodes[1].Nodes[6].IsExpanded;
            expander[19] = tvSpells.Nodes[1].Nodes[7].IsExpanded;
            expander[20] = tvSpells.Nodes[1].Nodes[8].IsExpanded;
            expander[21] = tvSpells.Nodes[1].Nodes[9].IsExpanded;
            return expander;
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
                        chkFavoriteSpell.Checked = selectedSpell.Favorited;
                        tbSpellName.Text = selectedSpell.Name;
                        tbSpellClass.Text = selectedSpell.Class;
                        tbSpellLevel.Text = selectedSpell.Level;
                        tbSpellDomain.Text = selectedSpell.Domain;
                        tbSpellComponents.Text = selectedSpell.Components;
                        tbSpellCastingTime.Text = selectedSpell.Casting;
                        tbSpellRange.Text = selectedSpell.Range;
                        tbSpellTarget.Text = selectedSpell.Target;
                        tbSpellEffect.Text = selectedSpell.Effect;
                        tbSpellDuration.Text = selectedSpell.Duration;
                        tbSpellSaving.Text = selectedSpell.SavingThrow;
                        tbSpellResistance.Text = selectedSpell.SpellResistance;
                        tbSource.Text = selectedSpell.SourceBook;
                        rtbSpellDescription.Text = selectedSpell.Description;
                    }

                }
            }
        }

        private bool SpellExists(List<Spell> spells, Spell spell, out int indexMatch)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                if (spells[i].Name == spell.Name && spells[i].Level == spell.Level)
                {
                    indexMatch = i;
                    return true;
                }
            }
            indexMatch = -1;
            return false;
        }

        private void BtnSaveSpellChanges_Click(object sender, EventArgs e)
        {
            if (tbSpellName.Text == "")
            {
                MessageBox.Show("Spell Name must be filled out", "Spell Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(tbSpellLevel.Text, "[0-9]{1}"))
            {
                MessageBox.Show("Spell Level must be a number 0 - 9", "Spell Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Spell dedicateSpell = new Spell();
            dedicateSpell.Favorited = chkFavoriteSpell.Checked;
            dedicateSpell.Name = tbSpellName.Text;
            dedicateSpell.Class = tbSpellClass.Text;
            dedicateSpell.Level = tbSpellLevel.Text;
            dedicateSpell.Domain = tbSpellDomain.Text;
            dedicateSpell.Components = tbSpellComponents.Text;
            dedicateSpell.Casting = tbSpellCastingTime.Text;
            dedicateSpell.Range = tbSpellRange.Text;
            dedicateSpell.Target = tbSpellTarget.Text;
            dedicateSpell.Effect = tbSpellEffect.Text;
            dedicateSpell.Duration = tbSpellDuration.Text;
            dedicateSpell.SavingThrow = tbSpellSaving.Text;
            dedicateSpell.SpellResistance = tbSpellResistance.Text;
            dedicateSpell.SourceBook = tbSource.Text;
            dedicateSpell.Description = rtbSpellDescription.Text;

            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");

            if (tbOverrideSpellXML.Text != "")
            {
                xmlPath = tbOverrideSpellXML.Text;
            }

            Spells spells = FileParser.GetSpellsFromXML(xmlPath);

            if (SpellExists(spells, dedicateSpell, out int removal))
            {
                if (MessageBox.Show("Would you like to overwrite?", "Spell already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    spells.RemoveAt(removal);
                    spells.Add(dedicateSpell);
                    ClearSpellPage();
                }
            }
            else
            {
                spells.Add(dedicateSpell);
                ClearSpellPage();
            }

            FileParser.DedicateSpellsToXML(xmlPath, spells);
        }

        private void BtnClearSpell_Click(object sender, EventArgs e)
        {
            ClearSpellPage();
        }

        private void ClearSpellPage()
        {
            rtbSpellText.Clear();
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
        }

        private void BtnRefreshSummons_Click(object sender, EventArgs e)
        {
            RefreshSummonPage();
        }

        private void BtnClearSummonPage_Click(object sender, EventArgs e)
        {
            ClearSummonPage();
        }

        private void BtnSaveSummonChanges_Click(object sender, EventArgs e)
        {
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml");

            if (tbOverrideSummonXML.Text != "")
            {
                xmlPath = tbOverrideSummonXML.Text;
            }

            tbOverrideSummonXML.Text = xmlPath;

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

            SummonTemplate st = new SummonTemplate();
            st.SummonSpell = cbSummonSpell.Text;
            st.Name = tbName.Text;
            st.Abilities = new Abilities((int)nudSTR.Value, (int)nudDEX.Value, (int)nudCON.Value, (int)nudINT.Value, (int)nudWIS.Value, (int)nudCHA.Value);
            st.Saves = new Saves((int)nudFort.Value, (int)nudRef.Value, (int)nudWill.Value);

            st.Level = (int)nudLevel.Value;
            st.HitDie = (int)nudHD.Value;
            st.NaturalArmor = (int)nudNatural.Value;
            st.BAB = (int)nudBAB.Value;
            st.Grapple = (int)nudGrapple.Value;

            if (cbSize.SelectedItem.ToString() == "Diminuitive") { st.Size = DruidAssistant.Size.Diminuitive; }
            else if (cbSize.SelectedItem.ToString() == "Fine") { st.Size = DruidAssistant.Size.Fine; }
            else if (cbSize.SelectedItem.ToString() == "Tiny") { st.Size = DruidAssistant.Size.Tiny; }
            else if (cbSize.SelectedItem.ToString() == "Small") { st.Size = DruidAssistant.Size.Small; }
            else if (cbSize.SelectedItem.ToString() == "Medium") { st.Size = DruidAssistant.Size.Medium; }
            else if (cbSize.SelectedItem.ToString() == "Large") { st.Size = DruidAssistant.Size.Large; }
            else if (cbSize.SelectedItem.ToString() == "Huge") { st.Size = DruidAssistant.Size.Huge; }
            else if (cbSize.SelectedItem.ToString() == "Gargantuan") { st.Size = DruidAssistant.Size.Gargantuan; }
            else if (cbSize.SelectedItem.ToString() == "Colossal") { st.Size = DruidAssistant.Size.Colossal; }
            else { st.Size = DruidAssistant.Size.Medium; }

            st.Type = tbType.Text;
            st.Environment = tbEnvironment.Text;
            st.Space = tbSpace.Text;
            st.Reach = tbReach.Text;
            st.Movement = tbMovement.Text;

            st.Skills = rtbSkills.Text.Split(',').ToList();
            st.Feats = tbFeats.Text.Split(',').ToList();
            st.SpecAtks = tbSpecAtks.Text.Split(',').ToList();
            st.SpecQual = tbSpecQual.Text.Split(',').ToList();
            st.Notes = rtbCombat.Text.Split('\r').ToList();

            st.Attacks = FileParser.GetAttacks(rtbAttacks.Text);
            st.FullAttacks = FileParser.GetFullAttacks(rtbFullAttacks.Text);

            SummonTemplates summons = FileParser.GetSummonsFromXML(xmlPath);

            if (SummonExists(summons, st, out int removal))
            {
                if (MessageBox.Show("Would you like to overwrite?", "Summon already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    summons.RemoveAt(removal);
                    summons.Add(st);
                    ClearSummonPage();
                }
            }
            else
            {
                summons.Add(st);
                ClearSummonPage();
            }

            FileParser.DedicateSummonsToXML(xmlPath, summons);
            RefreshSummonPage();
        }

        private void ClearSummonPage()
        {
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

        private bool SummonExists(List<SummonTemplate> summons, SummonTemplate summon, out int indexMatch)
        {
            for (int i = 0; i < summons.Count; i++)
            {
                if (summons[i].Name == summon.Name && summons[i].SummonSpell == summon.SummonSpell)
                {
                    indexMatch = i;
                    return true;
                }
            }
            indexMatch = -1;
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
                        LoadPreview((SummonTemplate)tvTemplates.SelectedNode.Tag);
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void LoadPreview(SummonTemplate st)
        {
            cbSummonSpell.SelectedItem = st.SummonSpell;
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

        private void BtnTryParse_Click(object sender, EventArgs e)
        {
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
            SummonTemplate st = FileParser.GetSummonTemplateFromText(rtbSummonText.Text);
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
                string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml");

                if (tbOverrideSummonXML.Text != "")
                {
                    xmlPath = tbOverrideSummonXML.Text;
                }

                tbOverrideSummonXML.Text = xmlPath;

                if (!File.Exists(xmlPath))
                {
                    MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (tvTemplates.SelectedNode != null)
                {
                    if (tvTemplates.SelectedNode.Tag is SummonTemplate)
                    {
                        if (MessageBox.Show(string.Format("Are you sure you want to delete {0} from this list?", ((SummonTemplate)tvTemplates.SelectedNode.Tag).Name), "Delete Summon Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
                        {
                            SummonTemplates summons = FileParser.GetSummonsFromXML(tbOverrideSummonXML.Text);

                            if (SummonExists(summons, (SummonTemplate)tvTemplates.SelectedNode.Tag, out int indexMatch))
                            {
                                summons.RemoveAt(indexMatch);
                            }

                            FileParser.DedicateSummonsToXML(tbOverrideSummonXML.Text, summons);
                            RefreshSummonPage();
                        }
                    }
                }
            }
        }

        private void TvSpells_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void BtnDev_Click(object sender, EventArgs e)
        {
            return;
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
                        SummonTemplate st = FileParser.GetSummonTemplateFromFile(files[j].FullName);
                        st.SummonSpell = thisSummon.Name;

                        SummonTemplates summons = FileParser.GetSummonsFromXML(tbOverrideSummonXML.Text);
                        if (SummonExists(summons, st, out int removal))
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

                        FileParser.DedicateSummonsToXML(tbOverrideSummonXML.Text, summons);
                    }
                }
            }
        }


        private void BtnDevSpells_Click(object sender, EventArgs e)
        {
            return;
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");

            if (tbOverrideSpellXML.Text != "")
            {
                xmlPath = tbOverrideSpellXML.Text;
            }

            tbOverrideSpellXML.Text = xmlPath;

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Spells spells = FileParser.GetSpellsFromXML(tbOverrideSpellXML.Text);

            for (int i = 0; i < spells.Count; i++)
            {
                spells[i].SourceBook = "PHB";
            }

            FileParser.DedicateSpellsToXML(tbOverrideSpellXML.Text, spells);
            //RefreshSpellPage();
        }

        private void TbOverrideSpellXML_Validating(object sender, CancelEventArgs e)
        {
            if (File.Exists(tbOverrideSpellXML.Text))
            {
                tbOverrideSpellXML.BackColor = Color.FromArgb(192, 255, 192);
            }
            else
            {
                tbOverrideSpellXML.BackColor = Color.FromArgb(255, 192, 192);
            }
        }

        private void TbOverrideSummonXML_Validating(object sender, CancelEventArgs e)
        {
            if (File.Exists(tbOverrideSpellXML.Text))
            {
                tbOverrideSummonXML.BackColor = Color.FromArgb(192, 255, 192);
            }
            else
            {
                tbOverrideSummonXML.BackColor = Color.FromArgb(255, 192, 192);
            }
        }

        private void TvSpells_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvSpells.SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right)
            {
                if (tvSpells.SelectedNode.Tag is Spell)
                {
                    Spell thisSpell = (Spell)tvSpells.SelectedNode.Tag;
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

        private void FavoriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");

            if (tbOverrideSpellXML.Text != "")
            {
                xmlPath = tbOverrideSpellXML.Text;
            }

            Spells spells = FileParser.GetSpellsFromXML(xmlPath);
            Spell thisSpell = (Spell)tvSpells.SelectedNode.Tag;

            if (SpellExists(spells, thisSpell, out int index))
            {
                if (spells[index].Favorited) { spells[index].Favorited = false; }
                else if (!spells[index].Favorited) { spells[index].Favorited = true; }
                else { spells[index].Favorited = true; }

                FileParser.DedicateSpellsToXML(xmlPath, spells);
            }

            RefreshSpellPage(GetSpellNodesExpansion());
        }
    }
}