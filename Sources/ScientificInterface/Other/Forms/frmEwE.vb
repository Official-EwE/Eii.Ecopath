#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports WeifenLuo.WinFormsUI
Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared
Imports System.ComponentModel
Imports System.Windows.Forms

#End Region ' Imports

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
''' <see cref="frmEwE.CoreComponents">frmEwE.CoreComponents</see></description>,</item>
''' <item><description>In the Unload event, clear the message sources(s) by
''' setting <see cref="frmEwE.CoreComponents">frmEwE.CoreComponents</see>
''' to Nothing,</description></item>
''' <item>Override <see cref="frmEwE.OnCoreMessage">frmEwE.OnCoreMessage</see>
''' and implement the response to the message.</item>
''' </list>
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmEwE
    Inherits DockContent
    Implements IUIElement

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
        Private m_dictSourceToForm As New Dictionary(Of eCoreComponentType, List(Of frmEwE))
        Private m_so As System.Threading.SynchronizationContext = Nothing
        Private m_core As cCore = Nothing

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
            Me.m_so = AppLauncher.GetInstance().SyncObject
            Me.m_core = cCore.GetInstance()

            Me.Initialize()
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
        ''' <param name="messageSource">The <see cref="eCoreComponentType">message source</see> to monitor 
        ''' for <see cref="eMessageType.DataAddedOrRemoved">DataAddedOrRemoved</see> messages.</param>
        ''' <remarks>
        ''' A registered form will receive <see cref="frmEwE.OnCoreMessage">OnCoreMessage</see>
        ''' calls for <see cref="eMessageType.DataAddedOrRemoved">DataAddedOrRemoved</see>
        ''' messages originating from the given <paramref name="messageSource">message source</paramref>.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Sub RegisterForm(ByVal form As frmEwE, ByVal messageSource As eCoreComponentType)
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
        ''' <param name="messageSource">The <see cref="eCoreComponentType">message source</see> to 
        ''' stop monitoring for <see cref="eMessageType.DataAddedOrRemoved">DataAddedOrRemoved</see>
        ''' messages.</param>
        ''' <remarks>
        ''' The form will no longer receive <see cref="frmEwE.OnCoreMessage">OnCoreMessage</see>
        ''' calls for <see cref="eMessageType.DataAddedOrRemoved">DataAddedOrRemoved</see>
        ''' messages originating from the given <paramref name="messageSource">message source</paramref>.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Sub UnregisterForm(ByVal form As frmEwE, ByVal messageSource As eCoreComponentType)
            Debug.Assert(Me.m_dictSourceToForm.ContainsKey(messageSource), String.Format("Form not defined for message source {0}", messageSource.ToString()))
            Me.m_dictSourceToForm(messageSource).Remove(form)
        End Sub

#End Region ' Public access

#Region " Message handling "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize this class to listen to messages from an EwE Core instance.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub Initialize()
            Me.ConfigMessageHandlers(True)
        End Sub

        Dim m_dtMessageHanders As New Dictionary(Of eCoreComponentType, cMessageHandler)

        Private Sub ConfigMessageHandler(ByVal src As eCoreComponentType, ByVal bSet As Boolean)

            Dim mh As cMessageHandler = Nothing

            If (src = eCoreComponentType.NotSet) Then Return

            If bSet Then
                mh = New cMessageHandler(AddressOf AllMessagesHandler, src, eMessageType.Any, Me.m_so)
                Me.m_dtMessageHanders(src) = mh
                Me.m_core.Messages.AddMessageHandler(mh)
            Else
                mh = Me.m_dtMessageHanders(src)
                Me.m_dtMessageHanders.Remove(src)
                Me.m_core.Messages.RemoveMessageHandler(mh)
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
            For Each src As eCoreComponentType In [Enum].GetValues(GetType(eCoreComponentType))
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

    Private m_uic As cUIContext = Nothing

    ''' <summary>Core state that determines the enabled state of a form.</summary>
    Private m_coreExecutionState As eCoreExecutionState = eCoreExecutionState.Idle
    ''' <summary>Array of message sources that invalidate the information displayed in a form.</summary>
    Private m_aMessageSources As eCoreComponentType() = Nothing
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
    End Sub

#End Region ' Constructors

#Region " Form overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form load event override, retrieves and applies the original form position.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)
        cFormPositionSettings.GetInstance().Apply(Me)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form close event override, stores the final form position.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        cFormPositionSettings.GetInstance().Store(Me)
        Me.CoreComponents = Nothing
        MyBase.OnFormClosed(e)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cUIContext">UI context</see> that 
    ''' </summary>
    ''' <remarks>
    ''' Override this method to connect to the EwE Core and other UI-context
    ''' settings.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overridable Property UIContext() As cUIContext _
        Implements IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Set(ByVal value As cUIContext)
            Me.m_uic = value
        End Set
    End Property

#End Region ' Form overrides

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the core execution state that a form needs for its content.
    ''' </summary>
    ''' -----------------------------------------------------------------------
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
    Public Property CoreComponents() As eCoreComponentType()
        Get
            Return m_aMessageSources
        End Get

        Set(ByVal value As eCoreComponentType())
            Dim fr As EwEFormRefresh = EwEFormRefresh.GetInstance()

            ' Detach
            If Me.m_aMessageSources IsNot Nothing Then
                For Each ms As eCoreComponentType In Me.m_aMessageSources
                    If ms <> eCoreComponentType.NotSet Then
                        fr.UnregisterForm(Me, ms)
                    End If
                Next
            End If

            ' Remember new
            Me.m_aMessageSources = value

            ' Attach
            If Me.m_aMessageSources IsNot Nothing Then
                For Each ms As eCoreComponentType In Me.m_aMessageSources
                    If ms <> eCoreComponentType.NotSet Then
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Flag stating whether a form is used to trigger model runs from.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property IsRunForm() As Boolean
        Get
            Return False
        End Get
    End Property

#End Region ' Share and enjoy

End Class
