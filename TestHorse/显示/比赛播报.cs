using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HorseRace.数据表;

namespace HorseRace
{
    public static class 比赛播报
    {
        private static List<int> 待播报阶段;
        public static void 初始化()
        {
            待播报阶段 = new List<int> { 2, 5, 17, 21 };
            事件管理器.实例.订阅(事件.游戏事件.进入新赛段, 领头马进入新阶段);
            事件管理器.实例.订阅(事件.游戏事件.体力耗尽, 领头马体力耗尽);
            事件管理器.实例.订阅(事件.游戏事件.起跑, 出迟播报);
        }

        public static void 结束()
        {
            事件管理器.实例.退订(事件.游戏事件.进入新赛段, 领头马进入新阶段);
            事件管理器.实例.退订(事件.游戏事件.体力耗尽, 领头马体力耗尽);
            事件管理器.实例.退订(事件.游戏事件.起跑, 出迟播报);
        }

        private static void 出迟播报(object sender, EventArgs e)
        {
            马 当前马 = sender as 马;
            double 当前马延迟 = (当前马.状态.Find(x => x.唯一名称 == "出闸") as 出闸).剩余延迟;
            double 累计延迟 = 当前比赛.进行时间 - 一帧时间 + 当前马延迟;
            if (累计延迟 > 1)
            {
                工具.打印($"这可真是世纪大出迟呢{当前马.名称}！", ConsoleColor.Red);
                Thread.Sleep(2000);
            }
            else if (累计延迟 > 0.095)
            {
                工具.打印($"哎呀！这是怎么了！{当前马.名称}处于很尴尬的位置从后方开始追赶。", ConsoleColor.Red);
                Thread.Sleep(2000);
            }
            else if (累计延迟 > 0.09)
            {
                工具.打印($"{当前马.名称}毫无疑问出迟了！！", ConsoleColor.Red);
                Thread.Sleep(1000);
            }
            else if (累计延迟 > 0.085)
            {
                工具.打印($"{当前马.名称}稍微有些出迟了！", ConsoleColor.Red);
                Thread.Sleep(1000);
            }
            else if (累计延迟 > 0.08)
            {
                工具.打印($"{当前马.名称}是出迟了吗！？", ConsoleColor.Red);
                Thread.Sleep(1000);
            }
        }

        public static void 冲线解说(List<马> 这一帧冲线的马)
        {
            if (当前比赛.赛后马.Count == 0)
            {
                if (这一帧冲线的马.Count != 1)
                {
                    工具.打印($"{string.Join("、", 这一帧冲线的马.Select(x => x.名称))}同时冲线！{这一帧冲线的马.FirstOrDefault().名称}是否在姿势上占有优势呢！", ConsoleColor.Yellow);
                }
                else
                {
                    马 当前马 = 这一帧冲线的马.FirstOrDefault();
                    double 富余时间 = 当前比赛.进行时间 - 当前马.冲线时间;
                    马 后方马 = 当前比赛.参赛马.OrderByDescending(x => x.已行进距离 - x.当前速度.最终属性 * 富余时间).FirstOrDefault();
                    double 着差 = 当前比赛.总长度 - (后方马.已行进距离 - 后方马.当前速度.最终属性 * 富余时间);
                    // 大差
                    if (着差 > 25)
                    {
                        工具.打印($"大差冲线！{当前马.名称}展现了压倒性的实力，制霸比赛！", ConsoleColor.Yellow);
                    }
                    // 超过2马身
                    else if (着差 > 5)
                    {
                        工具.打印($"轻松胜利！{当前马.名称}！还有能和这位选手竞争的赛马娘吗！", ConsoleColor.Yellow);
                    }
                    else
                    {
                        工具.打印($"{当前马.名称}越过终点线！漂亮的胜利！", ConsoleColor.Yellow);
                    }
                }
                Thread.Sleep(3000);
            }
            else if (当前比赛.赛后马.Count == 1)
            {
                switch (这一帧冲线的马.Count)
                {
                    case 1:
                        工具.打印($"第二名是{这一帧冲线的马.FirstOrDefault().名称}。", ConsoleColor.Yellow);
                        break;
                    case 2:
                        if (这一帧冲线的马.Last().名称 == "优秀素质")
                        {
                            工具.打印($"{这一帧冲线的马.Last().名称}再次获得了第三名！第二名是{这一帧冲线的马.FirstOrDefault().名称}。", ConsoleColor.Yellow);
                            Thread.Sleep(3000);
                        }
                        else
                        {
                            工具.打印($"第二名是{这一帧冲线的马.FirstOrDefault().名称}；第三名是{这一帧冲线的马.Last().名称}。", ConsoleColor.Yellow);
                            Thread.Sleep(2000);
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (当前比赛.赛后马.Count == 2)
            {
                if (这一帧冲线的马.FirstOrDefault().名称 == "优秀素质")
                {
                    工具.打印($"{这一帧冲线的马.FirstOrDefault().名称}再夺铜牌！", ConsoleColor.Yellow);
                    Thread.Sleep(3000);
                }
                else
                {
                    工具.打印($"{这一帧冲线的马.FirstOrDefault().名称}第三个冲线。", ConsoleColor.Yellow);
                    Thread.Sleep(1500);
                }
            }
        }

        private static void 领头马进入新阶段(object sender, EventArgs e)
        {
            马 当前马 = sender as 马;
            if (当前马.名次 == 1 && 数据表.当前比赛.参赛马.Count != 1)
            {
                if (待播报阶段.Count != 0 && 当前马.赛段 >= 待播报阶段.FirstOrDefault())
                {
                    switch (待播报阶段.FirstOrDefault())
                    {
                        case 2:
                            工具.打印($"{当前马.名称}率先脱出，拿下领先位置！", ConsoleColor.Yellow);
                            Thread.Sleep(2000);
                            break;
                        case 5:
                            工具.打印($"来到了中盘，目前领先的是{当前马.名称}！", ConsoleColor.Yellow);
                            Thread.Sleep(2000);
                            break;
                        case 17:
                            工具.打印($"来到了终盘，目前领先的是{当前马.名称}！", ConsoleColor.Yellow);
                            Thread.Sleep(2000);
                            break;
                        case 21:
                            马 后方马 = 数据表.当前比赛.参赛马[1];
                            double 差距 = 当前马.已行进距离 - 后方马.已行进距离;
                            if (差距 > 25)
                            {
                                工具.打印($"{当前马.名称}甩开后方很大的差距！在第二名的位置奔跑的是{后方马.名称}。", ConsoleColor.Yellow);
                            }
                            else if (差距 > 10)
                            {
                                工具.打印($"{当前马.名称}遥遥领先！现在第二名是{后方马.名称}。", ConsoleColor.Yellow);
                            }
                            else if (差距 > 5)
                            {
                                工具.打印($"{当前马.名称}保持领先！现在第二名是{后方马.名称}。", ConsoleColor.Yellow);
                            }
                            else if (差距 > 2.5)
                            {
                                工具.打印($"脱颖而出的是{当前马.名称}！但{后方马.名称}依然紧追不舍！", ConsoleColor.Yellow);
                            }
                            else
                            {
                                工具.打印($"{当前马.名称}、{后方马.名称}互不相让！", ConsoleColor.Yellow);
                            }

                            Thread.Sleep(2000);
                            break;
                        default:
                            break;
                    }
                    待播报阶段.RemoveAt(0);
                }
            }
        }

        private static void 领头马体力耗尽(object sender, EventArgs e)
        {
            马 当前马 = sender as 马;
            if (当前马.名次 == 1)
            {
                工具.打印($"{当前马.名称}虽然辛苦但仍然坚持着！", ConsoleColor.Red);
                Thread.Sleep(2000);
            }
            else if (当前马.名次 == 2 || 当前马.名次 == 3)
            {
                工具.打印($"{当前马.名称}到此为止了吗！", ConsoleColor.Red);
                Thread.Sleep(1000);
            }
        }
    }
}
