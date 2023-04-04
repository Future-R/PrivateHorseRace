using System;
using System.Collections.Generic;
/// <summary>
/// 便于触发事件的扩展类
/// </summary>
public static class 事件管理器扩展
{
    /// <summary>
    /// 无参触发事件
    /// </summary>
    public static void 触发事件(this object 触发源, string 事件名)
    {
        事件管理器.实例.触发事件(事件名, 触发源);
    }
    /// <summary>
    /// 带参触发事件
    /// </summary>
    public static void 触发事件(this object 触发源, string 事件名, EventArgs 参数)
    {
        事件管理器.实例.触发事件(事件名, 触发源, 参数);
    }

}
/// <summary>
/// 事件管理器
/// </summary>
public class 事件管理器 : 单例基类<事件管理器>
{
    private Dictionary<string, EventHandler> 函数字典 = new Dictionary<string, EventHandler>();

    /// <summary>
    /// 添加一个事件的监听者
    /// </summary>
    /// <param name="事件名">事件名</param>
    /// <param name="函数">事件处理函数</param>
    public void 订阅(string 事件名, EventHandler 函数)
    {
        if (函数字典.ContainsKey(事件名))
            函数字典[事件名] += 函数;
        else
            函数字典.Add(事件名, 函数);
    }
    /// <summary>
    /// 移除一个事件的监听者
    /// </summary>
    /// <param name="事件名">事件名</param>
    /// <param name="函数">事件处理函数</param>
    public void 退订(string 事件名, EventHandler 函数)
    {
        if (函数字典.ContainsKey(事件名))
            函数字典[事件名] -= 函数;
    }
    /// <summary>
    /// 触发事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="sender">触发源</param>
    public void 触发事件(string eventName, object sender)
    {
        if (函数字典.ContainsKey(eventName))
            函数字典[eventName]?.Invoke(sender, EventArgs.Empty);
    }
    /// <summary>
    /// 触发事件（有参数）
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="sender">触发源</param>
    /// <param name="args">事件参数</param>
    public void 触发事件(string eventName, object sender, EventArgs args)
    {
        if (函数字典.ContainsKey(eventName))
            函数字典[eventName]?.Invoke(sender, args);
    }
    /// <summary>
    /// 清空所有事件
    /// </summary>
    public void 清空()
    {
        函数字典.Clear();
    }
}
