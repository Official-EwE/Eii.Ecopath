'==============================================================================
'
' $Log: frmEwE.vb,v $
' Revision 1.1  2008/09/26 07:32:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/08/14 01:52:40  jeroens
' Form position stored and restored
'
' Revision 1.2  2008/07/16 15:12:59  jeroens
' Removed obsolete CLS compliancy fixes
'
' Revision 1.1  2008/06/04 00:55:48  jeroens
' Renamed
'
' Revision 1.11  2008/04/07 02:31:18  jeroens
' Cleaning up resources
'
' Revision 1.10  2007/12/09 22:15:16  jeroens
' * Simplified
'
' Revision 1.9  2007/11/02 22:20:07  jeroens
' * Uses StatusPanel-type of message handler registration
'
' Revision 1.8  2007/11/02 16:28:51  jeroens
' * Patched core message source update structure, must change similar to StatusPanel
'
' Revision 1.7  2007/10/19 02:19:35  jeroens
' * Changed abstract methods to overridable, class no longer MustInherit to allow derived forms to be edited in the Form designer.
'
' Revision 1.6  2007/10/16 14:22:19  jeroens
' * Fixed compiler warnings
'
' Revision 1.5  2007/10/15 15:17:56  jeroens
' * Responds to all message types
'
' Revision 1.4  2007/10/12 20:20:36  jeroens
' * Responds to time series messages
'
' Revision 1.3  2007/10/12 16:41:30  jeroens
' + Original message passed to OnCoreDataChanged
'
' Revision 1.2  2007/10/10 04:09:00  jeroens
' * Ok, the idea was brilliant but the execution sucked. Fixed silly bug...
'
' Revision 1.1  2007/10/10 02:28:31  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports WeifenLuo.WinFormsUI
Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared

#End Region

''' ---------------------------------------------------------------------------
''' <summary>
''' <para>Base class for forms in EwE6. This class provides a mechanism to respond to
''' core messages for updating its content without having to construct each
''' required message handler. Instead, a central message handling administration
''' dispatched messages to inherited forms.</para>
''' <para>
''' To build an EwE6 form based on this class to gain the benefits of automatic
''' message delivery, perform the following steps:
''' </para>
''' <list type="bullet">
''' <item><description>Inherit your form from EwE6Form,</description></item>
''' <item><description>In the Load event, specify the message source(s) that
''' the form class should respond to via 
''' <see cref="frmEwE.MessageSources">frmEwE.MessageSources</see></description>,</item>
''' <item><description>In the Unload event, clear the message sources(s) by
''' setting <see cref="frmEwE.MessageSources">frmEwE.MessageSources</see>
''' to Nothing,</description></item>
''' <item>Override <see cref="frmEwE.OnCoreMessage">frmEwE.OnCoreMessage</see>
''' and implement the response to the message.</item>
''' </list>
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmEwE
    : Inherits DockContent

#Region " Private helper classes "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Helper class; automatically instructs registered EwEForms to refresh their 
    ''' content whenever core data has been added or removed.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Private Class EwEFormRefresh

#Region " Internal admin "

        ''' <summary>Administration of registered forms per message source type.</summary>
        Private m_dictSourceToForm As New Dictionary(Of eMessageSource, List(Of frmEwE))

#End Region ' Internal admin

#Region " Singleton "

        ''' <summary>The one instance of this class.</summary>
        Private Shared __inst__ As EwEFormRefresh

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Private constructor to enforce singleton.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub New()
            ' Hmm, maybe this'd better be done by passing a core instance in?
            Me.Initialize(cCore.GetInstance())
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the singleton instance of this class.
        ''' </summary>
        ''' <returns>The singleton instance of this class.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetInstance() As EwEFormRefresh
            If EwEFormRefresh.__inst__ Is Nothing Then
                EwEFormRefresh.__inst__ = New EwEFormRefresh()
            End If
            Return EwEFormRefresh.__inst__
        End Function

#End Region ' Singleton

#Region " Public access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Register a form to automated refresh instructions.
        ''' </summary>
        ''' <param name="form">The <see cref="frmEwE">frmEwE</see> to register.</param>
        ''' <param name="messageSource">The <see cref="eMessageSource">message source</see> to monitor 
        ''' for <see cref="eMessageType.DataAddedOrRemoved">DataAddedOrRemoved</see> messages.</param>
        ''' <remarks>
        ''' A registered form will receive <see cref="frmEwE.OnCoreMessage">OnCoreMessage</see>
        ''' calls for <see cref="eMessageType.DataAddedOrRemoved">DataAddedOrRemoved</see>
        ''' messages originating from the given <paramref name="messageSource">message source</paramref>.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Sub RegisterForm(ByVal form As frmEwE, ByVal messageSource As eMessageSource)
            Dim lForms As List(Of frmEwE) = Nothing
            If Me.m_dictSourceToForm.ContainsKey(messageSource) Then
                lForms = Me.m_dictSourceToForm(messageSource)
            Else
                lForms = New List(Of frmEwE)
                Me.m_dictSourceToForm.Add(messageSource, lForms)
            End If

            If Not lForms.Contains(form) Then lForms.Add(form)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Unregister a form from automated refresh instructions.
        ''' </summary>
        ''' <param name="form">The <see cref="frmEwE">frmEwE</see> to unregister.</param>
        ''' <param name="messageSource">The <see cref="eMessageSource">message source</see> to 
        ''' stop monitoring for <see cref="eMessageType.DataAddedOrRemoved">DataAddedOrRemoved</see>
        ''' messages.</param>
        ''' <remarks>
        ''' The form will no longer receive <see cref="frmEwE.OnCoreMessage">OnCoreMessage</see>
        ''' calls for <see cref="eMessageType.DataAddedOrRemoved">DataAddedOrRemoved</see>
        ''' messages originating from the given <paramref name="messageSource">message source</paramref>.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Sub UnregisterForm(ByVal form As frmEwE, ByVal messageSource As eMessageSource)
            Debug.Assert(Me.m_dictSourceToForm.ContainsKey(messageSource), String.Format("Form not defined for message source {0}", messageSource.ToString()))
            Me.m_dictSourceToForm(messageSource).Remove(form)
        End Sub

#End Region ' Public access

#Region " Message handling "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize this class to listen to messages from an EwE Core instance.
        ''' </summary>
        ''' <param name="core">The <see cref="cCore">Core instance</see> to listen to.</param>
        ''' -----------------------------------------------------------------------
        Private Sub Initialize(ByVal core As cCore)
            Me.ConfigMessageHandlers(True)
        End Sub

        Dim m_dtMessageHanders As New Dictionary(Of eMessageSource, cMessageHandler)

        Private Sub ConfigMessageHandler(ByVal src As eMessageSource, ByVal bSet As Boolean)

            Dim mh As cMessageHandler = Nothing
            Dim core As cCore = cCore.GetInstance()

            If (src = eMessageSource.NotSet) Then Return

            If bSet Then
                mh = New cMessageHandler(AddressOf AllMessagesHandler, src, eMessageType.Any)
                Me.m_dtMessageHanders(src) = mh
                core.Messages.AddMessageHandler(mh)
            Else
                mh = Me.m_dtMessageHanders(src)
                Me.m_dtMessageHanders.Remove(src)
                core.Messages.RemoveMessageHandler(mh)
                mh = Nothing
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Hook up to core messages
        ''' </summary>
        ''' <param name="bSet">True to set, False to clear.</param>
        ''' -------------------------------------------------------------------
        Private Sub ConfigMessageHandlers(ByVal bSet As Boolean)

            ' Set up message handlers
            For Each src As eMessageSource In [Enum].GetValues(GetType(eMessageSource))
                Me.ConfigMessageHandler(src, bSet)
            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Universal messages listener.
        ''' </summary>
        ''' <param name="msg">The message to listen to.</param>
        ''' -------------------------------------------------------------------
        Private Sub AllMessagesHandler(ByRef msg As cMessage)

            Dim lForms As List(Of frmEwE) = Nothing

            If Me.m_dictSourceToForm.ContainsKey(msg.Source) Then
                lForms = Me.m_dictSourceToForm(msg.Source)
                If lForms IsNot Nothing Then
                    For Each form As frmEwE In lForms
                        form.OnCoreMessage(msg)
                    Next
                End If
            End If

        End Sub

#End Region ' Message handling 

    End Class

#End Region

#Region " Private variables "

    ''' <summary>Core state that determines the enabled state of a form.</summary>
    Private m_coreExecutionState As eCoreExecutionState = eCoreExecutionState.Idle
    ''' <summary>Array of message sources that invalidate the information displayed in a form.</summary>
    Private m_aMessageSources As eMessageSource() = Nothing
    ''' <summary>Flag stating whether this is an input grid.</summary>
    Private m_bIsInputForm As Boolean = False

#End Region ' Private variables

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Default constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        ' Call the fancy constructor
        Me.New(My.Resources.HEADER_EMPTY_PANEL)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fancy-schmanzy constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strText As String)
        ' Text gets displayed at the tab window
        Me.TabText = strText
        ' Text gets displayed at the window's MDI list
        Me.Text = strText
    End Sub

#End Region ' Constructors

#Region " Form events "

    Private Sub frmEwE_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        cFormPositionSettings.GetInstance().Store(Me)
    End Sub

    Private Sub frmEwE_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        cFormPositionSettings.GetInstance().Apply(Me)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Disposed handler, cleans up
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub EwEForm_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        ' Release message sources
        Me.MessageSources = Nothing
    End Sub

#End Region ' Form events

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Method that is called whenever core <see cref="eMessageType.DataAddedOrRemoved">Data Added Or Removed</see>
    ''' messages arrive from any <see cref="m_aMessageSources">Message Source</see> that
    ''' provide the data for a form instance. By implementing this method, inheriting forms
    ''' can entirely or selectively repopulate to stay in sync with the EwE core.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub OnCoreMessage(ByVal msg As cMessage)
    End Sub

    <CLSCompliant(False)> _
 Public Overridable Property CoreExecutionState() As eCoreExecutionState
        Get
            Return Me.m_coreExecutionState
        End Get
        Set(ByVal value As eCoreExecutionState)
            Me.m_coreExecutionState = value
        End Set
    End Property

#End Region ' Overrides

#Region " Core messages "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the message sources that supply this form with data. See
    ''' <see cref="OnCoreMessage">OnCoreMessage</see> for a desciption
    ''' how these flags are being used.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property MessageSources() As eMessageSource()
        Get
            Return m_aMessageSources
        End Get

        Set(ByVal value As eMessageSource())
            Dim fr As EwEFormRefresh = EwEFormRefresh.GetInstance()

            ' Detach
            If Me.m_aMessageSources IsNot Nothing Then
                For Each ms As eMessageSource In Me.m_aMessageSources
                    If ms <> eMessageSource.NotSet Then
                        fr.UnregisterForm(Me, ms)
                    End If
                Next
            End If

            ' Remember new
            Me.m_aMessageSources = value

            ' Attach
            If Me.m_aMessageSources IsNot Nothing Then
                For Each ms As eMessageSource In Me.m_aMessageSources
                    If ms <> eMessageSource.NotSet Then
                        fr.RegisterForm(Me, ms)
                    End If
                Next
            End If

        End Set
    End Property

#End Region ' Core messages

#Region " Share and enjoy "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether this form is an input form (true) or output form (false).
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Function IsInputForm(ByVal state As eCoreExecutionState) As Boolean
        Return (state = eCoreExecutionState.EcopathLoaded) Or _
               (state = eCoreExecutionState.EcosimLoaded) Or _
               (state = eCoreExecutionState.EcospaceLoaded) Or _
               (state = eCoreExecutionState.EcotracerLoaded)
    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether this form is an input form (true) or output form (false).
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Function IsOutputForm(ByVal state As eCoreExecutionState) As Boolean
        Return (state = eCoreExecutionState.EcopathCompleted) Or _
               (state = eCoreExecutionState.EcosimCompleted) Or _
               (state = eCoreExecutionState.EcospaceCompleted)
    End Function

#End Region ' Share and enjoy

End Class
