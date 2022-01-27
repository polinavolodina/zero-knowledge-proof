using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Security.Cryptography;

namespace methods4
{
    class Program
    {
        public static Random rand = new Random();
        public static BigInteger p, B, NR, r, l;
        public static List<BigInteger> PNR;
        public static int cnt;
        public static bool flag, flag1 = true, flag2 = true;
        public static List <BigInteger> Q = new List<BigInteger>();
        public static void DeleteFiles(bool flag = false)
        {
            foreach (var item in Directory.GetFiles(Environment.CurrentDirectory))
            {
                if (flag == false) 
                    continue;
                if (Path.GetExtension(item) == ".txt")
                    File.Delete(item);
            }
        }
        public static void ReadOptions()
        {
            using (var sr = new StreamReader(File.Open("ElipticCurve.txt", FileMode.Open), Encoding.Default))
            {
                p = BigInteger.Parse(sr.ReadLine());
                B = BigInteger.Parse(sr.ReadLine());
                r = BigInteger.Parse(sr.ReadLine());
                Q.Add(BigInteger.Parse(sr.ReadLine()));
                Q.Add(BigInteger.Parse(sr.ReadLine()));
            }
        }
        private static void Start()
        {
            if (!flag2)
                return;
            Console.WriteLine("Введите: ");
            Console.WriteLine("\t1 - Если хотите сгенерировать эллиптическую кривую");
            Console.WriteLine("\t2 - Если хотите сгенерировать значение l");
            Console.WriteLine("\t3 - Если хотите провести опознавание на основе диалоговых доказательств с нулевым разглашением");
            Console.WriteLine("\t0 - Выход");
        }
        private static void GenerateL()
        {
            Console.Write("Введите значение l: ");
            l = BigInteger.Parse(Console.ReadLine());
            try
            {
                using (var write = new StreamWriter(File.Open("l.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(l);
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }
        public static void Protocol()
        {
            try
            {
                Q.Clear();
                ReadOptions();
                int step = 0;
                try
                {
                    cnt = int.Parse(File.ReadAllText("cnt.txt"));
                }
                catch 
                {
                    cnt = 0;
                }
                while (true)
                {
                    if (!flag1)
                    {
                        flag1 = true;
                        return;
                    }
                    Console.WriteLine("Введите номер шага, который хотите исполнить: ");
                    Console.WriteLine("\t1 - Если хотите выполнить первый шаг алгоритма (сгенерировать случайный показатель k)");
                    Console.WriteLine("\t2 - Если хотите выполнить второй шаг алгоритма (убедиться, что rR = бесконечности, и сгенерировать случайный бит)");
                    Console.WriteLine("\t3 - Если хотите выполнить третий шаг алгоритма (предъявить показатель в зависимости от бита)");
                    Console.WriteLine("\t4 - Если хотите выполнить четвертый шаг алгоритма (итоговую проверку верификатора)");
                    Console.WriteLine("\t0 - Выход");
                    int mode;
                    flag = true;
                    try
                    {
                        mode = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        continue;
                    }
                    //Console.Clear();
                    try
                    {
                        switch (mode)
                        {
                            case 1:
                                {
                                    Step1();
                                    step = 1;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        DeleteFiles();
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 1");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 1 && step != 2)
                                    {
                                        DeleteFiles();
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 1");
                                        flag = false;
                                        break;
                                    }
                                    if (!Step2())
                                    {
                                        DeleteFiles();
                                        Console.WriteLine("Верификатор не убедился на 2 шаге");
                                        File.Delete("l.txt");
                                        flag2 = false;
                                        return;
                                    }
                                    step = 2;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        DeleteFiles();
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 2");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 2 && step != 3)
                                    {
                                        DeleteFiles();
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 2!");
                                        flag = false;
                                        break;
                                    }
                                    Step3();
                                    step = 3;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        DeleteFiles();
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 3");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 3 && step != 4)
                                    {
                                        DeleteFiles();
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 3!");
                                        flag = false;
                                        break;
                                    }
                                    if (step == 3)
                                        cnt++;
                                    try
                                    {
                                        using (var write = new StreamWriter(File.Open("cnt.txt", FileMode.OpenOrCreate)))
                                        {
                                            write.WriteLine(cnt);
                                        }
                                    }
                                    catch (FileNotFoundException)
                                    {
                                        throw new FileNotFoundException();
                                    }
                                    if (Step4())
                                    {
                                        Console.WriteLine("Претендент знает логарифм l с вероятностью " + (1 - (double)1 / Math.Pow(2, cnt)));
                                    }
                                    else
                                    {
                                        Console.WriteLine("Претендент не знает логарифм l");
                                        File.Delete("l.txt");
                                        flag2 = false;
                                        return;
                                    }
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(4);
                                    }
                                    step = 0;
                                    break;
                                }
                            case 0:
                                {
                                    return;
                                }
                            default:
                            {
                                Console.WriteLine("Введена некорректная команда");
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            Console.WriteLine("Выполнен шаг " + mode);

                            Console.ReadLine();
                            //Console.Clear();
                        }
                    }
                    catch (FileNotFoundException er)
                    {
                        Console.WriteLine(er.Message);
                        DeleteFiles();
                        return;
                    }
                }
            }
            catch (FormatException)
            {
                throw;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void Step1()
        {
            try
            {
                l = int.Parse(File.ReadAllText("l.txt"));
            }
            catch
            {
                Console.WriteLine("Ошибка! Не задано значение l");
                Console.ReadLine();
                flag = false;
                flag1 = false;
                return;
            }
            
            BigInteger k = rand.Next(2, (int)r);
            BigInteger _k = (k * l) % r;
            
            var P = QuickSumPoint(Q, l, B, p);
            var R = QuickSumPoint(P, k, B, p);

            try
            {
                using (var write = new StreamWriter(File.Open("P.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(P[0]);
                    write.WriteLine(P[1]);
                }

                using (var write = new StreamWriter(File.Open("R.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(R[0]);
                    write.WriteLine(R[1]);
                }

                using (var write = new StreamWriter(File.Open("k.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(k);
                }

                using (var write = new StreamWriter(File.Open("k_.txt", FileMode.Create)))
                {
                    write.WriteLine(_k);
                }
            }
            catch (FileNotFoundException)
            {
                 throw new FileNotFoundException();
            }
        }
        public static bool Step2()
        {
            try
            {
                var R = new List<BigInteger>();
                using (var sr = new StreamReader(File.Open("R.txt", FileMode.Open), Encoding.Default))
                {
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                }

                var infP = QuickSumPoint(R, r, B, p);
                
                if (infP.Any() || !R.Any())
                {
                    return false;
                }

                int b;
                b = rand.Next(0, 2);
               
                using (var write = new StreamWriter(File.Open("RandomBit.txt", FileMode.Create)))
                {
                    write.WriteLine(b);
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
            return true;
        }
        public static void Step3()
        {
            try
            {
                int b = int.Parse(File.ReadAllText("RandomBit.txt"));
                if (b == 0)
                {
                    File.Copy("k.txt", "TransmissionChannel.txt", true);
                }
                else if (b == 1)
                {
                    File.Copy("k_.txt", "TransmissionChannel.txt", true);
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }
        public static bool Step4()
        {
            try
            {
                int b = int.Parse(File.ReadAllText("RandomBit.txt"));
                var R = new List<BigInteger>();
                using (var sr = new StreamReader(File.Open("R.txt", FileMode.Open), Encoding.Default))
                {
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                }

                var P = new List<BigInteger>();
                using (var sr = new StreamReader(File.Open("P.txt", FileMode.Open), Encoding.Default))
                {
                    P.Add(BigInteger.Parse(sr.ReadLine()));
                    P.Add(BigInteger.Parse(sr.ReadLine()));
                }
                if (b == 0)
                {
                    var k = BigInteger.Parse(File.ReadAllText("TransmissionChannel.txt"));
                    var t = QuickSumPoint(P, k, B, p);
                    return R[0] == t[0] && R[1] == t[1];
                }
                else if (b == 1)
                {
                    var k_ = BigInteger.Parse(File.ReadAllText("TransmissionChannel.txt"));
                    var t = QuickSumPoint(Q, k_, B, p);
                    return R[0] == t[0] && R[1] == t[1];
                }
                return false;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }
        public static void GenerateEC()
        {
            int m = 5;
            BigInteger D = 3;
            bool flag = true;
            Console.Write("Введите длину характеристики поля l = ");
            int l = int.Parse(Console.ReadLine());
            if (l < 7)
            {
                Console.WriteLine("Длина характеристики поля < 7, маленькое значение, введите корректную длину!");
                return;
            }
            cnt = m;
            while (true)
            {
                step1:
                while (true)
                {
                    flag = true;
                    List<BigInteger> razl;
                    FindPrime(l);
                    razl = razlP(D, p);
                    if (razl.Count() == 0)
                        goto step1;
                    PNR = pnr(razl[0], razl[1], p);
                    if (PNR.Count() == 0) 
                        goto step1;
                    if (p != PNR[1])
                    {
                        for (int i = 1; i <= m; i++)
                        {
                            if (BigInteger.ModPow(p, i, PNR[1]) == 0)
                            {
                                cnt += 1;
                                if (cnt > 20)
                                {
                                    Console.WriteLine("Такого p не существует");
                                    Console.ReadKey();
                                    return;
                                }
                                goto step1;
                            }
                        }
                        break;
                    }
                    goto step1;
                }

                BigInteger conut = 2, j = 1;
                List<BigInteger> tochka = new List<BigInteger>();
                int counter = 0;
                step5:
                while (true)
                {
                    if (PNR[0] == PNR[1] * 2)
                        Proverka(ref flag, ref conut, 2);

                    if (PNR[0] == PNR[1] * 3)
                        Proverka(ref flag, ref conut, 3);

                    if (PNR[0] == PNR[1] * 1)
                        Proverka(ref flag, ref conut, 1);

                    if (PNR[0] == PNR[1] * 6)
                        Proverka(ref flag, ref conut, 6);

                    if (!flag) goto step6;
                    tochka = new List<BigInteger>();
                    for (BigInteger i = j; ; i++, j++)
                    {
                        BigInteger z = (B + i * i * i) % p;
                        if (Legendre(z, p) == 1)
                        {
                            if (SqrtMod(z, p) == 0) continue;
                            else
                            {
                                tochka.Add(i);
                                tochka.Add(SqrtMod(z, p));
                            }
                            i++; j++;
                            break;
                        }
                    }
                    BigInteger cnt = PNR[0];
                    List<BigInteger> mulp1p2 = QuickSumPoint(tochka, cnt, B, p);
                    if (mulp1p2.Any())
                    {
                        if (counter++ < 100) goto step5;
                        else goto step1;
                    }
                    break;
                }

                step6: if (!flag) continue;
                NR = PNR[0] / PNR[1];
                List<BigInteger> Q = QuickSumPoint(tochka, NR, B, p);
                if (!Q.Any() || Q[1] == 0)
                    continue;
                Console.WriteLine("p = " + p);
                Console.WriteLine("r = " + PNR[1]);
                Console.WriteLine("B = " + B);
                Console.WriteLine("Q = (" + Q[0] + ", " + Q[1] + ")");
                WriteToFiles(Q);
                break;
            }
        }
        static BigInteger Pow(BigInteger a, BigInteger b)
        {
            BigInteger result = 1;
            for (BigInteger i = 0; i < b; i++)
                result *= a;
        
            return result;
        }
        static bool IsPrime(BigInteger p)
        {
            BigInteger rounds = 30, t = p - 1;
            if (p == 2 || p == 3)
                return true;

            if (p < 2 || p % 2 == 0)
                return false;

            int s = 0;
            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }

            for (int i = 0; i < rounds; i++)
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] _a = new byte[p.ToByteArray().LongLength];
                BigInteger a;
                do
                {
                    rng.GetBytes(_a);
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= p - 2);

                BigInteger x = BigInteger.ModPow(a, t, p);
                if (x == 1 || x == p - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, p);

                    if (x == 1)
                        return false;

                    if (x == p - 1)
                        break;
                }

                if (x != p - 1)
                    return false;
            }
            return true;
        }
        private static void WriteToFiles(List <BigInteger> Q)
        {
            using (StreamWriter f = new StreamWriter(File.Open("ElipticCurve.txt", FileMode.Create), Encoding.Default))
            {
                f.WriteLine(p);
                f.WriteLine(B);
                f.WriteLine(PNR[1]);
                f.WriteLine(Q[0]);
                f.WriteLine(Q[1]);
            }
            return;
        }
        private static void WriteToFiles2(List <BigInteger> Q)
        {
            using (StreamWriter f = new StreamWriter(File.Open("ElipticCurve.txt", FileMode.Create), Encoding.Default))
            {
                f.WriteLine(p);
                f.WriteLine(B);
                f.WriteLine(r);
                f.WriteLine(Q[0]);
                f.WriteLine(Q[1]);
            }
            return;
        }
        static BigInteger randomBinBigInteger(int l)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] Nresult = new byte[BigInteger.Pow(2, l).ToByteArray().LongLength];
            BigInteger result;
            do
            {
                rng.GetBytes(Nresult);
                result = new BigInteger(Nresult);
            } while (result <= BigInteger.Pow(2, (l - 1)) || result >= BigInteger.Pow(2, l));
            return result;
        }
        static BigInteger GenSimple(int l)
        {
            BigInteger result;
            result = randomBinBigInteger(l);
            while (!IsPrime(result))
                result = randomBinBigInteger(l);
            return result;
        }
        private static void FindPrime(int l)
        {
            p = GenSimple(l);
            while ((p % 6) != 1)
                p = GenSimple(l);
        }
        static BigInteger BinaryEuclide(BigInteger a, BigInteger b)
        {
            BigInteger g = 1;
            while (a % 2 == 0 && b % 2 == 0)
            {
                a = a / 2;
                b = b / 2;
                g = 2 * g;
            }
            BigInteger u = a, v = b;
            while (u != 0)
            {
                while (u % 2 == 0) u = u / 2;
                while (v % 2 == 0) v = v / 2;
                if (u >= v) u = u - v;
                else v = v - u;
            }
            return g * v;
        }
        static Tuple<BigInteger,BigInteger> ExtendedEuclide(BigInteger a, BigInteger b)
        {
            BigInteger r0 = a, r1 = b, x0 = 1, x1 = 0, y0 = 0, y1 = 1, x, y, d;
            while (true)
            {
                BigInteger q = r0 / r1, r = r0 % r1;
                if (r == 0) 
                    break;
                else
                {
                    r0 = r1;
                    r1 = r;
                    x = x0 - q * x1;
                    x0 = x1;
                    x1 = x;
                    y = y0 - q * y1;
                    y0 = y1;
                    y1 = y;
                }
            }
            d = r1;
            x = x1;
            y = y1;
            return new Tuple<BigInteger,BigInteger>(x, y);
        }
        public static int Legendre(BigInteger a, BigInteger p)
        {
            if (p < 2) 
                Console.WriteLine("P должно быть больше 2");
            if (a == 0)
                return 0;
            
            if (a == 1)
                return 1;
            
            int result;
            if (a < 0)
            {
                result = Legendre(-a, p);
                BigInteger deg = (p - 1) / 2;
                if (deg % 2 != 0) result = -result;
            }
            else
            {
                if (a % 2 == 0)
                {
                    result = Legendre(a / 2, p);
                    BigInteger deg = (p * p - 1) / 8;
                    if (deg % 2 != 0) result = -result;
                }
                else
                {
                    result = Legendre(p % a, a);
                    BigInteger deg = (a - 1) * ((p - 1) / (4));
                    if (deg % 2 != 0) result = -result;
                }
            }
            return result;
        }
        static BigInteger Jacobi(BigInteger a, BigInteger n)
        {
            if (BinaryEuclide(a, n) != 1)
                return 0;
            
            BigInteger r = 1;
            if (a < 0)
            {
                a = -a;
                if (n % 4 == 3)
                    r = -r;
            }
            while (a != 0)
            {
                BigInteger k = 0;
                while (a % 2 == 0)
                {
                    k++;
                    a = a / 2;
                }
                if (k % 2 != 0)
                {
                    if (n % 8 == 3 || n % 8 == 5)
                        r = -r;
                }
                if (n % 4 == 3 && a % 4 == 3)
                    r = -r;
                
                BigInteger temp = a;
                a = n % a;
                n = temp;
            }
            return r;
        }
        static List<BigInteger> ComparisonSolution(BigInteger a, BigInteger b, BigInteger m)
        {
            List<BigInteger> answer = new List<BigInteger>();
            BigInteger d = BinaryEuclide(a, m);
            if (b % d != 0)
                return answer;
            
            else
            {
                BigInteger a1 = a / d, b1 = b / d, m1 = m / d;
                Tuple<BigInteger, BigInteger> xy = ExtendedEuclide(a1, m1);
                BigInteger x0 = (b1 * xy.Item1) % m1;
                while (x0 < 0) 
                    x0 = x0 + m1;
                answer.Add(x0 % m1);
            }
            return answer;
        }
        static BigInteger ReverseElement(BigInteger a, BigInteger m)
        {
            BigInteger d = BinaryEuclide(a, m);
            if (d != 1)
                return -1;
            
            else
            {
                List<BigInteger> answer = ComparisonSolution(a, 1, m);
                return answer[0];
            }
        }
        static BigInteger SqrtMod(BigInteger a, BigInteger p)
        {
            a += p;
            BigInteger jacobi = Jacobi(a, p);
            if (jacobi == -1)
                return 0;
            
            int N = 0;
            if (jacobi == 1)
            {
                for (int i = 2; i < p; i++)
                {
                    if (Jacobi(i, p) == -1)
                    {
                        N = i;
                        break; 
                    }
                }
            }
            BigInteger h = p - 1;
            int k = 0;
            while (h % 2 == 0)
            {
                k++;
                h = h / 2;
            }
            BigInteger a1 = (int)BigInteger.ModPow(a, (h + 1) / 2, p);
            BigInteger a2 = ReverseElement(a, p);
            BigInteger N1 = BigInteger.ModPow(N, h, p);
            BigInteger N2 = 1;
            BigInteger[] j = new BigInteger[k - 1];
            for (int i = 0; i <= k - 2; i++)
            {
                BigInteger b = (a1 * N2) % p;
                BigInteger c = (a2 * b * b) % p;
                BigInteger pow = Pow(2, k - 2 - i);
                BigInteger d = BigInteger.ModPow(c, pow, p);
                if (d == 1)
                    j[i] = 0;
                
                if (d == p - 1 || d - p == -1)
                    j[i] = 1;
                
                N2 = (N2 * (BigInteger.ModPow(N1, BigInteger.Pow(2, i) * j[i], p))) % p;
            }
            BigInteger answer = (a1 * N2) % p;
            BigInteger answer1 = (-answer + p) % p;
            return answer;
        }
         public static List<BigInteger> SumPoints(List<BigInteger> P1, List<BigInteger> P2, BigInteger A, BigInteger p)
        {
            List <BigInteger> answer = new List <BigInteger>();
            BigInteger x1 = P1[0],y1 = P1[1], x2 = P2[0], y2 = P2[1], alpha;
            if (x1 == x2 && y1 == y2)
            {
                BigInteger numerator = (3 * x1 * x1 + A) % p, denomerator = (2 * y1) % p;
                if (denomerator == 0) 
                    return answer;
                alpha = numerator * ReverseElement(denomerator, p) % p;
            }
            else
            {
                BigInteger numerator = (y2 - y1) % p, denomerator = (x2 - x1) % p;
                denomerator = denomerator >= 0 ? denomerator : denomerator + p;
                if (denomerator == 0) 
                    return answer;
                alpha = numerator * ReverseElement(denomerator, p) % p;
            }
            BigInteger xr = (alpha * alpha - x1 - x2) % p, yr = (-y1 + alpha * (x1 - xr)) % p;
            xr = xr >= 0 ? xr : xr + p;
            yr = yr >= 0 ? yr : yr + p;
            answer.Add(xr);
            answer.Add(yr);
            return answer;
        }
        public static string c10to2(BigInteger i)
        {
            string s = "";
            while (i > 0) { s = (i % 2).ToString() + s; i /= 2; }
            return s == "" ? "0" : s;
        }
         private static int CountBit(BigInteger P)
        {
            int count = 0;
            while (P > 0)
            {
                P >>= 1;
                count++;
            }
            return count;
        }
        private static List <BigInteger> QuickSumPoint(List<BigInteger> P, BigInteger cnt, BigInteger A, BigInteger p)
        {
            BigInteger lengthBase = CountBit(cnt);
            string c = c10to2(cnt);
            char[] b = c.ToCharArray();
            Array.Reverse(b);
            c = new string(b);
            List<List<BigInteger>> basePoints = new List<List<BigInteger>>();
            List<List<BigInteger>> result = new List<List<BigInteger>>();
            basePoints.Add(P);
            int k = 0;
            for (BigInteger i = 1; i <= cnt; i *= 2)
            {
                if (!basePoints[k].Any())
                {
                    break;
                }
                else
                {
                    basePoints.Add(SumPoints(basePoints[k], basePoints[k], A, p));
                }

                if (c[k] == '1')
                {
                    result.Add(basePoints[k]);
                }
                k++;
            }

            if (!result.Any())
                return new List <BigInteger>();
            List <BigInteger> resultPoint = result[0];

            for (int i = 1; i < result.Count; i++)
            {
                if (!resultPoint.Any())
                    resultPoint = result[i];
                else
                    resultPoint = SumPoints(resultPoint, result[i], A, p);
            }

            return resultPoint;
        }
        private static List<BigInteger> pnr(BigInteger c, BigInteger d, BigInteger p)
        {
            List<BigInteger> T = new List<BigInteger>();
            T.Add(c + 3 * d);
            T.Add(c - 3 * d);
            T.Add(2 * c);
            T.Add(c * (-2));
            T.Add(3 * d - c);
            T.Add(-c - 3 * d);
            for (int i = 0; i < T.Count; i++)
            {
                T[i] += (1 + p);
                if ((T[i] % 2).Equals(0) && IsPrime((T[i] / 2)))
                    return new List<BigInteger>() { T[i], T[i] / 2 };
                else if ((T[i] % 3).Equals(0) && IsPrime((T[i] / 3)))
                    return new List<BigInteger>() { T[i], T[i] / 3 };
                else if ((T[i] % 6).Equals(0) && IsPrime((T[i] / 6)))
                    return new List<BigInteger>() { T[i], T[i] / 6 };
                else if ((T[i] % 1).Equals(0) && IsPrime((T[i] / 1)))
                    return new List<BigInteger>() { T[i], T[i] / 1 };
            }
            return new List<BigInteger>();
        }
        public static BigInteger gcd(BigInteger a, BigInteger b)
        {
            if (b == 0)
                return a;
            return gcd(b, a % b);
        }
        public static bool isKvadrVich(BigInteger B, BigInteger p)
        {
            if (Legendre(B, p) == 1)
                return true;
            else return false;
        }
        public static bool isKubVich(BigInteger B, BigInteger p)
        {
            if (BigInteger.ModPow(B, (p - 1) / gcd(p - 1, 3), p) == 1)
                return true;
            else return false;
        }
        private static void Proverka(ref bool t, ref BigInteger conut, int num)
        {
            if (PNR[0] == PNR[1] * num)
            {
                for (BigInteger i = conut; ; i++)
                {
                    if (i > 1000000)
                    {
                        t = false;
                        break;
                    }
                    bool f = isKvadrVich((i + 1) % p, PNR[0]), h = isKubVich((i + 1) % p, PNR[0]);
                    if (num == 6 && f && h || num == 1 && !f && !h || num == 2 && !f && h || num == 3 && f && !h)
                    {
                        B = i;
                        conut = B + 1;
                        break;
                    }
                }
            }
        }
        public static List<BigInteger> razlP(BigInteger D, BigInteger p)
        {
            if (Legendre(-D, p) == -1) return new List<BigInteger>();
            BigInteger R = SqrtMod(-D, p);
            int i = 0;
            List<BigInteger> U = new List<BigInteger>(), M = new List<BigInteger>();
            U.Add(R);
            M.Add(p);
            do
            {
                M.Add((U[i] * U[i] + D) / M[i]);
                U.Add(BigInteger.Min(U[i] % M[i + 1], M[i + 1] - U[i] % M[i + 1]));
                i++;
            } while (M[i] != 1);
            i--;
            List<BigInteger> a = new List<BigInteger>(), b = new List<BigInteger>();
            for (int j = 0; j <= i; j++)
            {
                a.Add(0);
                b.Add(0);
            }
            a[i] = U[i];
            b[i] = 1;
            while (i != 0)
            {
                BigInteger znam = a[i] * a[i] + D * b[i] * b[i];
                if ((U[i - 1] * a[i] + D * b[i]) % znam == 0)
                    a[i - 1] = (U[i - 1] * a[i] + D * b[i]) / znam;
                else
                    a[i - 1] = (-U[i - 1] * a[i] + D * b[i]) / znam;

                if ((-a[i] + U[i - 1] * b[i]) % znam == 0)
                    b[i - 1] = (-a[i] + U[i - 1] * b[i]) / znam;
                else
                    b[i - 1] = (-a[i] - U[i - 1] * b[i]) / znam;
                i--;
            }
            List<BigInteger> res = new List<BigInteger>();
            res.Add(a[0]);
            res.Add(b[0]);
            return res;
        }
        static void Main(string[] args)
        {
            while (true)
            {
                step11:
                Start();
                int mode;
                try
                {
                    mode = int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    continue;
                }
                
                try
                {
                    switch (mode)
                    {
                        case 1:
                            {
                                File.Delete("cnt.txt");
                                cnt = 0;
                                try
                                {
                                    l = int.Parse(File.ReadAllText("l.txt"));
                                    DeleteFiles();
                                    try
                                    {
                                        using (var write = new StreamWriter(File.Open("l.txt", FileMode.OpenOrCreate)))
                                        {
                                            write.WriteLine(l);
                                        }
                                    }
                                    catch (FileNotFoundException)
                                    {
                                        throw new FileNotFoundException();
                                    }
                                    GenerateEC();
                                    Console.ReadLine();
                                    break;
                                }
                                catch
                                {
                                    GenerateEC();
                                    Console.ReadLine();
                                    break;
                                }
                                
                            }
                        case 2:
                            {
                                File.Delete("cnt.txt");
                                cnt = 0;
                                try
                                {
                                    Q.Clear();
                                    ReadOptions();
                                    DeleteFiles();
                                    WriteToFiles2(Q);
                                    GenerateL();
                                    Console.ReadLine();
                                    break;
                                }
                                catch
                                {
                                    GenerateL();
                                    Console.ReadLine();
                                    break;
                                }
                            }
                        case 3:
                            {
                                Protocol();
                                if(!flag2)
                                    return;
                                Console.ReadLine();
                                break;
                            }
                        case 0:
                            {
                                return;
                            }
                        default:
                            break;
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Ошибка! Нет параметров эллиптической кривой!");
                    Console.ReadLine();
                    goto step11;
                }
            }
        }
    }
}