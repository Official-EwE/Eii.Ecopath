'==============================================================================
'
' $Log: cPluginException.vb,v $
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

    Private m_bEnabled As Boolean = True

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginException.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Sub New(ByVal strMessage As String)
        Me.New(strMessage, Nothing)
    End Sub

    Public Sub New(ByVal strMessage As String, ByVal exception As Exception)
        MyBase.New(strMessage, exception)
    End Sub

    Public Sub New(ByVal exception As Exception)
        Me.New(exception.Message)
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Gets whether the plug-in assembly enabled state after processing the exception.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Property Enabled() As Boolean
        Get
            Return Me.m_bEnabled
        End Get
        Set(ByVal value As Boolean)
            Me.m_bEnabled = value
        End Set
    End Property

End Class
