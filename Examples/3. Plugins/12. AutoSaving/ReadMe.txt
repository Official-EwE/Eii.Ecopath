The AutoSave plugin project demonstrates how a plug-in can integrate
with the EwE auto-save system. Through a central UI (accessible from 
Menu > Tools > Options > File management OR via the Auto-save button 
on the EwE main tool bar) users can see which modules of EwE will 
automatically write output to disk, and where this information will be
placed. Plug-ins can register themselves to the auto-save system; this 
example shows how to accomplish this.

You can test and run this project by adding it to the EwE6 main solution:
 - Open the EwE6 solution in Visual Studio
 - Add this plug-in project as an existing project to the solution
 - In the EwE6 startup project ScientificInterface add a reference to this plug-in project
