using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static HorseRace.数据表;
using static HorseRace.工具;



namespace HorseRace
{
    class 主程序
    {
        static void Main(string[] args)
        {
            数据表.更新();

            打印("请输入赛马数量：");
            int 赛马数量 = int.Parse(Console.ReadLine());

            当前比赛 = 工具.随机取一(数据表.所有赛道);
            打印($"当前比赛：{当前比赛.名称}，{(当前比赛.是草地 == true ? "草地" : "泥地")}，{当前比赛.总长度}m，场地状况{场地状况调整配置表[当前比赛.场地状况].状况}");
            if (赛马数量 > 数据表.所有马的数量)
            {
                return;
            }
            当前比赛.参赛马 = 工具.不重复抽取(数据表.所有马, 赛马数量);
            管理器.比赛初始化();
            for (int i = 0; i < 当前比赛.参赛马.Count; i++)
            {
                打印($"{i + 1}号{当前比赛.参赛马[i].名称}，{跑法配置表[当前比赛.参赛马[i].跑法].跑法}跑法，状态{干劲配置表[当前比赛.参赛马[i].干劲].名称}");
            }
            //当前比赛.参赛马.ForEach((x) => 打印($"名称{x.名称}"));
            Console.ReadKey();

            打印("比赛开始！");
            while (当前比赛.总长度 > 当前比赛.参赛马.First().已行进距离)
            {
                for (int i = 0; i < 赛马数量; i++)
                {
                    打印($"{当前比赛.参赛马[i].名称}跑了{当前比赛.参赛马[i].Run()}米，进度{当前比赛.参赛马[i].已行进距离}米");
                }
                当前比赛.参赛马.Sort();
                打印("");
                Thread.Sleep(100);
            }
            打印("比赛结束");
            打印("结果：");
            当前比赛.参赛马.Sort();
            for (int i = 0; i < 赛马数量; i++)
            {
                打印($"第{i + 1}名：{当前比赛.参赛马[i].名称}");
            }
            Console.ReadKey();
        }
    }

}