   M S B A M L   `   `   `  ���� O  KAsyncSocketServerWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:AsyncSocketServerWPFAsyncSocketServerWPF  X TXceed.Wpf.Toolkit, Version=3.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit -    'AsyncSocketServerWPF.AccessDetailWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:AsyncSocketServerWPF     NwpfTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     -     � Title$  출입��$��14(� � 
ResizeMode$ NoResize=�$	��710��$	��500��.��� $��10 10 10 10q�.+�]� .+�� -    �Name$ 	orderForm��$��Right=�$	��350��$��10 0 0 0q�.+��� $��작업지시서��.���  � ColumnDefinitions �� $��1* ��� $��3* � � RowDefinitions �� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* �.+��� $��0��$��0��$��지시서번호��$��Right=�$��Center=��� -    ��Name$ 	tbOrderId��$��1��$��0��$��Stretch=� �� TextWrapping$
 Wrap=�$�� ��$��Center=��� $��0��$��1��$��작업일자��$��Right=�$��Center=�(  "System.Windows.Controls.DatePicker  -     Name$ 	dtpWorkDt��$��1��$��1��$��Stretch=�$��Center=�   SelectedDateFormat$ Short=��� $��0��$��2��$��필드1��$��Right=�$��Center=��� -   $ tbOrderField1��$��1��$��2��$��Stretch=�$
 Wrap=�$�� ��$��Center=��� $��0��$��3��$��필드2��$��Right=�$��Center=��� -   $ tbOrderField2��$��1��$��3��$��Stretch=�$
 Wrap=�$�� ��$��Center=��� $��0��$��4��$��필드3��$��Right=�$��Center=��� -   $ tbOrderField3��$��1��$��4��$��Stretch=�$
 Wrap=�$�� ��$��Center=�� -   $ 
accessForm��$	��Top=� �� $��1* ��� $��3* � �� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* �.+��� $��0��$��0��$��출입번호��$��Right=��� -	   $ tbAccessSeq��$��1��$��0��$��Stretch=�$
 Wrap=�$�� ��$��Center=�	 �� 
IsReadOnly	 . 
��������� $��0��$��1��$��	아이디��$��Right=��� -
   $ tbUserId��$��1��$��1��$��Stretch=�$
 Wrap=�$�� ��$��Center=�	 . 
��������� $��0��$��2��$��이름��$��Right=��� -   $ tbUserNm��$��1��$��2��$��Stretch=�$
 Wrap=�$�� ��$��Center=�	 . 
��������� $��0��$��3��$��차량번호��$��Right=��� -   
 ��Name$
 cbCarId��$��1��$��3��$��Stretch=�$��Center=� �� 
IsEditable .  �� DisplayMemberPath$ car_id�� �� SelectedValuePath$ car_id���� $��0��$��4��$��	동승자��$��Right=��� -   $ tbPsgCnt��$��1��$��4��$��Stretch=�$
 Wrap=�$��0��$��Center=��� $��0��$��5��$��출입목적��$��Right=��� -   $ 	tbPurpose��$��1��$��5��$��Stretch=�$
 Wrap=�$�� ��$��Center=��� $��0��$��6��$��출입시작��$��Right=�&   Xceed.Wpf.Toolkit.DateTimePicker  -     Name$ 
dtpStartDT��$��1��$��6��$��Stretch=�$��Center=�&   Xceed.Wpf.Toolkit.DateTimeUpDown   Format$ Custom=�   FormatString$% yyyy년 MM월 dd일 HH시 mm분���� $��0��$��7��$��허용시간��$��Right=��� -   $ 	tbAllowDt��$��1��$��7��$��Stretch=�$
 Wrap=�$�� ��$��Center=�	 . 
�������$��Right=��� $��0��$��8��$��출입종료��$��Right=�  -   $ dtpEndDT��$��1��$��8��$��Stretch=�$��Center=�$ Custom=�$% yyyy년 MM월 dd일 HH시 mm분���� $��0��$��9��$��출입여부��$��Right=��� -   $ 
tbAccessDt��$��1��$��9��$��Stretch=�$
 Wrap=�$�� ��$��Right=�$��Center=�	 . 
�������� $��Bottom=�$��0 10 0 0q�$��Bottom=� �� $��1* ��� $��1* � �� $��40 ��� $��40 �.+��� -    ��Name$ btnOrder��$��0��$��0��$��2��$��작업지시서 닫기���� -   $ 	btnCancel��$��0��$��1��$��취소���� -   $ btnSave��$��1��$��1��$��저장��