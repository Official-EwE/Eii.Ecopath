#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' =======================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' =======================================================================
    Public Class cLayerFactory

        Public Const cECOSEED_LAYER_NOVALUE As Integer = 0
        Public Const cECOSEED_LAYER_CURRENTVALUE As Integer = 1
        Public Const cECOSEED_LAYER_BESTVALUE As Integer = 2

        ''' <summary>
        ''' Build layer(s) for a given core data layer name.
        ''' </summary>
        ''' <param name="core"></param>
        ''' <param name="layerData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLayers(ByVal core As cCore, ByVal varName As eVarNameFlags, _
                Optional ByVal layerData As cEcospaceLayer = Nothing) As cLayer()

            Dim bmd As cEcospaceBasemap = core.EcospaceBasemap
            Dim brushProvider As New cEwEBrushProvider
            Dim avs As cVisualStyle() = Nothing
            Dim strID As String = ""
            Dim layer As cLayer = Nothing
            Dim renderer As cLayerRenderer = Nothing
            Dim editor As cLayerEditor = Nothing
            Dim vs As cVisualStyle = Nothing
            Dim lLayers As New List(Of cLayer)

            Select Case varName

                Case eVarNameFlags.LayerDepth

                    ' Get or create Visual Style
                    strID = bmd.getID()
                    vs = core.VisualStyle(strID)
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        vs.BackColour = Color.Transparent
                        core.VisualStyle(strID) = vs
                    End If

                    ' Represent depth as a .. depth layer! Whoohoo!
                    renderer = New cLayerRendererDepth(vs)
                    editor = New cLayerEditorRange()
                    If layerData Is Nothing Then layerData = bmd.LayerDepth
                    layer = New cLayer(layerData, renderer, editor, bmd, eVarNameFlags.LayerDepth)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_LAND

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerHabitat

                    avs = brushProvider.GetVisualStyles(core.nHabitats, cEwEBrushProvider.eBrushType.Glyphs)

                    For iHabitat As Integer = 1 To core.nHabitats - 1
                        Dim hab As cEcospaceHabitat = core.EcospaceHabitats(iHabitat)

                        ' Get or create Visual Style
                        strID = hab.getID()
                        vs = core.VisualStyle(strID)
                        If vs Is Nothing Then
                            vs = avs(iHabitat - 1)
                            core.VisualStyle(strID, False) = vs
                        End If

                        ' Create layer
                        renderer = New cLayerRendererBitmap(vs)
                        editor = New cLayerEditorTwoState()
                        If layerData Is Nothing Then layerData = bmd.LayerHabitat
                        layer = New cLayer(layerData, renderer, editor, iHabitat, 0, hab, eVarNameFlags.Name)
                        lLayers.Add(layer)

                    Next iHabitat

                Case eVarNameFlags.LayerRegion

                    ' Get or create Visual Style
                    strID = cValueID.GenerateAbstract(eDataTypes.EcospaceRegion, 1, "Regions")
                    vs = core.VisualStyle(strID)
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        vs.BackColour = Color.Transparent
                        core.VisualStyle(strID) = vs
                    End If

                    ' Represent regions as a gradient
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange()
                    If layerData Is Nothing Then layerData = bmd.LayerRegion
                    layer = New cLayer(layerData, renderer, editor, bmd, eVarNameFlags.Name)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_REGIONS

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPA

                    avs = brushProvider.GetVisualStyles(core.nMPAs, cEwEBrushProvider.eBrushType.HatchPattern)

                    For iMPA As Integer = 1 To core.nMPAs

                        Dim mpa As cEcospaceMPA = core.EcospaceMPAs(iMPA)

                        ' Get or create Visual Style
                        strID = mpa.getID()
                        vs = core.VisualStyle(strID)
                        If vs Is Nothing Then
                            vs = avs(iMPA)
                            core.VisualStyle(strID, False) = vs
                        End If

                        ' Create layer
                        renderer = New cLayerRendererHatch(vs)
                        editor = New cLayerEditorTwoState()
                        If layerData Is Nothing Then layerData = bmd.LayerMPA
                        layer = New cLayer(layerData, renderer, editor, iMPA, 0, mpa, eVarNameFlags.Name)

                        lLayers.Add(layer)

                    Next iMPA

                Case eVarNameFlags.LayerRelPP

                    ' Get or create Visual Style
                    strID = cValueID.GenerateAbstract(eDataTypes.EcospaceBasemap, CInt(bmd.GetVariable(eVarNameFlags.DBID)), eVarNameFlags.LayerRelPP)
                    vs = core.VisualStyle(strID)
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        core.VisualStyle(strID) = vs
                    End If

                    ' Represent as a solid colour
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorRange()
                    If layerData Is Nothing Then layerData = bmd.LayerRelPP
                    layer = New cLayer(layerData, renderer, editor, bmd, eVarNameFlags.LayerRelPP)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_RELPP

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerRelCin

                    ' Get or create Visual Style
                    strID = cValueID.GenerateAbstract(eDataTypes.EcospaceBasemap, CInt(bmd.GetVariable(eVarNameFlags.DBID)), eVarNameFlags.LayerRelCin)
                    vs = core.VisualStyle(strID)
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        core.VisualStyle(strID) = vs
                    End If

                    ' Represent as a solid colour
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorTwoState()
                    If layerData Is Nothing Then layerData = bmd.LayerRelCin
                    layer = New cLayer(layerData, renderer, editor, bmd, eVarNameFlags.LayerRelCin)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_RELCIN

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPASeed

                    vs = New cVisualStyle()
                    vs.ForeColour = Color.CornflowerBlue

                    ' Represent MPA seeds as a solid colour
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorTwoState()
                    If layerData Is Nothing Then layerData = bmd.LayerMPASeed
                    layer = New cLayer(layerData, renderer, editor, 1, 0, bmd, eVarNameFlags.LayerMPASeed)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_MPASEED

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPASeedCurrent

                    vs = New cVisualStyle()
                    vs.ForeColour = Color.LightGreen

                    ' Represent MPA seeds as a solid colour
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorTwoState()
                    If layerData Is Nothing Then Debug.Assert(False, "Cannot link to core data")
                    layer = New cLayer(layerData, renderer, editor, cECOSEED_LAYER_CURRENTVALUE, cECOSEED_LAYER_NOVALUE)
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

                    layer = New cLayer(layerData, renderer, editor, cECOSEED_LAYER_BESTVALUE, cECOSEED_LAYER_NOVALUE)
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
                        layer = New cLayer(layerData, renderer, editor)
                        layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_RANDOMBEST
                        layer.Editor.IsReadOnly = True

                        lLayers.Add(layer)
                    End If

                Case eVarNameFlags.LayerMigration

                    ' Get or create Visual Style
                    strID = cValueID.GenerateAbstract(eDataTypes.EcospaceBasemap, CInt(bmd.GetVariable(eVarNameFlags.DBID)), eVarNameFlags.LayerMigration)
                    vs = core.VisualStyle(strID)
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        core.VisualStyle(strID) = vs
                    End If

                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorMigration()
                    If layerData Is Nothing Then layerData = bmd.LayerMigration
                    layer = New cLayer(layerData, renderer, editor, bmd, eVarNameFlags.LayerMigration)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_MIGRATION

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerAdvection

                    ' Get or create Visual Style
                    strID = cValueID.GenerateAbstract(eDataTypes.EcospaceBasemap, CInt(bmd.GetVariable(eVarNameFlags.DBID)), eVarNameFlags.LayerAdvection)
                    vs = core.VisualStyle(strID)
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        core.VisualStyle(strID) = vs
                    End If

                    renderer = New cLayerRendererArrow(vs)
                    editor = New cLayerEditorAdvection()
                    If layerData Is Nothing Then layerData = bmd.LayerMigration
                    layer = New cLayer(layerData, renderer, editor, bmd, eVarNameFlags.LayerMigration)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_MIGRATION

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerPort

                    ' Get or create Visual Style
                    strID = cValueID.GenerateAbstract(eDataTypes.EcospaceBasemap, CInt(bmd.GetVariable(eVarNameFlags.DBID)), eVarNameFlags.LayerPort)
                    vs = core.VisualStyle(strID)
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.BurlyWood
                        core.VisualStyle(strID) = vs
                    End If

                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorFleet()
                    If layerData Is Nothing Then layerData = bmd.LayerPort
                    layer = New cLayer(layerData, renderer, editor, 1.0!, 0.0!, bmd, eVarNameFlags.LayerPort)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_PORT

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerSail

                    ' Get or create Visual Style
                    strID = cValueID.GenerateAbstract(eDataTypes.EcospaceBasemap, CInt(bmd.GetVariable(eVarNameFlags.DBID)), eVarNameFlags.LayerSail)
                    vs = core.VisualStyle(strID)
                    If vs Is Nothing Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        core.VisualStyle(strID) = vs
                    End If

                    ' Represent as a solid colour
                    renderer = New cLayerRendererValue(vs)
                    editor = New cLayerEditorFleet()
                    editor.IsEditable = False
                    If layerData Is Nothing Then layerData = bmd.LayerSailingCost
                    layer = New cLayer(layerData, renderer, editor, bmd, eVarNameFlags.LayerSail)
                    layer.Name = My.Resources.ECOSPACE_BASEMAP_LAYERS_SAILINGCOST

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerImportance

                    For iLayer As Integer = 1 To core.nImportanceLayers

                        Dim src As cEcospaceLayerImportance = core.EcospaceBasemap.LayerImportance(iLayer)

                        ' Get or create Visual Style
                        strID = src.getID()
                        vs = core.VisualStyle(strID)
                        If vs Is Nothing Then
                            vs = New cVisualStyle()
                            vs.ForeColour = Color.Black
                            core.VisualStyle(strID) = vs
                        End If

                        ' Create layer
                        renderer = New cLayerRendererValue(vs)
                        editor = New cLayerEditorTwoState()
                        layer = New cLayer(bmd.LayerImportance(iLayer), renderer, editor, src, eVarNameFlags.Name)

                        lLayers.Add(layer)

                    Next iLayer

            End Select

            Return lLayers.ToArray()

        End Function

        Public Shared Function GetLayerGroup(ByVal varName As eVarNameFlags) As String

            Dim strGroup As String = ""
            Select Case varName

                Case eVarNameFlags.LayerDepth
                    strGroup = My.Resources.ECOSPACE_BASEMAP_LAYERS_LAND

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
