#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Controls.Map.Layers

#End Region ' Imports

' ToDo: create default colour ramps!

Namespace Controls.Map

    ''' =======================================================================
    ''' <summary>
    ''' Factory for returning <see cref="cLayer">UI layer wrappers</see> for 
    ''' <see cref="cEcospaceLayer">Ecospace basemap layer data.</see>
    ''' </summary>
    ''' =======================================================================
    Public Class cLayerFactoryBase

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Build layer(s) for a given core data layer name.
        ''' </summary>
        ''' <param name="uic">UI context to connect layer to.</param>
        ''' <param name="layerData">Optional data to attach to the layer. If no
        ''' data is given the layer will attempt to get its data from the 
        ''' Ecospace basemap.</param>
        ''' <returns>An array of layers</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetLayers(ByVal uic As cUIContext, _
                                              ByVal varName As eVarNameFlags, _
                                              Optional ByVal layerData As cEcospaceLayer = Nothing) As cLayer()

            Dim lLayers As New List(Of cLayer)

            Dim core As cCore = uic.Core
            Dim bmd As cEcospaceBasemap = core.EcospaceBasemap
            Dim layer As cLayer = Nothing
            Dim key As cValueID = Nothing
            Dim ad As cAuxiliaryData = Nothing
            Dim avs As cVisualStyle() = Nothing
            Dim renderer As cLayerRenderer = Nothing
            Dim editor As cLayerEditor = Nothing
            Dim vs As cVisualStyle = Nothing
            Dim brushProvider As New cEwEBrushProvider

            Select Case varName

                Case eVarNameFlags.LayerDepth

                    ' Depth layer identified by basemap
                    key = New cValueID(eDataTypes.EcospaceLayerDepth, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererDepth(vs)
                    editor = New cLayerEditorDepth()
                    If layerData Is Nothing Then layerData = bmd.LayerDepth
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerDepth)
                    lLayers.Add(layer)

                Case eVarNameFlags.LayerHabitat

                    avs = brushProvider.GetVisualStyles(core.nHabitats, cEwEBrushProvider.eBrushType.Glyphs)

                    For iHabitat As Integer = 1 To core.nHabitats - 1
                        Dim hab As cEcospaceHabitat = core.EcospaceHabitats(iHabitat)

                        key = New cValueID(eDataTypes.EcospaceLayerHabitat, hab.DBID, eVarNameFlags.Name)
                        ad = core.AuxillaryData(key)

                        ' Get or create Visual Style
                        vs = ad.VisualStyle
                        If vs Is Nothing Then
                            vs = avs(iHabitat - 1)
                            ad.AllowValidation = False
                            ad.VisualStyle = vs
                            ad.AllowValidation = True
                        End If

                        ' Create layer
                        renderer = New cLayerRendererValue(vs)
                        editor = New cLayerEditorRange()
                        layer = New cLayer(uic, bmd.LayerHabitat(iHabitat), renderer, editor, hab, eVarNameFlags.Name)
                        lLayers.Add(layer)

                    Next iHabitat

                Case eVarNameFlags.LayerHabitatCapacityInput

                    key = New cValueID(eDataTypes.EcospaceLayerHabitatCapacityInput, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorGroup(GetType(ucLayerEditorHabitatCapacity))
                    layerData = bmd.LayerHabitatCapacityInput
                    layer = New cLayer(uic, layerData, renderer, editor, cCore.NULL_VALUE, cCore.NULL_VALUE, bmd, eVarNameFlags.LayerHabitatCapacity)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerHabitatCapacity

                    key = New cValueID(eDataTypes.EcospaceLayerHabitatCapacity, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorGroup(GetType(ucLayerEditorGroup))
                    editor.IsReadOnly = True
                    layerData = bmd.LayerHabitatCapacity
                    layer = New cLayer(uic, layerData, renderer, editor, cCore.NULL_VALUE, cCore.NULL_VALUE, bmd, eVarNameFlags.LayerHabitatCapacity)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerRegion

                    ' This is screwed-up: one key (and one layer) for all regions
                    key = New cValueID(eDataTypes.EcospaceLayerRegion, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange(GetType(ucLayerEditorRegion))
                    editor.CellValueMax = core.nRegions
                    editor.IsEditable = (core.nRegions > 0)
                    If layerData Is Nothing Then layerData = bmd.LayerRegion
                    layer = New cLayer(uic, layerData, renderer, editor, layerData, eVarNameFlags.Name)
                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPA

                    avs = brushProvider.GetVisualStyles(core.nMPAs, cEwEBrushProvider.eBrushType.HatchPattern)

                    For iMPA As Integer = 1 To core.nMPAs

                        Dim mpa As cEcospaceMPA = core.EcospaceMPAs(iMPA)
                        key = New cValueID(eDataTypes.EcospaceLayerMPA, mpa.DBID, eVarNameFlags.Name)
                        ad = core.AuxillaryData(key)

                        ' Get or create Visual Style
                        vs = ad.VisualStyle
                        If (vs Is Nothing) Then
                            vs = avs(iMPA)
                            ad.AllowValidation = False
                            ad.VisualStyle = vs
                            ad.AllowValidation = True
                        End If

                        ' Create layer
                        renderer = New cLayerRendererHatch(vs)
                        editor = New cLayerEditorTwoState()
                        If layerData Is Nothing Then layerData = bmd.LayerMPA
                        layer = New cLayer(uic, layerData, renderer, editor, iMPA, 0, mpa, eVarNameFlags.Name)

                        lLayers.Add(layer)

                    Next iMPA

                Case eVarNameFlags.LayerRelPP

                    key = New cValueID(eDataTypes.EcospaceLayerRelPP, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange()
                    If layerData Is Nothing Then layerData = bmd.LayerRelPP
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerRelPP)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerRelCin

                    key = New cValueID(eDataTypes.EcospaceLayerRelCin, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange()
                    If layerData Is Nothing Then layerData = bmd.LayerRelCin
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerRelCin)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMigration

                    key = New cValueID(eDataTypes.EcospaceLayerMigration, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorMigration()
                    If layerData Is Nothing Then layerData = bmd.LayerMigration
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerMigration)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerAdvection

                    key = New cValueID(eDataTypes.EcospaceLayerAdvection, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererWindEwE5(vs)
                    If layerData Is Nothing Then layerData = bmd.LayerAdvection
                    layer = New cLayer(uic, layerData, renderer, Nothing, bmd, eVarNameFlags.LayerAdvection)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerWind

                    key = New cValueID(eDataTypes.EcospaceLayerWind, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererWindEwE5(vs)
                    editor = New cLayerEditorVector(GetType(ucLayerEditorVector))
                    If layerData Is Nothing Then layerData = bmd.LayerWind
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerWind)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerUpwelling

                    key = New cValueID(eDataTypes.EcospaceLayerFlow, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererUpwelling(vs)
                    editor = New cLayerEditorRange()
                    If layerData Is Nothing Then layerData = bmd.LayerUpwelling
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerUpwelling)
                    layer.Name = "Upwelling"

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMLD

                    key = New cValueID(eDataTypes.EcospaceLayerMLD, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererText(vs) ' MLD rendered as text on top of gradiented layers such as habitats, etc
                    editor = New cLayerEditorMLD()
                    If layerData Is Nothing Then layerData = bmd.LayerMixedLayerDepths
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, varName)
                    layer.Name = "Mixed layer depths"

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerPort

                    key = New cValueID(eDataTypes.EcospaceLayerPort, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorFleet(GetType(ucLayerEditorPort))
                    If layerData Is Nothing Then layerData = bmd.LayerPort
                    layer = New cLayer(uic, layerData, renderer, editor, 1.0!, 0.0!, bmd, eVarNameFlags.LayerPort)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerSail

                    key = New cValueID(eDataTypes.EcospaceLayerSail, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorFleet(GetType(ucLayerEditorSailCost))
                    If layerData Is Nothing Then layerData = bmd.LayerSailingCost
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerSail)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerImportance

                    For iLayer As Integer = 1 To core.nImportanceLayers

                        Dim src As cEcospaceLayerImportance = core.EcospaceBasemap.LayerImportance(iLayer)
                        key = New cValueID(src.DataType, src.DBID, eVarNameFlags.Name)
                        ad = core.AuxillaryData(key)

                        vs = ad.VisualStyle
                        If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                        renderer = New cLayerRendererValue(vs)
                        editor = New cLayerEditorTwoState()
                        layer = New cLayer(uic, src, renderer, editor, src, eVarNameFlags.Name)

                        lLayers.Add(layer)

                    Next iLayer

                Case eVarNameFlags.LayerDriver

                    For iLayer As Integer = 1 To core.nEnvironmentalLayers

                        Dim src As cEcospaceLayerDriver = core.EcospaceBasemap.LayerDriver(iLayer)
                        key = New cValueID(src.DataType, src.DBID, eVarNameFlags.Name)
                        ad = core.AuxillaryData(key)

                        vs = ad.VisualStyle
                        If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                        renderer = New cLayerRendererValue(vs)
                        editor = New cLayerEditorRange()
                        layer = New cLayer(uic, src, renderer, editor, src, eVarNameFlags.Name)

                        lLayers.Add(layer)

                    Next iLayer

                Case Else
                    Debug.Assert(False, "No layers available for this varname")

            End Select

            Return lLayers.ToArray()

        End Function

        Public Overridable Function GetLayerGroup(ByVal varName As eVarNameFlags) As String

            Dim strGroup As String = ""
            Select Case varName

                Case eVarNameFlags.LayerDepth
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_DEPTH

                Case eVarNameFlags.LayerHabitat
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_HABITATS

                Case eVarNameFlags.LayerHabitatCapacity, _
                     eVarNameFlags.LayerHabitatCapacityInput
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_HABCAP

                Case eVarNameFlags.LayerRegion
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_REGIONS

                Case eVarNameFlags.LayerMPA
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_MPAS

                Case eVarNameFlags.LayerRelPP, _
                     eVarNameFlags.LayerRelCin, _
                     eVarNameFlags.LayerMigration
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_MISC

                Case eVarNameFlags.LayerPort, _
                      eVarNameFlags.LayerSail
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_FISHING

                Case eVarNameFlags.LayerImportance
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_IMPORTANCE

                Case eVarNameFlags.LayerDriver
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_DRIVERS

            End Select
            Return strGroup

        End Function

    End Class

End Namespace
