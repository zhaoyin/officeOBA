   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      X TXceed.Wpf.Toolkit, Version=2.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4C&clr-namespace:Xceed.Wpf.Toolkit.ThemesXceed.Wpf.Toolkit.Themes 5      5clr-namespace:Xceed.Wpf.ToolkitXceed.Wpf.Toolkit 5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   BusyIndicatorTemplate �� 
TargetType%   Xceed.Wpf.Toolkit.BusyIndicator'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard g� 	BeginTime g� Duration �� 
TargetName	 �� TargetProperty
 R� KeyTime R� Value  System.Windows.Visibility ��Name ��Name    OverlayStyle ��Name+  %Xceed.Wpf.Toolkit.Themes.ResourceKeys   ControlNormalBorderKey �� CornerRadius � RowDefinitions    BusyContent    BusyContentTemplate    ProgressBarStyle �� 
TargetType �� Property  PanelBackgroundBrush �� Value�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      .sys&clr-namespace:System;assembly=mscorlib  5      1themes&clr-namespace:Xceed.Wpf.Toolkit.Themes  5      )localclr-namespace:Xceed.Wpf.Toolkit  5        5      �� 5   
   $$ ../../Themes/Aero2/Common.xaml?�6   %M  &	        '   �    7  7   �� 5      "   64   .R�5    
   �  5!        5"      VisibilityStates��. 5#        
Hidden��. 5$      �� .��5%      T� $ 00:00:00X�6<   $ 00:00:00.001D�6Q   $ busycontent��6i   $	 (UIElement.Visibility)�6�   ..�5&   "   i� $
 00:00:00��69    5'   &    5(   *   	Collapsed=�6@   5*   #   5+      T� 5,      $ 00:00:00X�6<   $ 00:00:00.001D�6Q   $ overlay��6i   $	 (UIElement.Visibility)�6�   ..�5-   "   i� $
 00:00:00��69    5.   &    5/   *   	Collapsed=�6@   51   #   52      53      54        55      Visible��. 56      �� .��57      T� $ 00:00:00X�6<   $ 00:00:00.001D�6Q   $ busycontent��6i   $	 (UIElement.Visibility)�6�   ..�58   "   i� $
 00:00:00��69    59   &    5:   *   Visible=�6>   5<   #   5=      T� 5>      $ 00:00:00X�6<   $ 00:00:00.001D�6Q   $ overlay��6i   $	 (UIElement.Visibility)�6�   ..�5?   "   i� $
 00:00:00��69    5@   &    5A   *   Visible=�6>   5C   #   5D      5E      5F      5G        5H      BusyStatusStates��. 5I        Idle��. 5J      �� .��5K      T� $ 00:00:00X�6<   $ 00:00:00.001D�6Q   $ content��6i   $	 (Control.IsEnabled)�6�   ..�5L   "   i� $
 00:00:00��69    5M   &   ��5N   *   True��6<   5P   #   5Q      5R      5S        5T      Busy��. 5U      �� .��5V      T� $ 00:00:00X�6<   $ 00:00:00.001D�6Q   $ content��6i   $	 (Control.IsEnabled)�6�   ..�5W   "   i� $
 00:00:00��69    5X   &   ��5Y   *   	False��6=   5[   #   5\      5]      5^      5_      .+�5a      �� $ content��6   #��z��6.   #��z��6R   #��z��6�   #��z��6�   ��.  |�.  6(  �� 5b      $ overlay��6   #��z 6)   �� 5c      $ busycontent��6   ��5d      � 5e      $��Center=�6   $��Center=�68   .+�5f      �� 
�������$��1q�64   #���  6H   $ 0��6�   ._�5g      �� $��1q�6%   #��z��60   $ 0��6Z   ._�5h   "   � $	��150��6'    5i   &   �� 5j   *   �� 5k   *   $
��Auto �68   .+�5m   &   �� #��z 67   #��z 6_   $��8q�6�   � 5n   &   $��1��62   #��z 6?   5o   #   5p      5q      5r      5t      5u      5v      �� 5x      "   6   .�5y   
   ��  � ��8   6'   �� 5z   
    �@  BusyContent$ Please wait...��6(   �� 5{   
    � �� .  6&   �� 5|   
    � |� .  6&   �� 5}   
    �@  OverlayStyle 5~      �� 5      " ��6   .�5�      ��  � ��
 �����6-   �� 5�       �@J�Opacity$	 0.5Q�60   5�      5�      �� 5�   
    �@  ProgressBarStyle 5�      �� 5�      " �6   .�5�      ��  �@�IsIndeterminate . 68   �� 5�       � ��$ 15��6/   �� 5�       � ��$ 8,0,8,8q�6/   5�      5�      �� 5�   
    �@  DisplayAfter$ 
00:00:00.1h�6)   �� 5�   
    � ��$ Stretch=�60   �� 5�   
    � ��$ Stretch=�6.   �� 5�   
    � ��$ Stretch=�67   �� 5�   
    � ��$ Stretch=�65   �� 5�   
    � ��8  6%   5�      5�      