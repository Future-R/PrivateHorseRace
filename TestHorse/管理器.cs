﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HorseRace.数据表;

namespace HorseRace
{
    internal class 管理器
    {
        // 比赛的初始化
        public static void 比赛初始化()
        {
            foreach (马 当前马 in 当前比赛.参赛马)
            {
                // 决定跑法，目前先随机
                // 随机算法为，往权重池中添加适性等级-1数量的跑法，然后随机抽
                List<int> 跑法权重池 = new List<int>();
                for (int i = 1; i < 当前马.领头适性; i++)
                {
                    跑法权重池.Add(0);
                }
                for (int i = 1; i < 当前马.前列适性; i++)
                {
                    跑法权重池.Add(1);
                }
                for (int i = 1; i < 当前马.居中适性; i++)
                {
                    跑法权重池.Add(2);
                }
                for (int i = 1; i < 当前马.后追适性; i++)
                {
                    跑法权重池.Add(3);
                }
                当前马.跑法 = 工具.随机取一(跑法权重池);

                // 计算初始属性
                // 调整后速度值 = 基础速度 * 赛道加成 + 场地状况调整 + 育成模式加值
                // 调整后耐力值 = 基础耐力 * 育成模式加值
                // 调整后力量值 = 基础力量 + 场地状况调整 + 育成模式加值
                // 调整后根性值 = 基础根性 + 育成模式加值
                // 调整后智力值 = 基础智力 * 跑法系数 + 育成模式加值

                int 场地速度调整值 = 场地状况调整配置表[当前比赛.场地状况].泥地速度调整;
                int 场地力量调整值 = 场地状况调整配置表[当前比赛.场地状况].泥地力量调整;
                if (当前比赛.是草地)
                {
                    场地速度调整值 = 场地状况调整配置表[当前比赛.场地状况].草地速度调整;
                    场地力量调整值 = 场地状况调整配置表[当前比赛.场地状况].草地力量调整;
                }
                //float 跑法系数 = 1;
                //switch (当前马.跑法)
                //{
                //    default:
                //        break;
                //}

                当前马.修正速度 = 当前马.基础速度 + 场地速度调整值;
                当前马.修正力量 = 当前马.基础力量 + 场地力量调整值;
                //当前马.修正智力 先不做
                
            }


            // 根据智力检定技能是否可以发动
            // 计算调整后属性
            // 计算出闸延迟
            // 计算是否在2~9段焦躁
            // 根据耐力算出体力
            // 设置初始速度
        }

        // 每0.05秒调用，相当于每秒20逻辑帧
        public static void 逻辑帧()
        {
            // 检查弯道是否结束
            // 检查技能发动
            // 如果在冲刺阶段，更新冲刺状态
            // 更新目标速度
            // 计算加速度
            // 更新比赛阶段
            // 计算行进距离
            // 检查坡道
            // 检查弯道是否开始
        }
    }
}
