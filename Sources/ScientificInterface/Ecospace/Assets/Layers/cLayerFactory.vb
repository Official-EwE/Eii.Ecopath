#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwECore.Auxiliary

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' =======================================================================
    ''' <summary>
    ''' Factory for returning <see cref="cLayer">UI layer wrappers</see> for 
    ''' <see cref="cEcospaceLayer">Ecospace basemap layer data.</see>
    ''' </summary>
    ''' =======================================================================
    Public Class cLayerFactory

        Public Const cECOSEED_LAYER_NOVALUE As Integer = 0
        Public Const cECOSEED_LAYER_CURRENTVALUE As Integer = 1
        Public Const cECOSEED_LAYER_BESTVALUE As Integer = 2

        ''' <summary>
        ''' Build layer(s) for a given core data layer name.
        ''' </summary>
        ''' <param name="uic">UI context to connect layer to.</param>
        ''' <param name="layerData"></param>
        ''' <returns></returns>
        Public Shared Function GetLayers(ByVal uic As cUIContext, _
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

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        vs.BackColour = Color.Transparent
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    ' Represent depth as a .. depth layer! Whoohoo!
                    renderer = New cLayerRendererDepth(vs)
                    editor = New cLayerEditorRange(GetType(ucLayerEditorDepth))
                    If layerData Is Nothing Then layerData = bmd.LayerDepth
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerDepth)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_DEPTH

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
                        renderer = New cLayerRendererBitmap(vs)
                        editor = New cLayerEditorTwoState()
                        If layerData Is Nothing Then layerData = bmd.LayerHabitat
                        layer = New cLayer(uic, layerData, renderer, editor, iHabitat, 0, hab, eVarNameFlags.Name)
                        lLayers.Add(layer)

                    Next iHabitat

                Case eVarNameFlags.LayerRegion

                    ' This is screwed-up: one key (and one layer) for all regions
                    key = New cValueID(eDataTypes.EcospaceLayerRegion, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        vs.BackColour = Color.Transparent
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    ' Represent regions as a gradient
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange(GetType(ucLayerEditorRegion))
                    editor.CellValueMax = core.nRegions
                    editor.IsEditable = (core.nRegions > 0)
                    If layerData Is Nothing Then layerData = bmd.LayerRegion
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.Name)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_REGIONS

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPA

                    avs = brushProvider.GetVisualStyles(core.nMPAs, cEwEBrushProvider.eBrushType.HatchPattern)

                    For iMPA As Integer = 1 To core.nMPAs

                        Dim mpa As cEcospaceMPA = core.EcospaceMPAs(iMPA)
                        key = New cValueID(eDataTypes.EcospaceLayerMPA, mpa.DBID, eVarNameFlags.Name)
                        ad = core.AuxillaryData(key)

                        ' Get or create Visual Style
                        vs = ad.VisualStyle
                        If vs Is Nothing Then
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

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    ' Represent as a solid colour
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange()
                    If layerData Is Nothing Then layerData = bmd.LayerRelPP
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerRelPP)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_RELPP

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerRelCin

                    key = New cValueID(eDataTypes.EcospaceLayerRelCin, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    ' Represent as a solid colour
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange()
                    If layerData Is Nothing Then layerData = bmd.LayerRelCin
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerRelCin)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_RELCIN

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPASeed

                    vs = New cVisualStyle()
                    vs.ForeColour = Color.CornflowerBlue

                    ' Represent MPA seeds as a solid colour
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorTwoState()
                    If layerData Is Nothing Then layerData = bmd.LayerMPASeed
                    layer = New cLayer(uic, layerData, renderer, editor, 1, 0, bmd, eVarNameFlags.LayerMPASeed)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_MPASEED

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPASeedCurrent

                    vs = New cVisualStyle()
                    vs.ForeColour = Color.LightGreen

                    ' Represent MPA seeds as a solid colour
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorTwoState()
                    If layerData Is Nothing Then Debug.Assert(False, "Cannot link to core data")
                    layer = New cLayer(uic, layerData, renderer, editor, cECOSEED_LAYER_CURRENTVALUE, cECOSEED_LAYER_NOVALUE)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_SEEDCURRENT
                    layer.Editor.IsReadOnly = True

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPASeedBest

                    vs = New cVisualStyle()
                    vs.ForeColour = Color.DarkGreen
                    vs.BackColour = Color.Transparent

                    ' Represent MPA seeds as a solid colour
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorTwoState()

                    layer = New cLayer(uic, layerData, renderer, editor, cECOSEED_LAYER_BESTVALUE, cECOSEED_LAYER_NOVALUE)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_SEEDBEST
                    layer.Editor.IsReadOnly = True

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPARandom

                    If layerData IsNot Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        vs.BackColour = Color.Blue

                        renderer = New cLayerRendererValue(vs)
                        editor = New cLayerEditorRange()
                        layer = New cLayer(uic, layerData, renderer, editor)
                        layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_RANDOMBEST
                        layer.Editor.IsReadOnly = True

                        lLayers.Add(layer)
                    End If

                Case eVarNameFlags.LayerMigration

                    key = New cValueID(eDataTypes.EcospaceLayerMigration, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorMigration()
                    If layerData Is Nothing Then layerData = bmd.LayerMigration
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerMigration)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_MIGRATION

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerAdvection

                    key = New cValueID(eDataTypes.EcospaceLayerAdvection, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    renderer = New cLayerRendererWindEwE5(vs)
                    If layerData Is Nothing Then layerData = bmd.LayerAdvection
                    layer = New cLayer(uic, layerData, renderer, Nothing, bmd, eVarNameFlags.LayerAdvection)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_ADVECTION

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerWind

                    key = New cValueID(eDataTypes.EcospaceLayerWind, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    renderer = New cLayerRendererWindEwE5(vs)
                    editor = New cLayerEditorVector(GetType(ucLayerEditorVector))
                    If layerData Is Nothing Then layerData = bmd.layerWind
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerWind)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_WIND

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerUpwelling

                    key = New cValueID(eDataTypes.EcospaceLayerFlow, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    renderer = New cLayerRendererUpwelling(vs)
                    editor = New cLayerEditorRange()
                    If layerData Is Nothing Then layerData = bmd.LayerUpwelling
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerUpwelling)
                    layer.Name = "Upwelling"

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMLD

                    key = New cValueID(eDataTypes.EcospaceLayerMLD, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    renderer = New cLayerRendererText(vs) ' MLD rendered as text on top of gradiented layers such as habitats, etc
                    editor = New cLayerEditorMLD()
                    If layerData Is Nothing Then layerData = bmd.LayerMixedLayerDepths
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, varName)
                    layer.Name = "Mixed layer depths"

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerPort

                    key = New cValueID(eDataTypes.EcospaceLayerPort, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.BurlyWood
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorFleet(GetType(ucLayerEditorPort))
                    If layerData Is Nothing Then layerData = bmd.LayerPort
                    layer = New cLayer(uic, layerData, renderer, editor, 1.0!, 0.0!, bmd, eVarNameFlags.LayerPort)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_PORT

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerSail

                    key = New cValueID(eDataTypes.EcospaceLayerSail, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If

                    ' Represent as a solid colour
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorFleet(GetType(ucLayerEditorSailCost))
                    If layerData Is Nothing Then layerData = bmd.LayerSailingCost
                    layer = New cLayer(uic, layerData, renderer, editor, bmd, eVarNameFlags.LayerSail)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_SAILINGCOST

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerImportance

                    For iLayer As Integer = 1 To core.nImportanceLayers

                        Dim src As cEcospaceLayerImportance = core.EcospaceBasemap.LayerImportance(iLayer)
                        key = New cValueID(eDataTypes.EcospaceLayerImportance, src.DBID, eVarNameFlags.Name)
                        ad = core.AuxillaryData(key)

                        ' Get or create Visual Style
                        vs = ad.VisualStyle
                        If vs Is Nothing Then
                            vs = New cVisualStyle()
                            vs.ForeColour = Color.Black
                            ad.AllowValidation = False
                            ad.VisualStyle = vs
                            ad.AllowValidation = True
                        End If

                        ' Create layer
                        renderer = New cLayerRendererValue(vs)
                        editor = New cLayerEditorTwoState()
                        layer = New cLayer(uic, bmd.LayerImportance(iLayer), renderer, editor, src, eVarNameFlags.Name)

                        lLayers.Add(layer)

                    Next iLayer

                Case Else
                    Debug.Assert(False, "No layers available for this varname")

            End Select

            Return lLayers.ToArray()

        End Function

        Public Shared Function GetLayerGroup(ByVal varName As eVarNameFlags) As String

            Dim strGroup As String = ""
            Select Case varName

                Case eVarNameFlags.LayerDepth
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_DEPTH

                Case eVarNameFlags.LayerHabitat
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_HABITATS

                Case eVarNameFlags.LayerRegion
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_REGIONS

                Case eVarNameFlags.LayerMPA
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_MPAS

                Case eVarNameFlags.LayerRelPP, eVarNameFlags.LayerRelCin, _
                     eVarNameFlags.LayerMigration
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_NUMERICAL

                Case eVarNameFlags.LayerMPASeed, eVarNameFlags.LayerMPASeedBest, eVarNameFlags.LayerMPASeedCurrent
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_ECOSEED

                Case eVarNameFlags.LayerMPARandom
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_RANDOMSEARCH

                Case eVarNameFlags.LayerPort, eVarNameFlags.LayerSail
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_FISHING

                Case eVarNameFlags.LayerImportance
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_IMPORTANCE

            End Select
            Return strGroup

        End Function

    End Class

End Namespace
