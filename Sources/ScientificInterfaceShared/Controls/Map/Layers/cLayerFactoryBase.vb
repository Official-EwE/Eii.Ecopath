' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.Map

    ''' =======================================================================
    ''' <summary>
    ''' Factory for returning <see cref="cDisplayLayer">display layers</see> for 
    ''' given <see cref="cEcospaceLayer">Ecospace data layers.</see>
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
                                              ByVal varName As eVarNameFlags) As cDisplayRasterLayer()

            Dim lLayers As New List(Of cDisplayRasterLayer)

            Dim core As cCore = uic.Core
            Dim bmd As cEcospaceBasemap = core.EcospaceBasemap
            Dim layer As cDisplayRasterLayer = Nothing
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
                    renderer.RenderMode = Definitions.eLayerRenderType.Always
                    editor = New cLayerEditorDepth()
                    layer = New cDisplayRasterLayer(uic, bmd.LayerDepth, renderer, editor, bmd, eVarNameFlags.LayerDepth)
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
                        renderer.RenderMode = Definitions.eLayerRenderType.Grouped

                        editor = New cLayerEditorHabitat()
                        layer = New cDisplayRasterLayer(uic, bmd.LayerHabitat(iHabitat), renderer, editor, hab, eVarNameFlags.Name, sValueClear:=0)
                        lLayers.Add(layer)

                    Next iHabitat

                Case eVarNameFlags.LayerHabitatCapacityInput

                    key = New cValueID(eDataTypes.EcospaceLayerHabitatCapacityInput, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorGroup(GetType(ucLayerEditorHabitatCapacity))
                    layer = New cDisplayRasterLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerHabitatCapacityInput), _
                                            renderer, editor, eCoreCounterTypes.nGroups, bmd, eVarNameFlags.LayerHabitatCapacityInput)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerHabitatCapacity

                    key = New cValueID(eDataTypes.EcospaceLayerHabitatCapacity, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorGroup(GetType(ucLayerEditorGroup))
                    editor.IsReadOnly = True

                    layer = New cDisplayRasterLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerHabitatCapacity), _
                                            renderer, editor, eCoreCounterTypes.nGroups, bmd, eVarNameFlags.LayerHabitatCapacity)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerRegion

                    key = New cValueID(eDataTypes.EcospaceLayerRegion, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorRegion()

                    layer = New cDisplayRasterLayer(uic, bmd.LayerRegion, renderer, editor, bmd, eVarNameFlags.LayerRegion)
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
                        renderer.RenderMode = Definitions.eLayerRenderType.Always

                        editor = New cLayerEditorTwoState()
                        layer = New cDisplayRasterLayer(uic, bmd.LayerMPA(iMPA), renderer, editor, mpa, eVarNameFlags.Name, 1, 0)

                        lLayers.Add(layer)

                    Next iMPA

                Case eVarNameFlags.LayerRelPP

                    key = New cValueID(eDataTypes.EcospaceLayerRelPP, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorRange()
                    layer = New cDisplayRasterLayer(uic, bmd.LayerRelPP, renderer, editor, bmd, eVarNameFlags.LayerRelPP)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerRelCin

                    key = New cValueID(eDataTypes.EcospaceLayerRelCin, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorRange()
                    layer = New cDisplayRasterLayer(uic, bmd.LayerRelCin, renderer, editor, bmd, eVarNameFlags.LayerRelCin)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMigration

                    key = New cValueID(eDataTypes.EcospaceLayerMigration, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    ' Get or create Visual Style
                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorMigration()
                    layer = New cDisplayRasterLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerMigration), _
                                            renderer, editor, eCoreCounterTypes.nGroups, bmd, eVarNameFlags.LayerMigration)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerAdvection

                    key = New cValueID(eDataTypes.EcospaceLayerAdvection, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererWindEwE5(vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Always
                    layer = New cDisplayRasterLayer(uic, bmd.LayerAdvection, renderer, Nothing, bmd, eVarNameFlags.LayerAdvection)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerWind

                    key = New cValueID(eDataTypes.EcospaceLayerWind, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererWindEwE5(vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Always
                    editor = New cLayerEditorVector(Nothing)
                    layer = New cDisplayRasterLayer(uic, bmd.LayerWind, renderer, editor, bmd, eVarNameFlags.LayerWind)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerUpwelling

                    ' ToDo: globalize this

                    key = New cValueID(eDataTypes.EcospaceLayerFlow, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererUpwelling(vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Always
                    editor = New cLayerEditorRange()
                    layer = New cDisplayRasterLayer(uic, bmd.LayerUpwelling, renderer, editor, bmd, eVarNameFlags.LayerUpwelling)
                    layer.Name = "Upwelling"

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMLD

                    ' ToDo: globalize this

                    key = New cValueID(eDataTypes.EcospaceLayerMLD, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererText(vs) ' MLD rendered as text on top of gradiented layers such as habitats, etc
                    renderer.RenderMode = Definitions.eLayerRenderType.Always
                    editor = New cLayerEditorMLD()
                    layer = New cDisplayRasterLayer(uic, bmd.LayerMixedLayerDepths, renderer, editor, bmd, varName)
                    layer.Name = "Mixed layer depths"

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerPort

                    key = New cValueID(eDataTypes.EcospaceLayerPort, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererSymbol(vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Always
                    editor = New cLayerEditorPorts(GetType(ucLayerEditorPort))
                    layer = New cDisplayRasterLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerPort), renderer, editor, eCoreCounterTypes.nFleets, bmd, eVarNameFlags.LayerPort, 1.0!, 0.0!)
                    lLayers.Add(layer)

                Case eVarNameFlags.LayerSail

                    key = New cValueID(eDataTypes.EcospaceLayerSail, bmd.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(vs)
                    renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected
                    editor = New cLayerEditorSailCost(GetType(ucLayerEditorSailCost))
                    layer = New cDisplayRasterLayerBundle(uic, bmd.Layers(eVarNameFlags.LayerSail), renderer, editor, eCoreCounterTypes.nFleets, bmd, eVarNameFlags.LayerSail)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerImportance

                    For iLayer As Integer = 1 To core.nImportanceLayers

                        Dim src As cEcospaceLayerImportance = core.EcospaceBasemap.LayerImportance(iLayer)
                        key = New cValueID(src.DataType, src.DBID, eVarNameFlags.Name)
                        ad = core.AuxillaryData(key)

                        vs = ad.VisualStyle
                        If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                        renderer = New cLayerRendererValue(vs)
                        renderer.ScaleMin = 0
                        renderer.RenderMode = Definitions.eLayerRenderType.Selected
                        editor = New cLayerEditorRange()
                        layer = New cDisplayRasterLayer(uic, src, renderer, editor, src, eVarNameFlags.Name)

                        lLayers.Add(layer)

                    Next iLayer

                Case eVarNameFlags.LayerDriver

                    For iLayer As Integer = 1 To core.nEnvironmentalDriverLayers

                        Dim src As cEcospaceLayerDriver = core.EcospaceBasemap.LayerDriver(iLayer)
                        key = New cValueID(src.DataType, src.DBID, eVarNameFlags.Name)
                        ad = core.AuxillaryData(key)

                        vs = ad.VisualStyle
                        If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                        renderer = New cLayerRendererValue(vs)
                        'renderer.ScaleMin = 0
                        renderer.RenderMode = Definitions.eLayerRenderType.Selected
                        editor = New cLayerEditorRange()
                        layer = New cDisplayRasterLayer(uic, src, renderer, editor, src, eVarNameFlags.Name)

                        lLayers.Add(layer)

                    Next iLayer

                Case eVarNameFlags.LayerExclusion

                    Dim src As cEcospaceLayerExclusion = core.EcospaceBasemap.LayerExclusion
                    key = New cValueID(src.DataType, src.DBID, eVarNameFlags.Name)
                    ad = core.AuxillaryData(key)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then
                        vs = New cVisualStyle(ad)
                        vs.ForeColour = Color.Red
                        vs.BackColour = Color.OrangeRed
                        vs.HatchStyle = Drawing2D.HatchStyle.DiagonalCross
                    End If
                    renderer = New cLayerRendererExclusion(vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected
                    editor = New cLayerEditorTwoState(GetType(ucLayerEditorExclusion), False)
                    layer = New cDisplayRasterLayer(uic, src, renderer, editor, src, eVarNameFlags.Name, CSng(True), CSng(False))

                    lLayers.Add(layer)

                Case Else
                    Debug.Assert(False, "No layers available for this varname")

            End Select

            Return lLayers.ToArray()

        End Function

        Public Overridable Function GetLayerGroup(ByVal varName As eVarNameFlags) As String

            Dim strGroup As String = ""
            Select Case varName

                Case eVarNameFlags.LayerDepth, _
                     eVarNameFlags.LayerExclusion
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
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_DATA

                Case eVarNameFlags.LayerPort, _
                      eVarNameFlags.LayerSail
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_FISHING

                Case eVarNameFlags.LayerImportance
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_IMPORTANCE

                Case eVarNameFlags.LayerDriver
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_ENVDRIVERS

                Case eVarNameFlags.LayerBiomassForcing
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_BIOMASSFORCING

                Case eVarNameFlags.LayerBiomassRelativeForcing
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_BIOMASSRELATIVEFORCING '"Relative biomass forcing"

                Case eVarNameFlags.LayerAdvection, _
                     eVarNameFlags.LayerMLD, _
                     eVarNameFlags.LayerWind, _
                     eVarNameFlags.LayerUpwelling
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_ADVECTION

            End Select
            Return strGroup

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of a <see cref="ScientificInterfaceShared.Commands.cCommand"/> that can
        ''' be triggered to modify the <see cref="ICoreInputOutput">core items </see>
        ''' reflected by a type of layer.
        ''' </summary>
        ''' <param name="varName">The <see cref="eVarNameFlags"/> to obtain the 
        ''' edit command for.</param>
        ''' <returns>A command name, or an empty string if not applicable.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetLayerEditCommand(ByVal varName As eVarNameFlags) As String

            Dim strCommand As String = ""
            Select Case varName

                Case eVarNameFlags.LayerHabitat
                    strCommand = cEditHabitatsCommand.cCOMMAND_NAME

                Case eVarNameFlags.LayerMPA
                    strCommand = cEditMPAsCommand.cCOMMAND_NAME

                Case eVarNameFlags.LayerRegion
                    strCommand = cEditRegionsCommand.cCOMMAND_NAME

                Case eVarNameFlags.LayerImportance
                    strCommand = cEditImportanceLayersCommand.cCOMMAND_NAME

                Case eVarNameFlags.LayerDriver
                    strCommand = cEditDriverLayersCommand.cCOMMAND_NAME

            End Select
            Return strCommand

        End Function

    End Class

End Namespace
