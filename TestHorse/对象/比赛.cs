﻿using System;
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
        public double 总长度 { get; set; }
        public bool 是草地 { get; set; }

        // 0良好 1略差 2差 3极差
        public int 场地状况 { get; set; }
        public List<马> 参赛马 { get; set; }

        public List<马> 赛后马 = new List<马>();

        public double 进行时间 { get; set; }

        // 前三名中跑法最靠前的，关系到其他马的位置意识
        public 马 领头马 { get; set; }


        public 距离 距离类型
        {
            get
            {
                距离 返回值 = 距离.未知;
                if (总长度 < 1600)
                {
                    返回值 = 距离.短距离;
                }
                else if (总长度 < 2000)
                {
                    返回值 = 距离.英哩赛;
                }
                else if (总长度 < 2500)
                {
                    返回值 = 距离.中距离;
                }
                else if (总长度 >= 2500)
                {
                    返回值 = 距离.长距离;
                }
                return 返回值;
            }
        }

        // 赛道基准速度 = 20 - (赛道长度 - 2000) / 1000
        public double 赛道基准速度
        {
            get
            {
                double 返回值 = 20 - (总长度 - 2000) / 1000;
                return 返回值 > 1 ? 返回值 : 1;
            }
        }

        public enum 距离
        {
            未知 = -1,
            短距离 = 0,
            英哩赛 = 1,
            中距离 = 2,
            长距离 = 3
        }
    }
}
