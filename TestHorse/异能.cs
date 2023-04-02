using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    public class 异能
    {
        public static string 唯一名称;
        public static string 标签;
        public static int 优先级;
        public static float 持续时间;
        public static string 添加者;
        public static 马 承担者;

        public delegate void 添加时();
        public delegate void 运行时();
        public delegate void 移除时();
    }
}
