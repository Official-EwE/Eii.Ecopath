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

Imports System.Text
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' On-board helper class that actively updates model-derived settings in the style guide.
''' </summary>
''' -----------------------------------------------------------------------
Friend Class cStyleGuideUpdater
    Implements IDisposable

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_bIsEcopathLoaded As Boolean = False

    Private m_sm As cCoreStateMonitor = Nothing
    Private m_propNumDigits As cProperty = Nothing
    Private m_propGroupDigits As cProperty = Nothing
    Private m_propUnitTime As cIntegerProperty = Nothing
    Private m_propUnitTimeText As cStringProperty = Nothing
    Private m_propUnitCurrency As cIntegerProperty = Nothing
    Private m_propUnitCurrencyText As cStringProperty = Nothing
    Private m_propUnitMonetary As cStringProperty = Nothing

#End Region ' Private vars

    Public Sub New(uic As cUIContext)

        ' Sanity check
        Debug.Assert(uic IsNot Nothing)

        Me.m_uic = uic
        Me.m_sm = Me.m_uic.Core.StateMonitor

        AddHandler Me.m_sm.CoreExecutionStateEvent, AddressOf OnCoreStateEvent
        AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        If (Me.m_uic IsNot Nothing) Then

            RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            RemoveHandler Me.m_sm.CoreExecutionStateEvent, AddressOf OnCoreStateEvent

            Me.m_uic = Nothing
            Me.m_sm = Nothing

        End If
    End Sub

#Region " Private vars "

    Private Sub OnCoreStateEvent(csm As cCoreStateMonitor)
        If Me.m_bIsEcopathLoaded <> csm.HasEcopathLoaded Then
            Me.m_bIsEcopathLoaded = csm.HasEcopathLoaded
            Me.Update()
        End If
    End Sub

    Private ReadOnly Property Core() As cCore
        Get
            Return Me.m_uic.Core
        End Get
    End Property

    Private ReadOnly Property StyleGuide() As cStyleGuide
        Get
            Return Me.m_uic.StyleGuide
        End Get
    End Property

    Private Sub Update()

        Dim pm As cPropertyManager = Me.m_uic.PropertyManager

        Me.StyleGuide.SuspendEvents()
        Me.StyleGuide.ResetVisibleFlags(False)

        If Me.m_bIsEcopathLoaded Then

            Me.m_propGroupDigits = pm.GetProperty(Core.EwEModel, eVarNameFlags.GroupDigits)
            Me.m_propNumDigits = pm.GetProperty(Core.EwEModel, eVarNameFlags.NumDigits)
            AddHandler Me.m_propGroupDigits.PropertyChanged, AddressOf OnNumberFormatChanged
            AddHandler Me.m_propNumDigits.PropertyChanged, AddressOf OnNumberFormatChanged

            Me.m_propUnitCurrency = DirectCast(pm.GetProperty(Core.EwEModel, eVarNameFlags.UnitCurrency), cIntegerProperty)
            Me.m_propUnitCurrencyText = DirectCast(pm.GetProperty(Core.EwEModel, eVarNameFlags.UnitCurrencyCustomText), cStringProperty)
            AddHandler Me.m_propUnitCurrency.PropertyChanged, AddressOf OnCurrencyUnitChanged
            AddHandler Me.m_propUnitCurrencyText.PropertyChanged, AddressOf OnCurrencyUnitChanged

            Me.m_propUnitTime = DirectCast(pm.GetProperty(Core.EwEModel, eVarNameFlags.UnitTime), cIntegerProperty)
            Me.m_propUnitTimeText = DirectCast(pm.GetProperty(Core.EwEModel, eVarNameFlags.UnitTimeCustomText), cStringProperty)
            AddHandler Me.m_propUnitTime.PropertyChanged, AddressOf OnTimeUnitChanged
            AddHandler Me.m_propUnitTimeText.PropertyChanged, AddressOf OnTimeUnitChanged

            Me.m_propUnitMonetary = DirectCast(pm.GetProperty(Core.EwEModel, eVarNameFlags.UnitMonetary), cStringProperty)
            AddHandler Me.m_propUnitMonetary.PropertyChanged, AddressOf OnMonetaryUnitChanged

            Me.OnCurrencyUnitChanged(m_propUnitCurrency, cProperty.eChangeFlags.All)
            Me.OnTimeUnitChanged(m_propUnitTime, cProperty.eChangeFlags.All)
            Me.OnMonetaryUnitChanged(m_propUnitMonetary, cProperty.eChangeFlags.All)
            Me.OnNumberFormatChanged(m_propNumDigits, cProperty.eChangeFlags.All)

            ' Load item visibility settings from model
            Dim ad As cAuxiliaryData = Me.Core.AuxillaryData("StyleGuide")
            Me.StyleGuide.LoadPeristentSettings(ad.Settings)

        Else

            RemoveHandler Me.m_propNumDigits.PropertyChanged, AddressOf OnNumberFormatChanged
            RemoveHandler Me.m_propGroupDigits.PropertyChanged, AddressOf OnNumberFormatChanged
            Me.m_propNumDigits = Nothing
            Me.m_propGroupDigits = Nothing

            RemoveHandler Me.m_propUnitCurrency.PropertyChanged, AddressOf OnCurrencyUnitChanged
            RemoveHandler Me.m_propUnitCurrencyText.PropertyChanged, AddressOf OnCurrencyUnitChanged
            Me.m_propUnitCurrency = Nothing
            Me.m_propUnitCurrencyText = Nothing

            RemoveHandler Me.m_propUnitTime.PropertyChanged, AddressOf OnTimeUnitChanged
            RemoveHandler Me.m_propUnitTimeText.PropertyChanged, AddressOf OnTimeUnitChanged
            Me.m_propUnitTime = Nothing
            Me.m_propUnitTimeText = Nothing

            RemoveHandler Me.m_propUnitMonetary.PropertyChanged, AddressOf OnMonetaryUnitChanged
            Me.m_propUnitMonetary = Nothing

        End If

        Me.StyleGuide.ResumeEvents(False)

    End Sub

    Private Sub OnCurrencyUnitChanged(prop As cProperty, ct As cProperty.eChangeFlags)
        With Me.StyleGuide
            .SuspendEvents()
            .CurrencyUnit = DirectCast(Me.m_propUnitCurrency.GetValue(), eUnitCurrencyType)
            .CustomCurrencyUnitText = CStr(Me.m_propUnitCurrencyText.GetValue())
            .ResumeEvents()
        End With
    End Sub

    Private Sub OnTimeUnitChanged(prop As cProperty, ct As cProperty.eChangeFlags)
        With Me.StyleGuide
            .SuspendEvents()
            .TimeUnit = DirectCast(Me.m_propUnitTime.GetValue(), eUnitTimeType)
            .CustomTimeUnitText = CStr(Me.m_propUnitTimeText.GetValue())
            .ResumeEvents()
        End With
    End Sub

    Private Sub OnMonetaryUnitChanged(prop As cProperty, ct As cProperty.eChangeFlags)
        With Me.StyleGuide
            .SuspendEvents()
            .MonetaryUnit = DirectCast(Me.m_propUnitMonetary.GetValue(), String)
            .ResumeEvents()
        End With
    End Sub

    Private Sub OnNumberFormatChanged(prop As cProperty, ct As cProperty.eChangeFlags)
        With Me.StyleGuide
            .SuspendEvents()
            .NumDigits = CInt(Me.m_propNumDigits.GetValue())
            .GroupDigits = CBool(Me.m_propGroupDigits.GetValue())
            .ResumeEvents()
        End With
    End Sub

    Private Sub OnStyleGuideChanged(cf As cStyleGuide.eChangeType)

        If ((cf And (cStyleGuide.eChangeType.GroupVisibility Or cStyleGuide.eChangeType.FleetVisibility)) > 0) Then

            If (Me.Core.StateMonitor.HasEcopathLoaded) Then
                Dim ad As cAuxiliaryData = Me.Core.AuxillaryData("StyleGuide")
                Me.StyleGuide.SavePeristentSettings(ad.Settings)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Load the style guide from application settings
    ''' </summary>
    Public Sub Load()

        With Me.StyleGuide

            .SuspendEvents()

            .ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT) = My.Settings.ColorDefaultText
            .ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_BACKGROUND) = My.Settings.ColorDefaultBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_TEXT) = My.Settings.ColorNameText
            .ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_BACKGROUND) = My.Settings.ColorNameBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT) = My.Settings.ColorFailedResultText
            .ApplicationColor(cStyleGuide.eApplicationColorType.FAILEDVALIDATION_TEXT) = My.Settings.ColorFailedValidationText
            .ApplicationColor(cStyleGuide.eApplicationColorType.GENERICERROR_BACKGROUND) = My.Settings.ColorErrorBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.COMPUTED_TEXT) = My.Settings.ColorComputedValuesText
            .ApplicationColor(cStyleGuide.eApplicationColorType.REMARKS_BACKGROUND) = My.Settings.ColorRemarksBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.SUM_BACKGROUND) = My.Settings.ColorSumBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.READONLY_BACKGROUND) = My.Settings.ColorReadOnlyBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.CHECKED_BACKGROUND) = My.Settings.ColorCheckedBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND) = My.Settings.ColorMissingParamBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.IMAGE_BACKGROUND) = My.Settings.ColorImageBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND) = My.Settings.ColorPlotsBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.MAP_BACKGROUND) = My.Settings.ColorMapBackground
            .ApplicationColor(cStyleGuide.eApplicationColorType.PREDATOR) = My.Settings.ColorPredator
            .ApplicationColor(cStyleGuide.eApplicationColorType.PREY) = My.Settings.ColorPrey
            .ApplicationColor(cStyleGuide.eApplicationColorType.PEDIGREE) = My.Settings.ColorPedigree

            .ThumbnailSize = My.Settings.ThumbnailSize
            ' Fix: do not allow disabling of legend viz
            If (My.Settings.ShowLegends = TriState.False) Then My.Settings.ShowLegends = TriState.UseDefault
            .ShowLegends = DirectCast(My.Settings.ShowLegends, TriState)
            .ShowPedigree = My.Settings.ShowPedigree
            .UseTransparentBackgrounds = My.Settings.UseTransparentBackgrounds

            .MapReferenceLayerFile = My.Settings.MapLayerRefFile
            .MapReferenceLayerTL = New PointF(My.Settings.MapLayerRefLonMin, My.Settings.MapLayerRefLatMax)
            .MapReferenceLayerBR = New PointF(My.Settings.MapLayerRefLonMax, My.Settings.MapLayerRefLatMin)
            .ShowMapsExcludedCells = My.Settings.MapShowExcludedCells
            .ShowMapsMPAs = My.Settings.MapShowMPAs
            .ShowMapLabels = My.Settings.MapShowLabels
            .ShowMapsDateInLabels = My.Settings.MapShowLabelDate
            .ShowMapsIndexInLabels = My.Settings.MapShowLabelIndex
            .InvertMapLabelColor = My.Settings.MapShowLabelInvertedColor
            .MapLabelPosHorizontal = CType(My.Settings.MapLabelPosHorz, StringAlignment)
            .MapLabelPosVertical = CType(My.Settings.MapLabelPosVert, StringAlignment)
            .UseHabitatAreaCorrection = My.Settings.UseHabitatAreaCorrection

            .PreferredDPI = My.Settings.OutputDPI

            .EcoBaseFields(cStyleGuide.eEcobaseFieldType.CountryName) = My.Settings.CountryNames
            .EcoBaseFields(cStyleGuide.eEcobaseFieldType.EcosystemType) = My.Settings.EcosystemTypes
            .ClearCustomColorRamps()
            For Each ramp As cARGBColorRamp In Me.StringToARGBColorRamps(My.Settings.ColorRampsCustom)
                .AddCustomColorRamp(ramp)
            Next

        End With

        Me.StringToFontSetting(My.Settings.FontTitle, cStyleGuide.eApplicationFontType.Title)
        Me.StringToFontSetting(My.Settings.FontSubtitle, cStyleGuide.eApplicationFontType.SubTitle)
        Me.StringToFontSetting(My.Settings.FontScale, cStyleGuide.eApplicationFontType.Scale)

        Me.StyleGuide.ResumeEvents()

    End Sub

    Public Sub Save()

        With Me.StyleGuide

            My.Settings.ColorDefaultText = .ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
            My.Settings.ColorDefaultBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_BACKGROUND)
            My.Settings.ColorNameText = .ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_TEXT)
            My.Settings.ColorNameBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_BACKGROUND)
            My.Settings.ColorFailedResultText = .ApplicationColor(cStyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT)
            My.Settings.ColorFailedValidationText = .ApplicationColor(cStyleGuide.eApplicationColorType.FAILEDVALIDATION_TEXT)
            My.Settings.ColorErrorBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.GENERICERROR_BACKGROUND)
            My.Settings.ColorComputedValuesText = .ApplicationColor(cStyleGuide.eApplicationColorType.COMPUTED_TEXT)
            My.Settings.ColorRemarksBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.REMARKS_BACKGROUND)
            My.Settings.ColorSumBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.SUM_BACKGROUND)
            My.Settings.ColorReadOnlyBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.READONLY_BACKGROUND)
            My.Settings.ColorCheckedBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.CHECKED_BACKGROUND)
            My.Settings.ColorMissingParamBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND)
            My.Settings.ColorImageBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.IMAGE_BACKGROUND)
            My.Settings.ColorPlotsBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND)
            My.Settings.ColorMapBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.MAP_BACKGROUND)
            My.Settings.ColorPredator = .ApplicationColor(cStyleGuide.eApplicationColorType.PREDATOR)
            My.Settings.ColorPrey = .ApplicationColor(cStyleGuide.eApplicationColorType.PREY)
            My.Settings.ColorPedigree = .ApplicationColor(cStyleGuide.eApplicationColorType.PEDIGREE)

            My.Settings.ThumbnailSize = .ThumbnailSize
            My.Settings.ShowLegends = .ShowLegends
            My.Settings.ShowPedigree = .ShowPedigree
            My.Settings.UseTransparentBackgrounds = .UseTransparentBackgrounds

            My.Settings.MapLayerRefFile = .MapReferenceLayerFile
            My.Settings.MapLayerRefLonMin = .MapReferenceLayerTL.X
            My.Settings.MapLayerRefLonMax = .MapReferenceLayerBR.X
            My.Settings.MapLayerRefLatMin = .MapReferenceLayerBR.Y
            My.Settings.MapLayerRefLatMax = .MapReferenceLayerTL.Y
            My.Settings.MapShowExcludedCells = .ShowMapsExcludedCells
            My.Settings.MapShowMPAs = .ShowMapsMPAs
            My.Settings.MapShowLabels = .ShowMapLabels
            My.Settings.MapShowLabelDate = .ShowMapsDateInLabels
            My.Settings.MapShowLabelIndex = .ShowMapsIndexInLabels
            My.Settings.MapLabelPosHorz = .MapLabelPosHorizontal
            My.Settings.MapLabelPosVert = .MapLabelPosVertical
            My.Settings.MapShowLabelInvertedColor = .InvertMapLabelColor

            My.Settings.UseHabitatAreaCorrection = .UseHabitatAreaCorrection

            My.Settings.CountryNames = .EcoBaseFields(cStyleGuide.eEcobaseFieldType.CountryName)
            My.Settings.EcosystemTypes = .EcoBaseFields(cStyleGuide.eEcobaseFieldType.EcosystemType)

            My.Settings.OutputDPI = .PreferredDPI

            My.Settings.ColorRampsCustom = Me.ARGBColorRampsToString(.CustomARGBColorRamps)

        End With

        My.Settings.FontTitle = Me.FontSettingToString(cStyleGuide.eApplicationFontType.Title)
        My.Settings.FontSubtitle = Me.FontSettingToString(cStyleGuide.eApplicationFontType.SubTitle)
        My.Settings.FontScale = Me.FontSettingToString(cStyleGuide.eApplicationFontType.Scale)

    End Sub

    Private Sub StringToFontSetting(strSetting As String, ft As cStyleGuide.eApplicationFontType)

        Dim astrBits As String() = strSetting.Split(","c)
        If astrBits.Length >= 1 Then
            Try
                Me.StyleGuide.FontFamilyName(ft) = astrBits(0)
            Catch ex As Exception
                Me.StyleGuide.FontFamilyName(ft) = ""
            End Try
        End If
        If astrBits.Length >= 2 Then
            Try
                Me.StyleGuide.FontStyle(ft) = DirectCast(CInt(astrBits(1)), FontStyle)
            Catch ex As Exception
                Me.StyleGuide.FontStyle(ft) = FontStyle.Regular
            End Try
        End If
        If astrBits.Length >= 3 Then
            Try
                Me.StyleGuide.FontSize(ft) = cStringUtils.ConvertToSingle(astrBits(2))
            Catch ex As Exception
                Me.StyleGuide.FontSize(ft) = 0.0!
            End Try
        End If
    End Sub

    Private Function FontSettingToString(ft As cStyleGuide.eApplicationFontType) As String

        Dim sb As New StringBuilder()
        sb.Append(Me.StyleGuide.FontFamilyName(ft))
        sb.Append(",")
        sb.Append(CInt(Me.StyleGuide.FontStyle(ft)))
        sb.Append(",")
        sb.Append(cStringUtils.FormatSingle(Me.StyleGuide.FontSize(ft)))
        Return sb.ToString()

    End Function

    Private Function StringToARGBColorRamps(strSetting As String) As cARGBColorRamp()

        Dim ramps As New List(Of cARGBColorRamp)

        If Not String.IsNullOrWhiteSpace(strSetting) Then
            Dim items() As String = strSetting.Split(New String() {";"c}, StringSplitOptions.RemoveEmptyEntries)
            For i As Integer = 0 To items.Count - 1
                Dim item As cARGBColorRamp = Me.StringToARGBColorRamp(items(i))
                If (item IsNot Nothing) Then ramps.Add(item)
            Next
        End If
        Return ramps.ToArray()

    End Function

    Private Function StringToARGBColorRamp(item As String) As cARGBColorRamp
        Try
            Dim bits As String() = cStringUtils.SplitQualified(item, ","c)
            Dim name As String = bits(0)
            Dim colors As New List(Of Color)
            Dim breaks As New List(Of Double)
            For j As Integer = 1 To bits.Count - 1 Step 2
                colors.Add(Color.FromArgb(Convert.ToInt32(bits(j), 16)))
                breaks.Add(cStringUtils.ConvertToDouble(bits(j + 1)))
            Next
            Return New cARGBColorRamp(name, colors.ToArray(), breaks.ToArray())
        Catch ex As Exception
            ' Ok, that didn't work - plow on
        End Try
        Return Nothing

    End Function

    Private Function ARGBColorRampsToString(ramps As cARGBColorRamp()) As String

        Dim sb As New StringBuilder()
        If (ramps IsNot Nothing) Then
            For i As Integer = 0 To ramps.Count - 1
                If (i > 0) Then sb.Append(";")
                sb.Append(ARGBColorRampToString(ramps(i)))
            Next
        End If
        Return sb.ToString()

    End Function

    Private Function ARGBColorRampToString(ramp As cARGBColorRamp) As String
        Dim sb As New StringBuilder()
        sb.Append("""" & ramp.Name.Replace("""", "") & """")
        For j As Integer = 0 To ramp.GradientBreaks.Count - 1
            Dim clr As Color = ramp.GradientColors(j)
            sb.Append("," & cStringUtils.ToHexString(New Byte() {clr.A, clr.R, clr.G, clr.B}))
            sb.Append("," & cStringUtils.FormatNumber(ramp.GradientBreaks(j)))
        Next
        Return sb.ToString()
    End Function

#End Region

End Class

