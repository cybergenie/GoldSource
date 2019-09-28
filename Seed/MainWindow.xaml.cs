using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
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
using STIL_NET;

namespace Seed
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();            
            
        }

        //private void Start_Click(object sender, RoutedEventArgs e)
        //{
           
            
        //    //temp= ConfigurationManager.AppSettings["status"];            
        //    //if (temp=="ON")
        //    //{
        //    //    temp = "OFF";
        //    //    this.Start.Background = new SolidColorBrush(Colors.Red);
        //    //    this.Start.Content = string.Format("停止\n测量");
        //    //}
        //    //else if (temp == "OFF")
        //    //{
        //    //    temp = "ON";
        //    //    this.Start.Background = new SolidColorBrush(Colors.LightGreen);
        //    //    this.Start.Content =string.Format( "开始\n测量");

        //    //}
        //}

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
