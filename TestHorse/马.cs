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
        public float 已行进距离 { get; set; }
        public float 用时 { get; set; }
        public int 速度 { get; set; }
        public int 耐力 { get; set; }
        public int 力量 { get; set; }
        public int 意志 { get; set; }
        public int 智力 { get; set; }

        // 当前速度
        public float current_speed = 0;
        public float target_speed = 0;
        //public int running_style = 0;
        // 终盘超人
        public float change_order_up_end_after = 0;
        // 附近马娘
        public float near_count = 0;

        public float Run()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            float distance = r.Next(1, this.速度 + 1);
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
