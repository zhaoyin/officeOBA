   M S B A M L   `   `   `  ����M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent  5   �   $  Cooperativeness.Documents.Editor   AboutDialog   6	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    6   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6a   7Fluent$clr-namespace:Fluent;assembly=Fluent     6�   -   5        $��AboutDialog��5   �     � Title$  About��6    � Icon$ 	about.ico��6    � 
ResizeMode$ 	CanResize=�6$   $	��480��6;   $	��640��6H   $	��480��6T   $	��640��6d    � WindowStartupLocation$ CenterOwner=�6s    � ShowInTaskbar .  .��5      � ���	#7BFFFFFF.+�5   
   �� -   5        $��
Rectangle1��5   &   $��0,0,0,42q�6   ���	#7BFFFFFF���	#3FFFFFFF J� Opacity$ 1Q�6\      Fluent.Button  5   
   -   5        $��OKButton��5   �      SizeDefinition/   )Fluent.Converters.SizeDefinitionConverter$ Middle 6      Icon$" ../../Images/Common/ok16.png��60      Header$ OK��6T   $��Right=�6`   $��	0,0,10,10q�6|   $��Bottom=�6�   	 �� IsCancel	 . 
 �� 	IsDefault
 .  � ToolTip 5         Fluent.ScreenTip  5         Title$ OK��6#      Text$ Close the Dialog��6.      Image$" ../../Images/Common/ok48.png��6F   5	      �� 5
   
   -   5        $��AppNameLabel��5
   /   $��43��6   $��	1,214,0,0q�6   $	��Top=�6C   $��28(�6[   $��Center=�6i   $��App Name��6�    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5         ShadowDepth$ 0Q�6#      Color$ White��63   5      �� 5   
   -   5        $��	CopyLabel��5      $��32��6!   $��	1,299,0,0q�6-   $	��Top=�6@   
���   �$��Center=�6l   $��12(�6�    5        5      $ 0Q�6#   $ White��63   .��5      	Copyright5      �� 5   
   -   5        $��VersionLabel��5   T   $��Center=�6   $��30��65   $��	1,263,0,0q�6A   $	��Top=�6h   $��18(�6�   $��Center=�6�   $��Version��6�    5        5      $ 0Q�6#   $ White��63   5      �� 5   
   -   5        $��AppLogo��5   1   $��../../Images/editor.png��6   $	��256��6@   $��	1,-24,0,0q�6M   $	��Top=�6`     5   
   -   5        $��LicenseButton��5   �   $ Middle 6   $% ../../Images/Help/license16.png��60   $ License��6W   $
��Left=�6h   $��	10,0,0,10q�6�   $��Bottom=�6�    5        5      $ License��6#   $+ %Show/Hide the Document.Editor License��63   $% ../../Images/Help/license48.png��6`   5        5    
   -	   5        $��UpdateButton��5    �   $ Middle 6   $- '../../Images/Help/checkforupdates16.png��60   $ Check for Updates��6_   $��Center=�6z   $��0,0,0,52q�6�   $��Bottom=�6�    5!        5"      $ Check for Updates��6#   $* $Check for Updates to Document.Editor��6=   $- '../../Images/Help/checkforupdates48.png��6i   5$      � 5%   
   -
   5        $��	UpdateBox��5%   &   $y�	Collapsed=�6   ���	#7AFFFFFF.+�5&      �� .+�5'      �� -   5        $��
UpdateText��5'      $��Checking for updates...��6.   $��22(�6M   $��Center=�6[   �� 5(      -   5        $��ProgressBox��5(       �� Orientation$ 
Horizontal=�60   $��0,4,0,0q�6I   $��Center=�6Z   .+�5)      � -   5        $��UpdateProgressbar��5)   "   $	��512��6;   $��24��6G    � IsIndeterminate . 6S   �� 5*      -   5        $��FilesizeTextBlock��5*   7   $y�	Collapsed=�6    $��Filesize��6P   $��
-508,0,0,0q�6`   $��16(�6t   5+      �� 5,      -   5        $��ApplyUpdateButton��5,   0   $y�	Collapsed=�6   $��0,4,0,0q�6I   $��32��6Z   $	��512��6f   $��16(�6r   .��5-      �� $ 
Horizontal=�6!   .+�5.      �� $��16��6    $��16��6,   $"��../../Images/Common/ok16.png��67   �� 5/      $%��Apply Update (Restart required)��6$   $��2,0,0,0q�6K   50      51      �� 52      -   5        $��CancelUpdateButton��52   0   $y�	Collapsed=�6   $��0,4,0,0q�6J   $��32��6[   $��16(�6g   $	��512��6u   .��53      �� $ 
Horizontal=�6!   .+�54      �� $��16��6    $��16��6,   $&�� ../../Images/Common/cancel16.png��67   �� 55      $��Cancel Update��6$   $��2,0,0,0q�69   56      57      �� 58      -   5        $��WhatsNewTextBlock��58   3   $y�	Collapsed=�6   $��What's New:��6L   $��0,4,0,0q�6_   $��24(�6p   �� 59      -   5        $��WhatsNewTextBox��59   1   $y�	Collapsed=�6   $��0,4,0,0q�6H   $	��256��6Y   $��16(�6f   
���   �
������ ���	#000000006�   5:      5;      �� 5<   
   -   5        $��TextBox1��5<   8    �� 
IsReadOnly . $��
0,-2,-2,40q�6$    �� TextWrapping$ WrapWithOverflow=�6H   " �� VerticalScrollBarVisibility$
 Auto=�6h   $y�Hidden=�6�   ���	#7BFFFFFF���	#FF000000���	#7BFFFFFF6�   5=      5>      