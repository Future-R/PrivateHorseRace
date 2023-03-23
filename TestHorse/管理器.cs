using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    internal class 管理器
    {
        // 比赛的初始化
        public void 比赛初始化()
        {
            // 计算初始属性
            // 根据智力检定技能是否可以发动
            // 计算调整后属性
            // 计算出闸延迟
            // 计算是否在2~9段焦躁
            // 根据耐力算出体力
            // 设置初始速度
        }

        // 每0.05秒调用，相当于每秒20逻辑帧
        public void 逻辑帧()
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
