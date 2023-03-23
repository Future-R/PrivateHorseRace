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
        public int 场地重度 { get; set; }
        public List<马> 参赛马 { get; set; }
    }
}
