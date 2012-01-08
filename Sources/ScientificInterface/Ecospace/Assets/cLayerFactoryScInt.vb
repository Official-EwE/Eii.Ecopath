#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Controls.Map.Layers

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' =======================================================================
    ''' <summary>
    ''' Factory for returning <see cref="cLayer">UI layer wrappers</see> for 
    ''' <see cref="cEcospaceLayer">Ecospace basemap layer data.</see>
    ''' </summary>
    ''' =======================================================================
    Friend Class cLayerFactoryInternal
        Inherits cLayerFactoryBase

        Public Const cECOSEED_LAYER_NOVALUE As Integer = 0
        Public Const cECOSEED_LAYER_CURRENTVALUE As Integer = 1
        Public Const cECOSEED_LAYER_BESTVALUE As Integer = 2

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
        Public Overrides Function GetLayers(ByVal uic As cUIContext, _
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
 
            Select Case varName

                Case eVarNameFlags.LayerMPASeed

                    vs = New cVisualStyle()
                    vs.ForeColour = Color.CornflowerBlue

                    ' Represent MPA seeds as a solid colour
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorTwoState()
                    If layerData Is Nothing Then layerData = bmd.LayerMPASeed
                    layer = New cLayer(uic, layerData, renderer, editor, 1, 0, bmd, eVarNameFlags.LayerMPASeed)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPASeedCurrent

                    Debug.Assert(layerData IsNot Nothing, "Cannot link to core data")

                    vs = New cVisualStyle()
                    vs.ForeColour = Color.LightGreen

                    ' Represent MPA seeds as a solid colour
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorTwoState()
                    layer = New cLayer(uic, layerData, renderer, editor, cECOSEED_LAYER_CURRENTVALUE, cECOSEED_LAYER_NOVALUE)
                    layer.Name = My.Resources.ECOSPACE_LAYER_SEEDCURRENT ' Use local layer name
                    layer.Editor.IsReadOnly = True

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPASeedBest

                    Debug.Assert(layerData IsNot Nothing, "Cannot link to core data")

                    vs = New cVisualStyle()
                    vs.ForeColour = Color.DarkGreen
                    vs.BackColour = Color.Transparent

                    ' Represent MPA seeds as a solid colour
                    renderer = New cLayerRendererSymbol(vs)
                    editor = New cLayerEditorTwoState()

                    layer = New cLayer(uic, layerData, renderer, editor, cECOSEED_LAYER_BESTVALUE, cECOSEED_LAYER_NOVALUE)
                    layer.Name = My.Resources.ECOSPACE_LAYER_SEEDBEST
                    layer.Editor.IsReadOnly = True

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMPARandom

                    If (layerData IsNot Nothing) Then
                        vs = New cVisualStyle()
                        vs.ForeColour = Color.Black
                        vs.BackColour = Color.Blue

                        renderer = New cLayerRendererValue(vs)
                        editor = New cLayerEditorRange()
                        layer = New cLayer(uic, layerData, renderer, editor)
                        layer.Name = My.Resources.ECOSPACE_LAYER_RANDOMBEST
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

                    lLayers.Add(layer)

                Case Else
                    ' Return default
                    lLayers.AddRange(MyBase.GetLayers(uic, varName, layerData))

            End Select

            Return lLayers.ToArray()

        End Function

        Public Overrides Function GetLayerGroup(ByVal varName As eVarNameFlags) As String

            Dim strGroup As String = ""
            Select Case varName

                Case eVarNameFlags.LayerMPASeed, _
                    eVarNameFlags.LayerMPASeedBest, _
                    eVarNameFlags.LayerMPASeedCurrent
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_ECOSEED

                Case eVarNameFlags.LayerMPARandom
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_MPARANDOM

                Case Else
                    Return MyBase.GetLayerGroup(varName)

            End Select
            Return strGroup

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a collection with the EwE foundation layers.
        ''' </summary>
        ''' <param name="uic"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function BaseRasterLayers(uic As cUIContext) As cLayer()

            Dim lLayers As New List(Of cLayer)

            lLayers.AddRange(Me.GetLayers(uic, EwEUtils.Core.eVarNameFlags.LayerDepth))
            lLayers.AddRange(Me.GetLayers(uic, EwEUtils.Core.eVarNameFlags.LayerMPA))
            lLayers.AddRange(Me.GetLayers(uic, EwEUtils.Core.eVarNameFlags.LayerHabitat))
            lLayers.AddRange(Me.GetLayers(uic, EwEUtils.Core.eVarNameFlags.LayerHabitatCapacityInput))
            lLayers.AddRange(Me.GetLayers(uic, EwEUtils.Core.eVarNameFlags.LayerRelPP))
            lLayers.AddRange(Me.GetLayers(uic, EwEUtils.Core.eVarNameFlags.LayerRelCin))
            lLayers.AddRange(Me.GetLayers(uic, EwEUtils.Core.eVarNameFlags.LayerImportance))

            Return lLayers.ToArray()

        End Function
    End Class

End Namespace