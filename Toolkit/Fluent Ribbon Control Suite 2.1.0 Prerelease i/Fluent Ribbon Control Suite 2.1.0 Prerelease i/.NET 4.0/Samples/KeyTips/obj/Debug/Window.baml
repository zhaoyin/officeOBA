   M S B A M L   `   `   `  ����M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent  5       Fluent.Sample.KeyTips"   Fluent.Sample.KeyTips.Window   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      7Fluent$clr-namespace:Fluent;assembly=Fluent     5        � Title$  Fluent.Sample.KeyTips��5      $	��500��5      $	��250��5      ���	#FFEBEDF0.��5	      � .+�5
         Fluent.Ribbon     QuickAccessItems 5          Fluent.QuickAccessMenuItem  5       m� 	IsChecked .    Target 5      ��  �� ElementName$ 	buttonRed��  5       .  5      �� $ 
buttonGray��   Tabs. 5         Fluent.RibbonTabItem     Fluent.KeyTip   Keys$ A��6      Header$ Auto Placement��5         ReduceOrder$ ZC,ZC,ZC��5      	   Groups.	 5         Fluent.RibbonGroupBox  -   5        
  Name$
 ZC��5      $ ZC��5         Header$ Group��5      .@�5         Fluent.SplitButton  -   5          Name$ 	buttonRed��5      $ R��5         Fluent.DropDownButton   Header$	 Red��5          Icon$ Images\Red.png��5!         	LargeIcon$ Images\RedLarge.png��5"      %   Fluent.RibbonAttachedProperties   RibbonSizeDefinition$ Large��5#      .@�5$      	   Fluent.MenuItem	  $ P��6   $
��Pink��5%       m� Icon$ Images\Pink.png��5&      	  5'      $ R��6   $	��Red��5(      $ Images\Red.png��5)      .@�5*   	   	  $ O��6   $��Orange��5+      $ Images\Orange.png��5,      5-   	   5.      
   Fluent.Button
  5/      -   5         
 Name$ 
buttonGray��5/      $ G��50       
  Header$
 Grey��51       
  Icon$ Images\Gray.png��52       
  	LargeIcon$ Images\GrayLarge.png��53      $ Large��54      
  55      $ O��6   $ Orange��56      $ Images\Orange.png��57      $ Images\OrangeLarge.png��58      $ Large��59      
  5:      $ P��6   $
 Pink��5;      $ Images\Pink.png��5<      $ Images\PinkLarge.png��5=      $ Large��5>      
  5?      $ Y��6   $ Yellow��5@      $ Images\Yellow.png��5A      $ Images\YellowLarge.png��5B      $ Large��5C      
  5D      $ B��6   $ Brown��5E      $ Images\Brown.png��5F      $ Images\BrownLarge.png��5G      $ Large��5H      
  5I      $ X1��6   $
 Gray��5J      $ Images\Gray.png��5K      $ Images\GrayLarge.png��5L      $ Middle��5M      
  5N      $ X2��6   $
 Gray��5O      $ Images\Gray.png��5P      $ Images\GrayLarge.png��5Q      $ Middle��5R      
  5S      $ X3��6   $
 Gray��5T      $ Images\Gray.png��5U      $ Images\GrayLarge.png��5V      $ Middle��5W      5X      5Y        5b      $ C��6   $ Custom Placement��5c      .	 5d        $ Group��6   .@�5e      
  $ Center��6   $ Images\Green.png��5f      $ Images\GreenLarge.png��5g         AutoPlacement .     HorizontalAlignment$ Center=�5i         VerticalAlignment$ Center=�5j      $ C��5k      
  5l      $
 Left��6   $ Images\Gray.png��5m      $ Images\GrayLarge.png��5n       .  $
 Left=�5p      $ Center=�5q      $ L��5r      
  5s      $	 Top��6   $ Images\Yellow.png��5t      $ Images\YellowLarge.png��5u       .  $ Center=�5w      $	 Top=�5x      $ T��5y      5z      5{         Menu 5~         Fluent.Backstage  5         Content. 5�          Fluent.BackstageTabControl  .@�5�         Fluent.BackstageTabItem  $ N��6       Header$	 New��5�        5�      $ P��6    $ Print��5�      
  5�      $ B��6   $
 Blue��5�      $ Images\Blue.png��5�      5�      5�      5�      �� 5�      $(��"* Press Alt or F10 to show KeyTips��6   $��Bottom=�68   $��10q�6S   5�      5�      