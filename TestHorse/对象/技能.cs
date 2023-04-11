using System.Collections.Generic;

namespace HorseRace
{
    public class 技能 : 异能
    {
        public override string 唯一名称 { get; set; }
        public override List<string> 标签组 { get; set; }
        public override double 持续时间 => -1;
        public override string 添加者 { get; set; }
        public override 马 承担者 { get; set; }


        public string 技能描述 { get; set; }
        public bool 需要智力检定 { get; set; }
        public 技能稀有度 稀有度 { get; set; }

        public string 条件限制 { get; set; }
        public string 触发条件 { get; set; }

        public List<技能类型> 类型 { get; set; }

        public double 技能持续事件 { get; set; }
        public double 技能冷却时间 { get; set; }

        public enum 技能稀有度
        {
            未知,
            普通,
            传说,
            独特,
            进化
        }

        public enum 技能条件限制
        {
            通用,
            短距离,
            英里,
            中距离,
            长距离,
            泥地,
            领头,
            前列,
            居中,
            后追
        }

        public enum 技能触发条件
        {
            直线,
            弯道,
            序盘,
            中盘,
            终盘,
            冲刺阶段,
            最终直线,
            最终弯道,
            最后冲刺
        }

        public enum 技能类型
        {
            永续,
            瞬间,
            反击,
            妨害,
            出闸,
            视野,
            耐力恢复,
            速度,
            加速度,
        }

        public override void 添加时()
        {
            base.添加时();
        }

        public override void 运行时()
        {
            base.运行时();
        }

        public override void 移除时()
        {
            base.移除时();
        }
    }
}
