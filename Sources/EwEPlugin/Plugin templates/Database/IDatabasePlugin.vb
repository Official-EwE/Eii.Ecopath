Option Strict On

Namespace Data

    ''' =======================================================================
    ''' <summary>
    ''' Plug-in point to allow plug-ins to track database operations with EwE.
    ''' </summary>
    ''' =======================================================================
    Public Interface IDatabasePlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Execution interface for the EwE open database plug-in point. This 
        ''' method is invoked whenever the EwE core opens a database connection.
        ''' </summary>
        ''' <returns>
        ''' True if the plug-in point executed succesfully, or False otherwise.
        ''' This is rather tricky; returning False here may stop EwE6 from being
        ''' </returns>
        ''' -----------------------------------------------------------------------
        Function Open(ByVal strName As String) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Execution interface for the EwE close database plug-in point. This 
        ''' method is invoked whenever the EwE core closes a database connection.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub Close()

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Execution interface for a plug-in point to report whether is has any
        ''' pending changes. This method is invoked whenever the EwE core polls
        ''' for unsaved modifications prior to undertaking actions that may cause 
        ''' data to be lost.
        ''' </summary>
        ''' <returns>
        ''' True if a plug-in has unsaved data, False otherwise.
        ''' </returns>
        ''' -----------------------------------------------------------------------
        Function IsModified() As Boolean

    End Interface

End Namespace ' Data
