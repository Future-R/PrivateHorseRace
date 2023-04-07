using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    public class 异能
    {
        public virtual string 唯一名称 { get; set; }
        public virtual List<string> 标签组 { get; set; }
        public virtual string 添加者 { get; set; }
        public virtual 马 承担者 { get; set; }
        public virtual double 持续时间 { get; set; }

        public enum 原因
        {
            系统规则,
            自身效果,
            时间结束,
        }
        public void 添加(原因 添加原因)
        {
            this.触发事件(事件.游戏事件.异能被添加, new 事件.异能被添加事件 { 异能 = this, 添加原因 = 添加原因 });
            承担者.状态.Add(this);
            添加时();
        }

        public void 移除(原因 移除原因)
        {
            this.触发事件(事件.游戏事件.异能被移除, new 事件.异能被移除事件 { 异能 = this, 移除原因 = 移除原因 });
            承担者.状态.Remove(this);
            移除时();
        }
        public virtual void 添加时() { }

        public virtual void 运行时() { }

        public virtual void 移除时() { }
    }
}
