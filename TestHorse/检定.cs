using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    public static class 检定
    {
        // 焦躁率 = ((6.5/log10(0.1*智力+1))^2)%
        // 如果焦躁，赛马将在2~9赛段随机1段触发焦躁，此逻辑在外部实现
        public static bool 焦躁检定(int 智力)
        {
            double 焦躁率 = Math.Pow((6.5 / Math.Log10(0.1 * 智力 + 1)), 2) / 100;
            double 随机结果 = 工具.随机.NextDouble();
            return 焦躁率 > 随机结果;
        }

        public static double 计算冲线具体时间(double 当前用时, double 当前速度, double 已行进距离)
        {
            double 距离差 = 已行进距离 - 数据表.当前比赛.总长度;
            double 多余时间 = 距离差 / 当前速度;
            double 具体冲线时间 = 当前用时 - 多余时间;
            return 具体冲线时间;
        }

        //public static double 计算着差(double 多余时间, double 冲线速度, double 已行进距离)
        //{
        //    double 前方冲线时自己已行进距离 = 已行进距离 - 冲线速度 * 多余时间;
        //    工具.打印($"前方冲线时自己已行进距离{前方冲线时自己已行进距离} = 已行进距离{已行进距离} - 冲线速度{冲线速度}*多余时间{多余时间}");
        //    return 数据表.当前比赛.总长度 - 前方冲线时自己已行进距离;
        //}
    }
}
