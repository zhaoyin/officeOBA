   M S B A M L   `   `   `  ���� M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent   Fluent.Sample.Toolbars#   Fluent.Sample.Toolbars.Window   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     7Fluent$clr-namespace:Fluent;assembly=Fluent       � Title$  Fluent.Sample.Toolbars��$	��500��$	��250�����	#FFEBEDF0.��� .+�   Fluent.Ribbon     Tabs.    Fluent.RibbonTabItem     Header$	 Tab��   ReduceOrder$ Group,Group,Group��   Groups.    Fluent.RibbonGroupBox  -   $��Group��   Header$ Group��.@�   Fluent.RibbonToolBar     LayoutDefinitions *   $Fluent.RibbonToolBarLayoutDefinition  %   Fluent.RibbonAttachedProperties   
RibbonSize$ Large=�   Rows.    Fluent.RibbonToolBarRow  	   Children.	 +   %Fluent.RibbonToolBarControlDefinition  
   Target$
 spinner��   Width$	 127Q�  .	   $
 buttonGreen��  $
 
buttonGray��  $
 buttonYellow��  $
 buttonBrown��$ Middle=�  $ Middle=�.   .	   $
 spinner��$ 90Q�  .	   $
 buttonGreen��  $
 
buttonGray��  $
 buttonYellow��  $
 buttonBrown��  $ Small=�.   .	   $
 spinner��$ 45Q�  .	   $
 buttonGreen��  $
 
buttonGray��  .	   $
 buttonYellow��  $
 buttonBrown��   Children. 	   Fluent.Spinner	  -    	 Name$ spinner��
   Fluent.Button
  -    
 Name$ buttonGreen�� 
  Header$ Green�� 
  Icon$ Images\Green.png�� 
  	LargeIcon$ Images\GreenLarge.png��
  -   $ 
buttonGray��$
 Gray��$ Images\Gray.png��$ Images\GrayLarge.png��
  -   $ buttonYellow��$ Yellow��$ Images\Yellow.png��$ Images\YellowLarge.png��
  -   $ buttonBrown��$ Brown��$ Images\Brown.png��$ Images\BrownLarge.png��  $ Group to fill empty space��.@�� $	��300��