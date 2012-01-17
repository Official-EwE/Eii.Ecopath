#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style

#End Region ' Imports

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
        ''' Build user interface layer(s) for given core data.
        ''' </summary>
        ''' <param name="uic">UI context to connect layer to.</param>
        ''' <param name="varName">Name of the core variable to wrap</param>
        ''' <returns>An array of layers</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetLayers(ByVal uic As cUIContext, _
                                              ByVal varName As eVarNameFlags) As cLayer()

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

            Select Case varName

                Case eVarNameFlags.LayerDepth

                    ' Depth layer identified by basemap
                    key = New cValueID(eDataTypes.EcospaceLayerDepth, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererDepth(vs)
                    editor = New cLayerEditorDepth()
                    layer = New cLayer(uic, bmd.LayerDepth, renderer, editor, bmd, eVarNameFlags.LayerDepth)
                    lLayers.Add(layer)

                Case eVarNameFlags.LayerHabitat

                    avs = uic.StyleGuide.GetVisualStyles(core.nHabitats, cStyleGuide.eBrushType.Glyphs)

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
                        renderer = New cLayerRendererBitmap(vs)
                        editor = New cLayerEditorHabitat()
                        layer = New cLayer(uic, bmd.LayerHabitat(iHabitat), renderer, editor, hab, eVarNameFlags.Name, sValueClear:=0)
                        lLayers.Add(layer)

                    Next iHabitat

                Case eVarNameFlags.LayerHabitatCapacityInput

                    key = New cValueID(eDataTypes.EcospaceLayerHabitatCapacityInput, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorGroup(GetType(ucLayerEditorHabitatCapacity))
                    layer = New cLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerHabitatCapacityInput), _
                                            renderer, editor, eCoreCounterTypes.nGroups, bmd, eVarNameFlags.LayerHabitatCapacityInput)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerHabitatCapacity

                    key = New cValueID(eDataTypes.EcospaceLayerHabitatCapacity, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorGroup(GetType(ucLayerEditorGroup))
                    editor.IsReadOnly = True
                    layer = New cLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerHabitatCapacity), _
                                            renderer, editor, eCoreCounterTypes.nGroups, bmd, eVarNameFlags.LayerHabitatCapacity)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerRegion

                    key = New cValueID(eDataTypes.EcospaceLayerRegion, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRegion()
                    layer = New cLayer(uic, bmd.LayerRegion, renderer, editor)
                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPA

                    avs = uic.StyleGuide.GetVisualStyles(core.nMPAs, cStyleGuide.eBrushType.HatchPattern)

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
                        layer = New cLayer(uic, bmd.LayerMPA, renderer, editor, mpa, eVarNameFlags.Name, iMPA, 0)

                        lLayers.Add(layer)

                    Next iMPA

                Case eVarNameFlags.LayerRelPP

                    key = New cValueID(eDataTypes.EcospaceLayerRelPP, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange()
                    layer = New cLayer(uic, bmd.LayerRelPP, renderer, editor, bmd, eVarNameFlags.LayerRelPP)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerRelCin

                    key = New cValueID(eDataTypes.EcospaceLayerRelCin, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange()
                    layer = New cLayer(uic, bmd.LayerRelCin, renderer, editor, bmd, eVarNameFlags.LayerRelCin)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMigration

                    key = New cValueID(eDataTypes.EcospaceLayerMigration, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorMigration()
                    layer = New cLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerMigration), _
                                            renderer, editor, eCoreCounterTypes.nGroups, bmd, eVarNameFlags.LayerMigration)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerAdvection

                    key = New cValueID(eDataTypes.EcospaceLayerAdvection, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererWindEwE5(vs)
                    layer = New cLayer(uic, bmd.LayerAdvection, renderer, Nothing, bmd, eVarNameFlags.LayerAdvection)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerWind

                    key = New cValueID(eDataTypes.EcospaceLayerWind, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererWindEwE5(vs)
                    editor = New cLayerEditorVector(GetType(ucLayerEditorVector))
                    layer = New cLayer(uic, bmd.LayerWind, renderer, editor, bmd, eVarNameFlags.LayerWind)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerUpwelling

                    ' ToDo: globalize this

                    key = New cValueID(eDataTypes.EcospaceLayerFlow, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererUpwelling(vs)
                    editor = New cLayerEditorRange()
                    layer = New cLayer(uic, bmd.LayerUpwelling, renderer, editor, bmd, eVarNameFlags.LayerUpwelling)
                    layer.Name = "Upwelling"

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMLD

                    ' ToDo: globalize this

                    key = New cValueID(eDataTypes.EcospaceLayerMLD, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererText(vs) ' MLD rendered as text on top of gradiented layers such as habitats, etc
                    editor = New cLayerEditorMLD()
                    layer = New cLayer(uic, bmd.LayerMixedLayerDepths, renderer, editor, bmd, varName)
                    layer.Name = "Mixed layer depths"

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerPort

                    key = New cValueID(eDataTypes.EcospaceLayerPort, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorFleet(GetType(ucLayerEditorPort))
                    layer = New cLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerPort), renderer, editor, eCoreCounterTypes.nFleets, bmd, eVarNameFlags.LayerPort, 1.0!, 0.0!)
                    lLayers.Add(layer)

                Case eVarNameFlags.LayerSail

                    key = New cValueID(eDataTypes.EcospaceLayerSail, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorFleet(GetType(ucLayerEditorSailCost))
                    layer = New cLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerSail), renderer, editor, eCoreCounterTypes.nFleets, bmd, eVarNameFlags.LayerSail)

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
