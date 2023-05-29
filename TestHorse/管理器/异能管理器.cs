using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    public static class 异能管理器
    {
        private static List<异能> 异能池 = new List<异能>();

        // 遍历异能池时不能直接添加或移除其中的异能，所以用另外的池子记录一下，等遍历完了一并添加和移除
        private static List<异能> 将被添加的异能;
        private static List<异能> 将被移除的异能;

        public static void 初始化()
        {
            异能池 = new List<异能>();
            将被添加的异能 = new List<异能>();
            将被移除的异能 = new List<异能>();
            事件管理器.实例.订阅(事件.游戏事件.异能被移除, 记录将被移除的异能);
        }

        public static void 回收()
        {
            foreach (var 异能 in 异能池)
            {
                异能.移除(异能.原因.系统规则);
            }
        }

        public static void 遍历运行时()
        {
            异能池.ForEach(x => x.运行时());
            异能池.RemoveAll(x => 将被移除的异能.Contains(x));
            异能池.AddRange(将被添加的异能);
            将被添加的异能.Clear();
            将被移除的异能.Clear();
        }

        public static void 添加异能(异能 异能, string 添加者, 马 承担者, 异能.原因 添加原因 = 异能.原因.系统规则)
        {
            异能.添加者 = 添加者;
            异能.承担者 = 承担者;
            异能.添加(添加原因);

            将被添加的异能.Add(异能);
        }

        public static void 添加异能(异能 异能, 马 承担者, 异能.原因 添加原因 = 异能.原因.系统规则)
        {
            异能.添加者 = "系统";
            异能.承担者 = 承担者;
            异能.添加(添加原因);

            将被添加的异能.Add(异能);
        }

        public static bool 有标签(马 马, string 标签)
        {
            return 马.状态.Any(异能 => 异能.标签组.Contains(标签));
        }

        public static 异能 获取异能(马 马, string 名称)
        {
            return 马.状态.Where(异能 => 异能.唯一名称 == 名称).FirstOrDefault();
        }

        private static void 记录将被移除的异能(object sender, EventArgs e)
        {
            事件.异能被移除事件 事件数据 = e as 事件.异能被移除事件;
            将被移除的异能.Add(事件数据.异能);
        }
    }
}
