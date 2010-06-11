Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SAUPUtil.Misc.GeoCode

Public Class frmModelDescription

    Private m_fpName As cEwEFormatProvider = Nothing
    Private m_fpDescription As cEwEFormatProvider = Nothing
    Private m_fpAuthor As cEwEFormatProvider = Nothing
    Private m_fpContact As cEwEFormatProvider = Nothing
    Private m_fpArea As cEwEFormatProvider = Nothing
    Private m_fpNumDigits As cEwEFormatProvider = Nothing
    Private m_fpGroupDigits As cEwEFormatProvider = Nothing
    Private m_fpPSD As cEwEFormatProvider = Nothing
    Private m_fpFirstYear As cEwEFormatProvider = Nothing
    Private m_fpNumYears As cEwEFormatProvider = Nothing
    Private m_fpLatMin As cEwEFormatProvider = Nothing
    Private m_fpLatMax As cEwEFormatProvider = Nothing
    Private m_fpLonMin As cEwEFormatProvider = Nothing
    Private m_fpLonMax As cEwEFormatProvider = Nothing

    ' Unit properties
    Private m_propUnitCurrency As cIntegerProperty = Nothing
    Private m_propUnitCurrencyText As cStringProperty = Nothing
    Private m_propUnitTime As cIntegerProperty = Nothing
    Private m_propUnitTimeText As cStringProperty = Nothing
    Private m_propUnitMonetary As cIntegerProperty = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        MyBase.OnLoad(e)

        If Me.UIContext Is Nothing Then Return

        Dim eweModel As cEwEModel = Me.UIContext.Core.EwEModel()
        Dim psdParms As cPSDParameters = Me.UIContext.Core.ParticleSizeDistributionParameters()
        Dim pm As cPropertyManager = Me.UIContext.PropertyManager
        Dim appl As AppLauncher = AppLauncher.GetInstance()

        Me.m_fpName = New cPropertyFormatProvider(Me.UIContext, Me.m_tbName, eweModel, eVarNameFlags.Name)
        Me.m_fpDescription = New cPropertyFormatProvider(Me.UIContext, Me.m_tbDescription, eweModel, eVarNameFlags.Description)
        Me.m_fpAuthor = New cPropertyFormatProvider(Me.UIContext, Me.m_tbAuthor, eweModel, eVarNameFlags.Author)
        Me.m_fpContact = New cPropertyFormatProvider(Me.UIContext, Me.m_tbContact, eweModel, eVarNameFlags.Contact)
        Me.m_fpArea = New cPropertyFormatProvider(Me.UIContext, Me.m_tbArea, eweModel, eVarNameFlags.Area)
        Me.m_fpNumDigits = New cPropertyFormatProvider(Me.UIContext, Me.m_udNumDigits, eweModel, eVarNameFlags.NumDigits)
        Me.m_fpGroupDigits = New cPropertyFormatProvider(Me.UIContext, Me.m_cbGroupDigits, eweModel, eVarNameFlags.GroupDigits)
        Me.m_fpFirstYear = New cPropertyFormatProvider(Me.UIContext, Me.m_nudFirstYear, eweModel, eVarNameFlags.EcopathFirstYear)
        Me.m_fpNumYears = New cPropertyFormatProvider(Me.UIContext, Me.m_nudNumYears, eweModel, eVarNameFlags.EcopathNumYears)
        Me.m_fpLatMax = New cPropertyFormatProvider(Me.UIContext, Me.m_nudNorth, eweModel, eVarNameFlags.LatMax)
        Me.m_fpLatMin = New cPropertyFormatProvider(Me.UIContext, Me.m_nudSouth, eweModel, eVarNameFlags.LatMin)
        Me.m_fpLonMin = New cPropertyFormatProvider(Me.UIContext, Me.m_nudWest, eweModel, eVarNameFlags.LonMin)
        Me.m_fpLonMax = New cPropertyFormatProvider(Me.UIContext, Me.m_nudEast, eweModel, eVarNameFlags.LonMax)

        Me.m_fpPSD = New cPropertyFormatProvider(Me.UIContext, Me.m_chkPSD, psdParms, eVarNameFlags.PSDEnabled)

        Me.m_propUnitCurrency = DirectCast(pm.GetProperty(Me.UIContext.Core.EwEModel, eVarNameFlags.UnitCurrency), cIntegerProperty)
        AddHandler Me.m_propUnitCurrency.PropertyChanged, AddressOf OnUnitCurrencyChanged

        Me.m_propUnitCurrencyText = DirectCast(pm.GetProperty(Me.UIContext.Core.EwEModel, eVarNameFlags.UnitCurrencyCustomText), cStringProperty)
        AddHandler Me.m_propUnitCurrencyText.PropertyChanged, AddressOf OnUnitCurrencyTextChanged

        Me.m_propUnitTime = DirectCast(pm.GetProperty(Me.UIContext.Core.EwEModel, eVarNameFlags.UnitTime), cIntegerProperty)
        AddHandler Me.m_propUnitTime.PropertyChanged, AddressOf OnUnitTimeChanged

        Me.m_propUnitTimeText = DirectCast(pm.GetProperty(Me.UIContext.Core.EwEModel, eVarNameFlags.UnitTimeCustomText), cStringProperty)
        AddHandler Me.m_propUnitTimeText.PropertyChanged, AddressOf OnUnitTimeTextChanged

        Me.m_propUnitMonetary = DirectCast(pm.GetProperty(Me.UIContext.Core.EwEModel, eVarNameFlags.UnitMonetary), cIntegerProperty)
        AddHandler Me.m_propUnitMonetary.PropertyChanged, AddressOf OnUnitMonetaryChanged

        Me.m_cmbMonetaryUnit.UIContext = Me.UIContext

        ' Listen to shapes data added or removed messages
        Me.CoreComponents = Nothing

        Me.PatchCurrencyUnitRadioButtonText(Me.rbWetWeight, eUnitCurrencyType.WetWeight)
        Me.PatchCurrencyUnitRadioButtonText(Me.rbCalorie, eUnitCurrencyType.Calorie)
        Me.PatchCurrencyUnitRadioButtonText(Me.rbCarbon, eUnitCurrencyType.Carbon)
        Me.PatchCurrencyUnitRadioButtonText(Me.rbJoules, eUnitCurrencyType.Joules)
        Me.PatchCurrencyUnitRadioButtonText(Me.rbDryWeight, eUnitCurrencyType.DryWeight)
        Me.PatchCurrencyUnitRadioButtonText(Me.rbNitrogen, eUnitCurrencyType.Nitrogen)
        Me.PatchCurrencyUnitRadioButtonText(Me.rbPhosporus, eUnitCurrencyType.Phosporous)

        ' Kick!
        Me.OnUnitCurrencyChanged(Me.m_propUnitCurrency, cProperty.eChangeFlags.All)
        Me.OnUnitCurrencyTextChanged(Me.m_propUnitCurrencyText, cProperty.eChangeFlags.All)
        Me.OnUnitTimeChanged(Me.m_propUnitTime, cProperty.eChangeFlags.All)
        Me.OnUnitTimeTextChanged(Me.m_propUnitTimeText, cProperty.eChangeFlags.All)
        Me.OnUnitMonetaryChanged(Me.m_propUnitMonetary, cProperty.eChangeFlags.All)

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        Me.m_fpArea.Release()
        Me.m_fpAuthor.Release()
        Me.m_fpContact.Release()
        Me.m_fpDescription.Release()
        Me.m_fpName.Release()
        Me.m_fpNumDigits.Release()
        Me.m_fpGroupDigits.Release()
        Me.m_fpPSD.Release()
        Me.m_fpFirstYear.Release()
        Me.m_fpNumYears.Release()
        Me.m_fpLatMin.Release()
        Me.m_fpLatMax.Release()
        Me.m_fpLonMin.Release()
        Me.m_fpLonMax.Release()

        ' Clean up ( not really necessary since bas class takes care of this, but hey :) )
        Me.CoreComponents = Nothing

        RemoveHandler Me.m_propUnitCurrency.PropertyChanged, AddressOf OnUnitCurrencyChanged
        Me.m_propUnitCurrency = Nothing

        RemoveHandler Me.m_propUnitCurrencyText.PropertyChanged, AddressOf OnUnitCurrencyTextChanged
        Me.m_propUnitCurrencyText = Nothing

        RemoveHandler Me.m_propUnitTime.PropertyChanged, AddressOf OnUnitTimeChanged
        Me.m_propUnitTime = Nothing

        RemoveHandler Me.m_propUnitTimeText.PropertyChanged, AddressOf OnUnitTimeTextChanged
        Me.m_propUnitTimeText = Nothing

        RemoveHandler Me.m_propUnitMonetary.PropertyChanged, AddressOf OnUnitMonetaryChanged
        Me.m_propUnitMonetary = Nothing

        MyBase.OnFormClosed(e)

    End Sub

#Region " Unit handling "

    Dim m_bInUpdate As Boolean = False

#Region " Currency "

    Private Sub PatchCurrencyUnitRadioButtonText(ByVal rb As RadioButton, ByVal uct As eUnitCurrencyType)
        rb.Text = String.Format(rb.Text, Me.StyleGuide.CurrencyUnitText(uct))
    End Sub

    Private Sub OnUnitCurrencyRadioChanged(ByVal sender As Object, ByVal eventargs As EventArgs) _
        Handles rbWetWeight.CheckedChanged, rbJoules.CheckedChanged, rbCalorie.CheckedChanged, _
                rbCarbon.CheckedChanged, rbDryWeight.CheckedChanged, rbNitrogen.CheckedChanged, rbPhosporus.CheckedChanged, _
                rbCurrencyEnergyOther.CheckedChanged, rbNutrientOther.CheckedChanged, rbCurrencyEnergyOther.CheckedChanged

        If (Me.m_propUnitCurrency Is Nothing) Then Return

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True
        If Me.rbWetWeight.Checked Then m_propUnitCurrency.SetValue(eUnitCurrencyType.WetWeight)
        If Me.rbJoules.Checked Then m_propUnitCurrency.SetValue(eUnitCurrencyType.Joules)
        If Me.rbCalorie.Checked Then m_propUnitCurrency.SetValue(eUnitCurrencyType.Calorie)
        If Me.rbCarbon.Checked Then m_propUnitCurrency.SetValue(eUnitCurrencyType.Carbon)
        If Me.rbDryWeight.Checked Then m_propUnitCurrency.SetValue(eUnitCurrencyType.DryWeight)
        If Me.rbNitrogen.Checked Then m_propUnitCurrency.SetValue(eUnitCurrencyType.Nitrogen)
        If Me.rbPhosporus.Checked Then m_propUnitCurrency.SetValue(eUnitCurrencyType.Phosporous)
        If Me.rbCurrencyEnergyOther.Checked Then m_propUnitCurrency.SetValue(eUnitCurrencyType.CustomEnergy)
        If Me.rbNutrientOther.Checked Then m_propUnitCurrency.SetValue(eUnitCurrencyType.CustomNutrient)
        Me.m_bInUpdate = False

    End Sub

    Private Sub OnUnitCurrencyChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True

        Select Case DirectCast(prop.GetValue(), eUnitCurrencyType)
            Case eUnitCurrencyType.WetWeight : Me.rbWetWeight.Checked = True
            Case eUnitCurrencyType.Joules : Me.rbJoules.Checked = True
            Case eUnitCurrencyType.Calorie : Me.rbCalorie.Checked = True
            Case eUnitCurrencyType.Carbon : Me.rbCarbon.Checked = True
            Case eUnitCurrencyType.DryWeight : Me.rbDryWeight.Checked = True
            Case eUnitCurrencyType.Nitrogen : Me.rbNitrogen.Checked = True
            Case eUnitCurrencyType.Phosporous : Me.rbPhosporus.Checked = True
            Case eUnitCurrencyType.CustomEnergy : Me.rbCurrencyEnergyOther.Checked = True
            Case eUnitCurrencyType.CustomNutrient : Me.rbNutrientOther.Checked = True
        End Select

        Me.m_bInUpdate = False
    End Sub

    Private Sub OnUnitCurrencyTextChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
        If Me.rbCurrencyEnergyOther.Checked Then
            Me.tbCurrencyEnergyOther.Text = CStr(prop.GetValue())
            Me.tbCurrencyNutrientOther.Text = ""
        Else
            Me.tbCurrencyNutrientOther.Text = CStr(prop.GetValue())
            Me.tbCurrencyEnergyOther.Text = ""
        End If
    End Sub

    Private Sub OnCustomEnergyTextValidated(ByVal sender As Object, ByVal eventargs As EventArgs) Handles tbCurrencyEnergyOther.Validated
        Me.m_propUnitCurrencyText.SetValue(tbCurrencyEnergyOther.Text)
    End Sub

    Private Sub OnCustomEnergySetFocus(ByVal sender As Object, ByVal eventargs As EventArgs) Handles tbCurrencyEnergyOther.GotFocus
        Me.rbCurrencyEnergyOther.Checked = True
    End Sub

    Private Sub OnCustomNutrientTextValidated(ByVal sender As Object, ByVal eventargs As EventArgs) Handles tbCurrencyNutrientOther.Validated
        Me.m_propUnitCurrencyText.SetValue(tbCurrencyNutrientOther.Text)
    End Sub

    Private Sub OnCustomNutrientSetFocus(ByVal sender As Object, ByVal eventargs As EventArgs) Handles tbCurrencyNutrientOther.GotFocus
        Me.rbNutrientOther.Checked = True
    End Sub

#End Region ' Currency

#Region " Time "

    Private Sub OnUnitTimeRadioChanged(ByVal sender As Object, ByVal eventargs As EventArgs) _
        Handles rbYear.CheckedChanged, rbDay.CheckedChanged, rbTimeOther.CheckedChanged

        If (Me.m_propUnitCurrency Is Nothing) Then Return

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True

        If Me.rbYear.Checked Then m_propUnitTime.SetValue(eUnitTimeType.Year)
        If Me.rbDay.Checked Then m_propUnitTime.SetValue(eUnitTimeType.Day)
        If Me.rbTimeOther.Checked Then m_propUnitTime.SetValue(eUnitTimeType.Custom)

        Me.m_bInUpdate = False

    End Sub

    Private Sub OnUnitTimeChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True

        Select Case DirectCast(prop.GetValue(), eUnitTimeType)
            Case eUnitTimeType.Year : Me.rbYear.Checked = True
            Case eUnitTimeType.Day : Me.rbDay.Checked = True
            Case eUnitTimeType.Custom : Me.rbTimeOther.Checked = True
        End Select

        Me.m_bInUpdate = False
    End Sub

    Private Sub OnUnitTimeTextChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
        Me.txbTimeOther.Text = CStr(prop.GetValue())
    End Sub

    Private Sub OnUnitTimeSetFocus(ByVal sender As Object, ByVal eventargs As EventArgs) Handles txbTimeOther.GotFocus
        Me.rbTimeOther.Checked = True
    End Sub

    Private Sub OnUnitTimeTextValidated(ByVal sender As Object, ByVal eventargs As EventArgs) Handles txbTimeOther.Validated
        Me.m_propUnitTimeText.SetValue(txbTimeOther.Text)
    End Sub

#End Region ' Time

#Region " Monetary "

    Private Sub m_cmbMonetaryUnit_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_cmbMonetaryUnit.SelectedIndexChanged
        Me.m_propUnitMonetary.SetValue(Me.m_cmbMonetaryUnit.Unit)
    End Sub

    Private Sub OnUnitMonetaryChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True
        Me.m_cmbMonetaryUnit.Unit = DirectCast(prop.GetValue(), eUnitMonetaryType)
        Me.m_bInUpdate = False
    End Sub

#End Region ' Monetary

#End Region ' Unit handling

#Region " Events "

    Private Sub m_btnLookup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnLookup.Click

        Dim lookup As New cGoogleMapsLookup()

        If lookup.FindLocation(Me.m_tbModelAreaName.Text) Then
            If MsgBox("Found '" & lookup.Term & "', do you want to use this?", MsgBoxStyle.YesNo Or MsgBoxStyle.Question) = MsgBoxResult.Yes Then
                Me.m_tbModelAreaName.Text = lookup.Term
                Me.m_nudSouth.Value = CDec(lookup.LatMin)
                Me.m_nudNorth.Value = CDec(lookup.LatMax)
                Me.m_nudWest.Value = CDec(lookup.LonMin)
                Me.m_nudEast.Value = CDec(lookup.LonMax)
            End If
        End If

    End Sub

#End Region ' Events

End Class