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
        public static double 持续时间;
        public static string 添加者;
        public static 马 承担者;

        //public virtual void 初始化() {事件管理器.实例.订阅(事件.游戏事件.异能被添加, 添加时()) }
        public virtual void 添加时() { }

        public virtual void 运行时() { }

        public virtual void 移除时() { }
    }
}
