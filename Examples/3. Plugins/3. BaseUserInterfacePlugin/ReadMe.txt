The BaseUserInterfacePlugin project demonstrates basic integration capabilities
of plug-ins with the EwE6 user interface. 

The plug-in integrates itself in the EwE6 main menu and navigation tree, and 
brings up a form that docks itself into the EwE6 scientific interface. The form
is given the so called 'UIContext' of the running application, from which it 
borrows and displays the name of the current running model.

You can test and run this project by adding it to the EwE6 main solution:
 - Open the EwE6 / EwE6_express solution in Visual Studio
 - Add this plug-in project as an existing project to the solution
 - In the EwE6 startup project ScientificInterface add a reference to this plug-in project
