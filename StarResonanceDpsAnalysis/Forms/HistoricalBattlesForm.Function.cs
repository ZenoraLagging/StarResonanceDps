using StarResonanceDpsAnalysis.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarResonanceDpsAnalysis.Forms
{
    public partial class HistoricalBattlesForm
    {
        public void ToggleTableView()
        {

            table_DpsDetailDataTable.Columns.Clear();

/*            table_DpsDetailDataTable.Columns = new AntdUI.ColumnCollection
            {
              new AntdUI.Column("Uid", "UID"),
                new AntdUI.Column("NickName", "昵称"),
                new AntdUI.Column("Profession", "职业"),
                new AntdUI.Column("CombatPower", "战力"),
                new AntdUI.Column("TotalDamage", "总伤害"),
                new AntdUI.Column("TotalDps", "秒伤"),
                new AntdUI.Column("CritRate", "暴击率"),
                new AntdUI.Column("LuckyRate", "幸运率"),
                new AntdUI.Column("CriticalDamage", "暴击伤害"),
                new AntdUI.Column("LuckyDamage", "幸运伤害"),
                new AntdUI.Column("CritLuckyDamage", "暴击且幸运的伤害"),
                new AntdUI.Column("MaxInstantDps", "最大瞬时Dps"),

                new AntdUI.Column("TotalHealingDone", "总治疗量"),
                new AntdUI.Column("TotalHps", "HPS/秒"),
                new AntdUI.Column("CriticalHealingDone", "暴击治疗量"),
                new AntdUI.Column("LuckyHealingDone", "幸运治疗量"),
                new AntdUI.Column("CritLuckyHealingDone", "暴击且幸运的治疗量"),
                new AntdUI.Column("MaxInstantHps", "最大瞬时 HPS"),
                new AntdUI.Column("DamageTaken", "承受伤害总量"),
               // new AntdUI.Column("Share","占比"),
                new AntdUI.Column("DmgShare","团队伤害占比(%)"),
            };*/


            table_DpsDetailDataTable.Columns = new AntdUI.ColumnCollection
            {
              new AntdUI.Column("Uid", "UID"),
                new AntdUI.Column("NickName", Properties.Strings.History_Nickname_Column),
                new AntdUI.Column("Profession", Properties.Strings.History_Profession_Column),
                new AntdUI.Column("CombatPower", Properties.Strings.History_CombatPower_Column),
                new AntdUI.Column("TotalDamage", Properties.Strings.SkillDetail_Label1_TotalDamage),
                new AntdUI.Column("TotalDps", Properties.Strings.SkillDetail_TotalDps),
                new AntdUI.Column("CritRate", Properties.Strings.SkillDetail_CritRate),
                new AntdUI.Column("LuckyRate", Properties.Strings.SkillDetail_Label5_Healing_Lucky),
                new AntdUI.Column("CriticalDamage", Properties.Strings.SkillDetail_Label7_CritDamage),
                new AntdUI.Column("LuckyDamage", Properties.Strings.SkillDetail_Label8_LuckyDamage),
                new AntdUI.Column("CritLuckyDamage", Properties.Strings.History_CritLuckyDmg_Column),
                new AntdUI.Column("MaxInstantDps", Properties.Strings.History_MaxInstantDPS_Column),

                new AntdUI.Column("TotalHealingDone", Properties.Strings.SkillDetail_Label1_TotalHealing),
                new AntdUI.Column("TotalHps", Properties.Strings.SkillDetail_Label2_HPS),
                new AntdUI.Column("CriticalHealingDone", Properties.Strings.SkillDetail_Label4_Healing_Crit),
                new AntdUI.Column("LuckyHealingDone", Properties.Strings.History_LuckyHealing_Column),
                new AntdUI.Column("CritLuckyHealingDone", Properties.Strings.History_CritLuckHeal_Column),
                new AntdUI.Column("MaxInstantHps", Properties.Strings.History_MaxInstantHPS_Column),
                new AntdUI.Column("DamageTaken", Properties.Strings.History_DamageTaken_Column),
               // new AntdUI.Column("Share","占比"),
                new AntdUI.Column("DmgShare",Properties.Strings.History_DmgShare_Column),
            };

            table_DpsDetailDataTable.Binding(DpsTableDatas.DpsTable);


        }
    }


}
