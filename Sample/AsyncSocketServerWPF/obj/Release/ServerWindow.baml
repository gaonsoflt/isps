   M S B A M L   `   `   `  ���� O  KAsyncSocketServerWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:AsyncSocketServerWPFAsyncSocketServerWPF  '    !AsyncSocketServerWPF.ServerWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:AsyncSocketServerWPF     -     � Title$  SERVER��$��603.391��$
��1100�� � WindowStartupLocation$ CenterScreen=� � WindowState$ 	Maximized=�$��14(�.��]� -   $��
dockPanel1��.+�� $��Right=�$	��200��$��
10,10,10,0q�.+�]� -   $��
dockPanel2��.+�� $	��Top=� � ColumnDefinitions �� $��5* ��� $��1* �.+��� -    ��Name$ label1��$��0��$��컨트롤러��$
��Bold"��� -    ��Name$ btnConf��$��1��$��Right=�
������ 
������ .���� $��Resources\ic_setting.png��$��30���� -   $ 	btnListen��$	��Top=�$��START��$
��Left=�$	��Top=�$	��200��$��50��$��0,0,0,10q��� -   $ btnClose��$	��Top=�$
��STOP��$
��Left=�$	��Top=�$	��200��$��50��$��0,0,0,10q�{�.  �� -    ��Name$ cbConnetType��$	��Top=�$��Serial Port 포함��$	��Top=�$��0,0,0,10q��� -	    ��Name$ 	cbComPort��$	��Top=�$��30��$��0,0,0,10q��� -
   $ 	cbComRate��$	��Top=�$��30��$��0,0,0,10q��� -   $ btnSerialOpen��$	��Top=�$
��OPEN��$
��Left=�$	��Top=�$	��200��$��50��$��0,0,0,10q��� -   $ btnSerialClose��$	��Top=�$��CLOSE��$
��Left=�$	��Top=�$	��200��$��50��$��0,0,0,10q�{�.  �� -   $ 	btnSysMgr��$	��Top=�$��시스템관리��$
��Left=�$	��Top=�$	��200��$��50��$��0,0,0,10q��� -   $ btnTestAuth��$	��Top=�$��지문테스트��$
��Left=�$	��Top=�$	��200��$��50��$��0,0,0,10q�{�.  �� -   $ btnLogClear��$	��Top=�$��로그삭제��$
��Left=�$	��Top=�$	��200��$��50��$��0,0,0,10q�{�. � $��Bottom=�$��
10,10,0,10q�$	��300��.+��� -    ��Name$ 	groupBox1��$��유저정보��$��Stretch=�$��Stretch=�$
��Bold"�]� -   $��
dockPanel3��$��10,20,10,10q�.+��� -   	 ��Name$	 pbImage��$
��Left=�$	��240��$	��320��$��0,0,10,0q��� -   $	 pbFPRef��$
��Left=�$	��240��$	��320��$��0,0,10,0q�� $
��Left=�$	��300��$
��Left=�.+�]� ��.  .+��� $	��Top=�$��	아이디��$��0,0,0,10q�$
��Bold"��� -   
 ��Name$

 tbId��$	��Top=�$��0,0,0,10q�
��������� $	��Top=�$��이름��$��0,0,0,10q�$
��Bold"��� -   $
 tbName��$	��Top=�$��0,0,0,10q�
��������� $	��Top=�$��	연락처��$��0,0,0,10q�$
��Bold"��� -   $
 tbPhone��$	��Top=�$��0,0,0,10q�
��������� $	��Top=�$
��GUID��$��0,0,0,10q�$
��Bold"��� -   $
 tbGuid��$	��Top=�$��0,0,0,10q�
��������� -   $ label2��$	��Top=�$	��Log��$��
10,10,10,0q�$
��Bold"��� -    ��Name$ lstText��$	��Top=�$��10,0,0,0q�