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
        public static int 所有马的数量 = 16;
        public static List<马> 所有马 = new List<马>();

        public static int 所有赛道的数量 = 16;
        public static List<比赛> 所有赛道 = new List<比赛>();

        public static 比赛 当前比赛 { get; set; }

        public static void 更新()
        {
            var random = new Random();

            Excel.Application excel = new Excel.Application();
            string 当前目录 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // 读取马的基础属性
            所有马.Clear();
            Excel.Workbook 工作簿 = excel.Workbooks.Open($"{当前目录}/马的基础属性.xlsx");

            Excel.Worksheet 工作表 = 工作簿.Sheets[1];
            for (int i = 0; i < 所有马的数量; i++)
            {
                所有马.Add(new 马());

                // 跳过第一行的表头，所以是0+2=2，从第二行开始度
                所有马[i].Id = (int)工作表.Cells[i + 2, 1].Value;
                所有马[i].名称 = 工作表.Cells[i + 2, 2].Value;
                所有马[i].速度 = (int)Math.Floor(工作表.Cells[i + 2, 3].Value);
                所有马[i].耐力 = (int)Math.Floor(工作表.Cells[i + 2, 4].Value);
                所有马[i].力量 = (int)Math.Floor(工作表.Cells[i + 2, 5].Value);
                所有马[i].意志 = (int)Math.Floor(工作表.Cells[i + 2, 6].Value);
                所有马[i].智力 = (int)Math.Floor(工作表.Cells[i + 2, 7].Value);

            }

            // 读取赛道属性
            工作簿 = excel.Workbooks.Open($"{当前目录}/比赛表.xlsx");
            工作表 = 工作簿.Sheets[1];
            for (int i = 0; i < 所有赛道的数量; i++)
            {
                所有赛道.Add(new 比赛());

                所有赛道[i].ID = (int)工作表.Cells[i + 2, 1].Value;
                所有赛道[i].名称 = 工作表.Cells[i + 2, 2].Value;
                所有赛道[i].是草地 = 工作表.Cells[i + 2, 5].Value == "草地";
                所有赛道[i].总长度 = (float)Math.Floor(工作表.Cells[i + 2, 6].Value);

                // 简单的均匀随机 0良好 1略差 2差 3极差
                所有赛道[i].场地重度 = random.Next(3);
                // TODO：随机天气

            }

            // 关闭读取
            工作簿.Close();
            excel.Quit();
        }
    }
}
