'==============================================================================
'
' $Log: IDatabasePlugin.vb,v $
' Revision 1.1  2009/02/25 07:15:02  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwEUtils.Database

Namespace Data

    Public Interface IDatabasePlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Open a database connection
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Function Open(ByVal strName As String) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Close a database connection
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub Close()

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' States whether a plug-in has pending data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Function IsModified() As Boolean

    End Interface

End Namespace ' Data
