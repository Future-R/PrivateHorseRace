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
        // 如果焦躁，赛马将在2~9赛段随机1段触发焦躁，此逻辑在焦躁异能上实现
        public static bool 焦躁检定(double 智力)
        {
            double 焦躁率 = Math.Pow((6.5 / Math.Log10(0.1 * 智力 + 1)), 2) / 100;
            double 随机结果 = 工具.随机.NextDouble();
            return 焦躁率 > 随机结果;
        }

        public static 马 更新领头马()
        {
            List<马> 列表 = 数据表.当前比赛.参赛马;
            列表.Sort();
            List<马> 新列表 = 列表.Count < 3 ? new List<马>(列表) : 列表.GetRange(0, 3);
            // 大逃 逃 先 差 追
            int[] 跑法顺序 = new int[] { 4, 0, 1, 2, 3 };
            foreach (int 跑法 in 跑法顺序)
            {
                foreach (马 item in 新列表)
                {
                    if (item.跑法 == 跑法)
                    {
                        return item;
                    }
                }
            }
            return 新列表.FirstOrDefault();
        }

        public static double 计算冲线具体时间(double 当前用时, double 当前速度, double 已行进距离)
        {
            double 距离差 = 已行进距离 - 数据表.当前比赛.总长度;
            double 多余时间 = 距离差 / 当前速度;
            double 具体冲线时间 = 当前用时 - 多余时间;
            return 具体冲线时间;
        }

        public static bool 是否满足非常模式(马 当前马)
        {
            // 如果有焦躁，智力检定必定通过
            bool 智力检定成功 = false;
            焦躁 焦躁 = (焦躁)异能管理器.获取异能(当前马, "焦躁");
            if (焦躁 != null && 焦躁.持续时间 > 0) 
            {
                智力检定成功 = true;
            }

            switch (当前马.跑法意识)
            {
                // 逃马意识
                case 0:
                case 4:
                    var 同跑法的马 = 数据表.当前比赛.参赛马.Where(x => x.跑法意识 == 当前马.跑法意识);

                    // 提速模式：位于头名，但距离后方马的距离小于4.5m (如果场上只有1只逃马，范围增加到12.5m, 大逃则总是增加到 17.5m) ，并且通过智力判定
                    if (当前马.名次 == 1)
                    {
                        马 第二名 = 数据表.当前比赛.参赛马.Where(x => x != 当前马).FirstOrDefault();
                        if (第二名 == null)
                        {
                            return false;
                        }

                        double 进入距离 = 位置意识.逃马提速距离;
                        if (同跑法的马.Count() == 1)
                        {
                            进入距离 = 位置意识.单逃提速距离;
                        }
                        if (当前马.跑法意识 == 4)
                        {
                            进入距离 = 位置意识.大逃提速距离;
                        }

                        if (当前马.已行进距离 - 第二名.已行进距离 < 进入距离)
                        {
                            if (智力检定成功)
                            {
                                异能管理器.添加异能(new 逃马提速(), 当前马);
                                return true;
                            }
                            // 智力检定
                            else if (工具.随机.NextDouble() < 20 * Math.Log10(当前马.智力属性.最终属性 * 0.1) / 100)
                            {
                                异能管理器.添加异能(new 逃马提速(), 当前马);
                                return true;
                            }
                        }
                        return false;
                    }

                    // 超越模式：在同一跑法的马娘中，此马娘并非头名，并且通过智力判定
                    if (当前马 != 同跑法的马.FirstOrDefault())
                    {
                        马 第二名 = 同跑法的马.Where(x => x != 当前马).FirstOrDefault();
                        if (第二名 == null)
                        {
                            return false;
                        }

                        // 智力检定，有焦躁的话百分百成功
                        if (智力检定成功)
                        {
                            异能管理器.添加异能(new 逃马超越(), 当前马);
                        }
                        else if (工具.随机.NextDouble() < 20 * Math.Log10(当前马.智力属性.最终属性 * 0.1) / 100)
                        {
                            异能管理器.添加异能(new 逃马超越(), 当前马);
                        }
                    }

                    return false;
                // 先马
                case 1:
                    return false;
                // 差马
                case 2:
                    return false;
                // 追马
                case 3:
                    return false;
                default:
                    工具.报错("错误的跑法意识");
                    return false;
            }
        }

        //public static double 计算着差(double 多余时间, double 冲线速度, double 已行进距离)
        //{
        //    double 前方冲线时自己已行进距离 = 已行进距离 - 冲线速度 * 多余时间;
        //    工具.打印($"前方冲线时自己已行进距离{前方冲线时自己已行进距离} = 已行进距离{已行进距离} - 冲线速度{冲线速度}*多余时间{多余时间}");
        //    return 数据表.当前比赛.总长度 - 前方冲线时自己已行进距离;
        //}
    }
}
