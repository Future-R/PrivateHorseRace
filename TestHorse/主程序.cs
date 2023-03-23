using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static HorseRace.数据表;



namespace HorseRace
{
    class 主程序
    {
        static void Main(string[] args)
        {
            数据表.更新();

            Console.WriteLine("请输入赛马数量：");
            int 赛马数量 = int.Parse(Console.ReadLine());

            当前比赛 = 工具.随机取一(数据表.所有赛道);

            if (赛马数量 > 数据表.所有马的数量)
            {
                return;
            }
            当前比赛.参赛马 = 工具.不重复抽取(数据表.所有马, 赛马数量);
            当前比赛.参赛马.ForEach((x) => Console.WriteLine($"名称{x.名称}"));
            Console.ReadKey();

            Console.WriteLine("比赛开始！");
            while (当前比赛.总长度 > 当前比赛.参赛马.First().已行进距离)
            {
                for (int i = 0; i < 赛马数量; i++)
                {
                    Console.WriteLine($"{当前比赛.参赛马[i].名称}跑了{当前比赛.参赛马[i].Run()}米，进度{当前比赛.参赛马[i].已行进距离}米");
                }
                当前比赛.参赛马.Sort();
                Console.WriteLine("");
                Thread.Sleep(100);
            }
            Console.WriteLine("比赛结束");
            Console.WriteLine("结果：");
            当前比赛.参赛马.Sort();
            for (int i = 0; i < 赛马数量; i++)
            {
                Console.WriteLine($"第{i + 1}名：{当前比赛.参赛马[i].名称}");
            }
            Console.ReadKey();
        }
    }

}