   M S B A M L   `   `   `  ����M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent  5   �   $  Cooperativeness.Documents.Editor   ShapeDialog   6	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    6   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6a   7Fluent$clr-namespace:Fluent;assembly=Fluent     6�     � Title$  Insert Shape��5       � Icon$ shape16.ico��6    � WindowStartupLocation$ CenterOwner=�6-    � ShowInTaskbar .  $	��480��6g   $	��640��6t   .��5      � ���	#7BFFFFFF.+�5   
      Fluent.Button  -   5        $��CancelButton��5   �      SizeDefinition/   )Fluent.Converters.SizeDefinitionConverter$ Middle 6      Icon$&  ../../Images/Common/cancel16.png��60      Header$ Cancel��6X   $��Right=�6h   $��	0,0,10,10q�6�   $��Bottom=�6�    �� IsCancel .  � ToolTip 5         Fluent.ScreenTip  5      	   Title$	 Cancel��6#   
   Text$!
 Cancel and Close the Dialog��62      Image$&  ../../Images/Common/cancel48.png��6U   5        5	   
   -   5        $��OKButton��5	   �   $ Middle 6   $" ../../Images/Common/ok16.png��60   $ OK��6T   $��Right=�6`   $��	0,0,77,10q�6|   $��Bottom=�6�    �� 	IsDefault .  5
        5      $	 OK��6#   $'
 !Close the Dialog and Insert Shape��6.   $" ../../Images/Common/ok48.png��6W   5         Fluent.ComboBox  5   
   -   5        $��TypeComboBox��5   P      Header$ Type:��6    �� 
IsEditable .  $��
10,10,10,0q�6<   $	��Top=�6d      Fluent.Spinner  5   
   -   5        $��SizeTextBox��5   l      Fluent.RibbonControl   Header$ Size:��6      Format$ 0��6(      Minimum$ 1Q�63      Maximum$
 5000Q�6?      Value$ 32Q�6N   $��	10,37,0,0q�6Y   $	��Top=�6   $
��Left=�6�   $	��100��6�   �� 5   
   -   5        $��
TextBlock3��5   N   $��23��6   $
��Left=�6    $��	10,64,0,0q�6;   $��Preview:��6`   $	��Top=�6p   �� 5   
   -   5        $��ScrollViewer1��5   ,   $��10,87,10,44q�6   ���	#7BFFFFFF$��Visible=�6X     5   
   -   5        $��BorderSizeTextBox��5   �   $ Border Size:��6   $ 0��6/   $ 1Q�6:   $
 5000Q�6F   $ 4Q�6U   $��Right=�6_   $��	0,37,10,0q�6{   $	��Top=�6�   $	��128��6�   5      5      