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

namespace Seed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            ViewModels.MainVM vm = this.DataContext as ViewModels.MainVM;

            if (vm != null)
            {
                if (vm.conf.MeasureStatus == true)
                {
                    vm.conf.MeasureStatus = false;
                    LaserSensor laserSensor = new LaserSensor();
                    List<LaserSensor.AcqData> datas = new List<LaserSensor.AcqData>();
                    laserSensor.GetLaserSensorData(datas);
                }
                else
                {
                    vm.conf.MeasureStatus = true;
                }
            }
            else if (vm.conf.MeasureStatus == false)
            {
                vm.conf.MeasureStatus = true;
            }
        }
    }
}
