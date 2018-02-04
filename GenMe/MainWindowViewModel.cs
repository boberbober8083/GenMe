using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Controls;

namespace GenMe
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _host;
        private string _login;
        private string _length;
        private string _r;
        private bool _useLegacyGenerator = false;

        internal MainWindowViewModel()
        {
            GenerateCommand = new CommandWithParamImpl(Generate);
            RegenerateCommand = new CommandWithParamImpl(Regenerate);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand GenerateCommand { get; set; }
        public ICommand RegenerateCommand { get; set; }

        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
                OnPropertyChanged("Host");
            }
        }

        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                _login = value;
                OnPropertyChanged("Login");
            }
        }

        public string Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
                OnPropertyChanged("Length");
            }
        }

        public bool UseLegacyGenerator
        {
            get { return _useLegacyGenerator; }
            set
            {
                _useLegacyGenerator = value;
                OnPropertyChanged("UseLegacyGenerator");
            }
        }

        public string R
        {
            get
            {
                return _r;
            }
            set
            {
                _r = value;
                OnPropertyChanged("R");
            }
        }

        private void Generate(object param)
        {
            if (!CheckInput())
            {
                MessageBox.Show("Incorrect input!");
                return;
            }

            string salt = ExtractSalt(param);
            var generator = this.UseLegacyGenerator 
                ? new Generator(_host, _login, _length, salt) 
                : new Generator2(_host, _login, _length, salt);
            R = generator.Generate();
        }

        private void Regenerate(object param)
        {
            if (!CheckInput())
            {
                MessageBox.Show("Incorrect input!");
                return;
            }

            string salt = ExtractSalt(param);
            var generator = this.UseLegacyGenerator 
                ? new Generator(_host, _login, _length, salt)
                : new Generator2(_host, _login, _length, salt);
            R = generator.Regenerate();
        }

        private string ExtractSalt(object param)
        {
            var passwordBox = param as PasswordBox;
            if (passwordBox == null)
            {
                return String.Empty;
            }
            return passwordBox.Password;
        }
 
        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(_host))
            {
                return false;
            }
            if (string.IsNullOrEmpty(_login))
            {
                return false;
            }
            if (string.IsNullOrEmpty(_length))
            {
                return false;
            }
            int len = Convert.ToInt32(_length);
            if (len < 8 || len > 128)
            {
                return false;
            }
            return true;
        }
    }
}
