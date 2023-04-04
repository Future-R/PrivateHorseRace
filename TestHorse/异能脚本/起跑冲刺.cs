using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    public class 起跑冲刺 : 异能
    {
        new string 唯一名称 = "起跑冲刺";
        new string 标签 = "系统";
        new int 优先级 = 100;
        new float 持续时间 = -1;
        new string 添加者 = "系统";
        new 马 承担者;

        public 起跑冲刺(马 对象)
        {
            承担者 = 对象;
            添加时();
        }

        void 添加时()
        {
            承担者.状态.Add(this);
            // 添加一个加速度 +24
            承担者.当前加速度.修正组.Add(new 数据表.属性修正
            {
                标签组 = new[] { 唯一名称 }.ToList(),
                优先级 = 优先级,
                是加算 = true,
                修正值 = 24,
                剩余持续时间 = 持续时间
            });
        }

        void 运行时()
        {
            if (承担者.速度.最终属性 > 0.85 * 数据表.当前比赛.赛道基准速度)
            {
                移除时();
            }
        }

        void 移除时()
        {
            // 移除一个加速度
            for (int i = 0; i < 承担者.当前加速度.修正组.Count; i++)
            {
                if (承担者.当前加速度.修正组[i].标签组.Contains(唯一名称))
                {
                    承担者.当前加速度.修正组.RemoveAt(i);
                    break;
                }
            }
            承担者.状态.Remove(this);
        }
    }
}
