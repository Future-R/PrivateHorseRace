using System.Collections.Generic;
using System.Linq;

namespace HorseRace
{
    public class EX提速 : 异能
    {
        public override string 唯一名称 => "EX提速";
        public override List<string> 标签组 => new List<string> { "位置意识" };
        public override double 持续时间 => -1;
        public override string 添加者 { get; set; }
        public override 马 承担者 { get; set; }

        private double 已持续距离;

        private 数据表.属性修正 目标速度修正 = new 数据表.属性修正
        {
            优先级 = 100,
            是加算 = false,
            修正值 = 2
        };

        public override void 添加时()
        {
            承担者.目标速度.修正组.Add(目标速度修正);
            已持续距离 = 0;
        }

        public override void 运行时()
        {
            var 前方马群 = 检定.获取前方马(承担者);
            // 前方不再有跑法位置本应在自身之后的马
            if (!前方马群.Where(x => x.跑法 > 承担者.跑法意识).Any())
            {
                移除(原因.系统规则);
            }
            double 赛段修正 = 1;
            if (承担者.跑法意识 == 马.跑法位置.爆领)
            {
                赛段修正 = 3;
            }
            已持续距离 += 承担者.当前速度.最终属性 * 数据表.一帧时间;
            if (已持续距离 > 位置意识.一个赛段的距离 * 赛段修正)
            {
                移除(原因.距离结束);
                return;
            }
        }

        public override void 移除时()
        {
            承担者.目标速度.修正组.Remove(目标速度修正);
        }
    }
}
