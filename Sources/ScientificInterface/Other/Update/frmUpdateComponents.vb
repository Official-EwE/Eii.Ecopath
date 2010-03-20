#Region " Imports "

Option Strict On
Imports System.Threading
Imports EwEPlugin

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Form, implements the interface that triggers a component update
''' </summary>
''' <remarks>
''' This form will start updating components automatically once shown, and 
''' will close automatically when all components have been verified and updated.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class frmUpdateComponents

#Region " Private vars "

    ''' <summary>The plug-in manager used to updates components.</summary>
    Private m_pm As cPluginManager = Nothing
    ''' <summary>The update thread.</summary>
    Private m_thrd As Thread = Nothing

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor; initializes a new instance of the update form.
    ''' </summary>
    ''' <param name="pm">The plug-in manager used to updates components.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal pm As cPluginManager)
        Me.InitializeComponent()
        Me.Text = My.Resources.GENERIC_CAPTION
        Me.m_pm = pm
    End Sub

#End Region ' Constructor

#Region " Framework overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        MyBase.OnLoad(e)
        ' Start listening to update events
        AddHandler Me.m_pm.AssemblyUpdating, AddressOf OnAssemblyUpdating
        ' Set initial message
        Me.UpdateControls("", 0)

    End Sub

    Protected Overrides Sub OnHandleDestroyed(ByVal e As System.EventArgs)
        ' Stop listening to update events
        RemoveHandler Me.m_pm.AssemblyUpdating, AddressOf OnAssemblyUpdating
        ' Done
        MyBase.OnHandleDestroyed(e)
    End Sub

    Protected Overrides Sub OnShown(ByVal e As System.EventArgs)
        MyBase.OnShown(e)

        ' Kick off update process
        If (Me.m_thrd Is Nothing) Then
            Me.m_thrd = New Thread(AddressOf UpdatePluginsThread)
            Me.m_thrd.Start()
        End If

    End Sub

#End Region ' Framework overrides

#Region " Events "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Abort button has been clicked.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnAbort(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAbort.Click
        If Me.m_thrd IsNot Nothing Then
            Try
                Me.m_thrd.Abort()
            Catch ex As Exception

            End Try
            Me.Close()
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Assembly update event handler.
    ''' </summary>
    ''' <param name="strName">Name of the component that is updated.</param>
    ''' <param name="sProgress">Update progress [0, 1]</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnAssemblyUpdating(ByVal strName As String, ByVal sProgress As Single)

        If Me.InvokeRequired Then
            Me.Invoke(New UpdateControlsDelegate(AddressOf UpdateControls), New Object() {strName, sProgress})
        Else
            Me.UpdateControls(strName, sProgress)
        End If

    End Sub

#End Region ' Events

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delegate to marshall updates from the update thread to the form.
    ''' </summary>
    ''' <param name="strName">Name of the component that is updated.</param>
    ''' <param name="sProgress">Update progress [0, 1]</param>
    ''' -----------------------------------------------------------------------
    Private Delegate Sub UpdateControlsDelegate(ByVal strName As String, ByVal sProgress As Single)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reflect updates from the update thread to the controls in the form.
    ''' </summary>
    ''' <param name="strName">Name of the component that is updated.</param>
    ''' <param name="sProgress">Update progress [0, 1]</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls(ByVal strName As String, ByVal sProgress As Single)

        If String.IsNullOrEmpty(strName) Then
            Me.m_lblInfo.Text = My.Resources.STATUS_PLEASE_WAIT
        Else
            Me.m_lblInfo.Text = String.Format(My.Resources.STATUS_UPDATE_CHECKING, strName)
            Me.m_pbProgress.Value = CInt(100 * sProgress)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delegate to marshall a close request to the form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Delegate Sub CloseDelegate()

#End Region ' Internals

#Region " Update thread "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Thread procedure to run updates.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdatePluginsThread()
        ' Run updates
        Me.m_pm.UpdatePlugins()
        ' Done, close form
        Me.Invoke(New CloseDelegate(AddressOf Me.Close))
    End Sub

#End Region ' Update thread

End Class

