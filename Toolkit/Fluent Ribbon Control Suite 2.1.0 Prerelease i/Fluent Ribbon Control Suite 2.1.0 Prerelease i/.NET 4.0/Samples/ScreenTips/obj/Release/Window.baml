   M S B A M L   `   `   `  ���� M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent   Fluent.Sample.ScreenTips%   Fluent.Sample.ScreenTips.Window   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     7Fluent$clr-namespace:Fluent;assembly=Fluent       � Title$  Fluent.Sample.ScreenTips��$	��500��$	��250�����	#FFEBEDF0.��� .+�   Fluent.Ribbon     QuickAccessItems     Fluent.QuickAccessMenuItem   m� 	IsChecked .    Target ��  �� ElementName$ 	buttonRed��   .  �� $ 
buttonGray��   Tabs.    Fluent.RibbonTabItem     Header$	 Tab��   Groups.    Fluent.RibbonGroupBox     Header$ Group��.@�   Fluent.Button  -   	  Name$	 
buttonGray��
   Header$

 Grey��   Icon$ Images\Gray.png��   	LargeIcon$ Images\GrayLarge.png�� � ToolTip    Fluent.ScreenTip     Title$
 Gray��   	HelpTopic$ Help for Gray ScreenTip��   Image$ Images\GrayLarge.png��   Text$I CThis ScreenTip is ribbon aligned. 
It has the image and handles F1.��  $
 Orange��{�.  $ Images\Orange.png��$ Images\OrangeLarge.png��   $ Orange��$	��250��$ Images\OrangeLarge.png��$8 2This control is disabled and has fixed width 250px��$ Help for Orange ScreenTip��   DisableReason$? 9This control is disabled to show 'disable reason' section��   Fluent.SplitButton  -     Name$ 	buttonRed��   Fluent.DropDownButton   Header$	 Red��   Icon$ Images\Red.png��   	LargeIcon$ Images\RedLarge.png��%	   Fluent.RibbonAttachedProperties 	  RibbonSizeDefinition$ Large��   $	 Red��   IsRibbonAligned .  $4 .This ScreenTip is NOT aligned to ribbon bottom��.@�
   Fluent.MenuItem
  $
��Pink�� m� Icon$ Images\Pink.png��   $
 Pink��$ Images\PinkLarge.png�� .  $- 'This ScreenTip is attached to menu item��
  $��Orange��$ Images\Orange.png��$ Regular Tooltip��