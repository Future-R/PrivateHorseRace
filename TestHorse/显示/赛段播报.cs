using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    public static class 赛段播报
    {
        private static List<int> 待播报阶段;
        public static void 初始化()
        {
            待播报阶段 = new List<int> { 2, 5, 17, 21 };
            事件管理器.实例.订阅(事件.游戏事件.进入新赛段, 领头马进入新阶段);
        }

        public static void 结束()
        {
            事件管理器.实例.退订(事件.游戏事件.进入新赛段, 领头马进入新阶段);
        }

        private static void 领头马进入新阶段(object sender, EventArgs e)
        {
            马 当前马 = sender as 马;
            if (当前马.名次 == 1 && 待播报阶段.Count != 0)
            {
                if (当前马.赛段 >= 待播报阶段.FirstOrDefault())
                {
                    switch (待播报阶段.FirstOrDefault())
                    {
                        case 2:
                            工具.打印($"{当前马.名称}率先脱出，拿下领先位置！", ConsoleColor.Yellow);
                            break;
                        case 5:
                            工具.打印($"来到了中盘，目前领先的是{当前马.名称}！", ConsoleColor.Yellow);
                            break;
                        case 17:
                            工具.打印($"来到了终盘，目前领先的是{当前马.名称}！", ConsoleColor.Yellow);
                            break;
                        case 21:
                            工具.打印($"通过大榉树，来到第四弯道！目前领先的是{当前马.名称}！", ConsoleColor.Yellow);
                            break;
                        default:
                            break;
                    }
                    待播报阶段.RemoveAt(0);
                }
            }
        }
    }
}
