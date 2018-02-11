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
    internal class Generator
    {
        protected const string _alphabet1 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        protected const string _alphabet2 = "_";
        protected const string _alphabet3 = "1234567890";

        protected string _host;
        protected string _login;
        protected string _length;
        protected string _salt;

        internal Generator(string host, string login, string length, string salt)
        {
            this._host = host;
            this._login = login;
            this._length = length;
            this._salt = salt;
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
            CreateRootDirIfNotExists();

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

        private void CreateRootDirIfNotExists()
        {
            string dir = GetRootDir();
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
        }

        protected static string Hash2(string first, string second)
        {
            return Sha1Str(first + Sha1Str(first + second));
        }

        protected static string Md5Str(string input)
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

        protected static string Sha1Str(string input)
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

        virtual public string DetectFileName()
        {
            string relativePath = Md5Str(_host) + "_" + Md5Str(_login) + "_" + Md5Str(_length) + "_"
                + Md5Str(_salt) + "_" + Hash2(_login, _salt) + "_" + Hash2(_host, _length);

            return System.IO.Path.Combine(GetRootDir(), relativePath);
        }

        protected static string GetRootDir()
        {
            string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string myPath = ".mygen";
            string dirName = System.IO.Path.GetDirectoryName(exePath);
            return System.IO.Path.Combine(dirName, myPath);
        }

        private void GenerateNewData(string path)
        {
            System.IO.File.WriteAllBytes(path, GenerateRandomBuffer());
        }

        private static byte[] GenerateRandomBuffer()
        {
            byte[] result = new byte[64 * 1024 * 8];
            RNGCryptoServiceProvider r =
                new RNGCryptoServiceProvider();
            r.GetNonZeroBytes(result);
            return result;
        }

        virtual protected string DecodeBytes(byte[] bytes)
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

    internal class Generator2 : Generator
    {
        internal Generator2(string host, string login, string length, string salt)
            : base(host, login, length, Sha1Str(salt))
        {
        }

        public override string DetectFileName()
        {
            string relativePath = Sha1Str(_host + "_" + _login + "_" + _length + "_" + _salt) 
                + "_" + Hash2(_login, _salt) + "_" + Hash2(_host, _length);
            return System.IO.Path.Combine(GetRootDir(), relativePath);
        }

        override protected string DecodeBytes(byte[] bytes)
        {
            var totalAlphabet = _alphabet1 + _alphabet2 + _alphabet3 + _salt;
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                char c = totalAlphabet[b % totalAlphabet.Length];
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
