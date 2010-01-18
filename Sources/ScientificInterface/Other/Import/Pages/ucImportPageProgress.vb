#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.Wizard
Imports System.Threading

#End Region ' Imports

Namespace Import

    ''' =======================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' =======================================================================
    Public Class ucImportPageProgress
        Implements IWizardPage

#Region " Private vars "

        ''' <summary>The wizard to operate on.</summary>
        Private m_wizard As cImportWizard = Nothing
        ''' <summary>Core message handler to intercept progress messages.</summary>
        Private m_mh As cMessageHandler = Nothing
        ''' <summary>Sync object to deal with progress messages.</summary>
        Private m_syncobj As SynchronizationContext = Nothing
        ''' <summary>Importing state flag.</summary>
        Private m_bImporting As Boolean = False

        Private m_threadImport As Thread = Nothing

#End Region ' Private vars

#Region " IWizardPage implementation "

        Public Sub Init(ByVal wizard As cWizard) _
            Implements IWizardPage.Init

            ' Sanity checks
            Debug.Assert(TypeOf (wizard) Is cImportWizard)

            Me.m_wizard = DirectCast(wizard, cImportWizard)

            ' Make sure we have a sync object
            Me.m_syncobj = SynchronizationContext.Current
            ' If there is no current context then create a new one on this thread.
            If (Me.m_syncobj Is Nothing) Then
                Me.m_syncobj = New System.Threading.SynchronizationContext()
            End If

            Me.m_mh = New cMessageHandler(AddressOf Me.ProgressMessageHandler, _
                                          eCoreComponentType.DataSource, _
                                          eMessageType.Any, Me.m_syncobj)
            Me.m_wizard.Core.Messages.AddMessageHandler(Me.m_mh)

            Me.StartImport()

        End Sub

        Public Sub Close() _
            Implements IWizardPage.Close

            Me.m_wizard.Core.Messages.RemoveMessageHandler(Me.m_mh)
            Me.m_mh = Nothing

        End Sub

        Public Function AllowNavBack() As Boolean _
            Implements IWizardPage.AllowNavBack
            Return True
        End Function

        Public Function AllowNavForward() As Boolean _
            Implements IWizardPage.AllowNavForward
            Return True
        End Function

        Public Function IsBusy() As Boolean _
            Implements IWizardPage.IsBusy
            Return (Me.m_bImporting = True)
        End Function

#End Region ' IWizardPage implementation

#Region " Events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Standard Core messages handler where all property updates are triggered
        ''' </summary>
        ''' <param name="msg">An arriving message</param>
        ''' -------------------------------------------------------------------
        Private Sub ProgressMessageHandler(ByRef msg As cMessage)
            If Not TypeOf msg Is cProgressMessage Then Return

            Try
                Dim pmsg As cProgressMessage = DirectCast(msg, cProgressMessage)
                Me.m_pb.Maximum = 0
                Me.m_pb.Maximum = 100
                Me.m_pb.Value = Math.Max(0, Math.Min(100, CInt(100 * pmsg.Progress)))

                Me.m_lbProgress.Text = pmsg.Message
            Catch ex As Exception

            End Try

            Me.Refresh()

        End Sub

#End Region ' Events

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the page is importing.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property IsImporting() As Boolean
            Get
                Return Me.m_bImporting
            End Get
            Set(ByVal value As Boolean)
                If Me.InvokeRequired Then

                End If
                Me.m_bImporting = value
                Me.m_wizard.PageChanged(Me)

            End Set
        End Property

#Region " Threaded import "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Import all user selected models.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub StartImport()

            Me.m_threadImport = New Thread(AddressOf PerformImportThread)
            Me.m_threadImport.Start()

        End Sub

        Private Sub PerformImportThread()

            Me.SetImportStatus(True, "")

            For Each setting As cImportWizard.cImportSettings In Me.m_wizard.ImportSettings
                If setting.Import Then
                    cApplicationStatusNotifier.SetStatusText("Importing " & setting.EwE6ModelName & "...", TriState.True)
                    Me.SetImportStatus(True, "Importing " & setting.EwE6ModelName)
                    If Me.m_wizard.Import(setting) Then
                        Me.SetImportStatus(True, "Imported " & setting.EwE6ModelName & " succesfully")
                    Else
                        Me.SetImportStatus(True, "Imported of " & setting.EwE6ModelName & " failed")
                    End If
                    cApplicationStatusNotifier.SetStatusText("", TriState.False)
                End If
            Next setting

            Me.SetImportStatus(False, "")

        End Sub

        Private Delegate Sub SetImportStatusDelegate(ByVal bImporting As Boolean, ByVal strStatus As String)

        Private Sub SetImportStatus(ByVal bImporting As Boolean, ByVal strStatus As String)
            If Me.InvokeRequired Then
                Me.Invoke(New SetImportStatusDelegate(AddressOf Me.SetImportStatus), New Object() {bImporting, strStatus})
            Else
                Me.IsImporting = bImporting

                If Not String.IsNullOrEmpty(strStatus) Then
                    Me.m_tbxSummary.Text = Me.m_tbxSummary.Text + strStatus + vbNewLine
                End If
            End If
        End Sub

#End Region ' Threaded import

#End Region ' Internals

    End Class

End Namespace
