using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using static HorseRace.数据表;

namespace HorseRace
{
    public class 马 : IComparable
    {
        public int Id { get; set; }
        public string 名称 { get; set; }
        public int 基础速度 { get; set; }
        public int 基础耐力 { get; set; }
        public int 基础力量 { get; set; }
        public int 基础意志 { get; set; }
        public int 基础智力 { get; set; }

        public int 草地适性 = 0;
        public int 泥地适性 = 0;

        // 0短1英2中3长
        public List<int> 距离适性;

        // 0逃1先2中3差4大逃
        public List<int> 跑法适性;

        public int 干劲系数 = 0;

        // 跑法 0逃1先2中3追4大逃
        public int 跑法 = -1;
        public int 干劲 = -1;

        public 属性 速度 = new 属性();
        public 属性 耐力 = new 属性();
        public 属性 力量 = new 属性();
        public 属性 意志 = new 属性();
        public 属性 智力 = new 属性();

        public float 已行进距离 { get; set; }
        // 向上取整，从1开始计数
        public int 赛段 { get { return (int)Math.Ceiling(已行进距离 / 当前比赛.总长度 * 24); } }
        // 0序盘1-4 1中盘5-16 2终盘17-20 3冲刺21-24
        public int 当前阶段
        {
            get
            {
                int 编号 = 赛段;
                if (编号 >= 1 && 编号 <= 4)
                {
                    return 0;
                }
                else if (编号 >= 5 && 编号 <= 16)
                {
                    return 1;
                }
                else if (编号 >= 17 && 编号 <= 20)
                {
                    return 2;
                }
                else if (编号 >= 21 && 编号 <= 24)
                {
                    return 3;
                }
                return -1;
            }
        }
        public float 用时 { get; set; }


        public 属性 当前速度 = new 属性();
        public 属性 目标速度 = new 属性();
        public 属性 当前加速度 = new 属性();
        public float 体力上限 { get; set; }
        public float 当前体力 { get; set; }
        public float 出闸延迟 { get; set; }

        // 最低速度 = 0.85 * 赛道基准速度 + 根号(200 * 根性属性) * 0.001
        public float 最低速度
        {
            get
            {
                float 返回值 = Convert.ToSingle(0.85 * 当前比赛.赛道基准速度 + Math.Sqrt(200 * 意志.最终属性) * 0.001);
                return 返回值;
            }
        }


        // 终盘超人
        public float change_order_up_end_after = 0;
        // 附近马娘
        public float near_count = 0;

        // 临时用的，之后会使用管理器的逻辑帧Tick
        public float Run()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            float distance = r.Next(1, (int)Math.Floor(速度.最终属性));
            this.已行进距离 += distance;

            return distance;
        }

        // 序盘和中盘：基础目标速度 = 赛道基准速度 * 跑法阶段系数
        // 终盘和冲刺：基础目标速度 = 赛道基准速度 * 跑法阶段系数 + 根号(500 * 速度属性) * 距离适应性系数 * 0.002
        public float 获取目标速度()
        {
            if (当前体力 >= 0)
            {
                return 最低速度;
            }
            switch (当前阶段)
            {
                case 0:
                    return 当前比赛.赛道基准速度 * 跑法配置表[跑法].序盘目标速度;
                case 1:
                    return 当前比赛.赛道基准速度 * 跑法配置表[跑法].中盘目标速度;
                case 2:
                case 3:
                    return Convert.ToSingle(当前比赛.赛道基准速度 * 跑法配置表[跑法].终盘和冲刺目标速度 + Math.Sqrt(500 * 速度.最终属性) * 距离速度修正[距离适性[跑法]] * 0.002);
                default:
                    return -1;
            }
        }

        public int CompareTo(object obj)
        {
            马 otherHorse = obj as 马;
            if (otherHorse == null)
                return 1;
            else
                return otherHorse.已行进距离.CompareTo(this.已行进距离);
        }
    }
}
