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
using System.Runtime;


namespace DruidAssistant
{
    public partial class Summoned : Form
    {
        public bool flag1;

        public SummonToForm currentST;

        public Summoned(SummonToForm st)
        {
            currentST = st;
            InitializeComponent();
            LoadPage(currentST);
        }

        private void Summoned_Load(object sender, EventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.Text = string.Format("Druid Summoning Assistant v{0}",
                    ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4));
            }

            nudFortMisc.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudRefMisc.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudWillMisc.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);

            nudACMisc.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudTouchMisc.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudFlatMisc.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);

            nudGrappleMisc.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudMeleeMisc.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudRangeMisc.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);

            nudSTR.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudDEX.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudCON.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudINT.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudWIS.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
            nudCHA.MouseWheel += new MouseEventHandler(MouseWheelSuppresser);
        }

        private void MouseWheelSuppresser(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void NudSTR_ValueChanged(object sender, EventArgs e)
        {
            labelSTRMod.Text = ScoreToMod(nudSTR.Value).ToString();
        }

        private void NudDEX_ValueChanged(object sender, EventArgs e)
        {
            labelDEXMod.Text = ScoreToMod(nudDEX.Value).ToString();
        }

        private void NudCON_ValueChanged(object sender, EventArgs e)
        {
            labelCONMod.Text = ScoreToMod(nudCON.Value).ToString();
        }

        private void NudINT_ValueChanged(object sender, EventArgs e)
        {
            labelINTMod.Text = ScoreToMod(nudINT.Value).ToString();
        }

        private void NudWIS_ValueChanged(object sender, EventArgs e)
        {
            labelWISMod.Text = ScoreToMod(nudWIS.Value).ToString();
        }

        private void NudCHA_ValueChanged(object sender, EventArgs e)
        {
            labelCHAMod.Text = ScoreToMod(nudCHA.Value).ToString();
        }

        private void LoadPage(SummonToForm st)
        {
            flag1 = true;
            rtbNotes.Clear();
            lbAttacks.Items.Clear();
            lbFeats.Items.Clear();
            lbAttacks.Items.Clear();
            lbFullAttacks.Items.Clear();
            lbSkills.Items.Clear();
            lbSpecialAttacks.Items.Clear();
            lbSpecialQualities.Items.Clear();


            for (int i = 0; i < st.Skills.Count; i++)
            {
                lbSkills.Items.Add(Summon.CorrectSkillModForward(st.Skills[i], st.Abilities));
            }

            for (int i = 0; i < st.Feats.Count; i++)
            {
                lbFeats.Items.Add(st.Feats[i]);
            }

            for (int i = 0; i < st.SpecAtks.Count; i++)
            {
                lbSpecialAttacks.Items.Add(st.SpecAtks[i]);
            }

            for (int i = 0; i < st.SpecQual.Count; i++)
            {
                lbSpecialQualities.Items.Add(st.SpecQual[i]);
            }

            for (int i = 0; i < st.Attacks.Count; i++)
            {
                lbAttacks.Items.Add(st.Attacks[i]);
            }

            for (int i = 0; i < st.FullAttacks.Count; i++)
            {
                lbFullAttacks.Items.Add(st.FullAttacks[i].ToString());
            }

            rtbNotes.AppendText(string.Join("\r", st.Notes));

            tbSummonName.Text = st.Name;
            nudRounds.Value = st.Rounds;

            nudSTR.Value = st.Abilities.Str;
            nudDEX.Value = st.Abilities.Dex;
            nudCON.Value = st.Abilities.Con;
            nudINT.Value = st.Abilities.Int;
            nudWIS.Value = st.Abilities.Wis;
            nudCHA.Value = st.Abilities.Cha;

            cbSize.Text = st.Size.ToString();
            tbType.Text = st.Type;
            nudInitiative.Value = st.Abilities.DexMod;
            tbSpeed.Text = st.Movement;

            labelACDex.Text = st.Abilities.DexMod.ToString();
            labelACNatural.Text = st.NaturalArmor.ToString();
            labelACSize.Text = Summon.GetSizeMod(st.Size).ToString();

            labelTouchDex.Text = st.Abilities.DexMod.ToString();
            labelTouchSize.Text = Summon.GetSizeMod(st.Size).ToString();

            labelFlatNatural.Text = st.NaturalArmor.ToString();
            labelFlatSize.Text = Summon.GetSizeMod(st.Size).ToString();

            tbSpaceReach.Text = string.Format("{0}, {1}", st.Space, st.Reach);

            labelFortBase.Text = st.Saves.Fort.ToString();
            labelFortAbility.Text = st.Abilities.ConMod.ToString();
            nudFortMisc.Value = st.MiscFort;

            labelRefBase.Text = st.Saves.Ref.ToString();
            labelRefAbility.Text = st.Abilities.DexMod.ToString();
            nudRefMisc.Value = st.MiscRef;

            labelWillBase.Text = st.Saves.Will.ToString();
            labelWillAbility.Text = st.Abilities.WisMod.ToString();
            nudWillMisc.Value = st.MiscWill;


            tbEnvironment.Text = st.Environment;
            tbHitDie.Text = string.Format("{0}d{1}", st.Level, st.HitDie);

            int toughnessAddition = GetToughnessHP(st);

            int prevMaxHP = int.Parse(nudMaxHP.Value.ToString());
            int newMaxHP = int.Parse(Math.Floor(st.Level * 1.000 * Math.Ceiling(st.HitDie * 1.000 / 2) + st.Level * .500 + st.Abilities.ConMod * 1.000 * st.Level + toughnessAddition).ToString());

            nudMaxHP.Value = decimal.Parse(newMaxHP.ToString());
            //new summon
            if (prevMaxHP == 0)
            {
                nudCurrentHP.Value = newMaxHP;
            }
            //nothing changed; regular update
            else if (prevMaxHP == newMaxHP)
            {

            }
            //CON changed
            else
            {
                int difference = prevMaxHP - currentST.CurrentHP;
                nudCurrentHP.Value = newMaxHP - difference;
            }

            labelBaseAttackBonus.Text = st.BAB.ToString();

            //nudGrappleBonus.Value =
            labelGrappleBase.Text = st.BAB.ToString();
            labelGrappleAbility.Text = st.Abilities.StrMod.ToString();
            labelGrappleSize.Text = Summon.GetGrappleSizeMod(st.Size).ToString();
            nudGrappleMisc.Value = st.MiscGrapple;

            //nudMeleeBonus.Value =
            labelMeleeBase.Text = labelBaseAttackBonus.Text;
            labelMeleeAbility.Text = (HasWeaponFinesse(st) ? Math.Max(st.Abilities.StrMod, st.Abilities.DexMod) : st.Abilities.StrMod).ToString();
            labelMeleeSize.Text = Summon.GetSizeMod(st.Size).ToString();
            nudMeleeMisc.Value = st.MiscMelee;

            //nudRangeBonus.Value = 
            labelRangeBase.Text = labelBaseAttackBonus.Text;
            labelRangeAbility.Text = st.Abilities.DexMod.ToString();
            labelRangeSize.Text = Summon.GetSizeMod(st.Size).ToString();
            nudRangeMisc.Value = st.MiscRange;
            flag1 = false;
        }

        private bool HasWeaponFinesse(SummonToForm st)
        {
            for (int i = 0; i < st.Feats.Count; i++)
            {
                if (st.Feats[i].Contains("WeaponFinesse"))
                {
                    return true;
                }
            }
            return false;
        }

        private int GetToughnessHP(SummonToForm st)
        {
            int toughnessHP = 0;
            for (int i = 0; i < st.Feats.Count; i++)
            {
                if (st.Feats[i].Contains("Toughness"))
                {
                    string toughness = Regex.Match(st.Feats[i], @"\(([^)]+)\)").Value;
                    if (toughness == "")
                    {
                        toughnessHP = 3;
                    }
                    else
                    {
                        toughnessHP = 3 * Convert.ToInt32(toughness.Replace("(", "").Replace(")", ""));
                    }
                }
            }

            return toughnessHP;
        }

        private void NudArmors_ValueChanged(object sender, EventArgs e)
        {
            RecalcArmors();
        }

        private void RecalcArmors()
        {
            labelAC.Text = (10 + Convert.ToInt32(labelACDex.Text) + Convert.ToInt32(labelACNatural.Text) + Convert.ToInt32(labelACSize.Text) + nudACMisc.Value).ToString();
            labelTouch.Text = (10 + Convert.ToInt32(labelTouchDex.Text) + Convert.ToInt32(labelTouchSize.Text) + nudTouchMisc.Value).ToString();
            labelFlat.Text = (10 + Convert.ToInt32(labelFlatNatural.Text) + Convert.ToInt32(labelFlatSize.Text) + nudFlatMisc.Value).ToString();
        }

        private static decimal ScoreToMod(decimal input)
        {
            return Math.Floor((input - 10) / 2);
        }

        private void StatsChanged(SummonToForm st)
        {
            st.Abilities.Str = Convert.ToInt32(nudSTR.Value);
            st.Abilities.Dex = Convert.ToInt32(nudDEX.Value);
            st.Abilities.Con = Convert.ToInt32(nudCON.Value);
            st.Abilities.Int = Convert.ToInt32(nudINT.Value);
            st.Abilities.Wis = Convert.ToInt32(nudWIS.Value);
            st.Abilities.Cha = Convert.ToInt32(nudCHA.Value);

            st.Size = Enum.GetValues(typeof(Size)).Cast<Size>().ToList().Find(x=>x.ToString() == cbSize.SelectedItem.ToString());

            for (int i = 0; i < st.Attacks.Count; i++)
            {
                Attack thisAttack = st.Attacks[i];
                if (thisAttack.Name == "GB Slam")
                {
                    thisAttack.Extra = Augments.Feats.GreenboundSummoning.SlamDamage(st.Size);
                }
            }

            LoadPage(st);
        }

        private void ModsTextChanged(object sender, EventArgs e)
        {
            if (currentST != null && !flag1)
            {
                StatsChanged(currentST);
            }
        }

        private void NudCurrentHP_ValueChanged(object sender, EventArgs e)
        {
            currentST.CurrentHP = Convert.ToInt32(nudCurrentHP.Value);
        }

        private void LbSkills_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                string tooltip = "";
                if (lbSkills.SelectedItem == null)
                {
                    return;
                }
                SkillMod sm = (lbSkills.SelectedItem as SkillMod);
                tooltip = string.Format("{0} ({1}) ", sm.Skill, sm.DefMod);
                tooltip += sm.AlternateAbility.STR.HasValue ? sm.AlternateAbility.STR.ToString() + " " : "";
                tooltip += sm.AlternateAbility.DEX.HasValue ? sm.AlternateAbility.DEX.ToString() + " " : "";
                tooltip += sm.AlternateAbility.CON.HasValue ? sm.AlternateAbility.CON.ToString() + " " : "";
                tooltip += sm.AlternateAbility.INT.HasValue ? sm.AlternateAbility.INT.ToString() + " " : "";
                tooltip += sm.AlternateAbility.WIS.HasValue ? sm.AlternateAbility.WIS.ToString() + " " : "";
                tooltip += sm.AlternateAbility.CON.HasValue ? sm.AlternateAbility.CON.ToString() + " " : "";
                ttSkills.SetToolTip(lbSkills, tooltip);
            }
            if (e.Button == MouseButtons.Right)
            {
                string tooltip = "";
                for (int i = 0; i < lbSkills.Items.Count; i++)
                {
                    SkillMod sm = (lbSkills.Items[i] as SkillMod);
                    tooltip += string.Format("{0} ({1}) ", sm.Skill, sm.DefMod);
                    tooltip += sm.AlternateAbility.STR.HasValue ? sm.AlternateAbility.STR.ToString() + " " : "";
                    tooltip += sm.AlternateAbility.DEX.HasValue ? sm.AlternateAbility.DEX.ToString() + " " : "";
                    tooltip += sm.AlternateAbility.CON.HasValue ? sm.AlternateAbility.CON.ToString() + " " : "";
                    tooltip += sm.AlternateAbility.INT.HasValue ? sm.AlternateAbility.INT.ToString() + " " : "";
                    tooltip += sm.AlternateAbility.WIS.HasValue ? sm.AlternateAbility.WIS.ToString() + " " : "";
                    tooltip += sm.AlternateAbility.CON.HasValue ? sm.AlternateAbility.CON.ToString() + " " : "";
                    tooltip += "\r";
                }
                ttSkills.SetToolTip(lbSkills, tooltip);
            }
        }

        private void RecalcGrapple()
        {
            labelGrappleBonus.Text = (Convert.ToInt32(labelGrappleBase.Text) + Convert.ToInt32(labelGrappleAbility.Text) + Convert.ToInt32(labelGrappleSize.Text) + nudGrappleMisc.Value).ToString();
        }

        private void RecalcMelee()
        {
            labelMeleeBonus.Text = (Convert.ToInt32(labelMeleeBase.Text) + Convert.ToInt32(labelMeleeAbility.Text) + Convert.ToInt32(labelMeleeSize.Text) + nudMeleeMisc.Value).ToString();
        }

        private void RecalcRange()
        {
            labelRangeBonus.Text = (Convert.ToInt32(labelRangeBase.Text) + Convert.ToInt32(labelRangeAbility.Text) + Convert.ToInt32(labelRangeSize.Text) + nudRangeMisc.Value).ToString();
        }

        private void GrappleValuesChanged(object sender, EventArgs e)
        {
            RecalcGrapple();
        }

        private void MeleeValuesChanged(object sender, EventArgs e)
        {
            RecalcMelee();
        }

        private void RangedValuesChanged(object sender, EventArgs e)
        {
            RecalcRange();
        }

        private void RecalcFort()
        {
            labelFortSave.Text = (Convert.ToInt32(labelFortBase.Text) + Convert.ToInt32(labelFortAbility.Text) + nudFortMisc.Value).ToString();
        }

        private void RecalcRef()
        {
            labelRefSave.Text = (Convert.ToInt32(labelRefBase.Text) + Convert.ToInt32(labelRefAbility.Text) + nudRefMisc.Value).ToString();
        }

        private void RecalcWill()
        {
            labelWillSave.Text = (Convert.ToInt32(labelWillBase.Text) + Convert.ToInt32(labelWillAbility.Text) + nudWillMisc.Value).ToString();
        }

        private void Fort_ValuesChanged(object sender, EventArgs e)
        {
            RecalcFort();
        }

        private void RefValuesChanged(object sender, EventArgs e)
        {
            RecalcRef();
        }

        private void WillValuesChanged(object sender, EventArgs e)
        {
            RecalcWill();
        }

        private void CbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentST != null && !flag1)
            {
                StatsChanged(currentST);
            }
        }

        private void LBAttacks_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ttSkills.SetToolTip(sender as ListBox, (sender as ListBox).SelectedItem.ToString());

            }
        }
    }
}