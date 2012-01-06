#Region " Imports "

Option Strict On

Imports EwEUtils.SpatialData
Imports EwECore.SpatialData
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwECore

#End Region ' Imports

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Dialog for linking external data to Ecospace layers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class dlgExternalData

#Region " Helper classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class, sorts <see cref="ISpatialDataAdapter"/>s by name, asc.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class cSpatialAdapterSorter
            Implements IComparer(Of ISpatialDataAdapter)

            Private m_fmt As New cVarnameTypeFormatter()

            Public Sub New()
            End Sub

            Public Function Compare(ByVal x As EwEUtils.SpatialData.ISpatialDataAdapter, _
                                    ByVal y As EwEUtils.SpatialData.ISpatialDataAdapter) As Integer _
                                Implements System.Collections.Generic.IComparer(Of EwEUtils.SpatialData.ISpatialDataAdapter).Compare
                If (x Is Nothing) Then Return 1
                If (y Is Nothing) Then Return -1
                Return String.Compare(Me.m_fmt.GetDescriptor(x.VarName), Me.m_fmt.GetDescriptor(y.VarName))
            End Function

        End Class

#End Region ' Helper classes

#Region " Private vars "

        ''' <summary>UI context to operate onto.</summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary>Ecospace message handler to respond to.</summary>
        Private m_mhEcospace As cMessageHandler = Nothing

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New(ByVal uic As cUIContext)
            MyBase.New()
            Me.InitializeComponent()
            Me.UIContext = uic
        End Sub

#End Region ' Construction / destruction

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Safety first
            If (Me.UIContext Is Nothing) Then Return

            Dim man As cSpatialDataConnectionManager = Me.m_uic.Core.SpatialDataConnectionManager
            Dim ecospaceModelParams As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters()
            Dim adapters As ISpatialDataAdapter() = Nothing

            Debug.Assert(man IsNot Nothing)

            ' Populate adapters list
            adapters = man.Adapters
            Array.Sort(adapters, New cSpatialAdapterSorter)
            For Each adt As ISpatialDataAdapter In adapters
                Me.m_lbxAdapters.Items.Add(adt)
            Next
            If Me.m_lbxAdapters.Items.Count > 0 Then
                Me.m_lbxAdapters.SelectedIndex = 0
            End If

            ' Ooh!
            Me.CenterToParent()

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            ' Release config screen
            Me.m_lbxAdapters.Items.Clear()
            Me.m_config.Adapter = Nothing
            Me.UIContext = Nothing

            ' Dome
            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Form overrides

#Region " Internals "

        Private Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
            Implements ScientificInterfaceShared.Controls.IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)

                ' Clean up
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhEcospace)
                    Me.m_mhEcospace.Dispose()
                    Me.m_mhEcospace = Nothing
                    Me.m_config.UIContext = Nothing
                End If

                Me.m_uic = value

                ' Set new
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_config.UIContext = Me.m_uic
                    Me.m_mhEcospace = New cMessageHandler(AddressOf OnEcospaceMessage, EwEUtils.Core.eCoreComponentType.EcoSpace, eMessageType.DataModified, Me.m_uic.SyncObject)
                    Me.m_uic.Core.Messages.AddMessageHandler(Me.m_mhEcospace)
                End If
            End Set
        End Property

#End Region ' Internals

#Region " Event handlers "

        Private Sub OnOK(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_btnOK.Click
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Private Sub OnDrawAdapterItem(sender As Object, e As System.Windows.Forms.DrawItemEventArgs) _
            Handles m_lbxAdapters.DrawItem

            If (e.Index < 0) Then
                e.DrawBackground()
                e.DrawFocusRectangle()
                Return
            End If

            Dim fmt As New cSpatialDataAdapterFormatter()
            Dim rcImage As Rectangle = Nothing
            Dim rcText As Rectangle = Nothing
            Dim adt As ISpatialDataAdapter = DirectCast(Me.m_lbxAdapters.Items(e.Index), ISpatialDataAdapter)

            e.DrawBackground()

            If (Me.UIContext.StyleGuide.IsRightToLeft) Then
                rcText = New Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 16 - 2, e.Bounds.Height)
                rcImage = New Rectangle(e.Bounds.Left + e.Bounds.Width - 16, e.Bounds.Top, 16, e.Bounds.Height)
            Else
                rcText = New Rectangle(e.Bounds.Left + 16 + 2, e.Bounds.Top, e.Bounds.Width - 16 - 2, e.Bounds.Height)
                rcImage = New Rectangle(e.Bounds.Left, e.Bounds.Top, 16, e.Bounds.Height)
            End If

            If adt.IsConnected Then
                e.Graphics.DrawImage(SharedResources.Database, rcImage)
            End If

            Using br As New SolidBrush(e.ForeColor)
                e.Graphics.DrawString(fmt.GetDescriptor(adt), Me.Font, br, rcText)
            End Using

            e.DrawFocusRectangle()

        End Sub

        Private Sub OnAdapterSelected(sender As System.Object, e As System.EventArgs) _
            Handles m_lbxAdapters.SelectedIndexChanged
            Me.m_config.Adapter = DirectCast(Me.m_lbxAdapters.SelectedItem, ISpatialDataAdapter)
        End Sub

        Private Sub OnEcospaceMessage(ByRef msg As cMessage)
            Me.m_lbxAdapters.Invalidate()
        End Sub

#End Region ' Event handlers

    End Class

End Namespace

