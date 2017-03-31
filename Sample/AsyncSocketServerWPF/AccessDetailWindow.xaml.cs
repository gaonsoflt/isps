using AsyncSocketServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static AsyncSocketServer.AccessInfoManager;

namespace AsyncSocketServerWPF
{
    /// <summary>
    /// AccessDetailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AccessDetailWindow : Window
    {
        AccessInfo accessInfo;
        AccessInfoManager accessMgr;
        CarInfoManager carMgr;
        const string customDateFormat = "yyyy-MM-dd tt hh:mm";
        DIALOG_MODE mode;
        TimeSpan minAccessTime = new TimeSpan(1, 0, 0); // 1 hour
        DateTime oldStartDt;
        DateTime oldEndDt;
        DataTable carIdLIst;

        public AccessDetailWindow()
        {
            InitializeComponent();
        }

        public AccessDetailWindow(DIALOG_MODE mode, AccessInfo accessInfo)
        {
            InitializeComponent();
            this.mode = mode;
            this.accessInfo = accessInfo;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            accessMgr = new AccessInfoManager();
            carMgr = new CarInfoManager();
            BindCarIdList();
            InitComponents();
        }

        private void LoadAccessInfoDB()
        {
            accessInfo = accessMgr.SelectAccessInfoWithOrder(accessInfo.seq);
        }

        private void BindCarIdList()
        {
            carIdLIst = carMgr.GetCarIdDBTable();
            cbCarId.ItemsSource = carIdLIst.DefaultView;
            cbCarId.SelectedIndex = -1;
        }

        private void InitComponents()
        {
            switch (mode)
            {
                case DIALOG_MODE.SAVE:
                    this.Title = "출입정보 등록";
                    if (accessInfo != null)
                    {
                        tbUserId.Text = accessInfo.user.Id.ToString();
                        tbUserNm.Text = accessInfo.user.Name.ToString();
                        DateTime dt = DateTime.Now;
                        dtpEndDT.Value = dt + minAccessTime;
                        dtpStartDT.Value = dt;
                        dtpWorkDt.SelectedDate = DateTime.Today;
                    }
                    break;
                case DIALOG_MODE.MODIFY:
                    this.Title = "출입정보 수정";
                    try
                    {
                        LoadAccessInfoDB();
                        tbAccessSeq.Text = accessInfo.seq.ToString();
                        tbUserId.Text = accessInfo.user.Id.ToString();
                        tbUserNm.Text = accessInfo.user.Name.ToString();
                        CheckAccessDt(accessInfo.access_dt);
                        tbPsgCnt.Text = accessInfo.psgCnt.ToString();
                        dtpEndDT.Value = accessInfo.allowEndDt;
                        dtpStartDT.Value = accessInfo.allowStartDt;
                        cbCarId.Text = accessInfo.carId;
                        tbPurpose.Text = accessInfo.purpose;
                        if (accessInfo.order != null)
                        {
                            tbOrderId.Text = accessInfo.order.orderId;
                            dtpWorkDt.SelectedDate = accessInfo.order.work_dt;
                        } else
                        {
                            dtpWorkDt.SelectedDate = DateTime.Today;
                        }
                    }
                    catch (Exception e)
                    {
                        if (MessageBox.Show("출입 정보를 찾을 수 없습니다.", "알림", MessageBoxButton.OK) == MessageBoxResult.OK)
                        {
                            this.Close();
                        }
                    }
                    break;
            }
        }

        private void CheckAccessDt(DateTime accessDt)
        {
            // [BBAEK] null 체크가 안됨
            if (accessDt == default(DateTime))
            {
                tbAccessDt.Text = "미출입";
            }
            else
            {
                tbAccessDt.Text = "출입 " + accessDt.ToString(customDateFormat);
                FreezeEditableForm(true);
            }
        }

        private void FreezeEditableForm(bool enable)
        {
            btnSave.IsEnabled = !enable;
            accessForm.IsEnabled = !enable;
            orderForm.IsEnabled = !enable;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("출입 등록/수정을 취소 하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dtpStartDT.Value == default(DateTime))
            {
                MessageBox.Show("출입시간일시를 입력하세요.", "알림", MessageBoxButton.OK);
                return;
            }
            else if (dtpEndDT.Value == default(DateTime))
            {
                MessageBox.Show("출입종료일시를 입력하세요.", "알림", MessageBoxButton.OK);
                return;
            }
            else if (tbPurpose.Text == "")
            {
                MessageBox.Show("출입 목적을 입력하세요.", "알림", MessageBoxButton.OK);
                return;
            }
            else if (cbCarId.SelectedValue == null)
            {
                MessageBox.Show("차량번호를 확인하세요.", "알림", MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                /* set access info */
                accessInfo.psgCnt = Int32.Parse(tbPsgCnt.Text.ToString());
                accessInfo.allowStartDt = dtpStartDT.Value.Value;
                accessInfo.allowEndDt = dtpEndDT.Value.Value;
                accessInfo.purpose = tbPurpose.Text;
                accessInfo.carId = cbCarId.Text;
                switch (mode)
                {
                    case DIALOG_MODE.SAVE:
                        accessInfo.user.Id = Int32.Parse(tbUserId.Text);
                        break;
                    case DIALOG_MODE.MODIFY:
                        accessInfo.seq = Int32.Parse(tbAccessSeq.Text.ToString());
                        break;
                }

                /* set order info */
                if (accessInfo.order == null)
                {
                    accessInfo.order = new OrderInfo();
                }
                accessInfo.order.orderId = tbOrderId.Text.ToString();
                accessInfo.order.work_dt = dtpWorkDt.SelectedDate.Value;

                /* insert update */
                if (accessMgr.SaveAccessInfoWithOrder(accessInfo) > 0)
                {
                    MessageBox.Show("정상적으로 처리 되었습니다.", "알림", MessageBoxButton.OK);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("정상적으로 처리되지 않았습니다.", "알림", MessageBoxButton.OK);
                }
            }
        }

        /* not use */
        private bool CaridValidationComboBox(string value)
        {
            bool itemExists = false;
            foreach (DataRowView drv in cbCarId.Items)
            {
                itemExists = (drv.Row.ItemArray[0].ToString() == value);
                if (itemExists)
                    break;
            }
            return itemExists;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dtpStartDT_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (dtpEndDT.Value == null || dtpStartDT.Value == null)
                return;
            TimeSpan ts = CalcurateDiffAccessDt();
            if (ts >= minAccessTime)
            {
                oldStartDt = dtpStartDT.Value.Value;
                UpdateDiffAccessDt(ts);
            }
            else
            {
                dtpStartDT.Value = oldStartDt;
                MessageBox.Show("출입허용시간은 최소 1시간 이상 되어야 합니다.", "알림", MessageBoxButton.OK);
            }
        }

        private void dtpEndDT_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (dtpEndDT.Value == null || dtpStartDT.Value == null)
                return;
            TimeSpan ts = CalcurateDiffAccessDt();
            if (ts >= minAccessTime)
            {
                oldEndDt = dtpEndDT.Value.Value;
                UpdateDiffAccessDt(ts);
            }
            else
            {
                dtpEndDT.Value = oldEndDt;
                MessageBox.Show("출입허용시간은 최소 1시간 이상 되어야 합니다.", "알림", MessageBoxButton.OK);
            }
        }

        private void UpdateDiffAccessDt(TimeSpan ts)
        {
            tbAllowDt.Text = ts.Days.ToString() + " 일 " + ts.Hours.ToString() + " 시간 " + ts.Minutes.ToString() + " 분";
        }

        private TimeSpan CalcurateDiffAccessDt()
        {
            return dtpEndDT.Value.Value - dtpStartDT.Value.Value;
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            if (orderForm.Visibility == Visibility.Collapsed)
            {
                orderForm.Visibility = Visibility.Visible;
                btnOrder.Content = "작업지시서 닫기";
                this.Width = 710;
            }
            else
            {
                orderForm.Visibility = Visibility.Collapsed;
                btnOrder.Content = "작업지시서 열기";
                this.Width = 350;
            }
        }
    }
}
