   M S B A M L   `   `   `  ����M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent  5       Fluent.Sample.Toolbars#   Fluent.Sample.Toolbars.Window   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      7Fluent$clr-namespace:Fluent;assembly=Fluent     5        � Title$  Fluent.Sample.Toolbars��5      $	��500��5      $	��250��5      ���	#FFEBEDF0.��5	      � .+�5
         Fluent.Ribbon     Tabs. 5         Fluent.RibbonTabItem     Header$	 Tab��6      ReduceOrder$ Group,Group,Group��5         Groups. 5         Fluent.RibbonGroupBox  -   5          Name$ Group��5         Header$ Group��5      .@�5         Fluent.RibbonToolBar     LayoutDefinitions 5      *   $Fluent.RibbonToolBarLayoutDefinition  5   	   %   Fluent.RibbonAttachedProperties   
RibbonSize$ Large=�6.   	   Rows.	 5   
      Fluent.RibbonToolBarRow  
   Children.
 5      +   %Fluent.RibbonToolBarControlDefinition     Target$ spinner��61      Width$	 127Q�5   .   5        5   
   .
 5        $ buttonGreen��61     5      $ 
buttonGray��61     5      $ buttonYellow��61     5      $ buttonBrown��61   $ Middle=�5   .   5      5   
     5"   	   $ Middle=�6.   .	 5#   
     .
 5$        $ spinner��61   $ 90Q�5%   .   5&        5'   
   .
 5(        $ buttonGreen��61     5)      $ 
buttonGray��61     5*      $ buttonYellow��61     5+      $ buttonBrown��61   5,      5-   
     50   	   $ Small=�6.   .	 51   
     .
 52        $ spinner��61   $ 45Q�53   .   54        55   
   .
 56        $ buttonGreen��61     57      $ 
buttonGray��61   58        59   
   .
 5:        $ buttonYellow��61     5;      $ buttonBrown��61   5<      5=   
      Children. 5A      	   Fluent.Spinner	  -   5         	 Name$ spinner��5A      
   Fluent.Button
  5B      -   5         
 Name$ buttonGreen��5B       
  Header$ Green��5C       
  Icon$ Images\Green.png��5D       
  	LargeIcon$ Images\GreenLarge.png��5E      
  5F      -   5        $ 
buttonGray��5F      $
 Gray��5G      $ Images\Gray.png��5H      $ Images\GrayLarge.png��5I      
  5J      -   5        $ buttonYellow��5J      $ Yellow��5K      $ Images\Yellow.png��5L      $ Images\YellowLarge.png��5M      
  5N      -   5        $ buttonBrown��5N      $ Brown��5O      $ Images\Brown.png��5P      $ Images\BrownLarge.png��5Q      5R      5S        5T      $ Group to fill empty space��6   .@�5U      � $	��300��6   5V      5W      5X      5Y      5Z      