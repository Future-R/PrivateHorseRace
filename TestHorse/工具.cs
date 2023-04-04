using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    static class 工具
    {
        private static readonly List<int> seed = new List<int>{
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
        151};

        public static Random 随机 = new Random();

        //public float GetPerlinRandom()
        //{
        //    return (float)(Mathf.PerlinNoise(seed.SelectOne(), timer) - 0.5) * 2;
        //}

        public static void 打印(string x) 
        {
            Console.WriteLine(x);
        }

        // 距离越长，变化越大
        public static List<float> 生成赛道海拔(float 距离)
        {
            return 生成柏林噪声(距离 / 100);
            //float 权重 = 距离 / 100;
            //int n = 24;
            //float[] randomNumbers = new float[n];
            //Random rand = new Random();
            //float startValue = (float)rand.NextDouble();
            //float endValue = (float)rand.NextDouble();
            //for (int i = 0; i < n; i++)
            //{
            //    float t = (float)i / (n - 1);
            //    randomNumbers[i] = (startValue * (1 - t) + endValue * t) * 权重;
            //}
            //return randomNumbers.ToList();
        }

        public static List<float> 生成柏林噪声(float 动态, int 长度 = 24)
        {
            List<float> 柏林噪声 = new List<float>(长度);
            float[] 噪声 = 生成白噪声(长度);

            for (int i = 0; i < 长度; i++)
            {
                float 噪声值 = 0;
                float 累积比例 = 0;
                float 比例 = 动态;

                for (int o = 1; o < 5; o++)
                {
                    int 音高 = 长度 >> o;
                    int 样本1 = (i / 音高) * 音高;
                    int 样本2 = (样本1 + 音高) % 长度;
                    float 混合比例 = (float)(i - 样本1) / (float)音高;
                    float 样本混合比例 = 噪声插值(噪声[样本1], 噪声[样本2], 混合比例);
                    噪声值 += 样本混合比例 * 比例;
                    累积比例 += 比例;
                    比例 /= 2;
                }

                柏林噪声.Add((float)Math.Round(噪声值 / 累积比例 * 10, 2));
            }

            return 柏林噪声;
        }

        private static float[] 生成白噪声(int 长度)
        {
            Random 随机数生成器 = new Random();
            float[] 白噪声 = new float[长度];

            for (int i = 0; i < 长度; i++)
            {
                白噪声[i] = (float)随机数生成器.NextDouble() % 1;
            }

            return 白噪声;
        }

        private static float 噪声插值(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }


        public static T 随机取一<T>(this List<T> list)
        {
            int index = 随机.Next(list.Count);
            return list[index];
        }

        public static void 打乱<T>(IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = 随机.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static List<T> 不重复抽取<T>(List<T> 目标列表, int 数量)
        {
            if (目标列表.Count < 数量)
            {
                return 目标列表;
            }

            List<T> 临时列表 = new List<T>(目标列表);
            打乱(临时列表);
            List<T> 返回值 = 临时列表.GetRange(0, 数量);

            return 返回值;
        }

        public static int 获取适性等级(string 字母)
        {
            int 返回值 = 字母.Trim() switch
            {
                "G" => 0,
                "F" => 1,
                "E" => 2,
                "D" => 3,
                "C" => 4,
                "B" => 5,
                "A" => 6,
                "S" => 7,
                _ => throw new ArgumentOutOfRangeException(nameof(字母))
            };
            return 返回值;
        }
    }
}
