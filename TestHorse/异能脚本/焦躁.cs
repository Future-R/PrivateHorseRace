﻿using System;
using System.Collections.Generic;

namespace HorseRace
{
    /*如果马娘将会焦躁，那么她会随机挑选2-9赛段的其中1段，在进入该赛段所在位置时立即焦躁。
     * 焦躁过程中，体力消耗速度会增加至1.6倍，并且会影响其位置意识模式使其爆冲。
     * 处于焦躁状态时会自动在所有位置意识的智力检查中成功。需要注意的是这只影响位置意识的AI，不会影响基于跑法的基础目标速度。
     * 逃马会立即进入提速模式。
     * 先行跑法会暂时按照逃跑法的位置意识模式行动。
     * 差跑法有75% 概率按照逃，25% 概率按照先行的位置意识模式行动。
     * 追跑法有70% 概率按照逃，20% 概率按照先行，10%按照差马的位置意识模式行动。
     * 在这个状态下每3秒都有55%的概率退出焦躁状态，或者焦躁已经持续了12秒也会解除。
     * 技能的焦躁debuff发动时会使得焦躁时间立即变成5秒，并在5秒后取消焦躁状态。这个效果重复发动时会重新计时。*/
    public class 焦躁 : 异能
    {
        public override string 唯一名称 => "焦躁";
        public override List<string> 标签组 => new List<string> { "系统" };
        public override double 持续时间 => -1;
        public override string 添加者 { get; set; }
        public override 马 承担者 { get; set; }

        private int 焦躁赛段;
        // 如果剩余时间>0，说明处于焦躁中，所有位置模式智力检定自动成功
        private double 剩余时间 = -1;

        private 数据表.属性修正 体力消耗修正 = new 数据表.属性修正 { 修正值 = 1.6, 是加算 = false, 优先级 = 100 };

        public override void 添加时()
        {
            // 随机2~9
            焦躁赛段 = 工具.随机.Next(8) + 2;
            事件管理器.实例.订阅(事件.游戏事件.进入新赛段, 焦躁发作检定);
        }

        public override void 移除时()
        {
            事件管理器.实例.退订(事件.游戏事件.进入新赛段, 焦躁发作检定);
            // 恢复跑法意识
            承担者.跑法意识 = 承担者.跑法;
            // 移除体力消耗
            承担者.体力消耗系数.修正组.Remove(体力消耗修正);
        }

        public override void 运行时()
        {
            if (剩余时间 >= 0)
            {
                // 原本的焦躁公式有点奇怪，我重新设计了一套
                // 每一帧都有 (COS(PI * 剩余时间 / 12) + 1) / 2 的概率退出焦躁
                // 表现和原版类似，前几秒退出的概率非常低，越后面越高，第12秒必定退出
                double 退出判定 = (Math.Cos(Math.PI * 剩余时间 / 12) + 1) / 2;
                if (工具.随机.NextDouble() <= 退出判定)
                {
                    移除(原因.系统规则);
                }

                剩余时间 -= 数据表.一帧时间;
                // 防止计算误差使得剩余时间变为负数
                if (剩余时间 < 0)
                {
                    剩余时间 = 0;
                }
            }
        }

        private void 焦躁发作检定(object sender, EventArgs e)
        {
            马 当前马 = sender as 马;
            if (当前马 == 承担者 && 当前马.赛段 == 焦躁赛段)
            {
                焦躁发作();
            }
        }

        public void 焦躁发作()
        {
            this.触发事件(事件.游戏事件.异能被发动, new 事件.异能被发动事件 { 发动原因 = 原因.系统规则, 异能 = this });
            // 给予12秒焦躁，每帧有概率提前结束
            剩余时间 = 12;
            // 体力消耗1.6倍
            承担者.体力消耗系数.修正组.Add(体力消耗修正);
            // 逃马清空位置模式CD，其它马改变跑法。这么看来只要耐足，逃马暴冲是正面效果啊
            double 随机结果 = 工具.随机.NextDouble();
            if (承担者.跑法 == 0 || 承担者.跑法 == 4)
            {
                // 清空位置意识CD，使得逃马接下来必定进入提速或超越模式
                位置意识.清空冷却(承担者);
            }
            else if (承担者.跑法 == 1)
            {
                // 先马按逃马意识跑
                承担者.跑法意识 = 0;
            }
            else if (承担者.跑法 == 2)
            {
                // 差马按逃马或先马意识跑
                if (随机结果 > 0.25)
                {
                    承担者.跑法意识 = 0;
                }
                else
                {
                    承担者.跑法意识 = 1;
                }
            }
            else
            {
                // 追马
                if (随机结果 > 0.3)
                {
                    承担者.跑法意识 = 0;
                }
                else if (随机结果 > 0.1)
                {
                    承担者.跑法意识 = 1;
                }
                else
                {
                    承担者.跑法意识 = 2;
                }
            }
        }
    }
}
