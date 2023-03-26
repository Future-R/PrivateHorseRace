using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    public class 比赛
    {
        public int ID { get; set; }
        public string 名称 { get; set; }
        public float 总长度 { get; set; }
        public bool 是草地 { get; set; }

        // 0良好 1略差 2差 3极差
        public int 场地状况 { get; set; }
        public List<马> 参赛马 { get; set; }

        public 距离 距离类型 = 距离.未知;

        public enum 距离
        {
            未知 = 0,
            短距离 = 1,
            英哩赛 = 2,
            中距离 = 3,
            长距离 = 4
        }

        public void 计算距离类型()
        {
            if (总长度 < 1600)
            {
                距离类型 = 距离.短距离;
            }
            else if(总长度 < 2000)
            {
                距离类型 = 距离.英哩赛;
            }
            else if (总长度 < 2500)
            {
                距离类型 = 距离.中距离;
            }
            else if(总长度 >= 2500)
            {
                距离类型 = 距离.长距离;
            }
        }
    }
}
