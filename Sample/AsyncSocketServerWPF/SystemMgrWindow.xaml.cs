using AsyncSocketServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using static AsyncSocketServer.UserManager;

namespace AsyncSocketServerWPF
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SystemMgrWindow : Window
    {
        int currentPage = 1;
        int pageTotal = 1;
        int total = 0;

        string[] chartTypeCbdatas = { "월별 출입신청 건수", "시간별 출입 건수" };
        string[] selectCountCbDatas = { "ALL", "5", "10", "20", "30", "50" };

        UserDB m_userDB;
        AccessInfoDB m_accessDB;
        CarInfoDB m_carDB;
        AccessHisDB m_historyDB;

        AverageInfoManager m_avrMgr;
        MyPerson m_user;
        CarInfo m_car;
        int m_accessSeq = 0;

        public SystemMgrWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            m_userDB = new UserDB();
            m_accessDB = new AccessInfoDB();
            m_carDB = new CarInfoDB();
            m_historyDB = new AccessHisDB();

            m_avrMgr = new AverageInfoManager();

            m_user = new MyPerson();
            m_car = new CarInfo();

            toolStripCbCount.ItemsSource = selectCountCbDatas;
            toolStripCbCount.SelectedIndex = 2;
            
            dtpAvgDate.SelectedDate = DateTime.Today;
            cbChartType.ItemsSource = chartTypeCbdatas;
            cbChartType.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateComponents();
        }

        private void UpdateComponents()
        {
            UpdateStatusMessage("");
            //TabItem ti = tabControl.SelectedItem as TabItem;
            //gbGroup.Header = ti.Header;

            if (tabAvg.IsSelected)
            {
                Console.WriteLine("TAB_AVERAGE");
                EnableCRUDButton(false);
                EnableSearchComponent(false);
                toolStripPaging.Visibility = Visibility.Hidden;
                UpdateAccessTotal();
                UpdateChart();
            }
            else if (tabAccess.IsSelected)
            {
                Console.WriteLine("TAB_ACCESS");
                lbKeyword.Content = "이름";
                DataTable tmp = m_userDB.GetUserDBTable(tbKeyword.Text, currentPage, GetPageCount());
                dgvAccessUser.ItemsSource = tmp.DefaultView;
                EnableCRUDButton(true);
                EnableSearchComponent(true);
                toolStripPaging.Visibility = Visibility.Visible;
            }
            else if (tabUser.IsSelected)
            {
                Console.WriteLine("TAB_USER");
                lbKeyword.Content = "이름";
                EnableCRUDButton(true);
                EnableSearchComponent(true);
                toolStripPaging.Visibility = Visibility.Visible;
            }
            else if (tabCar.IsSelected)
            {
                Console.WriteLine("TAB_CAR");
                lbKeyword.Content = "번호";
                EnableCRUDButton(true);
                EnableSearchComponent(true);
                toolStripPaging.Visibility = Visibility.Visible;
            }
            else if (tabHistory.IsSelected)
            {
                Console.WriteLine("TAB_ACCESS_HIS");
                lbKeyword.Content = "이름";
                EnableCRUDButton(false);
                EnableSearchComponent(true);
                toolStripPaging.Visibility = Visibility.Visible;
            }

            // datagridview paging
            currentPage = 1;
            RebindGridForPageChange(tbKeyword.Text);
            RefreshPagination();
        }

        private DataTable getUserDB()
        {
            string keyword = tbKeyword.Text;
            return m_userDB.GetUserDBTable(keyword);
        }

        private void UpdateStatusMessage(String msg)
        {
            Dispatcher.Invoke(new Action(() => {
                statusLabel.Text = msg;
            }));
        }

        private void EnableCRUDButton(bool enabled)
        {
            btnDelete.IsEnabled = enabled;
            btnCreate.IsEnabled = enabled;
            btnModify.IsEnabled = enabled;
        }

        private void EnableSearchComponent(bool enabled)
        {
            tbKeyword.IsEnabled = enabled;
            btnSearch.IsEnabled = enabled;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void dtpAvgDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComponents();
        }

        private void UpdateAccessTotal()
        {
            List<Dictionary<string, object>> result = m_avrMgr.SelectAccessTotalInfo(dtpAvgDate.SelectedDate.Value);
            long req = 0;
            long ok = 0;
            long no = 0;
            if (result != null)
            {
                req = (long)result[0]["req_total"];
                ok = (long)result[0]["access_cnt"];
                no = (long)result[0]["not_access_cnt"];
            }
            tbAccessReqTotal.Text = req.ToString();
            tbAccessTotal.Text = ok.ToString();
            tbNonAccessTotal.Text = no.ToString();
        }

        private void UpdateChart()
        {
            avgChart.Title = chartTypeCbdatas[cbChartType.SelectedIndex];
            List<Dictionary<string, object>> result;
            List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            try
            {
                switch (cbChartType.SelectedIndex)
                {
                    case 0:
                        result = m_avrMgr.SelectAccessMonthlyTotal(dtpAvgDate.SelectedDate.Value);
                        foreach (Dictionary<string, object> d in result)
                        {
                            valueList.Add(new KeyValuePair<string, int>(d["mon"].ToString(), Convert.ToInt32(d["count"])));
                        }
                        break;
                    case 1:
                        result = m_avrMgr.SelectAccessDailyTimeTotal(dtpAvgDate.SelectedDate.Value);
                        foreach (Dictionary<string, object> d in result)
                        {
                            valueList.Add(new KeyValuePair<string, int>(d["hm"].ToString(), Convert.ToInt32(d["count"])));
                        }
                        break;
                }
                avgChart.DataContext = valueList;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "에러", MessageBoxButton.OK);
            }
        }

        private int GetPageCount()
        {
            int count = 0;
            try
            {
                count = Int32.Parse(toolStripCbCount.SelectedItem.ToString());
            }
            catch
            {
                count = 0;
            }
            return count;
        }

        private void ToolStripButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = ((Button)sender);

                //Determining the current page
                if (button == btnBackward)
                    currentPage--;
                else if (button == btnForward)
                    currentPage++;
                else if (button == btnLast)
                    currentPage = pageTotal;
                else if (button == btnFirst)
                    currentPage = 1;
                else
                    currentPage = Convert.ToInt32(button.Content.ToString(), CultureInfo.InvariantCulture);

                if (currentPage < 1)
                    currentPage = 1;
                else if (currentPage > pageTotal)
                    currentPage = pageTotal;

                //Rebind the Datagridview with the data.
                RebindGridForPageChange(tbKeyword.Text.ToString());

                //Change the pagiantions buttons according to page number
                RefreshPagination();
            }
            catch (Exception) { }
        }

        private void RefreshPagination()
        {
            Button[] items = new Button[] { toolStripButton1, toolStripButton2, toolStripButton3, toolStripButton4, toolStripButton5 };

            //pageStartIndex contains the first button number of pagination.
            int pageStartIndex = 1;

            if (pageTotal > 5 && currentPage > 2)
                pageStartIndex = currentPage - 2;

            if (pageTotal > 5 && currentPage > pageTotal - 2)
                pageStartIndex = pageTotal - 4;

            for (int i = pageStartIndex; i < pageStartIndex + 5; i++)
            {
                if (i > pageTotal)
                {
                    items[i - pageStartIndex].Visibility = Visibility.Collapsed;
                }
                else
                {
                    //Changing the page numbers
                    items[i - pageStartIndex].Content = i.ToString(CultureInfo.InvariantCulture);
                    items[i - pageStartIndex].Visibility = Visibility.Visible;

                    //Setting the Appearance of the page number buttons
                    if (i == currentPage)
                    {
                        items[i - pageStartIndex].Background = Brushes.Black;
                        items[i - pageStartIndex].Foreground = Brushes.White;
                    }
                    else
                    {
                        items[i - pageStartIndex].Background = Brushes.White;
                        items[i - pageStartIndex].Foreground = Brushes.Black;
                    }
                }
            }

            //Enabling or Disalbing pagination first, last, previous , next buttons
            if (currentPage == 1)
            {
                btnBackward.IsEnabled = btnFirst.IsEnabled = false;
            }
            else
            {
                btnBackward.IsEnabled = btnFirst.IsEnabled = true;
            }

            if (currentPage == pageTotal)
            {
                btnForward.IsEnabled = btnLast.IsEnabled = false;
            }
            else
            {
                btnForward.IsEnabled = btnLast.IsEnabled = true;
            }
        }

        private void RebindGridForPageChange(string keyword)
        {
            total = 0;
            try
            {
                DataTable tmp;
                if (tabAvg.IsSelected)
                {
                    tmp = m_avrMgr.SelectAccessHistoryInfo(dtpAvgDate.SelectedDate.Value);
                    dgvAvgInfo.ItemsSource = tmp.DefaultView;
                }
                else if (tabAccess.IsSelected)
                {
                    //dt = m_userDB.GetUserDBTable(keyword, currentPage, GetPageCount());
                    //dgvAccessUser.DataSource = dt;
                    //dgvAccessUser.Refresh();
                    //total = Int32.Parse(dgvAccessUser.Rows[0].Cells["count"].Value.ToString());
                    tmp = m_accessDB.GetAccessInfoDBTable(m_user.Id, currentPage, GetPageCount());
                    dgvAccessInfo.ItemsSource = tmp.DefaultView;
                    Total = Int32.Parse(tmp.Rows[0].ItemArray[6].ToString());
                }
                else if (tabUser.IsSelected)
                {
                    tmp = m_userDB.GetUserDBTable(keyword, currentPage, GetPageCount());
                    dgvUser.ItemsSource = tmp.DefaultView;
                    Total = Int32.Parse(tmp.Rows[0].ItemArray[6].ToString());
                }
                else if (tabCar.IsSelected)
                {
                    tmp = m_carDB.GetCarInfoDBTable(keyword, currentPage, GetPageCount());
                    dgvCar.ItemsSource = tmp.DefaultView;
                    Total = Int32.Parse(tmp.Rows[0].ItemArray[3].ToString());
                }
                else if (tabHistory.IsSelected)
                {
                    tmp = m_historyDB.GetAccessHisDBTable(keyword, currentPage, GetPageCount());
                    dgvHistory.ItemsSource = tmp.DefaultView;
                    Total = Int32.Parse(tmp.Rows[0].ItemArray[5].ToString());
                }
                pageTotal = Convert.ToInt32(Math.Ceiling(Total * 1.0 / ((GetPageCount()) < 1 ? 1 : GetPageCount())));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public int Total
        {
            get { return this.total; }
            set
            {
                this.total = value;
                toolStripLbTotal.Text = this.total.ToString();
            }
        }
        
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            UpdateComponents();
        }

        private void cbChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComponents();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is TabControl)
            {
                tbKeyword.Text = "";
                UpdateComponents();
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView rowView = dataGrid.SelectedItem as DataRowView;
            if (rowView != null)
            {
                if (dataGrid == dgvAccessUser)
                {
                    m_user.Id = Int32.Parse(rowView.Row["user_id"].ToString());
                    m_user.Name = rowView.Row["user_nm"].ToString();
                    //UpdateStatusMessage("Selected row: USER_ID[" + m_user.Id + "]");
                    RebindGridForPageChange("");
                }
                else if (dataGrid == dgvAccessInfo)
                {
                    m_accessSeq = Int32.Parse(rowView.Row["access_info_sq"].ToString());
                    UpdateStatusMessage("Selected row: USER_ID[" + m_user.Id + "] ACCESS_INFO_SQ[" + m_accessSeq + "]");
                }
                else if (dataGrid == dgvUser)
                {
                    m_user.Id = Int32.Parse(rowView.Row["user_id"].ToString());
                    m_user.Name = rowView.Row["user_nm"].ToString();
                    UpdateStatusMessage("Selected row: USER_ID[" + m_user.Id + "]");
                }
                else if (dataGrid == dgvCar)
                {
                    m_car.id = rowView.Row["car_id"].ToString();
                    UpdateStatusMessage("Selected row: CAR_ID[" + m_car.id + "]");
                }
            }
        }


        private void UpdateDialogResult(bool windowReturn)
        {
            if (windowReturn)
            {
                UpdateComponents();
                UpdateStatusMessage("정상적으로 처리 되었습니다.");
            }
            else
            {
                UpdateStatusMessage("취소되었습니다.");
            }
        }

        private void CreateAccessInfo()
        {
            if (dgvAccessUser.Items.Count > 0)
            {
                AccessInfoManager.AccessInfo accessInfo = new AccessInfoManager.AccessInfo();
                accessInfo.user = m_user;
                AccessDetailWindow window = new AccessDetailWindow(AccessInfoManager.DIALOG_MODE.SAVE, accessInfo);
                UpdateDialogResult(window.ShowDialog().Value);
            }
            else
            {
                MessageBox.Show("유저 정보가 없습니다.", "알림", MessageBoxButton.OK);
            }
        }

        private void UpdateAccessInfo()
        {
            if (dgvAccessInfo.Items.Count > 0)
            {
                AccessInfoManager.AccessInfo accessInfo = new AccessInfoManager.AccessInfo();
                accessInfo.seq = m_accessSeq;
                accessInfo.user = m_user;
                AccessDetailWindow window = new AccessDetailWindow(AccessInfoManager.DIALOG_MODE.MODIFY, accessInfo);
                UpdateDialogResult(window.ShowDialog().Value);
            }
            else
            {
                MessageBox.Show("출입 정보가 없습니다.", "알림", MessageBoxButton.OK);
            }
        }

        private void DeleteAccessInfo()
        {
            if (dgvAccessInfo.Items.Count > 0)
            {
                if (MessageBox.Show("출입정보[" + m_accessSeq + "]를 삭제하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (m_accessDB.DeleteAccessInfo(m_accessSeq) > 0)
                    {
                        MessageBox.Show("삭제 되었습니다.", "알림", MessageBoxButton.OK);
                        UpdateComponents();
                    }
                    else
                    {
                        MessageBox.Show("삭제 되지 않았습니다.\n출입기록이 있는 경우에는 삭제되지 않습니다.", "알림", MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("출입 정보가 없습니다.", "알림", MessageBoxButton.OK);
            }
        }

        private void CreateUser()
        {
            UserDetailWindow window = new UserDetailWindow(UserManager.MODE.SAVE);
            UpdateDialogResult(window.ShowDialog().Value);
        }

        private void UpdateUser()
        {
            if (dgvUser.Items.Count > 0)
            {
                UserDetailWindow window = new UserDetailWindow(UserManager.MODE.MODIFY, m_user.Id);
                UpdateDialogResult(window.ShowDialog().Value);
            }
            else
            {
                MessageBox.Show("사용자 정보가 없습니다.", "알림", MessageBoxButton.OK);
            }
        }

        private void DeleteUser()
        {
            if (dgvUser.Items.Count > 0)
            {
                if (MessageBox.Show("사용자[" + m_user.Id + "]를 삭제하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (m_userDB.DeleteISPSUser(m_user.Id) > 0)
                    {
                        UpdateStatusMessage("Success delete user: " + m_user.Id);
                        UpdateComponents();
                    }
                    else
                    {
                        UpdateStatusMessage("Failed delete user: " + m_user.Id);
                    }
                }
            }
            else
            {
                MessageBox.Show("사용자 정보가 없습니다.", "알림", MessageBoxButton.OK);
            }
        }

        private void CreateCar()
        {
            CarDetailWindow window = new CarDetailWindow(CarInfoManager.DIALOG_MODE.SAVE);
            UpdateDialogResult(window.ShowDialog().Value);
        }

        private void UpdateCar()
        {
            if (dgvCar.Items.Count > 0)
            {
                CarDetailWindow window = new CarDetailWindow(CarInfoManager.DIALOG_MODE.MODIFY, m_car.id);
                UpdateDialogResult(window.ShowDialog().Value);
            }
            else
            {
                MessageBox.Show("차량 정보가 없습니다.", "알림", MessageBoxButton.OK);
            }
        }

        private void DeleteCar()
        {
            if (dgvCar.Items.Count > 0)
            {
                if (MessageBox.Show("차량정보[" + m_car.id + "]를 삭제하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (m_carDB.DeleteCarInfo(m_car.id) > 0)
                    {
                        UpdateStatusMessage("Success delete car: " + m_car.id);
                        UpdateComponents();
                    }
                    else
                    {
                        UpdateStatusMessage("Failed delete car: " + m_car.id);
                    }
                }
            }
            else
            {
                MessageBox.Show("차량 정보가 없습니다.", "알림", MessageBoxButton.OK);
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (tabAccess.IsSelected)
            {
                CreateAccessInfo();
            }
            else if (tabUser.IsSelected)
            {
                CreateUser();
            }
            else if (tabCar.IsSelected)
            {
                CreateCar();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (tabAccess.IsSelected)
            {
                DeleteAccessInfo();
            }
            else if (tabUser.IsSelected)
            {
                DeleteUser();
            }
            else if (tabCar.IsSelected)
            {
                DeleteCar();
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (tabAccess.IsSelected)
            {
                UpdateAccessInfo();
            }
            else if (tabUser.IsSelected)
            {
                UpdateUser();
            }
            else if (tabCar.IsSelected)
            {
                UpdateCar();
            }
        }
    }
}
