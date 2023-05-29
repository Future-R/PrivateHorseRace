﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HorseRace.数据表;

namespace HorseRace
{
    internal class 流程管理器
    {
        // 比赛的初始化
        public static void 比赛初始化()
        {
            foreach (马 当前马 in 当前比赛.参赛马)
            {
                // 随机干劲
                当前马.干劲 = 工具.随机.Next(5);
                double 干劲系数 = 干劲配置表[当前马.干劲].基础属性系数;
                // 决定跑法，目前先随机
                // 随机算法为，往权重池中添加适性等级的平方数量的跑法，然后随机抽
                // 举例说，距离适性A意味着(6^2=36)权重，G则是(0^2=0)
                // Range1~4，对应逃~差，0是爆领
                List<int> 跑法权重池 = Enumerable.Range(1, 4)
                    .SelectMany(i => Enumerable.Repeat(i, (int)Math.Pow(当前马.跑法适性[i], 2))).ToList();
                当前马.跑法 = (马.跑法位置)工具.随机取一(跑法权重池);

                // 计算初始属性
                // 调整后速度值 = 基础速度 * 赛道加成 + 场地状况调整 + 育成模式加值
                // 调整后耐力值 = 基础耐力 * 育成模式加值
                // 调整后力量值 = 基础力量 + 场地状况调整 + 育成模式加值
                // 调整后根性值 = 基础根性 + 育成模式加值
                // 调整后智力值 = 基础智力 * 跑法系数 + 育成模式加值

                int 场地速度调整值 = 场地状况调整配置表[当前比赛.场地状况].泥地速度调整;
                int 场地力量调整值 = 场地状况调整配置表[当前比赛.场地状况].泥地力量调整;
                if (当前比赛.是草地)
                {
                    场地速度调整值 = 场地状况调整配置表[当前比赛.场地状况].草地速度调整;
                    场地力量调整值 = 场地状况调整配置表[当前比赛.场地状况].草地力量调整;
                }

                当前马.速度属性.基础属性 = 干劲系数 * 当前马.基础速度 + 场地速度调整值;
                当前马.力量属性.基础属性 = 干劲系数 * 当前马.基础力量 + 场地力量调整值;
                当前马.耐力属性.基础属性 = 干劲系数 * 当前马.基础耐力;
                当前马.意志属性.基础属性 = 干劲系数 * 当前马.基础意志;
                当前马.智力属性.基础属性 = 干劲系数 * 当前马.基础智力 * 跑法智力修正[当前马.跑法适性[(int)当前马.跑法]];

                // TODO: 根据基础智力（受干劲影响）检定技能是否可以发动
                // TODO: 发动被动技能

                // 计算出闸延迟
                // TODO: 如果当前马为8号，则增加出闸时间0~0.05秒(?)
                异能管理器.添加异能(new 出闸((工具.随机.NextDouble() + 工具.随机.NextDouble()) * 0.05), 当前马);

                // 计算是否在2~9段焦躁
                if (检定.焦躁检定(当前马.智力属性.最终属性))
                {
                    异能管理器.添加异能(new 焦躁(), 当前马);
                }

                // 根据耐力算出体力
                // 最大体力值 = 跑法系数 * 耐力属性 + 赛道长度
                当前马.体力上限 = 跑法配置表[(int)当前马.跑法].体力系数 * 当前马.耐力属性.最终属性 + 当前比赛.总长度;
                当前马.当前体力 = 当前马.体力上限;

                当前比赛.赛后马.Clear();
            }

            位置意识.初始化();
        }

        // 开跑！
        public static void 开跑()
        {
            工具.打印("");
            工具.打印("比赛开始！");
            // 由于所有异能都会晚一帧添加，包括出闸，所以初始时间回调1帧
            当前比赛.进行时间 = -一帧时间;
            // 循环，直到最后一只马冲线
            // 逻辑帧中，已经冲线的马会移除出参赛马List，放入赛后马list
            while (当前比赛.参赛马.Count != 0)
            {
                逻辑帧();
                //当前比赛.参赛马.Sort();
                //工具.打印("");
                //Thread.Sleep(50);
            }
        }

        public static void 赛后结算()
        {
            工具.打印($"\n当前比赛：{当前比赛.名称}，{(当前比赛.是草地 == true ? "草地" : "泥地")}，{当前比赛.总长度}m，场地状况{场地状况调整配置表[当前比赛.场地状况].状况}");
            工具.打印("结果：");

            // 按冲线时间排名
            List<马> 名次表 = 当前比赛.赛后马.OrderBy(x => x.冲线时间).ToList();

            //// 第一名的着差是第二名着差的负数
            //if (名次表.Count > 1)
            //{
            //    名次表[0].着差 = -名次表[1].着差;
            //}

            for (int i = 0; i < 名次表.Count; i++)
            {
                马 当前马 = 名次表[i];
                当前马.名次 = i + 1;

                if (当前马 != 名次表.Last())
                {
                    // 输入前方马拉开的着差，赋予后方的马
                    // 着差为什么不存在后方马身上呢，因为后方马被连续超越时，着差会被覆盖
                    名次表[i + 1].着差文案 = 工具.获取差着文案(当前马.着差);
                }

                工具.打印($"{当前马.名次,2}着【{当前马.名称}】{当前马.着差文案}，用时{Math.Round(当前马.冲线时间 * 结算时间修正, 2)}");
            }
        }

        // 每0.05秒调用，相当于每秒20逻辑帧
        public static void 逻辑帧()
        {

            // 更新时间
            当前比赛.进行时间 += 一帧时间;

            检定.更新名次();

            // 更新位置意识模式
            当前比赛.领头马 = 检定.更新领头马();
            位置意识.逻辑帧();

            // TODO: 检查弯道是否结束

            // 检查技能发动
            异能管理器.遍历运行时();

            // 打印日志
            马 第一名 = 当前比赛.参赛马.First();
            马 第二名 = 当前比赛.参赛马.FirstOrDefault(马 => 马 != 第一名);
            if (第二名 != null)
            {
                工具.打印($"{第一名.名称}{Math.Round(第一名.已行进距离, 2)}  {Math.Round(第一名.已行进距离 - 第二名.已行进距离, 2)}{第二名.名称}");
            }
            else
            {
                工具.打印($"{第一名.名称}{Math.Round(第一名.已行进距离, 2),-15}");
            }

            foreach (马 当前马 in 当前比赛.参赛马)
            {
                // 更新目标速度
                // 目标速度 = 基础目标速度 * 位置意识系数 + 初始并道加成 + 技能调整值 + 坡道调整值 + 变道加成
                当前马.目标速度.基础属性 = 当前马.获取基础目标速度();
                if (当前马.目标速度.基础属性 > 30)
                {
                    当前马.目标速度.基础属性 = 30;
                }

                if (当前马.当前速度.最终属性 <= 当前马.目标速度.最终属性)
                {
                    // 计算加速度
                    // 加速度 = 基础加速度(平常0.0006，上坡0.0004) * 根号(500 * 力量属性) * 跑法阶段系数 * 场地适应性系数 * 距离适应性系数 + 技能调整值 + 起跑冲刺加值
                    当前马.当前加速度.基础属性 = 当前马.获取加速度();
                }
                else
                {
                    // 计算减速度
                    // 序盘-1.2 中盘-0.8 终盘-1.0 放缓-0.5 体力耗尽-1.2
                    if (当前马.当前体力 > 0)
                    {
                        switch (当前马.当前阶段)
                        {
                            case 0:
                                当前马.当前加速度.基础属性 = -1.2;
                                break;
                            case 1:
                                当前马.当前加速度.基础属性 = -0.8;
                                break;
                            case 2:
                                当前马.当前加速度.基础属性 = -1.0;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        当前马.当前加速度.基础属性 = -1.2;
                    }
                }

                // 更新速度
                当前马.当前速度.基础属性 += 当前马.当前加速度.最终属性 * 一帧时间;
                if (当前马.当前速度.基础属性 < 当前马.最低速度)
                {
                    当前马.当前速度.基础属性 = 当前马.最低速度;
                }
            }

            // 阻挡判定
            List<马> 阻挡马群 = new List<马>();
            foreach (马 当前马 in 当前比赛.参赛马)
            {
                var 前方马群 = 检定.获取前方马(当前马, 2);
                阻挡马群.Clear();
                // 随机两次取平均值更稳定
                double 基础突破能力 = 当前马.突破力 * (工具.随机.NextDouble() + 工具.随机.NextDouble()) / 2;
                foreach (var 前方马 in 前方马群)
                {
                    // 视野修正1~2
                    double 视野修正 = 1 + (当前马.视野.最终属性 * 0.05) * (前方马.已行进距离 - 当前马.已行进距离);
                    double 突破能力 = 基础突破能力 * 视野修正;
                    double 阻挡能力 = 前方马.阻挡力 * (工具.随机.NextDouble() + 工具.随机.NextDouble()) / 2;
                    if (阻挡能力 > 突破能力)
                    {
                        阻挡马群.Add(前方马);
                    }
                }
                if (阻挡马群.Any())
                {
                    当前马.被阻挡时间 += 一帧时间;
                    马 阻挡者 = 阻挡马群.Last();
                    当前马.触发事件(事件.游戏事件.被阻挡, new 事件.被阻挡事件 { 阻挡者 = 阻挡者 });

                    // 当前方被阻挡时，马娘的当前速度无法高于
                    // (0.998 + 0.012 * 前后距离 / 2) * 阻挡者的当前速度
                    double 最高速度 = (0.998 + 0.012 * (阻挡者.已行进距离 - 当前马.已行进距离) / 2) * 阻挡者.当前速度.最终属性;
                    if (当前马.当前速度.最终属性 > 最高速度)
                    {
                        当前马.当前速度.修正组.Add(new 属性修正 { 修正值 = 最高速度 - 当前马.当前速度.最终属性, 剩余持续时间 = 一帧时间 });
                    }
                    if (当前马.被阻挡时间 > 1)
                    {
                        工具.打印($"{当前马.名称}【被阻挡】=>{阻挡者.名称}", ConsoleColor.Red);
                    }
                }
                else
                {
                    if (当前马.被阻挡时间 > 一帧时间)
                    {
                        当前马.被阻挡时间 -= 一帧时间;
                    }
                    else
                    {
                        当前马.被阻挡时间 = 0;
                    }
                }
            }

            // 计算行进距离
            foreach (马 当前马 in 当前比赛.参赛马)
            {
                int 上一帧赛段 = 当前马.赛段;
                当前马.已行进距离 += 当前马.当前速度.最终属性 * 一帧时间;
                if (上一帧赛段 < 当前马.赛段)
                {
                    当前马.触发事件(事件.游戏事件.进入新赛段);
                }
            }
            // 刷新名次，方便解说
            检定.更新名次();

            // 因为遍历有执行的先后顺序，所以在行进距离全部更新后先告一段落，做位置上的结算
            // 接下来判断是否冲线，冲线的时候要回退一帧算着差
            List<马> 这一帧冲线的马 = new List<马>();
            foreach (马 当前马 in 当前比赛.参赛马)
            {
                // 更新比赛阶段
                // 判断是否冲线
                if (当前马.已行进距离 >= 当前比赛.总长度)
                {
                    当前马.冲线时间 = 检定.计算冲线具体时间(当前比赛.进行时间, 当前马.当前速度.最终属性, 当前马.已行进距离);
                    当前马.冲线速度 = 当前马.当前速度.最终属性;
                    // foreach中不能直接修改参赛马，所以放在循环结束后统一移除
                    这一帧冲线的马.Add(当前马);
                }

                // 体力消耗
                // 体力每秒消耗：20 * (当前速度 - 赛道基准速度 + 12)的平方 / 12的平方 * 状态调整值 * 场地状况调整值 * 意志系数
                // 状态调整值： 焦躁1.6 放缓模式0.6 下坡模式0.4
                // 意志系数： 序盘和终盘1 终盘和冲刺(1 + 200 / 根号(600 * 意志))
                double 意志系数 = 1;
                if (当前马.当前阶段 > 1)
                {
                    意志系数 += 200 / Math.Sqrt(600 * 当前马.意志属性.最终属性);
                }

                double 将扣体力 = 20 * Math.Pow(当前马.当前速度.最终属性 - 当前比赛.赛道基准速度 + 12, 2) * 意志系数 * 当前马.体力消耗系数.最终属性 / 144 * 一帧时间;
                if (将扣体力 > 当前马.当前体力 && 当前马.当前体力 > 0)
                {
                    当前马.触发事件(事件.游戏事件.体力耗尽);
                }
                当前马.当前体力 -= 将扣体力;
                if (当前马.当前体力 <= 0)
                {
                    当前马.当前体力 = 0;
                }

                // TODO: 检查坡道
                // TODO: 检查弯道是否开始

                // 回显
                //工具.打印($"{当前马.名称}速度{当前马.当前速度.最终属性}，进度{当前马.已行进距离}米");

                // 属性修正更新
                当前马.属性更新();
            }

            // 将冲线的马移出循环
            当前比赛.参赛马.RemoveAll(x => 这一帧冲线的马.Contains(x));
            // 将冲线马按冲线时间由大到小排序
            这一帧冲线的马.OrderByDescending(x => x.冲线时间);
            // 前方的马计算富余时间，后方的马根据富余时间*速度得到前方马冲线时自己的位置，再用总距离-位置得到着差赋予前方的马
            // 如果该马已经是这一帧中最后一个冲线的马，就再看还在跑的马的头名。如果这也没有，那就是末位了，不用算着差
            for (int i = 0; i < 这一帧冲线的马.Count; i++)
            {
                马 当前马 = 这一帧冲线的马[i];
                double 富余时间 = 当前比赛.进行时间 - 当前马.冲线时间;
                if (当前马 != 这一帧冲线的马.Last())
                {
                    马 后方马 = 这一帧冲线的马[i + 1];
                    当前马.着差 = 当前比赛.总长度 - (后方马.已行进距离 - 后方马.当前速度.最终属性 * 富余时间);
                }
                else
                {
                    // 如果后方还有马的话
                    if (当前比赛.参赛马.Count > 0)
                    {
                        // 当前马冲线时，后方所有马的行进距离由大到小排序第一个，就是冲线时最近的后方马
                        马 后方马 = 当前比赛.参赛马.OrderByDescending(x => x.已行进距离 - x.当前速度.最终属性 * 富余时间).FirstOrDefault();
                        当前马.着差 = 当前比赛.总长度 - (后方马.已行进距离 - 后方马.当前速度.最终属性 * 富余时间);
                    }
                }
            }
            // 冲线解说 & 将冲线的马添加到赛后马集合
            if (这一帧冲线的马.Count > 0)
            {
                比赛播报.冲线解说(这一帧冲线的马);
                当前比赛.赛后马.AddRange(这一帧冲线的马);
            }
        }
    }
}
