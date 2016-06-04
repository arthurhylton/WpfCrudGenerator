using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCrudGenerator.Classes;

namespace WpfCrudGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        List<CSharpProperty> CSharpProperties { get; set; }
        public string CSharpCode { get; set; }
        public string XamlCode { get; set; }
        public string BindingObject { get; set; }
        public MainWindow()
        {
            CSharpProperties = new List<CSharpProperty>();
            InitializeComponent();
            DataContext = this;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


        private bool ParseCSharp()
        {
            try
            {
                CSharpProperties.Clear();
                //assuming that each property must be on a new line
                //iterate over each line and convert string into CSharpProperty
                using (System.IO.StringReader reader = new System.IO.StringReader(CSharpCode))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] split = line.Trim().Split('{')[0].Split(' ');
                        CSharpProperties.Add(new CSharpProperty { AccessModifier = split[0], DataType = split[1], Name = split[2] });
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private string GenerateXaml(string cSharpCode)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in CSharpProperties)
            {
                stringBuilder.AppendLine(string.Format("<StackPanel Orientation=\"Horizontal\">"));
                stringBuilder.AppendLine(string.Format("<TextBlock Width=\"100\" Text=\"{0}:\" />", item.Name));
                if(string.IsNullOrWhiteSpace(BindingObject))
                {
                    stringBuilder.AppendLine(string.Format("<TextBox Width=\"200\" Text=\"{{Binding {0}}}\" />", item.Name));
                }
                else
                {
                    stringBuilder.AppendLine(string.Format("<TextBox Width=\"200\" Text=\"{{Binding {1}.{0}}}\" />", item.Name, BindingObject));
                }
                stringBuilder.AppendLine(string.Format("</StackPanel>"));
            }
            return stringBuilder.ToString();
        }

        private void ButtonGenerateXaml_Click(object sender, RoutedEventArgs e)
        {
            ParseCSharp();
            XamlCode = GenerateXaml(CSharpCode);
            RaisePropertyChanged("XamlCode");
        }
    }
}
