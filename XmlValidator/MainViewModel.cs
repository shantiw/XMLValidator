using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using XData.Data.Xml;

namespace XData.Tools
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _xml;
        public string Xml
        {
            get { return _xml; }
            set
            {
                if (_xml != value)
                {
                    _xml = value;
                    OnPropertyChanged();
                    (SaveAsCommand as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _xsd;
        public string Xsd
        {
            get { return _xsd; }
            set
            {
                if (_xsd != value)
                {
                    _xsd = value;
                    OnPropertyChanged();
                    (SaveAsCommand as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _messages;
        public string Messages
        {
            get { return _messages; }
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ValidateCommand { get; protected set; }

        public ICommand LoadCommand { get; protected set; }
        public ICommand SaveAsCommand { get; protected set; }
        public ICommand FormatCommand { get; protected set; }

        protected XmlValidator XmlValidator = new XmlValidator();

        public MainViewModel()
        {
            ValidateCommand = new DelegateCommand((_) =>
            {
                Messages = XmlValidator.Validate(Xml, Xsd);
            });

            LoadCommand = new DelegateCommand((obj) =>
            {
                if ((string)obj == "Xml")
                {
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        DefaultExt = ".xml",
                        Filter = "XML file(*.xml)|*.xml|All files(*.*)|*.*"
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        string fileName = dialog.FileName;
                        Xml = Load(fileName);
                    }
                }
                else
                {
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        DefaultExt = ".xsd",
                        Filter = "XML Schemas Definition file(*.xsd)|*.xsd|All files(*.*)|*.*"
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        string fileName = dialog.FileName;
                        Xsd = Load(fileName);
                    }
                }
            });

            SaveAsCommand = new DelegateCommand(
            (obj) =>
            {
                if ((string)obj == "Xml")
                {
                    if (string.IsNullOrWhiteSpace(Xml)) return;

                    SaveFileDialog dialog = new SaveFileDialog
                    {
                        DefaultExt = ".xml",
                        Filter = "XML file(*.xml)|*.xml|All files(*.*)|*.*"
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        string fileName = dialog.FileName;
                        SaveAs(fileName, Xml);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Xsd)) return;

                    SaveFileDialog dialog = new SaveFileDialog
                    {
                        DefaultExt = ".xsd",
                        Filter = "XML Schemas Definition file(*.xsd)|*.xsd|All files(*.*)|*.*"
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        string fileName = dialog.FileName;
                        SaveAs(fileName, Xsd);
                    }
                }
            },
            (obj) =>
            {
                if ((string)obj == "Xml")
                {
                    return !string.IsNullOrWhiteSpace(Xml);
                }
                else
                {
                    return !string.IsNullOrWhiteSpace(Xsd);
                }
            });

            FormatCommand = new DelegateCommand((obj) =>
            {
                if ((string)obj == "Xml")
                {
                    Xml = Format(Xml);
                }
                else
                {
                    Xsd = Format(Xsd);
                }
            });
        }

        protected static string Format(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return xml.Trim();

            XElement element = XElement.Parse(xml);
            return element.ToString();
        }

        protected static string Load(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                return sr.ReadToEnd();
            }
        }

        protected static void SaveAs(string fileName, string value)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                sw.Write(value);
                sw.Flush();
            }
        }


    }
}
