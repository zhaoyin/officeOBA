   M S B A M L   `   `   `  ����M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent  5       Fluent.Sample.Toolbars#   Fluent.Sample.Toolbars.Window   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      7Fluent$clr-namespace:Fluent;assembly=Fluent     5        � Title$  Fluent.Sample.Toolbars��5      $	��500��5      $	��250��5      ���	#FFEBEDF0.��5	      � .+�5
         Fluent.Ribbon     Tabs. 5         Fluent.RibbonTabItem     Header$	 Tab��6      ReduceOrder$ Group,Group,Group��5         Groups. 5         Fluent.RibbonGroupBox  -   5        $��Group��5         Header$ Group��5      .@�5         Fluent.RibbonToolBar     LayoutDefinitions 5      *   $Fluent.RibbonToolBarLayoutDefinition  5   	   %   Fluent.RibbonAttachedProperties   
RibbonSize$ Large=�6.      Rows. 5   
      Fluent.RibbonToolBarRow  	   Children.	 5      +   %Fluent.RibbonToolBarControlDefinition  
   Target$
 spinner��61      Width$	 127Q�5   .   5        5   
   .	 5        $
 buttonGreen��61     5      $
 
buttonGray��61     5      $
 buttonYellow��61     5      $
 buttonBrown��61   $ Middle=�5    .   5!      5"   
     5%   	   $ Middle=�6.   . 5&   
     .	 5'        $
 spinner��61   $ 90Q�5(   .   5)        5*   
   .	 5+        $
 buttonGreen��61     5,      $
 
buttonGray��61     5-      $
 buttonYellow��61     5.      $
 buttonBrown��61   5/      50   
     53   	   $ Small=�6.   . 54   
     .	 55        $
 spinner��61   $ 45Q�56   .   57        58   
   .	 59        $
 buttonGreen��61     5:      $
 
buttonGray��61   5;        5<   
   .	 5=        $
 buttonYellow��61     5>      $
 buttonBrown��61   5?      5@   
      Children. 5D      	   Fluent.Spinner	  -   5         	 Name$ spinner��5D      
   Fluent.Button
  5E      -   5         
 Name$ buttonGreen��5E       
  Header$ Green��5F       
  Icon$ Images\Green.png��5G       
  	LargeIcon$ Images\GreenLarge.png��5H      
  5I      -   5        $ 
buttonGray��5I      $
 Gray��5J      $ Images\Gray.png��5K      $ Images\GrayLarge.png��5L      
  5M      -   5        $ buttonYellow��5M      $ Yellow��5N      $ Images\Yellow.png��5O      $ Images\YellowLarge.png��5P      
  5Q      -   5        $ buttonBrown��5Q      $ Brown��5R      $ Images\Brown.png��5S      $ Images\BrownLarge.png��5T      5U      5V        5X      $ Group to fill empty space��6   .@�5Y      � $	��300��6   5Z      5[      5\      5]      5^      