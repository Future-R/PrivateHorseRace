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
    }
    public class 异能被添加事件 : EventArgs
    {
        public string 异能名称;
        public string 添加者;
        public 马 承担者;
    }
}
