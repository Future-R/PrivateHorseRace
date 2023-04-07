using System;
using System.Collections.Generic;

namespace HorseRace
{
    public class 起跑冲刺 : 异能
    {
        public override string 唯一名称 => "起跑冲刺";
        public override List<string> 标签组 => new List<string> { "系统" };
        public override double 持续时间 => -1;
        public override string 添加者 { get; set; }
        public override 马 承担者 { get; set; }

        public 数据表.属性修正 加速度提升 = new 数据表.属性修正
        {
            优先级 = 200,
            是加算 = true,
            修正值 = 24
        };

        public override void 添加时()
        {
            // 添加一个加速度 +24
            承担者.当前加速度.修正组.Add(加速度提升);
        }

        public override void 运行时()
        {
            // 根据系统规则，如果承担者的速度达到0.85 * 赛道基准速度，移除起跑冲刺
            if (承担者.速度属性.最终属性 > 0.85 * 数据表.当前比赛.赛道基准速度)
            {
                移除(原因.系统规则);
            }
        }

        public override void 移除时()
        {
            // 如果承担者身上还有这个buff的加成，那么移除那个加成
            if (承担者.当前加速度.修正组.Contains(加速度提升))
            {
                承担者.当前加速度.修正组.Remove(加速度提升);
            }
        }
    }
}
