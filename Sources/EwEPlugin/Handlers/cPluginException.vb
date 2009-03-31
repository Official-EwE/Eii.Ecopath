'==============================================================================
'
' $Log: cPluginException.vb,v $
' Revision 1.3  2009/03/31 16:09:01  jeroens
' Added Assembly back in :p
'
' Revision 1.2  2009/03/31 14:54:08  jeroens
' Now really used
'
' Revision 1.1  2006/08/31 15:20:33  jeroens
' * Moved
'
' Revision 1.1  2006/08/08 14:11:50  jeroens
' + Initial version
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Plugin exception
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginException
    Inherits Exception

    Private m_assembly As cPluginAssembly = Nothing

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginException.
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="exception"></param>
    ''' <param name="assembly"></param>
    ''' ---------------------------------------------------------------------------
    Public Sub New(ByVal assembly As cPluginAssembly, ByVal strMessage As String, ByVal exception As Exception)
        MyBase.New(strMessage, exception)
        Me.m_assembly = assembly
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginException.
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="assembly"></param>
    ''' ---------------------------------------------------------------------------
    Public Sub New(ByVal assembly As cPluginAssembly, ByVal strMessage As String)
        Me.New(assembly, strMessage, Nothing)
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginException.
    ''' </summary>
    ''' <param name="exception"></param>
    ''' <param name="assembly"></param>
    ''' ---------------------------------------------------------------------------
    Public Sub New(ByVal assembly As cPluginAssembly, ByVal exception As Exception)
        Me.New(assembly, exception.Message)
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Get the assembly that caused the exception.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public ReadOnly Property Assembly() As cPluginAssembly
        Get
            Return Me.m_assembly
        End Get
    End Property

End Class
