'==============================================================================
'
' $Log: ApplicationHelp.vb,v $
' Revision 1.1  2008/09/26 07:32:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2007/08/02 18:36:01  jeroens
' * Removed useless helpprovider class, built it myself to implement search, index, TOC
'
' Revision 1.2  2007/08/02 02:57:40  jeroens
' * Fixed help doc internal silly namepace issue; need to fix properly
'
' Revision 1.1  2007/03/25 13:15:51  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports System.Windows.Forms
Imports System.IO

''' -----------------------------------------
''' <summary>
''' Light-weight implementation of F1-driven application-wide help support.
''' </summary>
''' <remarks>
''' Note that this implementation does not support the use of multiple
''' help documents, which in case of EwE plugins might be desirable.
''' </remarks>
''' -----------------------------------------
Public Class ApplicationHelp
    Implements IMessageFilter

    ''' <summary>The owner of the app help.</summary>
    Private m_ctlOwner As Control = Nothing
    ''' <summary>Local help file.</summary>
    Private m_strHelpFile As String = ""
    ''' <summary>Help URL to invoke for a control without help text set.</summary>
    Private m_strDefaultHelpURL As String = ""
    ''' <summary>Subdirectory for content pages within the help file.</summary>
    Private m_strHelpRoot As String = ""
    ''' <summary>Control that currently has the help focus.</summary>
    Private m_ctlContext As Control = Nothing
    ''' <summary>Dictionary of help topics.</summary>
    Private m_dtHelpTopics As New Dictionary(Of Control, String)

    ''' -----------------------------------------
    ''' <summary>
    ''' Private constructor to enforce Singleton
    ''' </summary>
    ''' <param name="strHelpFile">Path to the help file to use.</param>
    ''' <param name="strDefaultHelpURL">Default help page URL.</param>
    ''' <param name="strHelpRoot">In-help subdirectory for help content pages.</param>
    ''' -----------------------------------------
    Public Sub New(ByVal ctlOwner As Control, ByVal strHelpFile As String, _
            Optional ByVal strDefaultHelpURL As String = "", Optional ByVal strHelpRoot As String = "")

        ' Remember owner
        Me.m_ctlOwner = ctlOwner
        ' Set help file
        Me.m_strHelpFile = strHelpFile
        ' Set default help url
        Me.m_strDefaultHelpURL = strDefaultHelpURL
        ' Set help root
        Me.m_strHelpRoot = strHelpRoot

        ' Start listening for 'F1' key presses
        System.Windows.Forms.Application.AddMessageFilter(Me)

    End Sub

    ''' -----------------------------------------
    ''' <summary>
    ''' Set the help URL to display for a particular control.
    ''' </summary>
    ''' <param name="ctl">The control to set the help URL for.</param>
    ''' <remarks>Note that this method does NOT capture the help focus.</remarks>
    ''' -----------------------------------------
    Public Property HelpTopic(ByVal ctl As Control) As String
        Get
            If Me.m_dtHelpTopics.ContainsKey(ctl) Then Return Me.m_dtHelpTopics(ctl)
            Return Me.m_strDefaultHelpURL
        End Get

        Set(ByVal strURL As String)
            If Me.m_dtHelpTopics.ContainsKey(ctl) Then Me.m_dtHelpTopics.Remove(ctl)
            If Not String.IsNullOrEmpty(strURL) Then Me.m_dtHelpTopics.Add(ctl, strURL)
        End Set
    End Property

    Public Property ActiveHelpControl() As Control
        Get
            Return Me.m_ctlContext
        End Get
        Set(ByVal ctl As Control)
            Me.m_ctlContext = ctl
        End Set
    End Property

    Public Sub ShowHelp(ByVal navType As HelpNavigator)

        Dim ctl As Control = Me.m_ctlContext

        If ctl Is Nothing Then ctl = Me.m_ctlOwner

        Select Case navType

            Case HelpNavigator.Topic
                If ctl Is Nothing Then Return
                Help.ShowHelp(ctl, Me.m_strHelpFile, Path.Combine(Me.m_strHelpRoot, Me.HelpTopic(ctl)))

            Case HelpNavigator.Find
                Help.ShowHelp(ctl, Me.m_strHelpFile, HelpNavigator.Find, ctl.Text)

            Case HelpNavigator.KeywordIndex
                Help.ShowHelpIndex(ctl, Me.m_strHelpFile)

            Case HelpNavigator.TableOfContents
                Help.ShowHelp(ctl, Me.m_strHelpFile, HelpNavigator.TableOfContents, ctl.Text)

            Case Else
                Debug.Assert(False, String.Format("Help mode {0} not supported", navType))

        End Select

    End Sub

    Public Function PreFilterMessage(ByRef m As System.Windows.Forms.Message) As Boolean _
            Implements System.Windows.Forms.IMessageFilter.PreFilterMessage

        Select Case m.Msg
            Case &H100 ' WM_KEYDOWN
                If CInt(m.WParam) = CInt(Keys.F1) Then
                    Me.ShowHelp(HelpNavigator.Topic)
                End If
        End Select
    End Function

End Class
