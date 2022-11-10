using System;
using System.Collections.Generic;
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

namespace ORO_Lb3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Drawing.Bitmap source;
        int counter = 0;

        public MainWindow()
        {
            InitializeComponent();
            source = null;
        }

        private void CobelCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (CobelCheckBox.IsChecked == null)
            {
                return;
            }
            else if ((bool)CobelCheckBox.IsChecked)
            {
                MaskBox.Visibility = Visibility.Hidden;
            }
            else
            {
                MaskBox.Visibility = Visibility.Visible;
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                "Portable Network Graphic (*.png)|*.png";

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                source = new System.Drawing.Bitmap(fileDialog.FileName);
                SourceImage.Source = new BitmapImage(new Uri(fileDialog.FileName));
            }
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (InverseCheckBox.IsChecked == null || CobelCheckBox.IsChecked == null)
            {
                return;
            }
            bool isNegative = (bool)InverseCheckBox.IsChecked;
            bool cobel = (bool)CobelCheckBox.IsChecked;
            bool bin = (bool)BinarizeCheckBox.IsChecked;

            if (cobel)
            {
                ContourParser cp = new ContourParser(source, isNegative, bin);
                cp.Result.Save($"Contour-{counter}.png");
            }
            else
            {
                int[,] mask = null;
                try
                {
                    mask = GetMatrix();
                }
                catch
                {
                    MessageBox.Show("Некоректна маска");
                    return;
                }

                ContourParser cp = new ContourParser(source, isNegative, bin, mask);
                cp.Result.Save($"Contour-{counter}.png");
            }

            ConvertedImage.Source = new BitmapImage(new Uri(@$"C:\Users\Sasha\source\repos\ORO_Lb3\ORO_Lb3\bin\Debug\net6.0-windows\Contour-{counter}.png"));
            counter++;
        }

        private int[,] GetMatrix()
        {
            var matrix = new int[3,3];

            matrix[0, 0] = int.Parse(ZeroZeroBox.Text);
            matrix[1, 0] = int.Parse(OneZeroBox.Text);
            matrix[2, 0] = int.Parse(TwoZeroBox.Text);

            matrix[0, 1] = int.Parse(ZeroOneBox.Text);
            matrix[1, 1] = int.Parse(OneOneBox.Text);
            matrix[2, 1] = int.Parse(TwoOneBox.Text);

            matrix[0, 2] = int.Parse(ZeroTwoBox.Text);
            matrix[1, 2] = int.Parse(OneTwoBox.Text);
            matrix[2, 2] = int.Parse(TwoTwoBox.Text);

            return matrix;
        }
    }
}
