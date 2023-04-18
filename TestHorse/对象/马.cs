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
        // 焦躁的时候会和跑法不一致
        public int 跑法意识 = -1;
        public int 干劲 = -1;

        public List<异能> 状态 = new List<异能>();

        // 存放所有属性，方便遍历更新
        public List<属性> 属性组 = new List<属性>();

        public 属性 速度属性 = new 属性();
        public 属性 耐力属性 = new 属性();
        public 属性 力量属性 = new 属性();
        public 属性 意志属性 = new 属性();
        public 属性 智力属性 = new 属性();

        public 属性 体力消耗系数 = new 属性 { 基础属性 = 1 };

        public char[] 属性优势 = new char[3] { '×', '×', '×' };

        public int 比赛编号 = 0;

        public double 已行进距离 { get; set; }
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

        public 属性 当前速度 = new 属性();
        public 属性 目标速度 = new 属性();
        public 属性 当前加速度 = new 属性();
        public 属性 视野 = new 属性 { 基础属性 = 20 };

        // 阻挡与被阻挡
        // 马娘游戏里有宽度，我没做宽度设定，所以用一个公式来拟合
        // 马娘做被阻挡判定时，会根据前方2m内所有马分别做判定
        // 阻挡马有一个阻挡力，主要受力量和智力影响。冲刺盘时，智力影响失效。
        // 被阻挡马有突破力，主要受力量、智力和视野影响。距离越近，视野影响越小。
        // 如果有复数的马娘同时通过阻挡判定，前后距离最近的那一只会被选择为阻挡对象。

        public double 阻挡力
        {
            get
            {
                if (赛段 > 2 && 赛段 < 21)
                {
                    return 力量属性.最终属性 * 0.8;
                }
                else
                {
                    return 力量属性.最终属性 + 智力属性.最终属性 * 0.5;
                }
            }
        }
        public double 突破力
        {
            get
            {
                return 力量属性.最终属性 + 智力属性.最终属性 * 0.8 + 意志属性.最终属性 * 0.2;
            }
        }


        public double 体力上限 { get; set; }

        private double _当前体力;
        public double 当前体力
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


        // 最低速度 = 0.85 * 赛道基准速度 + 根号(200 * 根性属性) * 0.001
        public double 最低速度
        {
            get
            {
                double 返回值 = 0.85 * 当前比赛.赛道基准速度 + Math.Sqrt(200 * 意志属性.最终属性) * 0.001;
                return 返回值;
            }
        }

        // 冲线数据
        public double 冲线时间 = 0;
        public double 冲线速度 = 0;
        public int 名次 = 0;
        public double 着差 = 0;
        public string 着差文案 = "优胜";

        public double 被阻挡时间 = 0;
        public double 终盘超人 = 0;
        public double 附近马娘 = 0;

        public 马()
        {
            属性组.AddRange(new[] { 速度属性, 耐力属性, 力量属性, 意志属性, 智力属性, 当前速度, 目标速度, 当前加速度, 体力消耗系数, 视野 });
        }

        // 序盘和中盘：基础目标速度 = 赛道基准速度 * 跑法阶段系数
        // 终盘和冲刺：基础目标速度 = 赛道基准速度 * 跑法阶段系数 + 根号(500 * 速度属性) * 距离适应性系数 * 0.002
        public double 获取基础目标速度()
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
                    return 当前比赛.赛道基准速度 * 跑法配置表[跑法].终盘和冲刺目标速度 + Math.Sqrt(500 * 速度属性.最终属性) * 距离速度修正[距离适性[跑法]] * 0.002;
                default:
                    return -1;
            }
        }

        // 加速度 = 基础加速度(平常0.0006，上坡0.0004) * 根号(500 * 力量属性) * 跑法阶段系数 * 场地适应性系数 * 距离适应性系数 + 技能调整值 + 起跑冲刺加值
        public double 获取加速度()
        {
            double 加速度 = 0.0006 * Math.Sqrt(500 * 力量属性.最终属性);
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
                            //工具.打印($"{名称}移除了{修正组[i].标签组.First()}");
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
