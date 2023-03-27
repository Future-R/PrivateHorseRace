using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

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

        public int 短距离适性 = 0;
        public int 英哩赛适性 = 0;
        public int 中距离适性 = 0;
        public int 长距离适性 = 0;

        public int 领头适性 = 0;
        public int 前列适性 = 0;
        public int 居中适性 = 0;
        public int 后追适性 = 0;

        public int 干劲系数 = 0;

        // 跑法 0逃1先2中3追4大逃
        public int 跑法 = -1;
        public int 干劲 = -1;

        public float 修正速度 { get; set; }
        public float 修正耐力 { get; set; }
        public float 修正力量 { get; set; }
        public float 修正意志 { get; set; }
        public float 修正智力 { get; set; }

        public float 已行进距离 { get; set; }
        public float 用时 { get; set; }

        // 当前速度
        public float current_speed = 0;
        // 目标速度
        public float target_speed = 0;
        

        // 终盘超人
        public float change_order_up_end_after = 0;
        // 附近马娘
        public float near_count = 0;

        // 临时用的，之后会使用管理器的逻辑帧Tick
        public float Run()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            float distance = r.Next(1, this.基础速度 + 1);
            this.已行进距离 += distance;

            return distance;
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
