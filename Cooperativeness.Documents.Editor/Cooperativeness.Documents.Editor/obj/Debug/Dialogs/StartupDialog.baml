   M S B A M L   `   `   `  ����M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent  5   �   $  Cooperativeness.Documents.Editor   StartupDialog   6	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    6!   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6c   7Fluent$clr-namespace:Fluent;assembly=Fluent     6�     � Title$  Startup��5      $	��220��6    � ShowInTaskbar .   � WindowStyle$ 
ToolWindow=�68    � WindowStartupLocation$ CenterOwner=�6Q    � 
ResizeMode$ NoResize=�6u   $	��440��6�   .��5      � ���	#7BFFFFFF.+�5   
      Fluent.Button  -   5        $��CloseButton��5   �      SizeDefinition/   )Fluent.Converters.SizeDefinitionConverter$ Middle 6      Icon$ ../Images/Common/ok16.png��60      Header$ Close��6Q   $��Right=�6`   $��	0,0,10,10q�6|   $��Bottom=�6�    � ToolTip 5         Fluent.ScreenTip  5      	   Title$	 Close��6#   
   Text$
 Close the Dialog��61      Image$ ../Images/Common/ok48.png��6I   5      �� 5	   
   -   5        $��
TextBlock1��5	   N   $��23��6   $
��Left=�6    $��	12,12,0,0q�6;   $	��Top=�6`   $��14(�6x   Welcome to Document.Editor5      �� 5   
   -   5        $��
TextBlock2��5   N   $��23��6   $
��Left=�6    $��	20,41,0,0q�6;   $	��Top=�6`   FDHello and welcome to Document.Editor, click Close to get started or:5         Fluent.CheckBox  5   
   -   5        $��ShowOnStartupCheckBox��5   �      Header$ Show on Startup��6   
���   � d� 	IsChecked$
 TrueY�6F   $
��Left=�6W   $��	10,0,0,10q�6r   $��Bottom=�6�     5   
   -   5        $��OnlineHelpButton��5      $ Online Help��60      ButtonBackstageStyle#���   6E   $��	10,74,0,0q�6t   $
��Left=�6�   $	��Top=�6�      	LargeIcon 5      �� 5      $��32��6   $��32��6$   $%��../Images/Help/onlinehelp32.png��6/    5        5      $	 Online Help��6#   $
 Get Help Online��67   $% ../Images/Help/onlinehelp48.png��6N   5        5   
   -   5        $��GetPluginsButton��5      $ Get Plugins��60   #���   6E   $
��Left=�6t   $��	97,74,0,0q�6�   $	��Top=�6�    5      �� 5      $��32��6   $��32��6$   $#��../Images/Tools/plugins32.png��6/    5        5      $	 Get Plugins��6#   $*
 $Get More Plugins for Document.Editor��67   $# ../Images/Tools/plugins48.png��6c   5        5    
   -   5        $��WebsiteButton��5       $ Get Plugins��6-   #���   6B   $
��Left=�6q   $��
184,74,0,0q�6�   $	��Top=�6�    5!      �� 5"      $��32��6   $��32��6$   $"��../Images/Help/website32.png��6/    5$        5%      $	 Website��6#   $
 Visit the Website��63   $" ../Images/Help/website48.png��6L   5'      5(      5)      