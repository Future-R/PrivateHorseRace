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
            Console.Title = "私密马赛 Private Derby";
            数据库更新();
            异能管理器.初始化();

            打印("请输入赛马数量：");
            int 赛马数量 = int.Parse(Console.ReadLine());

            当前比赛 = 工具.随机取一(所有赛道);
            //当前比赛 = 所有赛道[17];
            打印($"当前比赛：{当前比赛.名称}，{(当前比赛.是草地 == true ? "草地" : "泥地")}，{当前比赛.总长度}m，场地状况{场地状况调整配置表[当前比赛.场地状况].状况}");
            if (赛马数量 > 所有马的数量)
            {
                return;
            }
            当前比赛.参赛马 = 不重复抽取(所有马, 赛马数量);
            流程管理器.比赛初始化();
            状态显示.初始化();
            比赛播报.初始化();

            评委点评();

            for (int i = 0; i < 当前比赛.参赛马.Count; i++)
            {
                打印($"{当前比赛.参赛马[i].属性优势[0]}{当前比赛.参赛马[i].属性优势[1]}{当前比赛.参赛马[i].属性优势[2]}{i + 1}号{当前比赛.参赛马[i].名称}，{跑法配置表[当前比赛.参赛马[i].跑法].跑法}跑法，状态{干劲配置表[当前比赛.参赛马[i].干劲].名称}");
            }
            Console.ReadKey();
            流程管理器.开跑();
            流程管理器.赛后结算();
            Console.ReadKey();
        }

        // 暂时按速、耐、力的属性三个维度评分
        public static void 评委点评()
        {
            char[] 属性优势值 = new char[] { '◎', '○', '●', '▲', '△' };
            var 速度排序 = 当前比赛.参赛马.OrderByDescending(x => x.速度属性.最终属性 + 0.25f * x.力量属性.最终属性).ToArray();
            for (int i = 0; i < 速度排序.Count(); i++)
            {
                if (i < 属性优势值.Length)
                {
                    速度排序[i].属性优势[0] = 属性优势值[i];
                }
            }
            var 耐力排序 = 当前比赛.参赛马.OrderByDescending(x => x.耐力属性.最终属性 + 0.75f * x.意志属性.最终属性).ToArray();
            for (int i = 0; i < 耐力排序.Count(); i++)
            {
                if (i < 属性优势值.Length)
                {
                    耐力排序[i].属性优势[1] = 属性优势值[i];
                }
            }
            var 力量排序 = 当前比赛.参赛马.OrderByDescending(x => x.力量属性.最终属性 + 0.25f * x.智力属性.最终属性).ToArray();
            for (int i = 0; i < 力量排序.Count(); i++)
            {
                if (i < 属性优势值.Length)
                {
                    力量排序[i].属性优势[2] = 属性优势值[i];
                }
            }
        }
    }

}