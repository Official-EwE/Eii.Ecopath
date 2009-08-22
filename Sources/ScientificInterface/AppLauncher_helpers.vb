#Region " Imports "

Imports EwECore
Imports WeifenLuo.WinFormsUI.Docking
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports EwEPlugin
Imports System.Text

#End Region ' Imports

Partial Public Class AppLauncher

#Region " FormStateHelper "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class; maintains form enabled / availability states in the AppLauncher.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cEwEFormStateHelper

        Private WithEvents m_csm As cCoreStateMonitor
        Private m_dp As DockPanel

        Public Sub New(ByVal csm As cCoreStateMonitor, ByVal dp As DockPanel)
            Me.m_dp = dp
            Me.m_csm = csm
        End Sub

        Private Sub m_csm_CoreExecutionStateEvent(ByVal csm As cCoreStateMonitor) _
            Handles m_csm.CoreExecutionStateEvent
            Me.UpdateFormStates()
        End Sub

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

        Private Sub UpdateFormStates()

            Dim stateForm As eCoreExecutionState = eCoreExecutionState.Idle
            Dim bMustCloseForm As Boolean = False

            For Each f As frmEwE In Me.GetEwEForms()
                ' Think positive
                bMustCloseForm = False

                ' Get form state
                stateForm = f.CoreExecutionState

                ' Check if form should be disabled
                bMustCloseForm = ((Not Me.m_csm.IsExecutionStateSuperceded(stateForm)) And frmEwE.IsOutputForm(stateForm))

                If bMustCloseForm Then
                    ' #Yes: Close the form
                    f.Close()
                End If
            Next
        End Sub

    End Class

#End Region ' FormStateHelper

#Region " StyleGuideUpdater "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' On-board helper class that actively updates model-derived settings in the style guide.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class StyleGuideUpdater

        Private m_core As cCore = Nothing
        Private m_sg As cStyleGuide = Nothing
        Private m_bIsEcopathLoaded As Boolean = False

        Private m_sm As cCoreStateMonitor = Nothing
        Private m_propNumDigits As cProperty = Nothing
        Private m_propUnitTime As cIntegerProperty = Nothing
        Private m_propUnitTimeText As cStringProperty = Nothing
        Private m_propUnitCurrency As cIntegerProperty = Nothing
        Private m_propUnitCurrencyText As cStringProperty = Nothing
        Private m_propUnitMonetary As cIntegerProperty = Nothing
        Private m_propUnitMonetaryText As cStringProperty = Nothing

        Public Sub New(ByVal core As cCore, ByVal sg As cStyleGuide)

            Me.m_core = core
            Me.m_sm = core.StateMonitor
            Me.m_sg = sg

            AddHandler m_sm.CoreExecutionStateEvent, AddressOf OnCoreStateEvent

        End Sub

        Private Sub OnCoreStateEvent(ByVal csm As cCoreStateMonitor)
            If Me.m_bIsEcopathLoaded <> csm.HasEcopathLoaded Then
                Me.m_bIsEcopathLoaded = csm.HasEcopathLoaded
                Me.Update()
            End If
        End Sub

        Private Sub Update()

            Dim pm As cPropertyManager = cPropertyManager.GetInstance()

            Me.m_sg.SuspendEvents()

            If Me.m_bIsEcopathLoaded Then

                Me.m_propNumDigits = pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.NumDigits)
                AddHandler Me.m_propNumDigits.PropertyChanged, AddressOf OnNumDigitsChanged

                Me.m_propUnitCurrency = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitCurrency), cIntegerProperty)
                Me.m_propUnitCurrencyText = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitCurrencyCustomText), cStringProperty)
                AddHandler Me.m_propUnitCurrency.PropertyChanged, AddressOf OnCurrencyUnitChanged
                AddHandler Me.m_propUnitCurrencyText.PropertyChanged, AddressOf OnCurrencyUnitChanged

                Me.m_propUnitTime = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitTime), cIntegerProperty)
                Me.m_propUnitTimeText = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitTimeCustomText), cStringProperty)
                AddHandler Me.m_propUnitTime.PropertyChanged, AddressOf OnTimeUnitChanged
                AddHandler Me.m_propUnitTimeText.PropertyChanged, AddressOf OnTimeUnitChanged

                Me.m_propUnitMonetary = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitMonetary), cIntegerProperty)
                Me.m_propUnitMonetaryText = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitMonetaryCustomText), cStringProperty)
                AddHandler Me.m_propUnitMonetary.PropertyChanged, AddressOf OnMonetaryUnitChanged
                AddHandler Me.m_propUnitMonetaryText.PropertyChanged, AddressOf OnMonetaryUnitChanged

                Me.OnCurrencyUnitChanged(m_propUnitCurrency, cProperty.eChangeFlags.All)
                Me.OnTimeUnitChanged(m_propUnitTime, cProperty.eChangeFlags.All)
                Me.OnMonetaryUnitChanged(m_propUnitMonetary, cProperty.eChangeFlags.All)
                Me.OnNumDigitsChanged(m_propNumDigits, cProperty.eChangeFlags.All)
            Else
                RemoveHandler Me.m_propNumDigits.PropertyChanged, AddressOf OnNumDigitsChanged
                Me.m_propNumDigits = Nothing

                RemoveHandler Me.m_propUnitCurrency.PropertyChanged, AddressOf OnCurrencyUnitChanged
                RemoveHandler Me.m_propUnitCurrencyText.PropertyChanged, AddressOf OnCurrencyUnitChanged
                Me.m_propUnitCurrency = Nothing
                Me.m_propUnitCurrencyText = Nothing

                RemoveHandler Me.m_propUnitTime.PropertyChanged, AddressOf OnTimeUnitChanged
                RemoveHandler Me.m_propUnitTimeText.PropertyChanged, AddressOf OnTimeUnitChanged
                Me.m_propUnitTime = Nothing
                Me.m_propUnitTimeText = Nothing

                RemoveHandler Me.m_propUnitMonetary.PropertyChanged, AddressOf OnMonetaryUnitChanged
                RemoveHandler Me.m_propUnitMonetaryText.PropertyChanged, AddressOf OnMonetaryUnitChanged
                Me.m_propUnitMonetary = Nothing
                Me.m_propUnitMonetaryText = Nothing
            End If

            Me.m_sg.ResetVisibleFlags(False)
            Me.m_sg.ResumeEvents()

        End Sub

        Private Sub OnCurrencyUnitChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            Me.m_sg.SuspendEvents()
            Me.m_sg.CurrencyUnit = DirectCast(Me.m_propUnitCurrency.GetValue(), eUnitCurrencyType)
            Me.m_sg.CustomCurrencyUnitText = CStr(Me.m_propUnitCurrencyText.GetValue())
            Me.m_sg.ResumeEvents()
        End Sub

        Private Sub OnTimeUnitChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            Me.m_sg.SuspendEvents()
            Me.m_sg.TimeUnit = DirectCast(Me.m_propUnitTime.GetValue(), eUnitTimeType)
            Me.m_sg.CustomTimeUnitText = CStr(Me.m_propUnitTimeText.GetValue())
            Me.m_sg.ResumeEvents()
        End Sub

        Private Sub OnMonetaryUnitChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            Me.m_sg.SuspendEvents()
            Me.m_sg.MonetaryUnit = DirectCast(Me.m_propUnitMonetary.GetValue(), eUnitMonetaryType)
            Me.m_sg.CustomMonetaryUnitText = CStr(Me.m_propUnitMonetaryText.GetValue())
            Me.m_sg.ResumeEvents()
        End Sub

        Private Sub OnNumDigitsChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            Me.m_sg.SuspendEvents()
            Me.m_sg.NumDigits = CInt(Me.m_propNumDigits.GetValue())
            Me.m_sg.ResumeEvents()
        End Sub

        Public Sub Load()

            Me.m_sg.SuspendEvents()

            Me.m_sg.LoadDefaultApplicationColors()

            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT) = My.Settings.ColorDefaultText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_BACKGROUND) = My.Settings.ColorDefaultBackground
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_TEXT) = My.Settings.ColorNameText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_BACKGROUND) = My.Settings.ColorNameBackground
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT) = My.Settings.ColorFailedResultText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.FAILEDVALIDATION_TEXT) = My.Settings.ColorFailedValidationText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.GENERICERROR_TEXT) = My.Settings.ColorErrorText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.COMPUTED_TEXT) = My.Settings.ColorComputedValuesText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.FISHINGPRESSURE_TEXT) = My.Settings.ColorESPressureText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.PROFIT_TEXT) = My.Settings.ColorESProfitsText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.TOTALCATCH_TEXT) = My.Settings.ColorESTotalCatchText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.TROPHICLINK_TEXT) = My.Settings.ColorTrophicLinkText
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.REMARKS_BACKGROUND) = My.Settings.ColorRemarksBackground
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.SUM_BACKGROUND) = My.Settings.ColorSumBackground
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.READONLY_BACKGROUND) = My.Settings.ColorReadOnlyBackground
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.CHECKED_BACKGROUND) = My.Settings.ColorCheckedBackground
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND) = My.Settings.ColorMissingParamBackground
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.IMAGE_BACKGROUND) = My.Settings.ColorImageBackground
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND) = My.Settings.ColorPlotsBackground
            Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.MAP_BACKGROUND) = My.Settings.ColorMapBackground

            Me.StringToFontSetting(My.Settings.FontTitle, cStyleGuide.eApplicationFontType.Title)
            Me.StringToFontSetting(My.Settings.FontSubtitle, cStyleGuide.eApplicationFontType.SubTitle)
            Me.StringToFontSetting(My.Settings.FontLegend, cStyleGuide.eApplicationFontType.Legend)
            Me.StringToFontSetting(My.Settings.FontScale, cStyleGuide.eApplicationFontType.Scale)
            Me.StringToFontSetting(My.Settings.FontValue, cStyleGuide.eApplicationFontType.Value)

            Me.m_sg.ResumeEvents()

        End Sub

        Public Sub Save()

            My.Settings.ColorDefaultText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
            My.Settings.ColorDefaultBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_BACKGROUND)
            My.Settings.ColorNameText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_TEXT)
            My.Settings.ColorNameBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_BACKGROUND)
            My.Settings.ColorFailedResultText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT)
            My.Settings.ColorFailedValidationText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.FAILEDVALIDATION_TEXT)
            My.Settings.ColorErrorText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.GENERICERROR_TEXT)
            My.Settings.ColorComputedValuesText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.COMPUTED_TEXT)
            My.Settings.ColorESPressureText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.FISHINGPRESSURE_TEXT)
            My.Settings.ColorESProfitsText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.PROFIT_TEXT)
            My.Settings.ColorESTotalCatchText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.TOTALCATCH_TEXT)
            My.Settings.ColorTrophicLinkText = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.TROPHICLINK_TEXT)
            My.Settings.ColorRemarksBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.REMARKS_BACKGROUND)
            My.Settings.ColorSumBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.SUM_BACKGROUND)
            My.Settings.ColorReadOnlyBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.READONLY_BACKGROUND)
            My.Settings.ColorCheckedBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.CHECKED_BACKGROUND)
            My.Settings.ColorMissingParamBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND)
            My.Settings.ColorImageBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.IMAGE_BACKGROUND)
            My.Settings.ColorPlotsBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND)
            My.Settings.ColorMapBackground = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.MAP_BACKGROUND)

            My.Settings.FontTitle = Me.FontSettingToString(cStyleGuide.eApplicationFontType.Title)
            My.Settings.FontSubtitle = Me.FontSettingToString(cStyleGuide.eApplicationFontType.SubTitle)
            My.Settings.FontLegend = Me.FontSettingToString(cStyleGuide.eApplicationFontType.Legend)
            My.Settings.FontScale = Me.FontSettingToString(cStyleGuide.eApplicationFontType.Scale)
            My.Settings.FontValue = Me.FontSettingToString(cStyleGuide.eApplicationFontType.Value)

            My.Settings.Save()
        End Sub

        Private Sub StringToFontSetting(ByVal strSetting As String, ByVal ft As cStyleGuide.eApplicationFontType)

            Dim astrBits As String() = strSetting.Split(","c)
            If astrBits.Length >= 1 Then
                Try
                    Me.m_sg.FontFamilyName(ft) = astrBits(0)
                Catch ex As Exception
                    Me.m_sg.FontFamilyName(ft) = ""
                End Try
            End If
            If astrBits.Length >= 2 Then
                Try
                    Me.m_sg.FontStyle(ft) = DirectCast(CInt(astrBits(1)), FontStyle)
                Catch ex As Exception
                    Me.m_sg.FontStyle(ft) = FontStyle.Regular
                End Try
            End If
            If astrBits.Length >= 3 Then
                Try
                    Me.m_sg.FontSize(ft) = Single.Parse(astrBits(2))
                Catch ex As Exception
                    Me.m_sg.FontSize(ft) = 0.0!
                End Try
            End If
        End Sub

        Private Function FontSettingToString(ByVal ft As cStyleGuide.eApplicationFontType) As String

            Dim sb As New StringBuilder()
            sb.Append(Me.m_sg.FontFamilyName(ft))
            sb.Append(",")
            sb.Append(CInt(Me.m_sg.FontStyle(ft)))
            sb.Append(",")
            sb.Append(Me.m_sg.FontSize(ft))
            Return sb.ToString()

        End Function

    End Class

#End Region ' StyleGuideUpdater

#Region " MRUHelper "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, generates and analyses MRU strings
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class MRUHelper

        Public Enum eModuleType
            Ecosim
            Ecospace
            Ecotracer
            Dataset
        End Enum

        Private Shared Function ModuleKey(ByVal moduleType As eModuleType) As String
            Select Case moduleType
                Case eModuleType.Ecosim : Return ",Ecosim_scenario:"
                Case eModuleType.Ecospace : Return ",Ecospace_scenario:"
                Case eModuleType.Ecotracer : Return ",Ecotracer_scenario:"
                Case eModuleType.Dataset : Return ",Ecosim_dataset:"
            End Select
            Return ""
        End Function

        Public Shared Function GetMRUString(ByVal alstrMRU As ArrayList, ByVal strModelName As String, ByVal moduleType As eModuleType) As String

            Dim strModuleKey As String = MRUHelper.ModuleKey(moduleType)
            Dim strMRU As String = ""
            Dim iKeyPos As Integer = -1
            Dim iNextTerminatorPos As Integer = -1
            Dim iNameStartPos As Integer = -1

            ' For almost each MRU entry (..wtf..)
            For i As Integer = 0 To alstrMRU.Count - 2
                strMRU = CStr(alstrMRU.Item(i))
                If strMRU.StartsWith(strModelName) Then

                    ' Search scenario key
                    iKeyPos = strMRU.IndexOf(strModuleKey)
                    ' Found it?
                    If iKeyPos <> -1 Then
                        ' #Yes: try to extract scenario name
                        ' Find first pos of scenario name
                        iNameStartPos = iKeyPos + strModuleKey.Length
                        ' Find next terminator, if any
                        iNextTerminatorPos = strMRU.IndexOf(CChar(","), iKeyPos + 1)
                        ' Terminator not found?
                        If iNextTerminatorPos = -1 Then
                            ' #No terminator: name must be the rest of the string
                            Return strMRU.Substring(iNameStartPos)
                        Else
                            ' #Terminator: name must be all chars up to terminator
                            Return strMRU.Substring(iNameStartPos, iNextTerminatorPos - iNameStartPos)
                        End If
                    End If
                    ' No scenario name for this MRU entry
                    Return ""

                End If
            Next
            Return ""

        End Function

        Public Shared Sub UpdateMRUString(ByVal alstrMRU As ArrayList, ByVal strValue As String, ByVal mt As eModuleType)

            ' Item does not exist, abort!
            If (alstrMRU.Count = 1) Then Return

            Dim strMRU As String = CStr(alstrMRU.Item(0))
            Dim strModuleKey As String = MRUHelper.ModuleKey(mt)
            Dim iKeyPos As Integer = strMRU.IndexOf(strModuleKey)
            Dim iTerminatorPos As Integer = strMRU.IndexOf(CChar(","), iKeyPos + 1)
            Dim strLeft As String = String.Empty
            Dim strRight As String = String.Empty

            If iKeyPos = -1 Then
                If iTerminatorPos = -1 Then
                    strLeft = strMRU
                Else
                    strLeft = strMRU.Substring(0, iTerminatorPos)
                End If
            Else
                strLeft = strMRU.Substring(0, iKeyPos)
            End If
            If iTerminatorPos <> -1 Then
                strRight = strMRU.Substring(iTerminatorPos)
            End If
            ' Update MRU item
            alstrMRU.Item(0) = strLeft & strModuleKey & strValue & strRight

        End Sub

    End Class

#End Region ' MRUHelper

End Class
