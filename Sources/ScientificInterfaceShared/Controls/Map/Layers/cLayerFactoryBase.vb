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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore.Auxiliary
Imports EwECore.Style
Imports EwEUtils.UserInterface
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
        Public Overridable Function GetLayers(uic As cUIContext,
                                              varName As eVarNameFlags) As cDisplayLayerRaster()

            Dim lLayers As New List(Of cDisplayLayerRaster)

            Dim core As cCore = uic.Core
            Dim bmd As cEcospaceBasemap = core.EcospaceBasemap
            Dim layer As cDisplayLayerRaster = Nothing
            Dim ad As cAuxiliaryData = Nothing
            Dim avs As cVisualStyle() = Nothing
            Dim renderer As cLayerRenderer = Nothing
            Dim editor As cLayerEditor = Nothing
            Dim vs As cVisualStyle = Nothing
            Dim fmt As New cVarnameTypeFormatter()

            Select Case varName

                Case eVarNameFlags.LayerDepth

                    ad = Me.GetAuxillaryData(core, varName)

                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererDepth(uic, vs)
                    editor = New cLayerEditorDepth()
                    layer = New cDisplayLayerRaster(uic, bmd.LayerDepth, renderer, editor, bmd, varName)
                    lLayers.Add(layer)

                Case eVarNameFlags.LayerHabitat

                    avs = uic.StyleGuide.GetVisualStyles(core.nHabitats, cStyleGuide.eBrushType.Glyphs)

                    For iHabitat As Integer = 1 To core.nHabitats - 1

                        Dim hab As cEcospaceHabitat = core.EcospaceHabitats(iHabitat)

                        ' Get or create Visual Style
                        ad = Me.GetAuxillaryData(core, varName, iHabitat)
                        vs = ad.VisualStyle
                        If (vs Is Nothing) Then
                            vs = avs(iHabitat - 1)
                            ad.AllowValidation = False
                            ad.VisualStyle = vs
                            ad.AllowValidation = True
                        End If

                        ' Create layer
                        renderer = New cLayerRendererBitmap(uic, vs)
                        renderer.RenderMode = Definitions.eLayerRenderType.Grouped

                        editor = New cLayerEditorHabitat()
                        layer = New cDisplayLayerRaster(uic, bmd.LayerHabitat(iHabitat), renderer, editor, hab, eVarNameFlags.Name, sValueClear:=0)
                        lLayers.Add(layer)

                    Next iHabitat

                Case eVarNameFlags.LayerHabitatCapacity,
                     eVarNameFlags.LayerHabitatCapacityInput

                    If (core.nGroups > 0) Then

                        ad = Me.GetAuxillaryData(core, varName)
                        vs = ad.VisualStyle
                        If (vs Is Nothing) Then vs = New cVisualStyle(ad)

                        renderer = New cLayerRendererValue(uic, vs)
                        renderer.ScaleMin = 0
                        renderer.RenderMode = Definitions.eLayerRenderType.Selected

                        If (varName = eVarNameFlags.LayerHabitatCapacity) Then
                            editor = New cLayerEditorGroup(GetType(ucLayerEditorHabitatCapacityComputed))
                        Else
                            editor = New cLayerEditorGroup(GetType(ucLayerEditorHabitatCapacity))
                        End If

                        layer = New cDisplayLayerRasterBundle(uic, bmd.Layers(varName), renderer, editor, eCoreCounterTypes.nGroups, bmd, varName)
                        layer.Editor.IsReadOnly = (varName = eVarNameFlags.LayerHabitatCapacity)
                        lLayers.Add(layer)
                    End If

                Case eVarNameFlags.LayerRegion

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle
                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(uic, vs)
                    renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorRegion()

                    layer = New cDisplayLayerRaster(uic, bmd.LayerRegion, renderer, editor, bmd, varName)
                    lLayers.Add(layer)

                Case eVarNameFlags.LayerEffortZone

                    Dim src As cEcospaceLayerEffortZone = core.EcospaceBasemap.LayerEffortZone

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle

                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(uic, vs)
                    renderer.ScaleMin = 1
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected
                    editor = New cLayerEditorRange()
                    layer = New cDisplayLayerRaster(uic, src, renderer, editor, src, eVarNameFlags.Name)

                    lLayers.Add(layer)
                Case eVarNameFlags.LayerMPA

                    avs = uic.StyleGuide.GetVisualStyles(core.nMPAs, cStyleGuide.eBrushType.HatchPattern)

                    For iMPA As Integer = 1 To core.nMPAs

                        Dim mpa As cEcospaceMPA = core.EcospaceMPAs(iMPA)
                        ad = Me.GetAuxillaryData(core, varName, iMPA)

                        ' Get or create Visual Style
                        vs = ad.VisualStyle
                        If (vs Is Nothing) Then
                            vs = avs(iMPA)
                            ad.AllowValidation = False
                            ad.VisualStyle = vs
                            ad.AllowValidation = True
                        End If

                        ' Create layer
                        renderer = New cLayerRendererHatch(uic, vs)
                        renderer.RenderMode = Definitions.eLayerRenderType.Always

                        editor = New cLayerEditorTwoState()
                        layer = New cDisplayLayerRaster(uic, bmd.LayerMPA(iMPA), renderer, editor, mpa, eVarNameFlags.Name, 1, 0)

                        lLayers.Add(layer)

                    Next iMPA

                Case eVarNameFlags.LayerRelPP

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle

                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(uic, vs)
                    'renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorRange()
                    layer = New cDisplayLayerRaster(uic, bmd.LayerRelPP, renderer, editor, bmd, varName)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerContaminantRelativeDistribution

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle

                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(uic, vs)
                    'renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorRange()
                    layer = New cDisplayLayerRaster(uic, bmd.LayerRelCin, renderer, editor, bmd, varName)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerContaminantForcingAbsolute

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle

                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(uic, vs)
                    'renderer.ScaleMin = 0
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected

                    editor = New cLayerEditorRange()
                    layer = New cDisplayLayerRaster(uic, bmd.LayerContaminantForcingAbsolute, renderer, editor, bmd, varName)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerMigration

                    For iLayer As Integer = 1 To core.nGroups
                        Dim grp As cEcospaceGroupInput = core.EcospaceGroupInputs(iLayer)
                        If grp.IsMigratory Then
                            Dim src As cEcospaceLayerMigration = core.EcospaceBasemap.LayerMigration(iLayer)
                            ad = Me.GetAuxillaryData(core, varName, iLayer)
                            vs = ad.VisualStyle
                            If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                            renderer = New cLayerRendererValue(uic, vs)
                            renderer.RenderMode = Definitions.eLayerRenderType.Selected
                            DirectCast(renderer, cLayerRendererValue).SuppressZero = True
                            editor = New cLayerEditorMigration()
                            layer = New cDisplayLayerRaster(uic, src, renderer, editor, src, eVarNameFlags.Name)
                            lLayers.Add(layer)
                        End If
                    Next

                Case eVarNameFlags.LayerAdvection

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle

                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererWindEwE5(uic, vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected
                    editor = New cLayerEditorVelocity()

                    layer = New cDisplayLayerRaster(uic, core.EcospaceBasemap.LayerAdvection(0), renderer, editor, bmd, varName)

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerWind

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle

                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererWindEwE5(uic, vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected
                    editor = New cLayerEditorVelocity()

                    layer = New cDisplayLayerRaster(uic, core.EcospaceBasemap.LayerWind(0), renderer, editor, bmd, varName)
                    lLayers.Add(layer)

                Case eVarNameFlags.LayerUpwelling

                    ' ToDo: globalize this

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle

                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererUpwelling(uic, vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected
                    editor = New cLayerEditorUpwelling()
                    layer = New cDisplayLayerRaster(uic, bmd.LayerUpwelling, renderer, editor, bmd, varName)
                    layer.Name = "Upwelling"

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerPort

                    If (core.nFleets > 0) Then
                        ad = Me.GetAuxillaryData(core, varName)
                        vs = ad.VisualStyle

                        If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                        renderer = New cLayerRendererSymbol(uic, vs)
                        renderer.RenderMode = Definitions.eLayerRenderType.Always
                        editor = New cLayerEditorPorts(GetType(ucLayerEditorPort))
                        layer = New cDisplayLayerRasterBundle(uic, bmd.Layers(eVarNameFlags.LayerPort), renderer, editor, eCoreCounterTypes.nFleets, bmd, varName, CSng(True), CSng(False))
                        lLayers.Add(layer)
                    End If

                Case eVarNameFlags.LayerSail

                    If (core.nFleets > 0) Then

                        ad = Me.GetAuxillaryData(core, varName)
                        vs = ad.VisualStyle

                        If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                        renderer = New cLayerRendererValue(uic, vs)
                        renderer.ScaleMin = 0
                        renderer.RenderMode = Definitions.eLayerRenderType.Selected
                        editor = New cLayerEditorSailCost(GetType(ucLayerEditorSailCost))
                        layer = New cDisplayLayerRasterBundle(uic, bmd.Layers(eVarNameFlags.LayerSail), renderer, editor, eCoreCounterTypes.nFleets, bmd, varName)

                        lLayers.Add(layer)
                    End If

                Case eVarNameFlags.LayerDriver

                    For iLayer As Integer = 1 To core.nEnvironmentalDriverLayers

                        Dim src As cEcospaceLayerDriver = core.EcospaceBasemap.LayerDriver(iLayer)
                        ad = Me.GetAuxillaryData(core, varName, iLayer)
                        vs = ad.VisualStyle

                        If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                        renderer = New cLayerRendererValue(uic, vs)
                        'renderer.ScaleMin = 0
                        renderer.RenderMode = Definitions.eLayerRenderType.Selected
                        editor = New cLayerEditorRange()
                        layer = New cDisplayLayerRaster(uic, src, renderer, editor, src, eVarNameFlags.Name)

                        lLayers.Add(layer)

                    Next iLayer

                Case eVarNameFlags.LayerExclusion

                    Dim src As cEcospaceLayerExclusion = core.EcospaceBasemap.LayerExclusion

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle

                    If (vs Is Nothing) Then
                        vs = New cVisualStyle()
                        vs.ForeColour = VisualColor.FromArgb(&HFF060606)
                        vs.BackColour = VisualColor.FromArgb(64, 255, 255, 255)
                        vs.HatchStyle = VisualHatchStyle.BackwardDiagonal
                        ad.AllowValidation = False
                        ad.VisualStyle = vs
                        ad.AllowValidation = True
                    End If
                    renderer = New cLayerRendererExclusion(uic, vs)
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected
                    editor = New cLayerEditorTwoState(GetType(ucLayerEditorExclusion), False)
                    layer = New cDisplayLayerRaster(uic, src, renderer, editor, src, eVarNameFlags.Name, CSng(True), CSng(False))

                    lLayers.Add(layer)

                Case eVarNameFlags.LayerCellArea

                    Dim src As cEcospaceLayerCellArea = core.EcospaceBasemap.LayerCellArea

                    ad = Me.GetAuxillaryData(core, varName)
                    vs = ad.VisualStyle

                    If (vs Is Nothing) Then vs = New cVisualStyle(ad)
                    renderer = New cLayerRendererValue(uic, vs)
                    renderer.ScaleMin = 1
                    renderer.RenderMode = Definitions.eLayerRenderType.Selected
                    editor = New cLayerEditorRange()
                    layer = New cDisplayLayerRaster(uic, src, renderer, editor, src, eVarNameFlags.Name)

                    lLayers.Add(layer)

                Case Else
                    Debug.Assert(False, "No layers available for this varname")

            End Select

            Return lLayers.ToArray()

        End Function

        Public Overridable Function GetLayerGroup(varName As eVarNameFlags) As String

            Dim strGroup As String = ""
            Select Case varName

                Case eVarNameFlags.LayerDepth,
                     eVarNameFlags.LayerExclusion,
                     eVarNameFlags.LayerRelPP,
                     eVarNameFlags.LayerCellArea
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_BASEMAP

                Case eVarNameFlags.LayerContaminantRelativeDistribution,
                     eVarNameFlags.LayerContaminantForcingAbsolute
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_CONTAMINANTS

                Case eVarNameFlags.LayerHabitat
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_HABITATS

                Case eVarNameFlags.LayerHabitatCapacityInput
                    Return My.Resources.ECOSPACE_LAYERGROUP_CAPACITYINPUT

                Case eVarNameFlags.LayerHabitatCapacity
                    Return My.Resources.ECOSPACE_LAYERGROUP_COMPUTEDCAPACITY
                Case eVarNameFlags.LayerMigration
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_MIGRATION

                Case eVarNameFlags.LayerRegion
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_REGIONS

                Case eVarNameFlags.LayerMPA
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_MPAS

                Case eVarNameFlags.LayerPort,
                     eVarNameFlags.LayerSail,
                     eVarNameFlags.LayerEffortZone
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_FISHING

                Case eVarNameFlags.LayerDriver
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_ENVDRIVERS

                Case eVarNameFlags.LayerBiomassForcing
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_BIOMASSFORCING

                Case eVarNameFlags.LayerBiomassRelativeForcing
                    strGroup = My.Resources.ECOSPACE_LAYERGROUP_BIOMASSRELATIVEFORCING '"Relative biomass forcing"

                Case eVarNameFlags.LayerAdvection,
                     eVarNameFlags.LayerWind,
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
        Public Overridable Function GetLayerEditCommand(varName As eVarNameFlags) As String

            Dim strCommand As String = ""
            Select Case varName

                Case eVarNameFlags.LayerHabitat
                    strCommand = cEditHabitatsCommand.cCOMMAND_NAME

                Case eVarNameFlags.LayerMPA
                    strCommand = cEditMPAsCommand.cCOMMAND_NAME

                Case eVarNameFlags.LayerRegion
                    strCommand = cEditRegionsCommand.cCOMMAND_NAME

                Case eVarNameFlags.LayerEffortZone
                    strCommand = cEditEffortZonesCommand.cCOMMAND_NAME

                Case eVarNameFlags.LayerImportance
                    strCommand = cEditImportanceLayersCommand.cCOMMAND_NAME

                Case eVarNameFlags.LayerDriver
                    strCommand = cEditDriverLayersCommand.cCOMMAND_NAME

            End Select
            Return strCommand

        End Function

        Public Overridable Function GetAuxillaryData(core As cCore, l As cEcospaceLayer) As cAuxiliaryData

            If (l Is Nothing) Then Return Nothing
            Return Me.GetAuxillaryData(core, l.VarName, l.Index)

        End Function

        Public Overridable Function GetAuxillaryData(core As cCore, vn As eVarNameFlags, Optional iIndex As Integer = 0) As cAuxiliaryData

            If (core Is Nothing) Then Return Nothing

            Dim iScenario As Integer = core.ActiveEcospaceScenarioIndex
            If (iScenario <= 0) Then Return Nothing

            Dim DBID As Integer = core.EcospaceScenarios(iScenario).DBID
            Dim dt As eDataTypes = eDataTypes.NotSet

            Dim layers As cEcospaceLayer() = core.EcospaceBasemap.Layers(vn)
            Dim layer As cEcospaceLayer = layers(Math.Max(iIndex - 1, 0))

            dt = layer.DataType

            Select Case vn
                Case eVarNameFlags.LayerHabitat
                    DBID = core.EcospaceHabitats(iIndex).DBID
                Case eVarNameFlags.LayerMPA
                    DBID = core.EcospaceMPAs(iIndex).DBID
                Case eVarNameFlags.LayerDriver
                    DBID = layer.DBID
            End Select

            Dim key As New cValueID(dt, DBID, eVarNameFlags.Name)
            Return core.AuxillaryData(key)

        End Function

    End Class

End Namespace
