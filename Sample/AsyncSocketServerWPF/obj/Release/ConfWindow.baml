   M S B A M L   `   `   `  ���� O  KAsyncSocketServerWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:AsyncSocketServerWPFAsyncSocketServerWPF  %    AsyncSocketServerWPF.ConfWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:AsyncSocketServerWPF     -     � Title$  Configuration��$	��400��$	��300�� � 
ResizeMode$ NoResize=�$��14(�.��� .+�]� .+��� -    ��Name$ 
tabControl��$	��Top=�$	��320��.@��� -    ��Name$ tabDB��$��DataBase��.���  � ColumnDefinitions �� $��2* ��� $��3* � � RowDefinitions �� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* �.+��� $��0��$��0��$��2��$��Database Connection Info��$��Center=�$
��Left=�$
��Bold"��� $��0��$��1��$��Vendor��$��Center=�$��Right=�$
��Bold"�$��0 0 10 0q��� -    ��Name$ cbVendor��$��1��$��1��$��Stretch=�$��Stretch=����� *
VendorList+ �� UpdateSourceTrigger$ PropertyChanged=� �� SelectedItem �� *Vendor+$ PropertyChanged=��� $��0��$��2��$��IP��$��Center=�$��Right=�$
��Bold"�$��0 0 10 0q��� $��1��$��2��$��Center=����� *IP+$ PropertyChanged=��� $��0��$��3��$
��Port��$��Center=�$��Right=�$
��Bold"�$��0 0 10 0q��� -   	 ��Name$	 tbPort��$��1��$��3��$��Center=����� *Port+$ PropertyChanged=��� $��0��$��4��$��User(ID)��$��Center=�$��Right=�$
��Bold"�$��0 0 10 0q��� $��1��$��4��$��Center=����� *User+$ PropertyChanged=��� $��0��$��5��$��Password��$��Center=�$��Right=�$
��Bold"�$��0 0 10 0q��� $��1��$��5��$��Center=����� *
Password+$ PropertyChanged=��� $��0��$��6��$��Database(SID)��$��Center=�$��Right=�$
��Bold"�$��0 0 10 0q��� $��1��$��6��$��Center=����� *SID+$ PropertyChanged=�� $��Bottom=� �� $��1* ��� $��1* �.+��� -   
 ��Name$
 	btnCancel��$��0��$��취소���� -   $
 btnSave��$��1��$��저장��