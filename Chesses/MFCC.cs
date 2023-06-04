using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Chesses
{


    [StructLayout(LayoutKind.Sequential)]
    internal class WavHeader
    {
        public UInt32 ChunkId;// Содержит символы "RIFF" в ASCII кодировке
        public UInt32 ChunkSize;// Это оставшийся размер цепочки, начиная с этой позиции.
        public UInt32 Format;// Содержит символы "WAVE"
        public UInt32 Subchunk1Id;// Содержит символы "fmt "
        public UInt32 Subchunk1Size;// Это оставшийся размер подцепочки, начиная с этой позиции.
        public UInt16 AudioFormat;// Значения, отличающиеся от 1, обозначают некоторый формат сжатия.
        public UInt16 NumChannels;// Количество каналов. 
        public UInt32 SampleRate;// Частота дискретизации.
        public UInt32 ByteRate;// sampleRate * numChannels * bitsPerSample/8
        public UInt16 BlockAlign;// numChannels * bitsPerSample/8. Количество байт для одного сэмпла, включая все каналы.
        public UInt16 BitsPerSample;// Так называемая "глубиная" или точность звучания. 8 бит, 16 бит и т.д.
        public UInt32 Subchunk2Id;// Подцепочка "data" содержит аудио-данные и их размер.
        public UInt32 Subchunk2Size;// Количество байт в области данных. numSamples * numChannels * bitsPerSample/8
    }
    


    class MFCC
    {
        static string outputFilename = "all/hods/", wav = ".wav";
        private static List<List<double>> Changing(double[] x, uint sample_rate)
        {
            int Len = 1024;
            const int step = 441;
            const int M = 26; //количество mel фильтров
            const int mel = 12;
            List<List<double>> list = new List<List<double>>();
            double[,] H_m = MelFiltres(M, sample_rate, Len);
            for (int i = 0; i < x.Length; i += step)
            {
                double[] dop = new double[Len];
                for (int j = 0; j < Len; j++)
                {
                    if (i + j < x.Length) dop[j] = x[i + j];
                    else dop[j] = 0;
                }
                for (int j = 0; j < Len; j++)
                {
                    dop[j] = (0.53836 - 0.46164 * Math.Cos(2 * Math.PI * j / (Len - 1))) *dop[j];
                }
                Complex[] dop1 = fft(DoubleToComplex(dop, Len));
                double[] S = new double[M];
                for (int m = 0; m < M; m++)
                {
                    S[m] = 0;
                    for (int j = 0; j < Len; j++)
                    {
                        S[m] += Math.Pow(dop1[j].Magnitude, 2) * H_m[m, j]/Len;
                    }
                    S[m] = Math.Log(S[m]);
                }
                List<double> C = new List<double>();
                for (int l = 0; l < M; l++)
                {
                    double a = 0;
                    for (int m = 0; m < M; m++)
                    {
                        a += S[m] * Math.Cos(Math.PI * l * (m + 1 / 2) / M);
                    }
                    C.Add(a);
                }
                List<double> D = new List<double>();
                for (int l = 0; l < mel; l++)
                {
                    D.Add(C[l]);
                }
                list.Add(D);
            }
            return list;

        }
        private static double[,] MelFiltres(int M,uint sampleRate, int Framesize = 1024,int m=26)
        {
            double[] h = new double[M+2];
            double p = 8000;
            for (int i = 0; i < M + 2; i++)
            {
                h[i] = 1127.01048 * Math.Log(1 + (double)p*i / (700*(M+2)));
            }
            double[] f = new double[h.Length];
            for (int i = 0; i < M + 2; i++)
            {
                f[i] = Math.Floor((700*(Math.Pow(Math.E,h[i]/ 1127.01048) -1)*(Framesize+1))/sampleRate); 
            }
            double[,] H_m = new double[M, Framesize];
            for (int i = 1; i < M + 1; i++)
                {
                for (int k = 0; k < Framesize; k++)
                {
                    if (k < f[i - 1]) H_m[i-1, k] = 0;
                    else if (k < f[i]) H_m[i-1, k] = (k - f[i - 1]) / (f[i] - f[i - 1]);
                    else if (k <= f[i + 1]) H_m[i-1, k] = (f[i + 1] - k) / (f[i + 1] - f[i]);
                    else H_m[i-1, k] = 0;
                } 
            }
            return H_m;
        }
        private static Complex w(int k, int N)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }
        public static Complex[] fft(Complex[] x)
        {
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = fft(x_even);
                Complex[] X_odd = fft(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + w(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                }
            }
            return X;
        }
        public static Complex[] DoubleToComplex(double[] x, long N)
        {
            Complex[] ar = new Complex[N];
            for (long i = 0; i < N; i++)
            {
                ar[i] = new Complex(x[i], 0);
            }
            return ar;

        }
        private static void RMS_gate(double[] wav_PCM)
        {
            int k = 0;
            double RMS = 0;

            for (int j = 0; j < wav_PCM.Length; j++)
            {
                if (k < 100)
                {
                    RMS += Math.Pow((wav_PCM[j]), 2);
                    k++;
                }
                else
                {
                    if (Math.Sqrt(RMS / 100) < 0.005)
                        for (int i = j - 100; i <= j; i++) wav_PCM[i] = 0;
                    k = 0; RMS = 0;
                }
            }
        }
        private static void Normalize(double[] wav_PCM)
        {
            double[] abs_wav_buf = new double[wav_PCM.Length];
            for (int i = 0; i < wav_PCM.Length; i++)
                if (wav_PCM[i] < 0) abs_wav_buf[i] = -wav_PCM[i];   
                else abs_wav_buf[i] = wav_PCM[i];                    
            double max = abs_wav_buf.Max();
            double k = 1f / max;            

            for (int i = 0; i < wav_PCM.Length; i++)  
            {
                wav_PCM[i] = wav_PCM[i] * k;
            }
        }
        public static void Dop_main()
        {

            StreamWriter sw = new StreamWriter("all/neural.txt");
            string[] all_dir = Directory.GetDirectories("all/sounds");
            foreach (string dir in all_dir)
            {
                string[] all_file = Directory.GetFiles(dir);
                foreach (string file in all_file)
                {
                    var header = new WavHeader();
                    var headerSize = Marshal.SizeOf(header);
                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    var buffer = new byte[headerSize];
                    fileStream.Read(buffer, 0, headerSize);
                    var headerPtr = Marshal.AllocHGlobal(headerSize);
                    Marshal.Copy(buffer, 0, headerPtr, headerSize);
                    Marshal.PtrToStructure(headerPtr, header);
                    buffer = new byte[fileStream.Length - headerSize - 2];
                    fileStream.Read(buffer, 0, buffer.Length);
                    var N = buffer.Length * 8 / (header.NumChannels * header.BitsPerSample);
                    UInt16[] data = new UInt16[N];
                    for (int i = 0; i < N; i++)
                    {
                        data[i] = BitConverter.ToUInt16(buffer, 2 * i);
                    }  
                    double[] x = new double[N / 2];
                    for (int i = 0; i < N / 2; i++)
                    {
                        x[i] = (data[2 * i] + data[2 * i + 1]) / 2;
                    }   
                    RMS_gate(x);
                    Normalize(x);
                    List<List<double>> datas = Changing(x, header.SampleRate);
                    string s = dir.Substring(dir.Length - 2,1)=="\\"?dir.Substring(dir.Length-1) : dir.Substring(dir.Length - 2);
                    foreach (List<double> datas_i in datas)
                    {
                        foreach (double datas_i_j in datas_i)
                        {
                            if (Math.Abs(datas_i_j) < 1000)
                            {
                                s += "\t" + datas_i_j.ToString();
                            }
                            else
                            {
                                s += "\t0";
                            }
                        }
                    }
                    sw.WriteLine(s);
                    Marshal.FreeHGlobal(headerPtr);
                }
            }
            sw.Close();
        }

        public static void Handling(int file_index)
        {
            StreamWriter sw = new StreamWriter("all/hod.txt");
            var header = new WavHeader();
            var headerSize = Marshal.SizeOf(header);
            var fileStream = new FileStream(outputFilename + file_index.ToString() + wav, FileMode.Open, FileAccess.Read);
            var buffer = new byte[headerSize];
            fileStream.Read(buffer, 0, headerSize);
            var headerPtr = Marshal.AllocHGlobal(headerSize);
            Marshal.Copy(buffer, 0, headerPtr, headerSize);
            Marshal.PtrToStructure(headerPtr, header);
            buffer = new byte[fileStream.Length - headerSize-2];
            fileStream.Read(buffer, 0, buffer.Length);
            var N = buffer.Length * 8 / (header.NumChannels * header.BitsPerSample); 
            UInt16[] data = new UInt16[N];
            for (int i = 0; i < N; i++)
            {
                data[i] = BitConverter.ToUInt16(buffer, (2 * i));
            }  
            double[] x = new double[N / 2];
            for (int i = 0; i < N / 2; i++)
            {
                x[i] = (data[2 * i] + data[2 * i + 1]) / 2;
            }   
            N /=2;
            RMS_gate(x);
            for (int i = 0; i < 4; i++)
            {
                double[] x1 = new double[N / 4];
                for (int j = 0; j < N / 4; j++)
                {
                    x1[j] = x[i * N / 4 + j];
                }
                Normalize(x1);
                List<List<double>> datas = Changing(x1, header.SampleRate);
                string s = "1";
                foreach (List<double> datas_i in datas)
                {
                    foreach (double datas_i_j in datas_i)
                    {
                        if (Math.Abs(datas_i_j) < 1000)
                        {
                            s += "\t" + datas_i_j.ToString();
                        }
                        else
                        {
                            s += "\t0";
                        }
                    }
                }
                sw.WriteLine(s);
                
            }
            Marshal.FreeHGlobal(headerPtr);
            sw.Close();
        }

        public static void Handling1(int file_index)
        {
            StreamWriter sw = new StreamWriter("all/hod.txt");
            var header = new WavHeader();
            var headerSize = Marshal.SizeOf(header);
            var fileStream = new FileStream(outputFilename + file_index.ToString() + wav, FileMode.Open, FileAccess.Read);
            var buffer = new byte[headerSize];
            fileStream.Read(buffer, 0, headerSize);
            var headerPtr = Marshal.AllocHGlobal(headerSize);
            Marshal.Copy(buffer, 0, headerPtr, headerSize);
            Marshal.PtrToStructure(headerPtr, header);
            buffer = new byte[fileStream.Length - headerSize - 2];
            fileStream.Read(buffer, 0, buffer.Length);
            var N = buffer.Length * 8 / (header.NumChannels * header.BitsPerSample);
            UInt16[] data = new UInt16[N];
            for (int i = 0; i < N; i++)
            {
                data[i] = BitConverter.ToUInt16(buffer, (2 * i));
            }  
            double[] x = new double[N / 2];
            for (int i = 0; i < N / 2; i++)
            {
                x[i] = (data[2 * i] + data[2 * i + 1]) / 2;
            }  
            RMS_gate(x);
            Normalize(x);
            List<List<double>> datas = Changing(x, header.SampleRate);
            string s = "1";
            foreach (List<double> datas_i in datas)
            {
                foreach (double datas_i_j in datas_i)
                {
                    if (Math.Abs(datas_i_j) < 1000)
                    {
                        s += "\t" + datas_i_j.ToString();
                    }
                    else
                    {
                        s += "\t0";
                    }
                }
            }
            sw.WriteLine(s);
            Marshal.FreeHGlobal(headerPtr);
            sw.Close();
        }
    }
}
