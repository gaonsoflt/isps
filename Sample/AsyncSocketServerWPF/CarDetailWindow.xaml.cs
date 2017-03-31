using AsyncSocketServer;
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
using System.Windows.Shapes;
using static AsyncSocketServer.CarInfoManager;

namespace AsyncSocketServerWPF
{
    /// <summary>
    /// CarDetailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CarDetailWindow : Window
    {
        CarInfoManager.DIALOG_MODE mode;
        CarInfo m_car;
        CarInfoManager m_carMgr;

        public CarDetailWindow(CarInfoManager.DIALOG_MODE mode)
        {
            InitializeComponent();
            this.mode = mode;
            m_car = new CarInfo();
        }

        public CarDetailWindow(CarInfoManager.DIALOG_MODE mode, string carId)
        {
            InitializeComponent();
            this.mode = mode;
            m_car = new CarInfo();
            m_car.id = carId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_carMgr = new CarInfoManager();
            InitComponents();
        }

        private void LoadCarInfo(string carId)
        {
            m_car = new CarInfoDB().SelectCarInfo(carId);
        }

        private void InitComponents()
        {
            switch (mode)
            {
                case DIALOG_MODE.SAVE:
                    this.Title = "차량 등록";
                    tbCarId.IsReadOnly = false;
                    break;
                case DIALOG_MODE.MODIFY:
                    this.Title = "차량 수정";
                    LoadCarInfo(m_car.id);
                    try
                    {
                        tbCarId.Text = m_car.id.ToString();
                        tbOwner.Text = m_car.owner.ToString();
                    }
                    catch (Exception e)
                    {
                        if (MessageBox.Show("차량 정보가 없습니다.", "알림", MessageBoxButton.OK) == MessageBoxResult.OK)
                        {
                            this.Close();
                        }
                    }
                    tbCarId.IsReadOnly = true;
                    break;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("차량 등록/수정을 취소 하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbCarId.Text == "" || tbCarId == null)
            {
                MessageBox.Show("차량번호를 입력하세요.", "알림", MessageBoxButton.OK);
                return;
            }
            else if (tbOwner.Text == "" || tbOwner == null)
            {
                MessageBox.Show("차량소유자를 입력하세요.", "알림", MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                switch(mode)
                {
                    case DIALOG_MODE.SAVE:
                        if (m_carMgr.CheckExistCarId(tbCarId.Text.ToString()))
                        {
                            MessageBox.Show("동일한 차량번호가 존재합니다. 차량번호를 확인해주세요.", "알림", MessageBoxButton.OK);
                            return;
                        }
                        break;
                    case DIALOG_MODE.MODIFY:
                        break;
                }
                m_car.id = tbCarId.Text.ToString();
                m_car.owner = tbOwner.Text.ToString();
                int rtn = m_carMgr.SaveCarInfo(m_car);
                if (rtn > 0)
                {
                    MessageBox.Show("차량(" + m_car.id + ") 정보가 저장되었습니다.", "알림", MessageBoxButton.OK);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("차량(" + m_car.id + ") 정보가 저장되지 않았습니다.", "알림", MessageBoxButton.OK);
                }
            }
        }
    }
}
