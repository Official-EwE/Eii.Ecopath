'==============================================================================
'
' $Log: cPluginException.vb,v $
' Revision 1.1  2008/09/26 07:31:04  sherman
' --== DELETED HISTORY ==--
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

    ''' <summary>Assembly where the exception occurred.</summary>
    Private m_ass As cPluginAssembly = Nothing
    ''' <summary>Plugin where the exception occurred.</summary>
    Private m_ip As IPlugin = Nothing

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginException.
    ''' </summary>
    ''' <param name="strMessage">Message</param>
    ''' <param name="ass"><see cref="cPluginAssembly">Assembly</see> where
    ''' this exception occurred.</param>
    ''' <param name="ip"><see cref="IPlugin">Plugin</see> where the exception
    ''' occurred.</param>
    ''' ---------------------------------------------------------------------------
    Public Sub New(ByVal strMessage As String, _
        ByRef ass As cPluginAssembly, ByRef ip As IPlugin)

        MyBase.New(strMessage)

        Me.m_ass = ass
        Me.m_ip = ip
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the <see cref="cPluginAssembly">Plugin assembly</see> where the
    ''' exception occurred.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public ReadOnly Property Assembly() As cPluginAssembly
        Get
            Return Me.m_ass
        End Get
    End Property

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the <see cref="IPlugin">Plugin</see> where the exception occurred.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public ReadOnly Property Plugin() As IPlugin
        Get
            Return Me.m_ip
        End Get
    End Property

End Class
