using System.Collections.Generic;
using System.Linq;

namespace HorseRace
{
    public class 逃马超越 : 异能
    {
        public override string 唯一名称 => "逃马超越";
        public override List<string> 标签组 => new List<string> { "位置意识" };
        public override double 持续时间 => -1;
        public override string 添加者 { get; set; }
        public override 马 承担者 { get; set; }

        private 数据表.属性修正 目标速度修正 = new 数据表.属性修正
        {
            优先级 = 100,
            是加算 = false,
            修正值 = 1.05
        };

        public static double 退出距离;
        private static double 已持续距离 = 0;

        public override void 添加时()
        {
            // 目标速度 * 1.05
            承担者.目标速度.修正组.Add(目标速度修正);
            if (承担者.跑法意识 == 4)
            {
                退出距离 = 位置意识.大逃超越距离;
            }
            else
            {
                退出距离 = 位置意识.逃马超越距离;
            }
        }

        public override void 运行时()
        {
            //已经持续了一整个赛段（对于大逃跑法来说可以持续3个赛段）的长度后，马娘会回到通常模式。
            double 赛段修正 = 1;
            if (承担者.跑法意识 == 4)
            {
                赛段修正 = 3;
            }
            已持续距离 += 承担者.速度属性.最终属性 * 数据表.一帧时间;
            if (已持续距离 > 位置意识.一个赛段的距离 * 赛段修正)
            {
                移除(原因.距离结束);
                return;
            }
            // 相较同一跑法中第二名次的马，被拉开了10m (大逃为27.5)的距离时会提前结束。
            马 目标 = 数据表.当前比赛.参赛马.Where(x => x.跑法意识 == 承担者.跑法意识).Where(x => x != 承担者).FirstOrDefault();
            if (目标 == null)
            {
                // 没有其他马，提前退出
                移除(原因.目标丢失);
                return;
            }
            // 与目标拉开一段距离后退出
            if (承担者.已行进距离 - 目标.已行进距离 > 退出距离)
            {
                移除(原因.系统规则);
                return;
            }
        }

        public override void 移除时()
        {
            承担者.目标速度.修正组.Remove(目标速度修正);
        }
    }
}
