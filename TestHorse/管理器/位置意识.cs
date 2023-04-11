using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    /*位置意识在前10个赛段影响目标速度（整个序盘，再加上中盘的前半段）。在通常模式以外有5种不同的位置意识模式。
     * 对于逃马而言有2种模式：提速模式和超越模式。对非逃马而言也有2种模式：加快节奏模式和放缓节奏模式。
     * 在此之外，所有跑法还会有一个特殊的EX提速模式。
     * 如果满足一个非通常模式的进入条件，马娘就会进入那个模式，这通常要求马娘成功通过一次基于智力属性的判定。这个判定无论成功与否，有固定2秒的间隔。
     * 在满足非通常模式的退出条件，又或者非通常模式已经持续了一整个赛段（对于大逃跑法来说可以持续3个赛段）的长度后，马娘会回到通常模式。
     * 在回到通常模式后，马娘需要经过1秒的间隔才能重新开始进入非通常模式判定（再加上前面提及的2秒间隔，一般需要等待3秒。）*/
    public static class 位置意识
    {
        private static Dictionary<马, double> 冷却池 { get; set; }

        public static double 一个赛段的距离 { get; set; }

        public static double 逃马提速距离 = 4.5;
        public static double 单逃提速距离 = 12.5;
        public static double 大逃提速距离 = 17.5;
        public static double 逃马超越距离 = 10;
        public static double 大逃超越距离 = 27.5;



        public static void 初始化()
        {
            冷却池 = new Dictionary<马, double>();
            foreach (马 参赛马 in 数据表.当前比赛.参赛马)
            {
                冷却池.Add(参赛马, 2);
                参赛马.跑法意识 = 参赛马.跑法;
            }
            一个赛段的距离 = 数据表.当前比赛.总长度 / 24;
        }

        public static void 清空冷却(马 马)
        {
            冷却池[马] = 0;
        }

        public static void 逻辑帧()
        {
            List<马> 需要移出位置意识的马 = new List<马>();
            foreach (var item in 冷却池)
            {
                if (item.Key.赛段 > 10)
                {
                    需要移出位置意识的马.Add(item.Key);
                }
            }

            foreach (var item in 需要移出位置意识的马)
            {
                冷却池.Remove(item);
            }

            for (int i = 0; i < 冷却池.Count; i++)
            {
                var item = 冷却池.ElementAt(i);
                // 如果马身上没有位置意识异能，说明还在通常模式，计算CD
                if (!异能管理器.有标签(item.Key, "位置意识"))
                {
                    冷却池[item.Key] = item.Value - 数据表.一帧时间;

                    // 如果冷却完毕，检查条件是否满足，如果满足，进入智力判定
                    if (item.Value <= 0)
                    {
                        // 检定成功，冷却变为3秒（退出非常模式后才开始冷却），否则，冷却时间变为2秒
                        // 分发非常模式异能的逻辑和检定写在一起了
                        if (检定.是否满足非常模式(item.Key))
                        {
                            冷却池[item.Key] = 3;
                        }
                        else
                        {
                            冷却池[item.Key] = 2;
                        }
                    }
                }
            }
        }
    }
}
