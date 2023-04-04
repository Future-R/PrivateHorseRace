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

        public List<异能> 状态 = new List<异能>();

        // 存放所有属性，方便遍历更新
        public List<属性> 属性组 = new List<属性>();

        public 属性 速度 = new 属性();
        public 属性 耐力 = new 属性();
        public 属性 力量 = new 属性();
        public 属性 意志 = new 属性();
        public 属性 智力 = new 属性();

        public char[] 属性优势 = new char[3] { '×', '×', '×' };

        public float 已行进距离 { get; set; }
        // 向上取整，从1开始计数。但距离为0时仍可能返回0。
        public int 赛段 { get { return (int)Math.Ceiling(已行进距离 / 当前比赛.总长度 * 24); } }
        // 0序盘1-4 1中盘5-16 2终盘17-20 3冲刺21-24
        public int 当前阶段
        {
            get
            {
                int 编号 = 赛段;
                if (编号 >= 0 && 编号 <= 4)
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

        private float _当前体力;
        public float 当前体力
        {
            get { return _当前体力; }
            set
            {
                if (value < 0)
                    _当前体力 = 0;
                else if (value > 体力上限)
                    _当前体力 = 体力上限;
                else
                    _当前体力 = value;
            }
        }
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

        public 马()
        {
            属性组.AddRange(new[] { 速度, 耐力, 力量, 意志, 智力, 当前速度, 目标速度, 当前加速度 });
        }

        // 序盘和中盘：基础目标速度 = 赛道基准速度 * 跑法阶段系数
        // 终盘和冲刺：基础目标速度 = 赛道基准速度 * 跑法阶段系数 + 根号(500 * 速度属性) * 距离适应性系数 * 0.002
        public float 获取基础目标速度()
        {
            if (当前体力 <= 0)
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

        // 加速度 = 基础加速度(平常0.0006，上坡0.0004) * 根号(500 * 力量属性) * 跑法阶段系数 * 场地适应性系数 * 距离适应性系数 + 技能调整值 + 起跑冲刺加值
        public float 获取加速度()
        {
            float 加速度 = Convert.ToSingle(0.0006 * Math.Sqrt(500 * 力量.最终属性));
            加速度 *= 距离加速修正[距离适性[(int)当前比赛.距离类型]];
            if (当前比赛.是草地)
            {
                加速度 *= 场地加速修正[草地适性];
            }
            else
            {
                加速度 *= 场地加速修正[泥地适性];
            }
            switch (当前阶段)
            {
                case 0:
                    return 加速度 * 跑法配置表[跑法].序盘加速度;
                case 1:
                    return 加速度 * 跑法配置表[跑法].中盘加速度;
                case 2:
                case 3:
                    return 加速度 * 跑法配置表[跑法].终盘和冲刺加速度;
                default:
                    return -1;
            }
        }

        public void 属性更新()
        {
            List<List<属性修正>> 修正组表 = 属性组.Select(x => x.修正组).ToList();
            foreach (List<属性修正> 修正组 in 修正组表)
            {
                for (int i = 修正组.Count - 1; i >= 0; i--)
                {
                    属性修正 item = 修正组[i];
                    // -1表示无限时间
                    if (item.剩余持续时间 != -1)
                    {
                        item.剩余持续时间 -= 一帧时间;
                        if (item.剩余持续时间 < 0)
                        {
                            工具.打印($"{名称}移除了{修正组[i].标签组.First()}");
                            修正组.RemoveAt(i);
                        }
                    }
                }
            }

        }

        public int CompareTo(object obj)
        {
            if (!(obj is 马 otherHorse))
                return 1;
            else
                return otherHorse.已行进距离.CompareTo(this.已行进距离);
        }
    }
}
