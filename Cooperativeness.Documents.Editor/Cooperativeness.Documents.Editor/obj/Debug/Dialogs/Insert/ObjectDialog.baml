   M S B A M L   `   `   `  ����M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent  5   �   $  Cooperativeness.Documents.Editor   ObjectDialog   6	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    6    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6b   7Fluent$clr-namespace:Fluent;assembly=Fluent     6�     � Title$  Insert Object��5      $	��255��6   $	��283��6(    � 
ResizeMode$ NoResize=�64    � ShowInTaskbar .   � WindowStartupLocation$ CenterOwner=�6`    � Icon$ 
object.ico��6�   .��5      � ���	#7BFFFFFF.+�5   
   �� -   5        $��StackPanel1��5   '   $��0,0,0,44q�6   ���	#3FFFFFFF J� Opacity$ 1Q�6Q   .+�5         Fluent.RadioButton  -   5        $��O_Button��5   D      Header$ Button��6!   
���   � d� 	IsChecked$
 TrueY�6T   $��Center=�6e     5      -   5        $��O_RadioButton��5   J   $ Radio Button��6!   
���   �$��Center=�6_     5      -   5        $��
O_CheckBox��5   G   $ 	Check Box��6!   
���   �$��Center=�6Y     5      -   5        $��O_TextBlock��5   H   $ 
Text Block��6!   
���   �$��Center=�6[   5	         Fluent.Button  5
   
   -   5        $��CancelButton��5
   {      SizeDefinition/   )Fluent.Converters.SizeDefinitionConverter$ Middle 6   	   Icon$&	  ../../Images/Common/cancel16.png��60   
   Header$
 Cancel��6X   $��	0,0,10,10q�6h   $��Bottom=�6�   $��Right=�6�    �� IsCancel .  � ToolTip 5         Fluent.ScreenTip  5         Title$ Cancel��6#      Text$! Cancel and Close the Dialog��62      Image$&  ../../Images/Common/cancel48.png��6U   5        5   
   -   5        $��OKButton��5   �   $ Middle 6   $"	 ../../Images/Common/ok16.png��60   $
 OK��6T   $��Right=�6`   $��	0,0,77,10q�6|   $��Bottom=�6�    5        5      $ OK��6#   $( "Close the Dialog and Insert Object��6.   $" ../../Images/Common/ok48.png��6X   5        5   
   -   5        $��Button1��5   �   $ Middle 6   $1	 +../../Images/Insert/Object/properties16.png��60   $
 
Properties��6c   $
��Left=�6w   $��	10,0,0,10q�6�   $��Bottom=�6�   5      5      