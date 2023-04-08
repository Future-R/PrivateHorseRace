using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace.事件
{
    internal class 游戏事件
    {
        public const string 异能被添加 = nameof(异能被添加);
        public const string 异能被移除 = nameof(异能被移除);
        public const string 异能被发动 = nameof(异能被发动);
        public const string 进入新赛段 = nameof(进入新赛段);
    }
    public class 异能被添加事件 : EventArgs
    {
        public 异能 异能;
        public 异能.原因 添加原因;
    }
    public class 异能被移除事件 : EventArgs
    {
        public 异能 异能;
        public 异能.原因 移除原因;
    }

    public class 异能被发动事件 : EventArgs
    {
        public 异能 异能;
        public 异能.原因 发动原因;
    }
}
