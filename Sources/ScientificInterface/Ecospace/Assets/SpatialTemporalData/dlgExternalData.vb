#Region " Imports "

Option Strict On

Imports EwEUtils.SpatialData
Imports EwECore.SpatialData
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwECore
Imports ScientificInterface.Ecospace.Basemap.Layers

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
        ''' Helper class, sorts <see cref="cSpatialDataAdapter"/>s by name, asc.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class cSpatialAdapterSorter
            Implements IComparer(Of cSpatialDataAdapter)

            Private m_fmt As New cVarnameTypeFormatter()

            Public Sub New()
            End Sub

            Public Function Compare(ByVal x As cSpatialDataAdapter, _
                                    ByVal y As cSpatialDataAdapter) As Integer _
                                Implements System.Collections.Generic.IComparer(Of cSpatialDataAdapter).Compare
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
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.InitializeComponent()
            Me.UIContext = uic
        End Sub

#End Region ' Construction / destruction

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Safety first
            If (Me.UIContext Is Nothing) Then Return
            ' Sanity check
            Debug.Assert(Me.UIContext.Core.StateMonitor.HasEcospaceLoaded)

            Dim man As cSpatialDataConnectionManager = Me.m_uic.Core.SpatialDataConnectionManager
            Dim ecospaceModelParams As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters()
            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
            Dim adapters As cSpatialDataAdapter() = Nothing
            Dim fact As New cLayerFactoryInternal()

            Debug.Assert(man IsNot Nothing)

            ' Populate adapters list
            adapters = man.Adapters
            Array.Sort(adapters, New cSpatialAdapterSorter)

            For Each adt As cSpatialDataAdapter In adapters

                Dim strGroup As String = fact.GetLayerGroup(adt.VarName)
                Dim tnAdt As New TreeNode(strGroup)
                Dim layers() As cEcospaceLayer = bm.Layers(adt.VarName)

                ' Remeber
                tnAdt.Tag = adt

                If (layers.Length > 0) Then
                    Dim bExt As Boolean = False
                    For Each l As cEcospaceLayer In layers
                        Dim tnLayer As New TreeNode(l.Name)
                        tnLayer.Tag = l
                        tnAdt.Nodes.Add(tnLayer)
                        bExt = bExt Or l.IsExternalData
                    Next
                    Me.m_tvAdapters.Nodes.Add(tnAdt)
                    If bExt Then tnAdt.Expand()
                End If
            Next

            ' Create image list
            Me.m_ilConnections.Images.Add(SharedResources.database_NA)
            Me.m_ilConnections.Images.Add(SharedResources.Database)
            Me.m_tvAdapters.ImageList = Me.m_ilConnections

            ' Update images
            Me.UpdateNodeImages()

            ' Ooh!
            Me.CenterToParent()

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            ' Release config screen
            Me.m_tvAdapters.Nodes.Clear()
            Me.UIContext = Nothing
            ' Dome
            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Form overrides

#Region " Event handlers "

        Private Sub OnOK(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_btnOK.Click
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Private Sub OnAdapterSelected(sender As System.Object, e As TreeViewEventArgs) _
            Handles m_tvAdapters.AfterSelect

            Dim adt As cSpatialDataAdapter = Nothing
            Dim layer As cEcospaceLayer = Nothing

            Dim nd As TreeNode = e.Node
            If (TypeOf nd.Tag Is cEcospaceLayer) Then
                adt = DirectCast(nd.Parent.Tag, cSpatialDataAdapter)
                layer = DirectCast(nd.Tag, cEcospaceLayer)
                Me.m_config.SetConnection(adt, layer)
            End If

        End Sub

        Private Sub OnEcospaceMessage(ByRef msg As cMessage)
            If msg.Type = eMessageType.DataModified Then
                Me.UpdateNodeImages()
            End If
        End Sub

#End Region ' Event handlers

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

        Private Sub UpdateNodeImages()

            For Each ndAdt As TreeNode In Me.m_tvAdapters.Nodes
                Dim iNumConnected As Integer = 0
                For Each ndLayer As TreeNode In ndAdt.Nodes
                    Dim l As cEcospaceLayer = DirectCast(ndLayer.Tag, cEcospaceLayer)
                    If l.IsExternalData Then
                        iNumConnected += 1
                        ndLayer.ImageIndex = 1
                    Else
                        ndLayer.ImageIndex = 0
                    End If
                    ndLayer.SelectedImageIndex = ndLayer.ImageIndex
                Next
                If (iNumConnected > 0) Then
                    ndAdt.ImageIndex = 1
                Else
                    ndAdt.ImageIndex = 0
                End If
                ndAdt.SelectedImageIndex = ndAdt.ImageIndex
            Next
        End Sub

#End Region ' Internals

    End Class

End Namespace

