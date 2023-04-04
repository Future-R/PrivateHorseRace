using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace HorseRace
{
    public static class 数据表
    {
        public static float 一帧时间;
        public static int 所有马的数量;
        public static int 所有赛道的数量;
        public static int 保底属性;
        public static float 赛道属性加成倍率;

        public static List<马> 所有马 { get; set; }
        public static List<比赛> 所有赛道 { get; set; }

        public static List<场地状况调整> 场地状况调整配置表 { get; set; }

        public static List<float> 跑法智力修正 { get; set; }
        public static List<float> 距离速度修正 { get; set; }
        public static List<float> 场地加速修正 { get; set; }
        public static List<float> 距离加速修正 { get; set; }

        // 0绝佳4极差
        public static List<干劲配置> 干劲配置表 { get; set; }

        // 根据ID索引配置，0逃1先2中3追4大逃
        public static List<跑法配置> 跑法配置表 { get; set; }

        public class 干劲配置
        {
            public string 名称;
            public float 训练系数;
            public float 基础属性系数;
        }

        public class 属性
        {
            public float 基础属性;
            public float 最终属性
            {
                // 目前每次get都要重新计算，之后优化为修正组发生变化时发送事件通知才计算，性能会好很多
                get
                {
                    float 返回值 = 基础属性;
                    修正组.Sort();
                    foreach (var 修正 in 修正组)
                    {
                        if (修正.是加算)
                        {
                            返回值 += 修正.修正值;
                        }
                        else
                        {
                            返回值 *= 修正.修正值;
                        }
                    }
                    return 返回值;
                }
            }
            public List<属性修正> 修正组 = new List<属性修正>();
        }

        public class 属性修正 : IComparable
        {
            public List<string> 标签组;
            // 乘算默认100 加算默认200
            public int 优先级;
            // 加算还是乘算
            public bool 是加算;
            public float 修正值;
            // 约定-1为永续效果
            public float 剩余持续时间;

            public int CompareTo(object obj)
            {
                属性修正 对比值 = obj as 属性修正;
                if (对比值 == null)
                    return 1;
                else
                    return 对比值.优先级.CompareTo(this.优先级);
            }
        }

        public class 跑法配置
        {
            public string 跑法 { get; set; }
            public float 初始并道速度 { get; set; }
            public float 序盘目标速度 { get; set; }
            public float 中盘目标速度 { get; set; }
            public float 终盘和冲刺目标速度 { get; set; }
            public float 序盘加速度 { get; set; }
            public float 中盘加速度 { get; set; }
            public float 终盘和冲刺加速度 { get; set; }
            public float 体力系数 { get; set; }
            public float 位置意识下限 { get; set; }
            public float 位置意识上限 { get; set; }

        }

        public class 场地状况调整
        {
            public string 状况 { get; set; }
            public int 草地速度调整 { get; set; }
            public int 泥地速度调整 { get; set; }
            public int 草地力量调整 { get; set; }
            public int 泥地力量调整 { get; set; }
            public float 草地体力消耗 { get; set; }
            public float 泥地体力消耗 { get; set; }
        }

        public static 比赛 当前比赛 { get; set; }

        public static void 更新()
        {
            工具.打印("正在打开新世界");
            var random = new Random();

            Excel.Application excel = new Excel.Application();
            string 当前目录 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // 读取杂项表
            Excel.Workbook 工作簿 = excel.Workbooks.Open($"{当前目录}/杂项表.xlsx");
            Excel.Worksheet 工作表 = 工作簿.Sheets[1];

            一帧时间 = (float)工作表.Cells[2, 3].Value;
            所有马的数量 = (int)工作表.Cells[3, 3].Value;
            所有赛道的数量 = (int)工作表.Cells[4, 3].Value;
            保底属性 = (int)工作表.Cells[5, 3].Value;
            赛道属性加成倍率 = (float)工作表.Cells[6, 3].Value;

            // 读取马的基础属性
            工具.打印("正在浏览赛马");
            所有马 = new List<马>();
            工作簿 = excel.Workbooks.Open($"{当前目录}/马的基础属性.xlsx");
            工作表 = 工作簿.Sheets[1];
            for (int i = 0; i < 所有马的数量; i++)
            {
                所有马.Add(new 马());

                // 跳过第一行的表头，所以是0+2=2，从第二行开始读
                所有马[i].Id = (int)工作表.Cells[i + 2, 1].Value;
                所有马[i].名称 = 工作表.Cells[i + 2, 2].Value;
                所有马[i].基础速度 = (int)Math.Floor(工作表.Cells[i + 2, 3].Value);
                所有马[i].基础耐力 = (int)Math.Floor(工作表.Cells[i + 2, 4].Value);
                所有马[i].基础力量 = (int)Math.Floor(工作表.Cells[i + 2, 5].Value);
                所有马[i].基础意志 = (int)Math.Floor(工作表.Cells[i + 2, 6].Value);
                所有马[i].基础智力 = (int)Math.Floor(工作表.Cells[i + 2, 7].Value);
                // 这里跳了5列，成长率暂时用不到，7+5+1=13
                // 13~14场地适应 15~18距离适应 19~22跑法适应
                所有马[i].草地适性 = 工具.获取适性等级(工作表.Cells[i + 2, 13].Value);
                所有马[i].泥地适性 = 工具.获取适性等级(工作表.Cells[i + 2, 14].Value);
                
                所有马[i].距离适性 = new List<int>();
                for (int j = 15; j < 19; j++)
                {
                    所有马[i].距离适性.Add(工具.获取适性等级(工作表.Cells[i + 2, j].Value));
                }

                所有马[i].跑法适性 = new List<int>();
                for (int j = 19; j < 23; j++)
                {
                    所有马[i].跑法适性.Add(工具.获取适性等级(工作表.Cells[i + 2, j].Value));
                }
            }

            // 读取赛道属性
            工具.打印("正在随机赛道");
            所有赛道 = new List<比赛>();
            工作簿 = excel.Workbooks.Open($"{当前目录}/比赛表.xlsx");
            工作表 = 工作簿.Sheets[1];
            for (int i = 0; i < 所有赛道的数量; i++)
            {
                所有赛道.Add(new 比赛());

                所有赛道[i].ID = (int)工作表.Cells[i + 2, 1].Value;
                所有赛道[i].名称 = 工作表.Cells[i + 2, 2].Value;
                所有赛道[i].是草地 = 工作表.Cells[i + 2, 5].Value == "草地";
                所有赛道[i].总长度 = (float)Math.Floor(工作表.Cells[i + 2, 6].Value);

                // TODO：随机天气

                // 简单的均匀随机 0良好 1略差 2差 3极差
                所有赛道[i].场地状况 = random.Next(3);


            }

            // 读取场地状况配置表
            工具.打印("正在确认场地状况");
            场地状况调整配置表 = new List<场地状况调整>();
            工作簿 = excel.Workbooks.Open($"{当前目录}/场地状况.xlsx");
            工作表 = 工作簿.Sheets[1];
            // 跳过表头从第2行开始所以i=2，因为只有4列所以4+2=6
            for (int i = 2; i < 6; i++)
            {
                场地状况调整 条目 = new 场地状况调整
                {
                    状况 = 工作表.Cells[i, 1].Value,
                    草地速度调整 = (int)工作表.Cells[i, 2].Value,
                    泥地速度调整 = (int)工作表.Cells[i, 3].Value,
                    草地力量调整 = (int)工作表.Cells[i, 4].Value,
                    泥地力量调整 = (int)工作表.Cells[i, 5].Value,
                    草地体力消耗 = (float)工作表.Cells[i, 6].Value,
                    泥地体力消耗 = (float)工作表.Cells[i, 7].Value
                };
                场地状况调整配置表.Add(条目);
            }

            // 读取跑法配置表
            工具.打印("正在检查设施");
            跑法配置表 = new List<跑法配置>();
            工作簿 = excel.Workbooks.Open($"{当前目录}/跑法.xlsx");
            工作表 = 工作簿.Sheets[1];
            for (int i = 2; i < 7; i++)
            {
                跑法配置 条目 = new 跑法配置
                {
                    跑法 = 工作表.Cells[i, 1].Value,
                    初始并道速度 = (float)工作表.Cells[i, 2].Value,
                    序盘目标速度 = (float)工作表.Cells[i, 3].Value,
                    中盘目标速度 = (float)工作表.Cells[i, 4].Value,
                    终盘和冲刺目标速度 = (float)工作表.Cells[i, 5].Value,
                    序盘加速度 = (float)工作表.Cells[i, 6].Value,
                    中盘加速度 = (float)工作表.Cells[i, 7].Value,
                    终盘和冲刺加速度 = (float)工作表.Cells[i, 8].Value,
                    体力系数 = (float)工作表.Cells[i, 9].Value,
                    位置意识下限 = (float)工作表.Cells[i, 10].Value,
                    位置意识上限 = (float)工作表.Cells[i, 11].Value
                };
                跑法配置表.Add(条目);
            }

            // 读取干劲配置表
            干劲配置表 = new List<干劲配置>();
            工作簿 = excel.Workbooks.Open($"{当前目录}/干劲.xlsx");
            工作表 = 工作簿.Sheets[1];
            for (int i = 2; i < 7; i++)
            {
                干劲配置 条目 = new 干劲配置
                {
                    名称 = 工作表.Cells[i, 1].Value,
                    训练系数 = (float)工作表.Cells[i, 2].Value,
                    基础属性系数 = (float)工作表.Cells[i, 3].Value
                };
                干劲配置表.Add(条目);
            }

            // 读取适应配置表
            跑法智力修正 = new List<float>();
            距离速度修正 = new List<float>();
            场地加速修正 = new List<float>();
            距离加速修正 = new List<float>();
            
            工作簿 = excel.Workbooks.Open($"{当前目录}/适应性.xlsx");
            工作表 = 工作簿.Sheets[1];
            for (int i = 2; i < 10; i++)
            {
                跑法智力修正.Add((float)工作表.Cells[i, 2].Value);
                距离速度修正.Add((float)工作表.Cells[i, 3].Value);
                场地加速修正.Add((float)工作表.Cells[i, 4].Value);
                距离加速修正.Add((float)工作表.Cells[i, 5].Value);
            }

            // 关闭读取
            工作簿.Close();
            excel.Quit();
            工具.打印("读取完毕！");
        }
    }
}
