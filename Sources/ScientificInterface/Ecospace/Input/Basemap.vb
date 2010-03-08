#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports EwECore
Imports SAUPUtil.SAUPData
Imports SAUPUtil.SAUPData.Mapping
Imports SAUPUtil.Misc.Colours
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports ScientificInterface.Ecospace.Basemap.Layers

#End Region ' Imports

Namespace Ecospace.Basemap

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' This is the master class organizing and combining the different
    ''' basemap view components. In MVC terms, this class is the global
    ''' controller for the ecospace basemap interface.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class Basemap

#Region " Private vars "

        Private m_basemapData As cEcospaceBasemap = Nothing
        ''' <summary>The one and only administration of layers.</summary>
        Private m_layers As New List(Of cLayer)
        ''' <summary>The one and only control that renders the basemap.</summary>
        Private m_ucBasemap As ucBaseMap = Nothing
        ''' <summary>The one and only control that provides the layers interface.</summary>
        Private m_ucLayers As ucLayersControl = Nothing
        ''' <summary>Contaminant tracing on/off property.</summary>
        Private m_propContaminantTracing As cProperty = Nothing
        Private m_layerRelCin As cLayer = Nothing

        Private m_cmdEditBasemap As cCommand = Nothing
        Private m_cmdEditHabitats As cCommand = Nothing
        Private m_cmdEditRegions As cCommand = Nothing
        Private m_cmdEditMPAs As cCommand = Nothing

#End Region ' Private vars

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Public properties "

        Private Property Basemap() As cEcospaceBasemap

            Get
                Return Me.m_basemapData
            End Get

            Set(ByVal value As cEcospaceBasemap)

                ' Store ref
                Me.m_basemapData = value
                ' Initalize the m_ucBasemap
                Me.m_ucBasemap.Basemap = value
                ' Initialize layers from core data
                Me.LoadCoreValuesToBasemap()

            End Set

        End Property

#End Region ' Public properties

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim pm As cPropertyManager = Me.PropertyManager
            Dim source As cEcospaceModelParameters = Me.Core.EcospaceModelParameters()

            ' Initalize m_ucBasemap
            Me.m_ucBasemap = Me.m_zoomContainer.Map()
            Me.m_zoomContainer.UIContext = Me.UIContext

            ' Add LayersControl
            Me.m_ucLayers = New ucLayersControl(Me.UIContext)
            plLayers.Controls.Add(Me.m_ucLayers)

            Me.Basemap = Me.Core.EcospaceBasemap
            Me.m_ucBasemap.Editable = True

            Me.m_cmdEditBasemap = cmdh.GetCommand("EditBasemap")
            If (Not Object.ReferenceEquals(Me.m_cmdEditBasemap, Nothing)) Then
                Me.m_cmdEditBasemap.AddControl(Me.tsbEditBasemap)
                AddHandler Me.m_cmdEditBasemap.OnPreInvoke, AddressOf OnPreIvokeEditcommand
                AddHandler Me.m_cmdEditBasemap.OnPostInvoke, AddressOf OnPostIvokeEditcommand
            End If

            Me.m_cmdEditHabitats = cmdh.GetCommand("EditHabitats")
            If (Not Object.ReferenceEquals(Me.m_cmdEditHabitats, Nothing)) Then
                Me.m_cmdEditHabitats.AddControl(Me.tsbEditHabitats)
                AddHandler Me.m_cmdEditHabitats.OnPreInvoke, AddressOf OnPreIvokeEditcommand
                AddHandler Me.m_cmdEditHabitats.OnPostInvoke, AddressOf OnPostIvokeEditcommand
            End If

            Me.m_cmdEditMPAs = cmdh.GetCommand("EditMPAs")
            If (Not Object.ReferenceEquals(Me.m_cmdEditMPAs, Nothing)) Then
                Me.m_cmdEditMPAs.AddControl(Me.tsbEditMPA)
                AddHandler Me.m_cmdEditMPAs.OnPreInvoke, AddressOf OnPreIvokeEditcommand
                AddHandler Me.m_cmdEditMPAs.OnPostInvoke, AddressOf OnPostIvokeEditcommand
            End If

            Me.m_cmdEditRegions = cmdh.GetCommand("EditRegions")
            If (Not Object.ReferenceEquals(Me.m_cmdEditRegions, Nothing)) Then
                Me.m_cmdEditRegions.AddControl(Me.tsbEditRegion)
                AddHandler Me.m_cmdEditRegions.OnPreInvoke, AddressOf OnPreIvokeEditcommand
                AddHandler Me.m_cmdEditRegions.OnPostInvoke, AddressOf OnPostIvokeEditcommand
            End If

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}

            Me.m_propContaminantTracing = pm.GetProperty(source, eVarNameFlags.ConSimOnEcoSpace)
            AddHandler Me.m_propContaminantTracing.PropertyChanged, AddressOf OnContaminantTracingChanged
            Me.OnContaminantTracingChanged(Me.m_propContaminantTracing, cProperty.eChangeFlags.Value)

            Me.m_plEditor.Visible = False

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

            MyBase.OnFormClosed(e)

            RemoveHandler Me.m_propContaminantTracing.PropertyChanged, AddressOf OnContaminantTracingChanged

            ' Detach from message sources
            Me.CoreComponents = Nothing
            ' Clean up
            Me.RemoveAllLayers()

            Dim cmdh As cCommandHandler = Me.CommandHandler

            If (Not Object.ReferenceEquals(Me.m_cmdEditBasemap, Nothing)) Then
                Me.m_cmdEditBasemap.RemoveControl(Me.tsbEditBasemap)
                RemoveHandler Me.m_cmdEditBasemap.OnPreInvoke, AddressOf OnPreIvokeEditcommand
                RemoveHandler Me.m_cmdEditBasemap.OnPostInvoke, AddressOf OnPostIvokeEditcommand
                Me.m_cmdEditBasemap = Nothing
            End If

            If (Not Object.ReferenceEquals(Me.m_cmdEditHabitats, Nothing)) Then
                Me.m_cmdEditHabitats.RemoveControl(Me.tsbEditHabitats)
                RemoveHandler Me.m_cmdEditHabitats.OnPreInvoke, AddressOf OnPreIvokeEditcommand
                RemoveHandler Me.m_cmdEditHabitats.OnPostInvoke, AddressOf OnPostIvokeEditcommand
                Me.m_cmdEditHabitats = Nothing
            End If

            If (Not Object.ReferenceEquals(Me.m_cmdEditMPAs, Nothing)) Then
                Me.m_cmdEditMPAs.RemoveControl(Me.tsbEditRegion)
                RemoveHandler Me.m_cmdEditMPAs.OnPreInvoke, AddressOf OnPreIvokeEditcommand
                RemoveHandler Me.m_cmdEditMPAs.OnPostInvoke, AddressOf OnPostIvokeEditcommand
                Me.m_cmdEditMPAs = Nothing
            End If

            If (Not Object.ReferenceEquals(Me.m_cmdEditRegions, Nothing)) Then
                Me.m_cmdEditRegions.RemoveControl(Me.tsbEditRegion)
                RemoveHandler Me.m_cmdEditRegions.OnPreInvoke, AddressOf OnPreIvokeEditcommand
                RemoveHandler Me.m_cmdEditRegions.OnPostInvoke, AddressOf OnPostIvokeEditcommand
                Me.m_cmdEditRegions = Nothing
            End If

            Me.m_propContaminantTracing = Nothing

        End Sub

        Private Sub OnPreIvokeEditcommand(ByVal cmd As cCommand)
            Me.m_ucLayers.LockUpdates()
        End Sub

        Private Sub OnPostIvokeEditcommand(ByVal cmd As cCommand)
            Me.m_ucLayers.UnlockUpdates()
            ' Update map
            Me.m_ucBasemap.Refresh()
        End Sub

        Private Sub OnLayerChanged(ByVal layer As cLayer, ByVal changeFlag As cLayer.eChangeFlags)
            Dim layerSelect As cLayer = Nothing
            ' Is selection change?
            If ((changeFlag And cLayer.eChangeFlags.Selected) > 0) Then
                ' #Yes: Find newly selected layer
                For Each layerTemp As cLayer In Me.m_layers
                    ' Got it?
                    If layerTemp.IsSelected Then
                        ' #Yes: remember this
                        layerSelect = layerTemp
                        Exit For
                    End If
                Next
                ' Set selection
                Me.SelectedLayer = layerSelect
            End If
        End Sub

        Private Sub OnContaminantTracingChanged(ByVal prop As cProperty, ByVal cf As cProperty.eChangeFlags)
            If ((cf And cProperty.eChangeFlags.Value) = cf) Then
                If CBool(prop.GetValue()) Then
                    If (Me.m_layerRelCin Is Nothing) Then
                        Me.m_layerRelCin = cLayerFactory.GetLayers(Me.UIContext, eVarNameFlags.LayerRelCin)(0)
                        Me.AddLayer(Me.m_layerRelCin, cLayerFactory.GetLayerGroup(eVarNameFlags.LayerRelCin))
                    End If
                Else
                    If (Me.m_layerRelCin IsNot Nothing) Then
                        Me.RemoveLayer(Me.m_layerRelCin)
                        Me.m_layerRelCin = Nothing
                    End If
                End If
            End If
        End Sub

#End Region ' Events

#Region " Load Core Helpers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load fixed core layers from the core basemap data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub LoadCoreValuesToBasemap()

            Me.m_ucLayers.LockUpdates()

            ' Clean-up
            Me.RemoveAllLayers()

            Me.AddData(eVarNameFlags.LayerMPA)
            Me.AddData(eVarNameFlags.LayerPort)
            Me.AddData(eVarNameFlags.LayerSail, False)
            Me.AddData(eVarNameFlags.LayerMigration)
            Me.AddData(eVarNameFlags.LayerRelPP, False)
            'Me.AddData(eVarNameFlags.LayerRelCin) ' Added when property changes
            Me.AddData(eVarNameFlags.LayerRegion)
            Me.AddData(eVarNameFlags.LayerHabitat)
            Me.AddData(eVarNameFlags.LayerDepth)

            Me.m_ucLayers.UnlockUpdates()

            ' Update map
            Me.m_ucBasemap.Refresh()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper function to create the layers.  
        ''' </summary>
        ''' <param name="varName">The core variable to load basemap data for.</param>
        ''' -------------------------------------------------------------------
        Private Sub AddData(ByVal varName As eVarNameFlags, Optional ByVal bClearGroup As Boolean = True)

            Dim alayers As cLayer() = cLayerFactory.GetLayers(Me.UIContext, varName)
            Dim strGroup As String = cLayerFactory.GetLayerGroup(varName)

            ' Define group
            Me.m_ucLayers.AddGroup(strGroup, True, bClearGroup)

            For iLayer As Integer = 0 To alayers.Length - 1
                Me.AddLayer(alayers(iLayer), strGroup)
            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove all layers.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub RemoveAllLayers()
            Dim alayers As cLayer() = Me.m_layers.ToArray()
            For Each layer As cLayer In alayers
                Me.RemoveLayer(layer)
            Next
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a single layer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub AddLayer(ByVal l As cLayer, ByVal strGroup As String)
            Me.m_layers.Add(l)
            Me.m_ucBasemap.AddLayer(l)
            Me.m_ucLayers.AddLayer(l, strGroup)

            AddHandler l.LayerChanged, AddressOf OnLayerChanged
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a single layer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub RemoveLayer(ByVal l As cLayer)
            Me.m_layers.Remove(l)
            Me.m_ucBasemap.RemoveLayer(l)
            Me.m_ucLayers.RemoveLayer(l)

            RemoveHandler l.LayerChanged, AddressOf OnLayerChanged
            l.Dispose()
        End Sub

#End Region ' Load core helpers

#Region " Internals "

        ''' <summary>The layer currently selected by the user.</summary>
        Private m_layerSelected As cLayer = Nothing
        ''' <summary>The editor belonging to the selected layer, if any.</summary>
        Private m_editorGUISelected As ucLayerEditor = Nothing

        Private Property SelectedLayer() As cLayer
            Get
                Return Me.m_layerSelected
            End Get
            Set(ByVal layer As cLayer)

                If Object.ReferenceEquals(layer, Me.m_layerSelected) Then Return

                Me.SuspendLayout()

                If (Me.m_layerSelected IsNot Nothing) Then
                    ' Has editor GUI?
                    If (Me.m_editorGUISelected IsNot Nothing) Then
                        ' #Yes: remove layer editor GUI
                        RemoveHandler Me.m_editorGUISelected.OnChanged, AddressOf OnLayerEditorChanged
                        Me.m_plEditor.Controls.Remove(Me.m_editorGUISelected)
                        Me.m_plEditor.Visible = False
                        Me.m_editorGUISelected = Nothing
                    End If
                    Me.m_layerSelected.Editor.DestroyEditorControl()
                End If

                Me.m_layerSelected = layer

                If (Me.m_layerSelected IsNot Nothing) Then
                    ' Add layer editor GUI
                    Me.m_editorGUISelected = Me.m_layerSelected.Editor.CreateEditorControl()
                    If (Me.m_editorGUISelected IsNot Nothing) Then
                        Me.m_plEditor.Height = Me.m_editorGUISelected.Height
                        Me.m_editorGUISelected.Dock = DockStyle.Fill
                        Me.m_plEditor.Controls.Add(Me.m_editorGUISelected)
                        Me.m_plEditor.Visible = True
                        AddHandler Me.m_editorGUISelected.OnChanged, AddressOf OnLayerEditorChanged
                    End If
                End If

                Me.ResumeLayout()

            End Set
        End Property

        Private Sub OnLayerEditorChanged(ByVal editor As ucLayerEditor)
            Me.m_ucBasemap.UpdateCursorFeedback()
        End Sub

#End Region ' Internals

#Region " Mandatory overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            ' Refresh basemap on ANY data added or removed message from Ecospace
            If ((msg.Source = eCoreComponentType.EcoSpace) And (msg.Type = eMessageType.DataAddedOrRemoved)) Then
                ' Refresh it all
                Me.Basemap = Me.Core.EcospaceBasemap
            End If
        End Sub

#End Region ' Mandatory overrides

    End Class

End Namespace
