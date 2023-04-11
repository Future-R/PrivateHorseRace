using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HorseRace
{
    public static class 状态显示
    {
        public static void 初始化()
        {
            事件管理器.实例.订阅(事件.游戏事件.异能被发动, 异能被发动);
        }

        public static void 结束()
        {
            事件管理器.实例.退订(事件.游戏事件.异能被发动, 异能被发动);
        }

        private static void 异能被发动(object sender, EventArgs e)
        {
            异能 当前异能 = sender as 异能;
            工具.打印($"{当前异能.承担者.名称}【{当前异能.唯一名称}】", ConsoleColor.Red);
            Thread.Sleep(1000);
        }
    }
}
