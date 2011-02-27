#Region " Imports "

Imports System.Text
Imports EwECore
Imports ScientificInterfaceShared.Forms
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports EwEUtils.Utilities
Imports WeifenLuo.WinFormsUI.Docking

#End Region ' Imports

Partial Public Class AppLauncher

#Region " cFormStateHelper "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class; maintains form enabled / availability states in the AppLauncher.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cEwEFormStateHelper
        Implements IDisposable

#Region " Privates "

        ''' <summary>Core state monitor that is being observed.</summary>
        Private m_csm As cCoreStateMonitor = Nothing
        ''' <summary>Dock panel containing the forms to maintain.</summary>
        Private m_dp As DockPanel = Nothing
        ''' <summary>Core controller to work with.</summary>
        Private m_cc As cCoreController = Nothing

#End Region ' Privates

#Region " Construction "

        Public Sub New(ByVal csm As cCoreStateMonitor, _
                       ByVal cc As cCoreController, _
                       ByVal dp As DockPanel)
            Me.m_dp = dp
            Me.m_cc = cc
            Me.m_csm = csm

            AddHandler Me.m_csm.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
        End Sub

        Public Sub Dispose() _
            Implements IDisposable.Dispose

            If (Me.m_csm IsNot Nothing) Then
                RemoveHandler Me.m_csm.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
                Me.m_dp = Nothing
                Me.m_cc = Nothing
                Me.m_csm = Nothing
            End If
            GC.SuppressFinalize(Me)

        End Sub

#End Region ' Construction

#Region " Events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; responds to core execution state changes.
        ''' </summary>
        ''' <param name="csm">Core state monitor that threw the event.</param>
        ''' -------------------------------------------------------------------
        Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)
            Me.UpdateFormStates()
        End Sub

#End Region ' Events

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a list of all currently opened 
        ''' <see cref="frmEwE">frmEwE-derived forms</see>.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function GetEwEForms() As List(Of frmEwE)

            Dim l As New List(Of frmEwE)
            Dim f As frmEwE = Nothing

            ' Assess all docked windows
            If (Me.m_dp IsNot Nothing) Then
                For Each idc As IDockContent In Me.m_dp.Documents
                    If (TypeOf idc Is frmEwE) Then
                        f = DirectCast(idc, frmEwE)
                        l.Add(f)
                    End If
                Next
            End If

            ' Assess all floating windows
            For Each fw As FloatWindow In Me.m_dp.FloatWindows
                For iPane As Integer = 0 To fw.VisibleNestedPanes.Count - 1
                    For Each idc As IDockContent In fw.VisibleNestedPanes(iPane).Contents
                        If (TypeOf idc Is frmEwE) Then
                            f = DirectCast(idc, frmEwE)
                            If (l.IndexOf(f) = -1) Then
                                l.Add(f)
                            End If
                        End If
                    Next
                Next
            Next

            Return l

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Manage form states in response to the core execution state.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateFormStates()

            Dim stateForm As eCoreExecutionState = eCoreExecutionState.Idle
            Dim bMustCloseForm As Boolean = False
            Dim bMustDisableForm As Boolean = False

            ' JS 09Jan11: The application should NOT attempt to update the core state
            '             if the application is amidst responding to a core state change.
            '             Thus, while executing the following loop, the core controller
            '             should be disabled.
            Me.m_cc.Enabled = False

            Try

                For Each f As frmEwE In Me.GetEwEForms()

                    ' Get form state
                    stateForm = f.CoreExecutionState

                    ' Check if form should be closed
                    ' A form should be closed if its outputs are invalidated or then the data 
                    ' used to populate the form are no longer available.
                    If frmEwE.IsOutputForm(stateForm) Then
                        Select Case stateForm
                            Case eCoreExecutionState.EcopathCompleted
                                bMustCloseForm = Not Me.m_csm.HasEcopathRan
                            Case eCoreExecutionState.EcosimCompleted
                                bMustCloseForm = Not Me.m_csm.HasEcosimRan
                            Case eCoreExecutionState.EcospaceCompleted
                                bMustCloseForm = Not Me.m_csm.HasEcospaceRan
                        End Select
                    Else
                        bMustCloseForm = Me.m_csm.IsExecutionStateSuperceded(stateForm) = False

                        ' Check if form should be disabled
                        ' A form should be disabled if it is an input form; path, sim or space are running,
                        ' and the form is not used to start the runs.
                        bMustDisableForm = (Me.m_csm.IsEcopathRunning Or _
                                            Me.m_csm.IsEcosimRunning Or _
                                            Me.m_csm.IsEcospaceRunning) And _
                                           (Not f.IsRunForm())
                    End If

                    If bMustCloseForm Then
                        ' #Yes: Close the form
                        f.Close()
                    Else
                        ' #No: update enabled state
                        f.Enabled = (bMustDisableForm = False)
                    End If

                Next f

            Catch ex As Exception
                ' Whoah!
                cLog.Write("cEwEFormStateHelper: " & ex.Message)
            End Try

            Me.m_cc.Enabled = True

        End Sub

#End Region ' Internals

    End Class

#End Region ' cFormStateHelper

#Region " cStyleGuideUpdater "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' On-board helper class that actively updates model-derived settings in the style guide.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class StyleGuideUpdater

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

        Public Sub New(ByVal uic As cUIContext)

            ' Sanity check
            Debug.Assert(uic IsNot Nothing)

            Me.m_uic = uic
            Me.m_sm = Me.m_uic.Core.StateMonitor

            AddHandler m_sm.CoreExecutionStateEvent, AddressOf OnCoreStateEvent

        End Sub

        Private Sub OnCoreStateEvent(ByVal csm As cCoreStateMonitor)
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

            Me.StyleGuide.ResetVisibleFlags(False)
            Me.StyleGuide.ResumeEvents()

        End Sub

        Private Sub OnCurrencyUnitChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            With Me.StyleGuide
                .SuspendEvents()
                .CurrencyUnit = DirectCast(Me.m_propUnitCurrency.GetValue(), eUnitCurrencyType)
                .CustomCurrencyUnitText = CStr(Me.m_propUnitCurrencyText.GetValue())
                .ResumeEvents()
            End With
        End Sub

        Private Sub OnTimeUnitChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            With Me.StyleGuide
                .SuspendEvents()
                .TimeUnit = DirectCast(Me.m_propUnitTime.GetValue(), eUnitTimeType)
                .CustomTimeUnitText = CStr(Me.m_propUnitTimeText.GetValue())
                .ResumeEvents()
            End With
        End Sub

        Private Sub OnMonetaryUnitChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            With Me.StyleGuide
                .SuspendEvents()
                .MonetaryUnit = DirectCast(Me.m_propUnitMonetary.GetValue(), String)
                .ResumeEvents()
            End With
        End Sub

        Private Sub OnNumberFormatChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            With Me.StyleGuide
                .SuspendEvents()
                .NumDigits = CInt(Me.m_propNumDigits.GetValue())
                .GroupDigits = CBool(Me.m_propGroupDigits.GetValue())
                .ResumeEvents()
            End With
        End Sub

        Public Sub Load()

            With Me.StyleGuide

                .SuspendEvents()

                .ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT) = My.Settings.ColorDefaultText
                .ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_BACKGROUND) = My.Settings.ColorDefaultBackground
                .ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_TEXT) = My.Settings.ColorNameText
                .ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_BACKGROUND) = My.Settings.ColorNameBackground
                .ApplicationColor(cStyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT) = My.Settings.ColorFailedResultText
                .ApplicationColor(cStyleGuide.eApplicationColorType.FAILEDVALIDATION_TEXT) = My.Settings.ColorFailedValidationText
                .ApplicationColor(cStyleGuide.eApplicationColorType.GENERICERROR_TEXT) = My.Settings.ColorErrorText
                .ApplicationColor(cStyleGuide.eApplicationColorType.COMPUTED_TEXT) = My.Settings.ColorComputedValuesText
                .ApplicationColor(cStyleGuide.eApplicationColorType.FISHINGPRESSURE_TEXT) = My.Settings.ColorESPressureText
                .ApplicationColor(cStyleGuide.eApplicationColorType.PROFIT_TEXT) = My.Settings.ColorESProfitsText
                .ApplicationColor(cStyleGuide.eApplicationColorType.TOTALCATCH_TEXT) = My.Settings.ColorESTotalCatchText
                .ApplicationColor(cStyleGuide.eApplicationColorType.TROPHICLINK_TEXT) = My.Settings.ColorTrophicLinkText
                .ApplicationColor(cStyleGuide.eApplicationColorType.REMARKS_BACKGROUND) = My.Settings.ColorRemarksBackground
                .ApplicationColor(cStyleGuide.eApplicationColorType.SUM_BACKGROUND) = My.Settings.ColorSumBackground
                .ApplicationColor(cStyleGuide.eApplicationColorType.READONLY_BACKGROUND) = My.Settings.ColorReadOnlyBackground
                .ApplicationColor(cStyleGuide.eApplicationColorType.CHECKED_BACKGROUND) = My.Settings.ColorCheckedBackground
                .ApplicationColor(cStyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND) = My.Settings.ColorMissingParamBackground
                .ApplicationColor(cStyleGuide.eApplicationColorType.IMAGE_BACKGROUND) = My.Settings.ColorImageBackground
                .ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND) = My.Settings.ColorPlotsBackground
                .ApplicationColor(cStyleGuide.eApplicationColorType.MAP_BACKGROUND) = My.Settings.ColorMapBackground

                .ThumbnailSize = My.Settings.ThumbnailSize
                ' Fix: do not allow disabling of legend viz
                If (My.Settings.ShowLegends = TriState.False) Then My.Settings.ShowLegends = TriState.UseDefault
                .ShowLegends = My.Settings.ShowLegends
                .UseTransparentBackgrounds = My.Settings.UseTransparentBackgrounds

            End With

            Me.StringToFontSetting(My.Settings.FontTitle, cStyleGuide.eApplicationFontType.Title)
            Me.StringToFontSetting(My.Settings.FontSubtitle, cStyleGuide.eApplicationFontType.SubTitle)
            Me.StringToFontSetting(My.Settings.FontLegend, cStyleGuide.eApplicationFontType.Legend)

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
                My.Settings.ColorErrorText = .ApplicationColor(cStyleGuide.eApplicationColorType.GENERICERROR_TEXT)
                My.Settings.ColorComputedValuesText = .ApplicationColor(cStyleGuide.eApplicationColorType.COMPUTED_TEXT)
                My.Settings.ColorESPressureText = .ApplicationColor(cStyleGuide.eApplicationColorType.FISHINGPRESSURE_TEXT)
                My.Settings.ColorESProfitsText = .ApplicationColor(cStyleGuide.eApplicationColorType.PROFIT_TEXT)
                My.Settings.ColorESTotalCatchText = .ApplicationColor(cStyleGuide.eApplicationColorType.TOTALCATCH_TEXT)
                My.Settings.ColorTrophicLinkText = .ApplicationColor(cStyleGuide.eApplicationColorType.TROPHICLINK_TEXT)
                My.Settings.ColorRemarksBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.REMARKS_BACKGROUND)
                My.Settings.ColorSumBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.SUM_BACKGROUND)
                My.Settings.ColorReadOnlyBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.READONLY_BACKGROUND)
                My.Settings.ColorCheckedBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.CHECKED_BACKGROUND)
                My.Settings.ColorMissingParamBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND)
                My.Settings.ColorImageBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.IMAGE_BACKGROUND)
                My.Settings.ColorPlotsBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND)
                My.Settings.ColorMapBackground = .ApplicationColor(cStyleGuide.eApplicationColorType.MAP_BACKGROUND)

                My.Settings.ThumbnailSize = .ThumbnailSize
                My.Settings.ShowLegends = .ShowLegends
                My.Settings.UseTransparentBackgrounds = .UseTransparentBackgrounds

            End With

            My.Settings.FontTitle = Me.FontSettingToString(cStyleGuide.eApplicationFontType.Title)
            My.Settings.FontSubtitle = Me.FontSettingToString(cStyleGuide.eApplicationFontType.SubTitle)
            My.Settings.FontLegend = Me.FontSettingToString(cStyleGuide.eApplicationFontType.Legend)
            My.Settings.FontScale = Me.FontSettingToString(cStyleGuide.eApplicationFontType.Scale)

        End Sub

        Private Sub StringToFontSetting(ByVal strSetting As String, ByVal ft As cStyleGuide.eApplicationFontType)

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

        Private Function FontSettingToString(ByVal ft As cStyleGuide.eApplicationFontType) As String

            Dim sb As New StringBuilder()
            sb.Append(Me.StyleGuide.FontFamilyName(ft))
            sb.Append(",")
            sb.Append(CInt(Me.StyleGuide.FontStyle(ft)))
            sb.Append(",")
            sb.Append(cStringUtils.FormatSingle(Me.StyleGuide.FontSize(ft)))
            Return sb.ToString()

        End Function

    End Class

#End Region ' cStyleGuideUpdater

End Class
