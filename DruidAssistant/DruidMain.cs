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
        int sittingSpellID = 0;
        int sittingSummonIndex = 0;
        string sittingSpellXML;
        string sittingSummonXML;

        public DruidMain()
        {
            InitializeComponent();
            lvAugments.Columns[lvAugments.Columns.Count - 1].Width = -2;
            SetElementsFromSettings();
        }

        private void SetElementsFromSettings()
        {
            nudCasterLevel.Value = Properties.Settings.Default.CasterLevel;
            sittingSummonXML = tbSummonsXML.Text = Properties.Settings.Default.SummonsXML;
            sittingSpellXML = tbSpellsXML.Text = Properties.Settings.Default.SpellsXML;
        }

        private void DruidMain_Load(object sender, EventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.Text = string.Format("Druid Summoning Assistant v{0}", ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4));
            }

            RefreshSummonPage();
            RefreshSpellPage();

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

            st.Rounds = st.PCCasterLevel * (int.TryParse(nudDurationMultiplier.Value.ToString(), out int durationMult) ? durationMult : 1);
            return st;
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (tvTemplates.SelectedNode != null)
            {
                if (tvTemplates.SelectedNode.Tag is Summon selSumm)
                {
                    SummonToForm st = new SummonToForm(selSumm);
                    st = AugmentTemplate(st);
                    Summoned sc = new Summoned(st);
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
            if (!File.Exists(sittingSpellXML))
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

            Spells spells = Spells.Retrieve(sittingSpellXML);

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
            if (!File.Exists(sittingSpellXML))
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

            Spells spells = Spells.Retrieve(sittingSpellXML);

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
            if (!File.Exists(sittingSummonXML))
            {
                MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Summons summons = Summons.Retrieve(sittingSummonXML);

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

            rtbSkills.Text = string.Join(",", st.Skills);
            tbFeats.Text = string.Join(",", st.Feats);
            tbSpecAtks.Text = string.Join(",", st.SpecAtks);
            tbSpecQual.Text = string.Join(",", st.SpecQual);
            rtbCombat.Text = string.Join("\r", st.Notes);
            rtbAttacks.Text = string.Join("\r", st.Attacks);
            rtbFullAttacks.Text = string.Join("\r", st.FullAttacks);
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

        private void TvTemplates_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (!File.Exists(sittingSummonXML))
                {
                    MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (tvTemplates.SelectedNode != null)
                {
                    if (tvTemplates.SelectedNode.Tag is Summon selSumm)
                    {
                        if (MessageBox.Show(string.Format("Are you sure you want to delete {0} from this list?", ((Summon)tvTemplates.SelectedNode.Tag).Name), "Delete Summon Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
                        {
                            Summons summons = Summons.Retrieve(sittingSummonXML);
                            int indexMatch = summons.FindIndex(x => x.Name.ToUpper() == selSumm.Name.ToUpper() && x.SummonSpell.ToUpper() == selSumm.SummonSpell.ToUpper());

                            if (indexMatch > -1)
                            {
                                summons.RemoveAt(indexMatch);
                            }

                            summons.Save(sittingSummonXML);
                            RefreshSummonPage();
                        }
                    }
                }
            }
        }

        private void TvSpells_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (!File.Exists(sittingSpellXML))
                {
                    MessageBox.Show("XML file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (tvSpells.SelectedNode != null)
                {
                    if (tvSpells.SelectedNode.Tag is Spell selSpell)
                    {
                        if (MessageBox.Show(string.Format("Are you sure you want to delete {0} from this list?", ((Spell)tvSpells.SelectedNode.Tag).Name), "Delete Summon Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
                        {
                            Spells spells = Spells.Retrieve(sittingSpellXML);
                            int indexMatch = spells.FindIndex(x => x.Name.ToUpper() == selSpell.Name.ToUpper() && x.Level.ToUpper() == selSpell.Level.ToUpper());

                            if (indexMatch > -1)
                            {
                                spells.RemoveAt(indexMatch);
                            }

                            spells.Save(sittingSpellXML);
                            RefreshSpellPage(GetSpellNodesExpansion());
                        }
                    }
                }
            }
        }

        private void FavoriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Spells spells = Spells.Retrieve(sittingSpellXML);
            Spell thisSpell = (Spell)((TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).SourceControl).SelectedNode.Tag;  //((TreeView)((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent().tvSpells).SelectedNode.Tag;
            int indexMatch = spells.FindIndex(x => x.Name.ToUpper() == thisSpell.Name.ToUpper() && x.Level.ToUpper() == thisSpell.Level.ToUpper());

            if (indexMatch > -1)
            {
                if (spells[indexMatch].Favorited) { spells[indexMatch].Favorited = false; }
                else if (!spells[indexMatch].Favorited) { spells[indexMatch].Favorited = true; }
                else { spells[indexMatch].Favorited = true; }

                spells.Save(sittingSpellXML);
            }

            RefreshSpellPage(GetSpellNodesExpansion());
        }

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
            if (!File.Exists(sittingSummonXML))
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

            List<Size> sizes = Enum.GetValues(typeof(Size)).Cast<Size>().ToList();
            int index = sizes.FindIndex(x => x.ToString() == cbSize.SelectedItem.ToString());
            Size size = index > -1 ? sizes[index] : DruidAssistant.Size.Medium;

            Summon addSummon = new Summon
            {
                SummonSpell = cbSummonSpell.Text,
                Name = tbName.Text,
                Abilities = new Abilities((int)nudSTR.Value, (int)nudDEX.Value, (int)nudCON.Value, (int)nudINT.Value, (int)nudWIS.Value, (int)nudCHA.Value),
                Saves = new Saves((int)nudFort.Value, (int)nudRef.Value, (int)nudWill.Value),
                Level = (int)nudLevel.Value,
                HitDie = (int)nudHD.Value,
                NaturalArmor = (int)nudNatural.Value,
                BAB = (int)nudBAB.Value,
                Grapple = (int)nudGrapple.Value,
                Size = size,
                Type = tbType.Text,
                Environment = tbEnvironment.Text,
                Space = tbSpace.Text,
                Reach = tbReach.Text,
                Movement = tbMovement.Text,
                Skills = rtbSkills.Text.Split(',').ToList(),
                Feats = tbFeats.Text.Split(',').ToList(),
                SpecAtks = tbSpecAtks.Text.Split(',').ToList(),
                SpecQual = tbSpecQual.Text.Split(',').ToList(),
                Notes = rtbCombat.Text.Split('\r').ToList(),
                Attacks = FileParser.GetAttacks(rtbAttacks.Text),
                FullAttacks = FileParser.GetFullAttacks(rtbFullAttacks.Text)
            };

            Summons summons = Summons.Retrieve(sittingSummonXML);
            int indexMatch = summons.FindIndex(x => x.Name.ToUpper() == addSummon.Name.ToUpper() && x.SummonSpell.ToUpper() == addSummon.SummonSpell.ToUpper());

            if (indexMatch > -1)
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

            summons.Save(sittingSummonXML);
            RefreshSummonPage();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(sittingSummonXML))
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


                List<Size> sizes = Enum.GetValues(typeof(Size)).Cast<Size>().ToList();
                int index = sizes.FindIndex(x => x.ToString() == cbSize.SelectedItem.ToString());
                Size size = index > -1 ? sizes[index] : DruidAssistant.Size.Medium;

                Summon updateSummon = new Summon
                {
                    SummonSpell = cbSummonSpell.Text,
                    Name = tbName.Text,
                    Abilities = new Abilities((int)nudSTR.Value, (int)nudDEX.Value, (int)nudCON.Value, (int)nudINT.Value, (int)nudWIS.Value, (int)nudCHA.Value),
                    Saves = new Saves((int)nudFort.Value, (int)nudRef.Value, (int)nudWill.Value),
                    Level = (int)nudLevel.Value,
                    HitDie = (int)nudHD.Value,
                    NaturalArmor = (int)nudNatural.Value,
                    BAB = (int)nudBAB.Value,
                    Grapple = (int)nudGrapple.Value,
                    Size = size,
                    Type = tbType.Text,
                    Environment = tbEnvironment.Text,
                    Space = tbSpace.Text,
                    Reach = tbReach.Text,
                    Movement = tbMovement.Text,
                    Skills = rtbSkills.Text.Split(',').ToList(),
                    Feats = tbFeats.Text.Split(',').ToList(),
                    SpecAtks = tbSpecAtks.Text.Split(',').ToList(),
                    SpecQual = tbSpecQual.Text.Split(',').ToList(),
                    Notes = rtbCombat.Text.Split('\r').ToList(),
                    Attacks = FileParser.GetAttacks(rtbAttacks.Text),
                    FullAttacks = FileParser.GetFullAttacks(rtbFullAttacks.Text)
                };

                Summons summons = Summons.Retrieve(sittingSummonXML);
                int indexMatch = summons.FindIndex(x => x.Index == sittingSummonIndex);
                if (indexMatch > -1)
                {
                    summons[indexMatch] = updateSummon;
                }
                else
                {
                    MessageBox.Show("Summon cannot be found.", "Summon Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                summons.Save(sittingSummonXML);
                RefreshSummonPage();
            }
        }


        private void TvSpells_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            (sender as TreeView).SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right)
            {
                if ((sender as TreeView).SelectedNode.Tag is Spell thisSpell)
                {
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
            if (!File.Exists(sittingSpellXML))
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

            Spell addSpell = new Spell
            {
                Favorited = chkFavoriteSpell.Checked,
                Name = tbSpellName.Text,
                Class = tbSpellClass.Text,
                Level = tbSpellLevel.Text,
                Domain = tbSpellDomain.Text,
                Components = tbSpellComponents.Text,
                Casting = tbSpellCastingTime.Text,
                Range = tbSpellRange.Text,
                Target = tbSpellTarget.Text,
                Effect = tbSpellEffect.Text,
                Duration = tbSpellDuration.Text,
                SavingThrow = tbSpellSaving.Text,
                SpellResistance = tbSpellResistance.Text,
                SourceBook = tbSource.Text,
                Description = rtbSpellDescription.Text,
                PersonalNotes = rtbPersonalNotes.Text
            };

            Spells spells = Spells.Retrieve(sittingSpellXML);
            int indexMatch = spells.FindIndex(x => x.Name.ToUpper() == addSpell.Name.ToUpper() && x.Level.ToUpper() == addSpell.Level.ToUpper());

            if (indexMatch > -1)
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
            spells.Save(sittingSpellXML);
            RefreshSpellPage(GetSpellNodesExpansion());
        }

        private void SaveSpellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(sittingSpellXML))
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

                Spell updateSpell = new Spell
                {
                    Index = sittingSpellID,
                    Favorited = chkFavoriteSpell.Checked,
                    Name = tbSpellName.Text,
                    Class = tbSpellClass.Text,
                    Level = tbSpellLevel.Text,
                    Domain = tbSpellDomain.Text,
                    Components = tbSpellComponents.Text,
                    Casting = tbSpellCastingTime.Text,
                    Range = tbSpellRange.Text,
                    Target = tbSpellTarget.Text,
                    Effect = tbSpellEffect.Text,
                    Duration = tbSpellDuration.Text,
                    SavingThrow = tbSpellSaving.Text,
                    SpellResistance = tbSpellResistance.Text,
                    SourceBook = tbSource.Text,
                    Description = rtbSpellDescription.Text,
                    PersonalNotes = rtbPersonalNotes.Text
                };

                Spells spells = Spells.Retrieve(sittingSpellXML);
                int indexMatch = spells.FindIndex(x => x.Index == sittingSpellID);

                if (indexMatch > -1)
                {
                    spells[indexMatch] = updateSpell;
                }
                else
                {
                    MessageBox.Show("Spell cannot be found.", "Spell Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                spells.Save(sittingSpellXML);
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
            if (sender == tbSummonsXML) { Properties.Settings.Default.SummonsXML = tbSummonsXML.Text; }
            if (sender == tbSpellsXML) { Properties.Settings.Default.SpellsXML = tbSpellsXML.Text; }

            Properties.Settings.Default.Save();
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

        private void DruidMain_ResizeBegin(object sender, EventArgs e)
        {
            this.SuspendLayout();
        }

        private void DruidMain_ResizeEnd(object sender, EventArgs e)
        {
            this.ResumeLayout(true);
        }
    }
}