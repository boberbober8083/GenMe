using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MessageBox = System.Windows.MessageBox;
using MemoryStream = System.IO.MemoryStream;

namespace GenMe
{
    internal sealed class Generator
    {
        private const string _alphabet1 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _alphabet2 = "_";
        private const string _alphabet3 = "1234567890";

        private string _host;
        private string _login;
        private string _length;
        private string _p;

        internal Generator(string host, string login, string length, string p)
        {
            this._host = host;
            this._login = login;
            this._length = length;
            this._p = p;
        }

        internal string Regenerate()
        {
            string filePath = DetectFileName();
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            GenerateNewData(filePath);
            return Generate(filePath);
        }

        internal string Generate()
        {
            return Generate(DetectFileName());
        }

        private string Generate(string path)
        {
            string dirName = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dirName))
            {
                MessageBox.Show(String.Format("Invalid file directory name {0}", path));
                return String.Empty;
            }
            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("Invalid file name!");
                return String.Empty;
            }
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            int l = Convert.ToInt32(_length);
            string r = DecodeBytes(bytes);
            return Extract(r, l);
        }

        private static string Hash2(string first, string second)
        {
            return Sha1Str(first + Sha1Str(first + second));
        }

        private static string Md5Str(string input)
        {
            byte[] hash;
            using (var md5 = new MD5CryptoServiceProvider())
            {
                hash = md5.ComputeHash(Encoding.Unicode.GetBytes(input));
            }
            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        private static string Sha1Str(string input)
        {
            byte[] hash;
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                hash = sha1.ComputeHash(Encoding.Unicode.GetBytes(input));
            }
            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        private string DetectFileName()
        {
            string relativePath = Md5Str(_host) + "_" + Md5Str(_login) + "_" + Md5Str(_length) + "_"
                + Md5Str(_p) + "_" + Hash2(_login, _p) + "_" + Hash2(_host, _length);

            return System.IO.Path.Combine(GetRoot(), relativePath);
        }

        private static string GetRoot()
        {
            string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string myPath = ".mygen";
            string dirName = System.IO.Path.GetDirectoryName(exePath);
            return System.IO.Path.Combine(dirName, myPath);
        }

        private void GenerateNewData(string path)
        {
            System.IO.File.WriteAllBytes(path, GenerateLongRandomString());
        }

        byte[] GenerateLongRandomString()
        {
            long dt = System.DateTime.UtcNow.ToFileTimeUtc();
            int t1 = (int)(dt & uint.MaxValue);
            int t2 = (int)(dt >> 32);

            MemoryStream stream = new MemoryStream();

            Random rnd1 = new Random(t1);
            Random rnd2 = new Random(t2);
            for (int i = 0; i < 64 * 1024; ++i)
            {
                byte[] byte1 = new byte[4];
                rnd1.NextBytes(byte1);
                foreach (var b in byte1)
                {
                    stream.WriteByte(b);
                }

                byte[] byte2 = new byte[4];
                rnd2.NextBytes(byte2);
                foreach (var b in byte2)
                {
                    stream.WriteByte(b);
                }
            }

            stream.Flush();
            return stream.ToArray();
        }



        private static string DecodeBytes(byte[] bytes)
        {
            var totalAlphabet = _alphabet1 + _alphabet2 + _alphabet3;
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                char c = totalAlphabet[b % totalAlphabet.Length];
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static string Extract(string r, int l)
        {
            int p = 0;
            string[] candidate = new string[l + 1];
            for (int i = 0; i < r.Length; ++i)
            {
                string f = r.Substring(i, l);
                if (CheckForSymbols(f, _alphabet2) && CheckForSymbols(f, _alphabet3))
                {
                    candidate[p] = f;
                    ++p;
                }
                if (p == candidate.Length)
                {
                    return candidate.LastOrDefault();
                }
            }

            return candidate.LastOrDefault();
        }

        private static bool CheckForSymbols(string s, string alphabet)
        {
            foreach (char s1 in s)
            {
                if (alphabet.IndexOf(s1) != -1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
