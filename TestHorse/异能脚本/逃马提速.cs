using System.Collections.Generic;
using System.Linq;

namespace HorseRace
{
    public class 逃马提速 : 异能
    {
        public override string 唯一名称 => "逃马提速";
        public override List<string> 标签组 => new List<string> { "位置意识" };
        public override double 持续时间 => -1;
        public override string 添加者 { get; set; }
        public override 马 承担者 { get; set; }

        private 数据表.属性修正 目标速度修正 = new 数据表.属性修正
        {
            优先级 = 100,
            是加算 = false,
            修正值 = 1.4
        };

        public static double 退出距离 = 4.5;
        private static double 已持续距离;

        public override void 添加时()
        {
            已持续距离 = 0;
            // 目标速度 * 1.04
            //工具.打印($"{承担者.名称}目标速度由{承担者.目标速度.最终属性}提升为{承担者.目标速度.最终属性 * 目标速度修正.修正值}。当前速度{承担者.当前速度.最终属性}，当前加速度{承担者.当前加速度.最终属性}");
            承担者.目标速度.修正组.Add(目标速度修正);
            if (承担者.跑法意识 == 4)
            {
                退出距离 = 位置意识.大逃提速距离;
            }
            else if (数据表.当前比赛.参赛马.Where(x => x.跑法意识 == 承担者.跑法意识).Count() == 1)
            {
                退出距离 = 位置意识.单逃提速距离;
            }
            else
            {
                退出距离 = 位置意识.逃马提速距离;
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
            已持续距离 += 承担者.当前速度.最终属性 * 数据表.一帧时间;
            if (已持续距离 > 位置意识.一个赛段的距离 * 赛段修正)
            {
                //工具.打印($"{承担者.名称}经过了{已持续距离}m，当前目标速度{承担者.目标速度.最终属性}，当前速度{承担者.当前速度.最终属性}");
                移除(原因.距离结束);
                return;
            }
            // 目标是除了自己以外的最领先的马
            马 目标 = 数据表.当前比赛.参赛马.Where(马 => 马 != 承担者).FirstOrDefault();
            if (目标 == null)
            {
                // 没有其他马，提前退出
                移除(原因.目标丢失);
                return;
            }
            // 与目标拉开一段距离后退出
            if (承担者.已行进距离 - 目标.已行进距离 > 退出距离)
            {
                //工具.打印($"{承担者.名称}与后方{目标.名称}拉开距离{承担者.已行进距离 - 目标.已行进距离}m，当前目标速度{承担者.目标速度.最终属性}，当前速度{承担者.当前速度.最终属性}");
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
