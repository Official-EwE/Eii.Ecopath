#Region " Imports "

Option Strict On

Imports System.IO
Imports System.Windows.Forms
Imports EwEUtils.Win32Api

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Light-weight implementation of F1-driven application-wide help support.
''' </summary>
''' <remarks>
''' Note that this implementation does not support the use of multiple
''' help documents, which in case of EwE plugins might be desirable.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cHelp
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Private constructor to enforce Singleton
    ''' </summary>
    ''' <param name="strHelpFile">Path to the help file to use.</param>
    ''' <param name="strDefaultHelpURL">Default help page URL.</param>
    ''' <param name="strHelpRoot">In-help subdirectory for help content pages.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal ctlOwner As Control, _
                   ByVal strHelpFile As String, _
                   Optional ByVal strDefaultHelpURL As String = "", _
                   Optional ByVal strHelpRoot As String = "")

        ' Remember owner
        Me.m_ctlOwner = ctlOwner
        ' Set help file
        Me.m_strHelpFile = strHelpFile
        ' Set default help url
        Me.m_strDefaultHelpURL = strDefaultHelpURL
        ' Set help root
        Me.m_strHelpRoot = strHelpRoot

        ' Start listening for 'F1' key presses
        Application.AddMessageFilter(Me)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the help URL to display for a particular control.
    ''' </summary>
    ''' <param name="ctl">The control to set the help URL for.</param>
    ''' <remarks>Note that this method does NOT capture the help focus.</remarks>
    ''' -----------------------------------------------------------------------
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the control that is currently active for displaying help.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ActiveHelpControl() As Control
        Get
            Return Me.m_ctlContext
        End Get
        Set(ByVal ctl As Control)
            Me.m_ctlContext = ctl
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Show help!
    ''' </summary>
    ''' <param name="navType"></param>
    ''' -----------------------------------------------------------------------
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="m"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Protected Function PreFilterMessage(ByRef message As Message) As Boolean _
        Implements IMessageFilter.PreFilterMessage

        Select Case CInt(message.Msg)

            Case Win32.WM.WM_KEYDOWN
                If CInt(message.WParam) = CInt(Keys.F1) Then
                    Me.ShowHelp(HelpNavigator.Topic)
                End If

        End Select

    End Function

End Class
