using System.Collections.Generic;

namespace HorseRace
{
    // 当起跑延迟小于1帧时，起跑的加速和体力消耗并不会有任何变化，但在第一帧移动的距离会是
    // 加速后速度∗(1帧的时间−出闸延迟时间).
    //当起跑延迟大于1帧时，在第一帧不会产生任何加速，不会消耗体力，也不会移动距离。
    public class 出闸 : 异能
    {
        public override string 唯一名称 => "出闸";
        public override List<string> 标签组 => new List<string> { "系统" };
        public override double 持续时间 => -1;
        public override string 添加者 { get; set; }
        public override 马 承担者 { get; set; }

        public double 剩余延迟;

        public 出闸(double 延迟)
        {
            剩余延迟 = 延迟;
        }

        private 数据表.属性修正 速度修正 = new 数据表.属性修正 { 修正值 = 0, 是加算 = false, 优先级 = 300 };
        private 数据表.属性修正 加速度修正 = new 数据表.属性修正 { 修正值 = 0, 是加算 = false, 优先级 = 300 };

        public override void 添加时()
        {
            // 速度和加速度修正为0
            承担者.当前速度.修正组.Add(速度修正);
            承担者.当前加速度.修正组.Add(加速度修正);
        }

        public override void 运行时()
        {
            if (剩余延迟 > 数据表.一帧时间)
            {
                剩余延迟 -= 数据表.一帧时间;
            }
            else
            {
                承担者.当前速度.修正组.Add(new 数据表.属性修正
                {
                    修正值 = 数据表.一帧时间 - 剩余延迟,
                    是加算 = false,
                    优先级 = 200,
                    剩余持续时间 = 数据表.一帧时间,
                    标签组 = new List<string> { "系统" }
                });
                承担者.触发事件(事件.游戏事件.起跑);
                移除(原因.系统规则);
            }
        }

        public override void 移除时()
        {
            if (承担者.当前速度.修正组.Contains(速度修正))
            {
                承担者.当前速度.修正组.Remove(速度修正);
            }
            if (承担者.当前加速度.修正组.Contains(加速度修正))
            {
                承担者.当前加速度.修正组.Remove(加速度修正);
            }
            // 真正出闸
            // 设置初始速度
            承担者.当前速度.基础属性 = 3;
            // 起跑冲刺
            异能管理器.添加异能(new 起跑冲刺(), 承担者);
        }
    }
}
