using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace GenMe
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _host;
        private string _login;
        private string _length;
        private string _salt;
        private string _r;

        internal MainWindowViewModel()
        {
            GenerateCommand = new CommandImpl(Generate);
            RegenerateCommand = new CommandImpl(Regenerate);
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

        public string Salt
        {
            get
            {
                return _salt;
            }
            set
            {
                _salt = value;
                OnPropertyChanged("Salt");
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

        private void Generate()
        {
            if (!CheckInput())
            {
                MessageBox.Show("Incorrect input!");
                return;
            }

            var generator = new Generator(_host, _login, _length, _salt);
            R = generator.Generate();
        }

        private void Regenerate()
        {
            if (!CheckInput())
            {
                MessageBox.Show("Incorrect input!");
                return;
            }

            var generator = new Generator(_host, _login, _length, _salt);
            R = generator.Regenerate();
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
            if (string.IsNullOrEmpty(_salt))
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
