
Option Strict On
#Region "Imports"

Imports EwEPlugin
Imports EwEResultsExtractor
Imports EwECore
Imports EwENetworkAnalysis
Imports EwEUtils.Core
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Collections
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports Microsoft.Office.Interop

#End Region

Public Class frmResults

#Region "Enumerator(s)"

    Private Enum eResultTypes As Integer
        Biomass = 0
        BiomassIntegrated = 1
        FishingMortality = 2
        PredationMortality = 3
        ConsumptionBiomass = 4
        PredationPerPredator = 5
        FishMortFleetToPrey = 6
        GroupCatch = 7
        DietProportions = 8
        FleetCatch = 9
        FleetValue = 10
        BasicEstimates = 11
        KeyIndices = 12

    End Enum

#End Region

#Region "Private Fields"

    Private m_PluginInterface As frmResults
    Private m_bInitOK As Boolean
    Private m_core As cCore
    Private APredPreySelection As List(Of cPredatorPreySelection)
    Private Shared m_NumberTicked As Integer
    Private PredatorPreySelection As cSelectionData
    Private FleetPreySelection As cSelectionData
    Private PreyPredatorSelection As cSelectionData
    Private ParentOnlySelection As cSelectionData
    Private FleetOnlySelection As cSelectionData
    Private m_MyCheckBoxes As CheckBox()
    Private strPath As String
    Private FunctGroupWB As Excel.Workbook
    Private FisheriesWB As Excel.Workbook
    Private IndicatorsWB As Excel.Workbook
    Private nDataRows As Integer
    Private Const FuncGroupsFileName As String = "FunctionalGroups"
    Private Const FishFleetsFileName As String = "FisheriesGroups"
    Private Const IndicatorsFileName As String = "Indicators"
    Private Const DiagnosticsName As String = "Diagnostics"

    'Private TabCollection As List(Of cTab)

#End Region

    'Delegate that points to next sub to be executed when key-run button clicked
    Public Delegate Sub NextActionTickAll()
    'An instance of the delegate that points to next action 
    Public Shared NextAction As NextActionTickAll

    ' The boolean that determines whether checked event for tick boxes occurs
    Public Shared FireChecked As Boolean = True

#Region "Constructor(s)"

    Public Sub New()

        Me.InitializeComponent()

    End Sub

#End Region

    Public Sub StartForm(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form)

        Dim GroupNames As String() = Me.GetAllGroupNamesArray()
        Dim FleetNames As String() = Me.GetAllFleetNamesArray()

        frmPlugin = Me
        Me.Show()

        nDataRows = m_core.nEcosimTimeSteps

        'Get all group names for predators to create PredatorPreySelection & PreyPredSelection
        'Remember that EcoSimGroupOutputs are indexed from 1!!!
        Dim str(Me.m_core.nGroups - 1) As String
        For i As Integer = 1 To Me.m_core.nGroups
            str(i - 1) = Me.m_core.EcoSimGroupOutputs(i).Name
        Next
        'Create PredPreySelection object
        PredatorPreySelection = New cSelectionData("Predator to many prey", str)
        'Create PreyPredSelection object
        PreyPredatorSelection = New cSelectionData("Prey to many predators", str)
        'Create Parent object
        ParentOnlySelection = New cSelectionData("Parent only", str)

        'Get all groups names for fleet to create FleetPreySelection
        'Remember that EcosimFleetOutput is referenced from 0!!!
        Dim str2(Me.m_core.nFleets) As String
        For i As Integer = 0 To Me.m_core.nFleets
            str2(i) = Me.m_core.EcosimFleetOutput(i).Name
        Next
        ' Create FleetPreySelection
        FleetPreySelection = New cSelectionData("Fleet to many prey", str2)
        ' Create FleetOnlySelection
        FleetOnlySelection = New cSelectionData("Fleet only", str2)

    End Sub

    Public Sub Initialize(ByVal core As Object)
        Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        m_bInitOK = False
        Try
            If TypeOf core Is EwECore.cCore Then
                m_core = DirectCast(core, EwECore.cCore)
                m_bInitOK = True
                System.Console.WriteLine(Me.ToString & ".Initialize() Successfull.")
            Else
                System.Console.WriteLine(Me.ToString & ".Initialize() Failed.")
                Return
            End If
        Catch ex As Exception
            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".Initialize() Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return
        End Try
    End Sub


#Region "Event Handlers"

    Private Sub frmResults_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        'Me.Dispose()
    End Sub

    Private Sub btnSaveResults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveResults.Click

        Dim NumberChecks As Integer = 0
        Dim ex As New Excel.Application
        Dim CurrentPredator As cCreatedObjects

        If FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            strPath = FolderBrowserDialog1.SelectedPath

            If (MsgBox("NOTE: any results files previously saved to the location " & _
                   vbCrLf & "you have selected will be overwritten. " & _
                   vbCrLf & "Is this what you want?" & vbCrLf & vbCrLf & _
                   "(Saving may freeze EwE for a short time)", _
                   MsgBoxStyle.OkCancel) = MsgBoxResult.Cancel) Then
                Exit Sub
            End If

            lblPrgInfo.Show()
            prgSave.Visible = True
            Application.DoEvents()

            'Count how many dataselections have been checked
            If chkBiomass.Checked Then NumberChecks += 1
            If chkBiomassInteg.Checked Then NumberChecks += 1
            If chkConsumption.Checked Then NumberChecks += 1
            If chkFishingMortality.Checked Then NumberChecks += 1
            If chkPredationMortality.Checked Then NumberChecks += 1
            If chkPredationPerPredator.Checked Then NumberChecks += 1
            If chkFishMortFleetToPrey.Checked Then NumberChecks += 1
            If chkEffort.Checked Then NumberChecks += 1
            If chkCatch.Checked Then NumberChecks += 1
            If chkDietProportions.Checked Then NumberChecks += 1
            If chkCatchFleet.Checked Then NumberChecks += 1
            If chkFleetValue.Checked Then NumberChecks += 1
            If chkBasicEstimates.Checked Then NumberChecks += 1
            If chkKeyIndices.Checked Then NumberChecks += 1
            If chkMortalityCoefficients.Checked Then NumberChecks += 1
            If chkInitPredMort.Checked Then NumberChecks += 1
            If chkInitConsumption.Checked Then NumberChecks += 1
            If chkInitFishMort.Checked Then NumberChecks += 1
            If chkRespiration.Checked Then NumberChecks += 1
            If chkPreyOverlap.Checked Then NumberChecks += 1
            If chkPredOverlap.Checked Then NumberChecks += 1
            If chkElectivity.Checked Then NumberChecks += 1
            If chkInitFishingQuantities.Checked Then NumberChecks += 1
            If chkSearchRates.Checked Then NumberChecks += 1
            If chkInitFishingValues.Checked Then NumberChecks += 1

            prgSave.Minimum = 0
            prgSave.Maximum = NumberChecks
            prgSave.Value = 0
            prgSave.Step = 1
            Application.DoEvents()

            FunctGroupWB = ConnectWB(FuncGroupsFileName, ex)
            FisheriesWB = ConnectWB(FishFleetsFileName, ex)
            IndicatorsWB = ConnectWB(IndicatorsFileName, ex)

            If chkBiomass.Checked Then
                CreateBiomassCSV(CType(FunctGroupWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkBiomassInteg.Checked Then
                CreateBiomassIntegratedCSV(CType(FunctGroupWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkConsumption.Checked Then
                If chkConsumption.Checked Then
                    For PredatorIndex As Integer = 0 To PredatorPreySelection.CountSelected - 1
                        'Get Predator Parent-Child Object
                        CurrentPredator = PredatorPreySelection.GetSelectedItem(PredatorIndex)
                        CreateConsumptionCSV(CType(FunctGroupWB.Worksheets.Add, Excel.Worksheet), CurrentPredator)
                    Next
                    prgSave.PerformStep()
                End If
            End If
            If chkFishingMortality.Checked Then
                CreateFishingMortalityCSV(CType(FunctGroupWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkPredationMortality.Checked Then
                CreatePredationMortalityCSV(CType(FunctGroupWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkPredationPerPredator.Checked Then
                CreatePredationMortalityEachPredatorCSV(CType(FunctGroupWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkFishMortFleetToPrey.Checked Then
                CreateMortalityByFleetCSV(CType(FisheriesWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkEffort.Checked Then
                CreateEffort(CType(FisheriesWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkCatch.Checked Then
                CreateCatchCSV(CType(FunctGroupWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkDietProportions.Checked Then
                'Run for each Predator object
                For PredatorIndex As Integer = 0 To PredatorPreySelection.GetSelected.Count - 1
                    'Get Predator Parent-Child Object
                    CurrentPredator = PredatorPreySelection.GetSelectedItem(PredatorIndex)
                    CreateDietCSV(CType(FunctGroupWB.Worksheets.Add, Excel.Worksheet), CurrentPredator)
                Next

                prgSave.PerformStep()
            End If
            If chkCatchFleet.Checked Then
                CreateCatchByFleetCSV(CType(FisheriesWB.Worksheets.Add, Excel.Worksheet))
                CreateLandingsByFleetCSV(CType(FisheriesWB.Worksheets.Add, Excel.Worksheet))
                CreateDiscardsByFleetCSV(CType(FisheriesWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkFleetValue.Checked Then
                CreateValueCSV(CType(FisheriesWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkBasicEstimates.Checked Then
                CreateBasicEstimatesCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkKeyIndices.Checked Then
                CreateKeyIndicesCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkMortalityCoefficients.Checked Then
                CreateInitMortCoeffsCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkInitPredMort.Checked Then
                CreateInitPredMortCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkInitFishMort.Checked Then
                CreateInitFishingMortCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkInitConsumption.Checked Then
                CreateInitConsumptionCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkRespiration.Checked Then
                CreateRespirationCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkPreyOverlap.Checked Then
                CreateOverlapPreyCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkPredOverlap.Checked Then
                CreateOverlapPredCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkElectivity.Checked Then
                CreateElectivityCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkInitFishingQuantities.Checked Then
                CreateInitFishingQuantitiesCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkSearchRates.Checked Then
                CreateSearchRatesCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If
            If chkInitFishingValues.Checked Then
                CreateInitFishingValuesCSV(CType(IndicatorsWB.Worksheets.Add, Excel.Worksheet))
                prgSave.PerformStep()
            End If

            prgSave.Visible = False
            lblPrgInfo.Hide()

            FunctGroupWB.Save()
            FunctGroupWB = Nothing
            FisheriesWB.Save()
            FisheriesWB = Nothing
            IndicatorsWB.Save()
            IndicatorsWB = Nothing
            ex.Quit()

            Me.Close()

        End If

        ResetForm()

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
        ResetForm()
    End Sub

    Private Sub chkBiomass_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkBiomass.CheckedChanged
        Dim a As frmSelectParentOnly
        If FireChecked = False Then Exit Sub
        If chkBiomass.Checked = True And ParentOnlySelection.CountSelected = 0 Then
            a = frmSelectParentOnly.GetInstance(ParentOnlySelection, m_core)
            'Dim a As New frmSelectParentOnly(ParentOnlySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkBiomass.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkBiomassInteg_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkBiomassInteg.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkBiomassInteg.Checked = True And ParentOnlySelection.CountSelected = 0 Then
            Dim a As New frmSelectParentOnly(ParentOnlySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkBiomassInteg.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkConsumptionBiomass_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkConsumption.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkConsumption.Checked = True And PredatorPreySelection.CountSelected = 0 Then
            Dim a As New frmSelectPredatorPrey(PredatorPreySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkConsumption.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkPredationMortality_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPredationMortality.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkPredationMortality.Checked = True And ParentOnlySelection.CountSelected = 0 Then
            Dim a As New frmSelectParentOnly(ParentOnlySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkPredationMortality.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkFishingMortality_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkFishingMortality.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkFishingMortality.Checked = True And ParentOnlySelection.CountSelected = 0 Then
            Dim a As New frmSelectParentOnly(ParentOnlySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkFishingMortality.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkPredationPerPredator_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPredationPerPredator.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkPredationPerPredator.Checked = True And PreyPredatorSelection.CountSelected = 0 Then
            Dim a As New frmSelectPreyPredator(PreyPredatorSelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkPredationPerPredator.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkFishMortFleetToPrey_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkFishMortFleetToPrey.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkFishMortFleetToPrey.Checked = True And FleetPreySelection.CountSelected = 0 Then
            Dim a As New frmSelectFleetPrey(FleetPreySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkFishMortFleetToPrey.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkEffort_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkEffort.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkEffort.Checked = True And FleetOnlySelection.CountSelected = 0 Then
            Dim a As New frmSelectFleetOnly(FleetOnlySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkEffort.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkCatch_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCatch.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkCatch.Checked = True And ParentOnlySelection.CountSelected = 0 Then
            Dim a As New frmSelectParentOnly(ParentOnlySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkCatch.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkDietProportions_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDietProportions.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkDietProportions.Checked = True And PredatorPreySelection.CountSelected = 0 Then
            Dim a As New frmSelectPredatorPrey(PredatorPreySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkDietProportions.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkCatchFleet_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCatchFleet.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkCatchFleet.Checked = True And FleetPreySelection.CountSelected = 0 Then
            Dim a As New frmSelectFleetPrey(FleetPreySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkCatchFleet.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub chkValue_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkFleetValue.CheckedChanged
        If FireChecked = False Then Exit Sub
        If chkFleetValue.Checked = True And FleetOnlySelection.CountSelected = 0 Then
            Dim a As New frmSelectFleetOnly(FleetOnlySelection, m_core)
            a.Show()
            'When form is closed call this validation sub
            AddHandler a.FormExited, AddressOf ValidateObjectCreated
        End If
        If chkFleetValue.Checked = False Then DeleteObjects()
        SetSaveResultsState()
    End Sub

    Private Sub btnSetPredPrey_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetPredPrey.Click
        Dim a As New frmSelectPredatorPrey(PredatorPreySelection, m_core)
        AddHandler a.FormExited, AddressOf ValidateObjectCreated
    End Sub

    Private Sub btnSetFeetPrey_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim a As New frmSelectFleetPrey(FleetPreySelection, m_core)
        a.Show()
        AddHandler a.FormExited, AddressOf ValidateObjectCreated
    End Sub

    Private Sub btnSetPreyPred_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetPreyPred.Click
        Dim a As New frmSelectPreyPredator(PreyPredatorSelection, m_core)
        a.Show()
        AddHandler a.FormExited, AddressOf ValidateObjectCreated
    End Sub

    Private Sub btnSetParentOnly_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetParentOnly.Click
        Dim a As New frmSelectParentOnly(ParentOnlySelection, m_core)
        a.Show()
        AddHandler a.FormExited, AddressOf ValidateObjectCreated
    End Sub

    Private Sub btnSetCatchFleet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetFleetPrey.Click
        Dim a As New frmSelectFleetPrey(FleetPreySelection, m_core)
        a.Show()
        AddHandler a.FormExited, AddressOf ValidateObjectCreated
    End Sub

    Private Sub btnSetFleetOnly_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetFleetOnly.Click
        Dim a As New frmSelectFleetOnly(FleetOnlySelection, m_core)
        a.Show()
        AddHandler a.FormExited, AddressOf ValidateObjectCreated
    End Sub

    Private Sub btnTickAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOptions.Click
        FireChecked = False
        NextAction = New NextActionTickAll(AddressOf Me.PredatorPreyStage)

        'First stage is do parent only section
        Dim a As New frmSelectParentOnly(ParentOnlySelection, m_core)
        a.Show()

    End Sub

    Private Sub chkBasicEstimates_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkBasicEstimates.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkKeyIndices_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkKeyIndices.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkMortalityCoefficients_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkMortalityCoefficients.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkInitPredMort_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkInitPredMort.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkInitConsumption_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkInitConsumption.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkInitFishMort_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkInitFishMort.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkRespiration_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkRespiration.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkPreyOverlap_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPreyOverlap.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkPredOverlap_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPredOverlap.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkElectivity_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkElectivity.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkSearchRates_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkSearchRates.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkInitFishingQuantities_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkInitFishingQuantities.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkInitFishingValues_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkInitFishingValues.CheckedChanged
        SetSaveResultsState()
    End Sub

    Private Sub chkYearly_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkYearly.CheckedChanged
        If chkYearly.Checked Then
            nDataRows = CInt(Math.Floor(m_core.nEcosimTimeSteps / 12))
        Else
            nDataRows = m_core.nEcosimTimeSteps
        End If
    End Sub

#End Region

#Region "Functions"

    Protected Function CheckName(ByVal Name As String, ByVal wb As Excel.Workbook) As String

        Dim s As Excel.Worksheet
        For Each s In wb.Worksheets
            If UCase(s.Name) = UCase(Name) Then
                Name = CheckName(Name & "(1)", wb)
            End If
        Next
        Return Name

    End Function

    Public Function GetAllGroupNamesArray() As String()
        Dim str(Me.m_core.nGroups - 1) As String

        For i As Integer = 1 To Me.m_core.nGroups
            str(i - 1) = Me.m_core.EcoSimGroupOutputs(i).Name
        Next
        Return str

    End Function

    Public Function GetAllFleetNamesArray() As String()
        Dim str(Me.m_core.nFleets) As String

        For i As Integer = 0 To Me.m_core.nFleets
            str(i) = Me.m_core.EcosimFleetOutput(i).Name
        Next
        Return str

    End Function

    Private Function CreateListNames(ByVal InputStrings As List(Of String)) As String
        'Create a string of names for the list of input objects
        Dim CompiledNames As New StringBuilder()

        For i As Integer = 0 To InputStrings.Count - 2
            CompiledNames.Append("""" & InputStrings(i) & """" & ",")
        Next
        CompiledNames.Append("""" & InputStrings(InputStrings.Count - 1) & """")
        Return CompiledNames.ToString

    End Function

    Private Function GetPreyNames(ByVal PredPreyObject As cPredatorPreySelection) As String
        'Create a string of predator names for the list of prey a given predator selection
        Dim PreyNames As New StringBuilder()

        For i As Integer = 0 To PredPreyObject.CountPrey - 2
            PreyNames.Append("""" & PredPreyObject.PreyName(i) & """" & ",")
        Next
        PreyNames.Append("""" & PredPreyObject.PreyName(PredPreyObject.CountPrey - 1) & """")
        Return PreyNames.ToString

    End Function

    Private Function GetIndexGroup(ByVal Group As String) As Integer

        'Find out what the index number is for a given group in m_core.EcosimGroupOutputs
        Dim i As Integer = 1
        While i <= m_core.nGroups And m_core.EcoSimGroupOutputs(i).Name <> Group
            i += 1
        End While
        If i > m_core.nGroups Then
            Return -1
        Else
            Return i
        End If

    End Function

    Private Function GetIndexFleet(ByVal Fleet As String) As Integer

        'Find out what the index number is for a given fleet in m_core.EcosimGroupOutputs
        Dim i As Integer = 0
        While i <= m_core.nFleets
            If m_core.EcosimFleetOutput(i).Name = Fleet Then
                Exit While
            End If
            i += 1
        End While
        If i > m_core.nFleets Then
            Return -1
        Else
            Return i
        End If

    End Function

    Private Function ConnectWB(ByVal fileName As String, ByRef ex As Excel.Application) As Excel.Workbook

        Dim FileExists As Boolean = False
        Dim DirectInfo As New DirectoryInfo(strPath)
        Dim files As FileInfo() = DirectInfo.GetFiles
        Dim fwb As Excel.Workbook
        Dim fDateTime As DateTime = DateTime.Now

        fileName &= "(D" & fDateTime.Day & "-" & fDateTime.Month & "-" & fDateTime.Year & ")(T" & _
        fDateTime.Hour.ToString & "-" & fDateTime.Minute.ToString & "-" & fDateTime.Second.ToString & ")" ' & ".xls"

        fwb = ex.Workbooks.Add()
        fwb.SaveAs(strPath & "\" & fileName)

        Return fwb

    End Function

#End Region

#Region "Subroutines"

    Private Sub CreateBiomassCSV(ByVal sheet As Excel.Worksheet)

        Dim EwEIndex As Integer 'Index of group in EwE datastructure

        'Holds the array of data for all selected groups
        Dim ABiomass(,) As Single = Nothing
        If chkYearly.Checked Then
            ReDim ABiomass(ParentOnlySelection.CountSelected - 1, m_core.nEcosimYears - 1)
        Else
            ReDim ABiomass(ParentOnlySelection.CountSelected - 1, nDataRows - 1)
        End If

        'Gets a list of names for the selected groups
        Dim SelectedNames As List(Of String) = ParentOnlySelection.SelectedNames

        'Loops for each group in selected
        For ParentIndex = 0 To SelectedNames.Count - 1

            'Finds index for group wanting to get biomass of
            EwEIndex = GetIndexGroup(SelectedNames(ParentIndex))

            'Loop through EwE datastructure getting biomass for current group at each timestep
            If chkYearly.Checked Then
                For Year As Integer = 1 To m_core.nEcosimYears
                    For Month As Integer = 1 To 12
                        ABiomass(ParentIndex, Year - 1) += m_core.EcoSimGroupOutputs(EwEIndex).Biomass((Year - 1) * 12 + Month)
                    Next
                    ABiomass(ParentIndex, Year - 1) /= 12
                Next
            Else
                For TimeStep As Integer = 1 To nDataRows
                    ABiomass(ParentIndex, TimeStep - 1) = m_core.EcoSimGroupOutputs(EwEIndex).Biomass(TimeStep)
                Next
            End If

        Next

        SendToFileTabbed(ABiomass, SelectedNames, TabName:="Biomass", _
                         FileName:=FuncGroupsFileName, Sheet:=sheet, wb:=FunctGroupWB)

    End Sub

    Private Sub CreateBiomassIntegratedCSV(ByVal sheet As Excel.Worksheet)

        'Holds the array of data for all selected groups
        Dim ABiomassInteg(ParentOnlySelection.CountSelected - 1, 0) As Single
        Dim StartStepBiomass As Single
        Dim EndStepBiomass As Single
        Dim IntegStep As Single

        'Index of group in EwE datastructure
        Dim EwEIndex As Integer

        'Gets a list of names for the selected groups
        Dim SelectedNames As List(Of String) = ParentOnlySelection.SelectedNames

        'Loops for each group in selected
        For ParentIndex = 0 To SelectedNames.Count - 1

            'Finds index for group wanting to get biomass of
            EwEIndex = GetIndexGroup(SelectedNames(ParentIndex))

            For TimeStep As Integer = 2 To m_core.nEcosimTimeSteps

                'Remember that Biomass is changed to difference from initial biomass
                StartStepBiomass = m_core.EcoSimGroupOutputs(EwEIndex).Biomass(TimeStep - 1) _
                    - m_core.EcoPathGroupOutputs(EwEIndex).Biomass
                EndStepBiomass = m_core.EcoSimGroupOutputs(EwEIndex).Biomass(TimeStep) _
                    - m_core.EcoPathGroupOutputs(EwEIndex).Biomass

                'Calc. Integ. for step
                IntegStep = (StartStepBiomass + EndStepBiomass) / (2 * 12) 'Gives units tons*year

                'Add step to array
                ABiomassInteg(ParentIndex, 0) += IntegStep
            Next

        Next

        SendToFileTabbed(ABiomassInteg, SelectedNames, TabName:="BiomassIntegrated", _
                        FileName:=FuncGroupsFileName, Sheet:=sheet, wb:=FunctGroupWB)

    End Sub

    Private Sub CreateConsumptionCSV(ByVal sheet As Excel.Worksheet, ByVal CurrentPredator As cCreatedObjects)

        Dim AConsPerPrey(,) As Single
        Dim PreyNames As New StringBuilder()    'to create prey names for top .CSV file
        Dim PredatorIndexEcosim As Integer      'holds index in EwE m_core of Pred
        Dim PreyIndexEcosim As Integer          'holds index in EwE m_core of Prey

        'Index of group in EwE datastructure
        Dim EwEIndex As Integer
        'Current Parent-Child Object

        'Gets a list of names for the selected objects
        Dim SelectedNames As List(Of String) = PredatorPreySelection.SelectedNames

        'Get Predator index in EcoSim
        PredatorIndexEcosim = GetIndexGroup(CurrentPredator.ParentName)

        'Runs only if prey>0
        If CurrentPredator.CountChild > 0 Then

            'Find PredatorIndexEcosim in m_core.EcoSimGroupOutputs(PredatorIndexEcosim) for PredatorIndex
            EwEIndex = GetIndexGroup(CurrentPredator.ParentName)

            'Dim array for holding consumption values for each predprey
            AConsPerPrey = Nothing
            ReDim AConsPerPrey(CurrentPredator.CountChild - 1, nDataRows - 1)

            For PreyIndex As Integer = 0 To CurrentPredator.CountChild - 1

                'Find PreyIndexEcosim in m_core.EcoSimGroupOutputs(PredatorIndexEcosim) for PreyIndex
                PreyIndexEcosim = GetIndexGroup(CurrentPredator.ChildNames(PreyIndex))

                'Calculate consumption values for each prey of each predator for each year
                If chkYearly.Checked Then
                    For Year As Integer = 1 To m_core.nEcosimYears
                        For Month As Integer = 1 To 12
                            AConsPerPrey(PreyIndex, Year - 1) += _
                                m_core.EcoSimGroupOutputs(PredatorIndexEcosim).PreyPercentage(PreyIndexEcosim, (Year - 1) * 12 + Month) _
                                * m_core.EcoSimGroupOutputs(PredatorIndexEcosim).Biomass((Year - 1) * 12 + Month) _
                                * m_core.EcoSimGroupOutputs(PredatorIndexEcosim).ConsumpBiomass((Year - 1) * 12 + Month)
                        Next
                        AConsPerPrey(PreyIndex, Year - 1) /= 12
                    Next
                Else
                    For TimeStep As Integer = 1 To nDataRows
                        AConsPerPrey(PreyIndex, TimeStep - 1) = _
                            m_core.EcoSimGroupOutputs(PredatorIndexEcosim).PreyPercentage(PreyIndexEcosim, TimeStep) _
                            * m_core.EcoSimGroupOutputs(PredatorIndexEcosim).Biomass(TimeStep) _
                            * m_core.EcoSimGroupOutputs(PredatorIndexEcosim).ConsumpBiomass(TimeStep)
                    Next
                End If

            Next

            SendToFileTabbed(AConsPerPrey, CurrentPredator.ChildNames, _
                             TabName:="Consumpt_" & Mid(CurrentPredator.ParentName, 1, 22), _
                             FileName:=FuncGroupsFileName, Sheet:=sheet, wb:=FunctGroupWB)
        End If

    End Sub

    'Retrieves the F on each group
    Private Sub CreateFishingMortalityCSV(ByVal sheet As Excel.Worksheet)

        Dim AFishingMortality(ParentOnlySelection.CountSelected - 1, nDataRows - 1) As Single

        'Index of group in EwE datastructure
        Dim EwEIndex As Integer

        For ParentIndex As Integer = 0 To ParentOnlySelection.CountSelected - 1
            'Get Index of Parent in EwE
            EwEIndex = GetIndexGroup(ParentOnlySelection.SelectedNames(ParentIndex))

            If chkYearly.Checked Then

                For Year As Integer = 1 To m_core.nEcosimYears
                    For Month As Integer = 1 To 12
                        'Retrieve Fishing mortality for parent
                        AFishingMortality(ParentIndex, Year - 1) += _
                                        m_core.EcoSimGroupOutputs(EwEIndex).FishMort((Year - 1) * 12 + Month) - _
                                        m_core.EcoSimGroupOutputs(EwEIndex).PredMort((Year - 1) * 12 + Month)
                    Next
                    AFishingMortality(ParentIndex, Year - 1) /= 12
                Next
            Else
                For TimeStep As Integer = 1 To nDataRows

                    'Retrieve Fishing mortality for parent
                    AFishingMortality(ParentIndex, TimeStep - 1) = _
                                    m_core.EcoSimGroupOutputs(EwEIndex).FishMort(TimeStep) - _
                                    m_core.EcoSimGroupOutputs(EwEIndex).PredMort(TimeStep)
                Next
            End If

        Next

        SendToFileTabbed(AFishingMortality, ParentOnlySelection.SelectedNames, _
                     FileName:=FuncGroupsFileName, Sheet:=sheet, TabName:="FishMortAllFleet", _
                    wb:=FunctGroupWB)

    End Sub

    Private Sub CreatePredationMortalityCSV(ByVal sheet As Excel.Worksheet)

        'Dim APredationMortality(APredPreySelection.Count - 1, m_core.nEcosimTimeSteps - 1) As Single
        Dim APredationMortality(ParentOnlySelection.CountSelected - 1, nDataRows - 1) As Single

        'Index of group in EwE datastructure
        Dim EwEIndex As Integer

        If chkYearly.Checked Then
            For PredatorIndex As Integer = 0 To ParentOnlySelection.CountSelected - 1
                For TimeStep As Integer = 0 To nDataRows - 1

                    'Get Index of Parent in EwE
                    EwEIndex = GetIndexGroup(ParentOnlySelection.SelectedNames(PredatorIndex))

                Next
                For Year As Integer = 1 To m_core.nEcosimYears
                    For Month As Integer = 1 To 12
                        'Retrieve Predation mortality for parent
                        APredationMortality(PredatorIndex, Year - 1) += _
                                            m_core.EcoSimGroupOutputs(EwEIndex).PredMort((Year - 1) * 12 + Month)
                    Next
                    APredationMortality(PredatorIndex, Year - 1) /= 12
                Next
            Next
        Else
            For PredatorIndex As Integer = 0 To ParentOnlySelection.CountSelected - 1
                For TimeStep As Integer = 1 To nDataRows

                    'Get Index of Parent in EwE
                    EwEIndex = GetIndexGroup(ParentOnlySelection.SelectedNames(PredatorIndex))
                    'retrieve mortality for current predator at current timestep
                    APredationMortality(PredatorIndex, TimeStep - 1) = _
                    m_core.EcoSimGroupOutputs(EwEIndex).PredMort(TimeStep)

                Next
            Next
        End If



        SendToFileTabbed(APredationMortality, ParentOnlySelection.SelectedNames, _
                         TabName:="PredMort", FileName:=FuncGroupsFileName, _
                         Sheet:=sheet, wb:=FunctGroupWB)

    End Sub

    Private Sub CreatePredationMortalityEachPredatorCSV(ByVal sheet As Excel.Worksheet)

        'Count number of childs for all prey objects to dimension array holding mortalities
        Dim NumberOfChilds As Integer = 0
        For Each prey In PreyPredatorSelection.GetSelected
            NumberOfChilds += prey.CountChild
        Next
        Dim APredationMortality(NumberOfChilds - 1, nDataRows - 1) As Single

        'Index of group in EwE datastructure
        Dim EwEIndexPredator As Integer
        Dim EwEIndexPrey As Integer
        'Init column pointer
        Dim ColPointer As Integer = 0
        Dim Consumption As Single
        Dim CurrentPrey As cCreatedObjects
        Dim FileHeader As String = Nothing

        For PreyIndex As Integer = 0 To PreyPredatorSelection.CountSelected - 1

            CurrentPrey = PreyPredatorSelection.GetSelected(PreyIndex)
            EwEIndexPrey = GetIndexGroup(CurrentPrey.ParentName)

            For PredatorIndex As Integer = 0 To CurrentPrey.CountChild - 1

                EwEIndexPredator = GetIndexGroup(CurrentPrey.ChildNames(PredatorIndex))

                If chkYearly.Checked Then
                    For nYear As Integer = 1 To m_core.nEcosimYears
                        For nMonth As Integer = 1 To 12
                            Consumption = _
                                m_core.EcoSimGroupOutputs(EwEIndexPredator).PreyPercentage(EwEIndexPrey, (nYear - 1) * 12 + nMonth) _
                                * m_core.EcoSimGroupOutputs(EwEIndexPredator).Biomass((nYear - 1) * 12 + nMonth) _
                                * m_core.EcoSimGroupOutputs(EwEIndexPredator).ConsumpBiomass((nYear - 1) * 12 + nMonth)
                            APredationMortality(ColPointer, nYear - 1) += Consumption / m_core.EcoSimGroupOutputs(EwEIndexPrey).Biomass((nYear - 1) * 12 + nMonth)
                        Next
                        APredationMortality(ColPointer, nYear - 1) /= 12
                    Next
                Else
                    For TimeStep As Integer = 1 To nDataRows
                        Consumption = _
                            m_core.EcoSimGroupOutputs(EwEIndexPredator).PreyPercentage(EwEIndexPrey, TimeStep) _
                            * m_core.EcoSimGroupOutputs(EwEIndexPredator).Biomass(TimeStep) _
                            * m_core.EcoSimGroupOutputs(EwEIndexPredator).ConsumpBiomass(TimeStep)

                        APredationMortality(ColPointer, TimeStep - 1) = Consumption / m_core.EcoSimGroupOutputs(EwEIndexPrey).Biomass(TimeStep)
                    Next
                End If

                ColPointer += 1

            Next
        Next

        SendToFileTabbed(APredationMortality, PreyPredatorSelection.GetSelected, _
                         TabName:="PredMortEachPred", FileName:=FuncGroupsFileName, _
                         sheet:=sheet, wb:=FunctGroupWB)

    End Sub

    'Retrieves the partial F's on each group
    Private Sub CreateMortalityByFleetCSV(ByVal sheet As Excel.Worksheet)

        'Count number of childs for all prey objects to dimension array holding mortalities
        Dim NumberOfChilds As Integer = 0
        For Each prey In FleetPreySelection.GetSelected
            NumberOfChilds += prey.CountChild
        Next
        Dim AFishingMortality(NumberOfChilds - 1, nDataRows - 1) As Single

        'Index of group in EwE datastructure
        Dim EwEIndexFleet As Integer
        Dim EwEIndexPrey As Integer
        'Init column pointer
        Dim ColPointer As Integer = 0
        Dim FleetCatch As Single
        Dim Biomass As Single
        Dim CurrentFleet As cCreatedObjects
        Dim FileHeader As String = Nothing

        For FleetIndex As Integer = 0 To FleetPreySelection.CountSelected - 1
            CurrentFleet = FleetPreySelection.GetSelected(FleetIndex)

            'Get Index of fleet in EwE
            For i = 0 To m_core.nFleets - 1
                If m_core.EcosimFleetOutput(i).Name = CurrentFleet.ParentName Then
                    EwEIndexFleet = i
                    Exit For
                End If
            Next

            For PreyIndex As Integer = 0 To CurrentFleet.CountChild - 1
                EwEIndexPrey = GetIndexGroup(CurrentFleet.ChildNames(PreyIndex))
                If chkYearly.Checked Then
                    For nYear As Integer = 1 To m_core.nEcosimYears
                        For nMonth As Integer = 1 To 12
                            FleetCatch = m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, (nYear - 1) * 12 + nMonth)
                            Biomass = m_core.EcoSimGroupOutputs(EwEIndexPrey).Biomass((nYear - 1) * 12 + nMonth)
                            AFishingMortality(ColPointer, nYear - 1) += FleetCatch / Biomass
                        Next
                        AFishingMortality(ColPointer, nYear - 1) /= 12
                    Next
                Else
                    For TimeStep As Integer = 1 To nDataRows
                        'Get Catch Biomass
                        FleetCatch = m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, TimeStep)
                        Biomass = m_core.EcoSimGroupOutputs(EwEIndexPrey).Biomass(TimeStep)
                        AFishingMortality(ColPointer, TimeStep - 1) = FleetCatch / Biomass
                    Next
                End If
                ColPointer += 1
            Next
        Next

        SendToFileTabbed(AFishingMortality, FleetPreySelection.GetSelected, _
                         FileName:=FishFleetsFileName, sheet:=sheet, TabName:="FishMortPerFleet", _
                         wb:=FisheriesWB)

    End Sub

    'Calculates effort time series for each fleet
    Private Sub CreateEffort(ByVal sheet As Excel.Worksheet)

        Dim AEffort(FleetOnlySelection.CountSelected - 1, nDataRows) As Single
        Dim PartialF As Single
        Dim InitialPartialF As Single

        'Index of group in EwE datastructure
        Dim EwEIndexFleet As Integer
        Dim EwEIndexPrey As Integer

        'Init column pointer

        For FleetIndex As Integer = 0 To FleetOnlySelection.CountSelected - 1

            'Get Index of fleet in EwE
            EwEIndexFleet = 0
            For i = 0 To m_core.nFleets
                If m_core.EcosimFleetOutput(i).Name = FleetOnlySelection.SelectedNames(FleetIndex) Then
                    EwEIndexFleet = i
                    Exit For
                End If
            Next

            If EwEIndexFleet <> 0 Then

                'Find a functional group that is caught by fleet
                EwEIndexPrey = 1
                While m_core.FleetInputs(EwEIndexFleet).Landings(EwEIndexPrey) = 0 Or EwEIndexFleet > m_core.nGroups
                    EwEIndexPrey += 1
                End While

                If EwEIndexFleet > m_core.nGroups Then Exit Sub

                'Calculate initial partialF
                InitialPartialF = (m_core.FleetInputs(EwEIndexFleet).Landings(EwEIndexPrey) + _
                                    m_core.FleetInputs(EwEIndexFleet).Discards(EwEIndexPrey)) _
                                    / m_core.EcoPathGroupOutputs(EwEIndexPrey).Biomass

                'Calculate efforts
                AEffort(FleetIndex, 0) = 1
                If chkYearly.Checked Then
                    For nYear As Integer = 1 To m_core.nEcosimYears
                        For nMonth As Integer = 1 To 12
                            PartialF = m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, (nYear - 1) * 12 + nMonth) / _
                                        m_core.EcoSimGroupOutputs(EwEIndexPrey).Biomass((nYear - 1) * 12 + nMonth)
                            AEffort(FleetIndex, nYear) += PartialF
                        Next
                        AEffort(FleetIndex, nYear) /= (12 * InitialPartialF)
                    Next
                Else
                    For TimeStep As Integer = 1 To nDataRows
                        PartialF = m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, TimeStep) / _
                                    m_core.EcoSimGroupOutputs(EwEIndexPrey).Biomass(TimeStep)
                        AEffort(FleetIndex, TimeStep) = PartialF / InitialPartialF
                    Next
                End If

            Else

                For TimeStep As Integer = 0 To nDataRows
                    AEffort(FleetIndex, TimeStep) = -9999
                Next

            End If
        Next

        SendToFileTabbed(AEffort, FleetOnlySelection.SelectedNames, _
                         FileName:=FishFleetsFileName, Sheet:=sheet, TabName:="FishingEffort", _
                         wb:=FisheriesWB)

    End Sub

    Private Sub CreateCatchCSV(ByVal sheet As Excel.Worksheet)
        Dim EwEIndex As Integer 'Index of group in EwE datastructure

        'Holds the array of data for all selected groups
        Dim ACatch(ParentOnlySelection.CountSelected - 1, nDataRows - 1) As Single

        'Gets a list of names for the selected groups
        Dim SelectedNames As List(Of String) = ParentOnlySelection.SelectedNames

        'Loops for each group in selected
        For ParentIndex = 0 To SelectedNames.Count - 1

            'Finds index for group wanting to get biomass of
            EwEIndex = GetIndexGroup(SelectedNames(ParentIndex))

            'Loop through EwE datastructure getting Catch for current group at each timestep
            If chkYearly.Checked Then
                For nYear As Integer = 1 To m_core.nEcosimYears
                    For nMonth = 1 To 12
                        ACatch(ParentIndex, nYear - 1) += m_core.EcoSimGroupOutputs(EwEIndex).Yield((nYear - 1) * 12 + nMonth)
                    Next
                    ACatch(ParentIndex, nYear - 1) /= 12
                Next
            Else
                For TimeStep As Integer = 1 To nDataRows
                    ACatch(ParentIndex, TimeStep - 1) = m_core.EcoSimGroupOutputs(EwEIndex).Yield(TimeStep)
                Next
            End If

        Next

        SendToFileTabbed(ACatch, SelectedNames, FileName:=FuncGroupsFileName, _
                     Sheet:=sheet, TabName:="Catch", wb:=FunctGroupWB)

    End Sub

    Private Sub CreateCatchByFleetCSV(ByVal sheet As Excel.Worksheet)
        Dim EwEIndexFleet As Integer 'Index of group in EwE datastructure
        Dim EwEIndexPrey As Integer
        Dim ColPointer As Integer = 0 'To track col in array to put data
        Dim ColTitles As String = Nothing 'Title of columns in .CSV file
        'Used to hold ratio to seperate catch into discards and landings (should sum to 1)

        'Holds the array of data for all selected groups
        Dim ACatchByFleet(FleetPreySelection.CountSelectedChild - 1, nDataRows - 1) As Single

        'Gets a list of names for the selected groups
        Dim SelectedObjects As List(Of cCreatedObjects) = FleetPreySelection.GetSelected

        'Loops for each group in selected
        For FleetIndex = 0 To SelectedObjects.Count - 1

            'Finds index for group wanting to get values of
            EwEIndexFleet = GetIndexFleet(SelectedObjects(FleetIndex).ParentName)

            'Loop for each prey
            For Each Prey In SelectedObjects(FleetIndex).ChildNames
                EwEIndexPrey = GetIndexGroup(Prey)

                'Loop through EwE datastructure getting biomass for current group at each timestep
                If chkYearly.Checked Then
                    For nYear As Integer = 1 To m_core.nEcosimYears
                        For nMonth = 1 To 12
                            ACatchByFleet(ColPointer, nYear - 1) += m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, (nYear - 1) * 12 + nMonth)
                        Next
                        ACatchByFleet(ColPointer, nYear - 1) /= 12
                    Next
                Else
                    For TimeStep As Integer = 1 To m_core.nEcosimTimeSteps
                        ACatchByFleet(ColPointer, TimeStep - 1) = m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, TimeStep)
                    Next
                End If

                ColPointer += 1

            Next
        Next

        SendToFileTabbed(ACatchByFleet, SelectedObjects, _
                FileName:=FishFleetsFileName, sheet:=sheet, _
                TabName:="CatchPerFleetPerPrey", wb:=FisheriesWB)


    End Sub

    Private Sub CreateLandingsByFleetCSV(ByVal sheet As Excel.Worksheet)
        Dim EwEIndexFleet As Integer 'Index of group in EwE datastructure
        Dim EwEIndexPrey As Integer
        Dim ColPointer As Integer = 0 'To track col in array to put data
        Dim ColTitles As String = Nothing 'Title of columns in .CSV file
        'Used to hold ratio to seperate catch into discards and landings (should sum to 1)
        Dim PropLandings As Single
        Dim Landings As Single
        Dim Discards As Single

        'Holds the array of data for all selected groups
        Dim ACatchByFleet(FleetPreySelection.CountSelectedChild - 1, nDataRows - 1) As Single
        Dim ALandingsByFleet(FleetPreySelection.CountSelectedChild - 1, nDataRows - 1) As Single

        'Gets a list of names for the selected groups
        Dim SelectedObjects As List(Of cCreatedObjects) = FleetPreySelection.GetSelected

        'Loops for each group in selected
        For FleetIndex = 0 To SelectedObjects.Count - 1

            'Finds index for group wanting to get values of
            EwEIndexFleet = GetIndexFleet(SelectedObjects(FleetIndex).ParentName)

            'Loop for each prey
            For Each Prey In SelectedObjects(FleetIndex).ChildNames
                EwEIndexPrey = GetIndexGroup(Prey)

                'Calculate proportion of catch is landings and discards _
                'for given fleet and group
                Landings = 0
                Discards = 0
                If EwEIndexFleet = 0 Then
                    For i = 1 To m_core.nFleets
                        Landings += m_core.FleetInputs(i).Landings(EwEIndexPrey)
                        Discards += m_core.FleetInputs(i).Discards(EwEIndexPrey)
                    Next
                Else
                    Landings = m_core.FleetInputs(EwEIndexFleet).Landings(EwEIndexPrey)
                    Discards = m_core.FleetInputs(EwEIndexFleet).Discards(EwEIndexPrey)
                End If
                PropLandings = Landings / (Landings + Discards)

                'Loop through EwE datastructure getting biomass for current group at each timestep
                If chkYearly.Checked Then
                    For nYear As Integer = 1 To m_core.nEcosimYears
                        For nMonth As Integer = 1 To 12
                            ACatchByFleet(ColPointer, nYear - 1) += m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, (nYear - 1) * 12 + nMonth)
                        Next
                        ALandingsByFleet(ColPointer, nYear - 1) = ACatchByFleet(ColPointer, nYear - 1) * PropLandings / 12
                    Next
                Else
                    For TimeStep As Integer = 1 To m_core.nEcosimTimeSteps
                        ACatchByFleet(ColPointer, TimeStep - 1) = m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, TimeStep)
                        ALandingsByFleet(ColPointer, TimeStep - 1) = ACatchByFleet(ColPointer, TimeStep - 1) * PropLandings
                    Next
                End If
                ColPointer += 1

            Next
        Next

        SendToFileTabbed(ALandingsByFleet, SelectedObjects, _
                FileName:=FishFleetsFileName, sheet:=sheet, _
                TabName:="LandingsPerFleetPerPrey", wb:=FisheriesWB)

    End Sub

    Private Sub CreateDiscardsByFleetCSV(ByVal sheet As Excel.Worksheet)
        Dim EwEIndexFleet As Integer 'Index of group in EwE datastructure
        Dim EwEIndexPrey As Integer
        Dim ColPointer As Integer = 0 'To track col in array to put data
        Dim ColTitles As String = Nothing 'Title of columns in .CSV file
        'Used to hold ratio to seperate catch into discards and landings (should sum to 1)
        Dim PropLandings As Single
        Dim PropDiscards As Single
        Dim Landings As Single
        Dim Discards As Single

        'Holds the array of data for all selected groups
        Dim ACatchByFleet(FleetPreySelection.CountSelectedChild - 1, nDataRows - 1) As Single
        Dim ALandingsByFleet(FleetPreySelection.CountSelectedChild - 1, nDataRows - 1) As Single
        Dim ADiscardsByFleet(FleetPreySelection.CountSelectedChild - 1, nDataRows - 1) As Single

        'Gets a list of names for the selected groups
        Dim SelectedObjects As List(Of cCreatedObjects) = FleetPreySelection.GetSelected

        'Loops for each group in selected
        For FleetIndex = 0 To SelectedObjects.Count - 1

            'Finds index for group wanting to get values of
            EwEIndexFleet = GetIndexFleet(SelectedObjects(FleetIndex).ParentName)

            'Loop for each prey
            For Each Prey In SelectedObjects(FleetIndex).ChildNames
                EwEIndexPrey = GetIndexGroup(Prey)

                'Calculate proportion of catch is landings and discards _
                'for given fleet and group
                Landings = 0
                Discards = 0
                If EwEIndexFleet = 0 Then
                    For i = 1 To m_core.nFleets
                        Landings += m_core.FleetInputs(i).Landings(EwEIndexPrey)
                        Discards += m_core.FleetInputs(i).Discards(EwEIndexPrey)
                    Next
                Else
                    Landings = m_core.FleetInputs(EwEIndexFleet).Landings(EwEIndexPrey)
                    Discards = m_core.FleetInputs(EwEIndexFleet).Discards(EwEIndexPrey)
                End If
                PropLandings = Landings / (Landings + Discards)
                PropDiscards = Discards / (Landings + Discards)

                'Loop through EwE datastructure getting discards for current group at each timestep
                If chkYearly.Checked Then
                    For nYear As Integer = 1 To m_core.nEcosimYears
                        For nMonth As Integer = 1 To 12
                            ACatchByFleet(ColPointer, nYear - 1) += m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, (nYear - 1) * 12 + nMonth)
                        Next
                        ADiscardsByFleet(ColPointer, nYear - 1) = ACatchByFleet(ColPointer, nYear - 1) * PropDiscards / 12
                    Next
                Else
                    For TimeStep As Integer = 1 To m_core.nEcosimTimeSteps
                        ACatchByFleet(ColPointer, TimeStep - 1) = m_core.EcoSimGroupOutputs(EwEIndexPrey).CatchByFleet(EwEIndexFleet, TimeStep)
                        ADiscardsByFleet(ColPointer, TimeStep - 1) = ACatchByFleet(ColPointer, TimeStep - 1) * PropDiscards
                    Next
                End If

                ColPointer += 1

            Next
        Next

        SendToFileTabbed(ADiscardsByFleet, SelectedObjects, _
                FileName:=FishFleetsFileName, sheet:=sheet, _
                TabName:="DiscardsPerFleetPerPrey", wb:=FisheriesWB)

    End Sub

    Private Sub CreateDietCSV(ByVal sheet As Excel.Worksheet, ByVal CurrentPredator As cCreatedObjects)
        'Holds the diet of each prey at each time step for given predator
        Dim ADietOfPredator(,) As Single
        Dim PreyNames As New StringBuilder()    'to create prey names for top .CSV file
        Dim PredatorIndexEcosim As Integer      'holds index in EwE m_core of Pred
        Dim PreyIndexEcosim As Integer          'holds index in EwE m_core of Prey

        'Runs only if prey>0
        If CurrentPredator.CountChild > 0 Then

            'Get Predator index in EcoSim
            PredatorIndexEcosim = GetIndexGroup(CurrentPredator.ParentName)

            'Dim array for holding consumption values for each predprey
            ADietOfPredator = Nothing
            ReDim ADietOfPredator(CurrentPredator.CountChild - 1, nDataRows - 1)

            For PreyIndex As Integer = 0 To CurrentPredator.CountChild - 1

                'Find PreyIndexEcosim in m_core.EcoSimGroupOutputs(PredatorIndexEcosim) for PreyIndex
                PreyIndexEcosim = GetIndexGroup(CurrentPredator.ChildNames(PreyIndex))

                'Calculate consumption values for each prey of each predator for each year
                If chkYearly.Checked Then
                    For nYear As Integer = 1 To m_core.nEcosimYears
                        For nMonth As Integer = 1 To 12
                            ADietOfPredator(PreyIndex, nYear - 1) += m_core.EcoSimGroupOutputs(PredatorIndexEcosim).PreyPercentage(PreyIndexEcosim, (nYear - 1) * 12 + nMonth)
                        Next
                        ADietOfPredator(PreyIndex, nYear - 1) /= 12
                    Next
                Else
                    For TimeStep As Integer = 1 To nDataRows
                        ADietOfPredator(PreyIndex, TimeStep - 1) = m_core.EcoSimGroupOutputs(PredatorIndexEcosim).PreyPercentage(PreyIndexEcosim, TimeStep)
                    Next
                End If

            Next

            SendToFileTabbed(ADietOfPredator, CurrentPredator.ChildNames, _
                TabName:="DietOf" & Mid(CurrentPredator.ParentName, 1, 24), _
                FileName:=FuncGroupsFileName, Sheet:=sheet, wb:=FunctGroupWB)

        End If


    End Sub

    'Creates .CSV for the value of each selected fleet at each timestep
    Private Sub CreateValueCSV(ByVal sheet As Excel.Worksheet)

        Dim EwEIndexFleet As Integer 'Index of group in EwE datastructure

        'Holds the array of data for all selected Fleets
        Dim AValue(FleetOnlySelection.CountSelected - 1, nDataRows - 1) As Single

        'Gets a list of names for the selected groups
        Dim SelectedNames As List(Of String) = FleetOnlySelection.SelectedNames

        'Loops for each group in selected
        For FleetIndex = 0 To SelectedNames.Count - 1

            'Finds index for group wanting to get biomass of
            EwEIndexFleet = GetIndexFleet(SelectedNames(FleetIndex))

            'Loop through EwE datastructure getting Value for current group at each timestep
            If chkYearly.Checked Then
                For nYear As Integer = 1 To m_core.nEcosimYears
                    For nMonth As Integer = 1 To 12
                        AValue(FleetIndex, nYear - 1) += m_core.EcosimFleetOutput(EwEIndexFleet).Value((nYear - 1) * 12 + nMonth)
                    Next
                    AValue(FleetIndex, nYear - 1) /= 12
                Next

            Else
                For TimeStep As Integer = 1 To nDataRows
                    AValue(FleetIndex, TimeStep - 1) = m_core.EcosimFleetOutput(EwEIndexFleet).Value(TimeStep)
                Next
            End If

        Next

        SendToFileTabbed(AValue, SelectedNames, TabName:="Values", _
            FileName:=FishFleetsFileName, Sheet:=sheet, wb:=FisheriesWB)

    End Sub

    Private Sub CreateBasicEstimatesCSV(ByVal sheet As Excel.Worksheet)

        'Setup tab name
        sheet.Name = CheckName("BasicEstimates", IndicatorsWB)

        'Setup titles
        sheet.Cells(1, 2) = "Group Name"
        sheet.Cells(1, 3) = "Trophic Level"
        sheet.Cells(1, 4) = "Habitat area(fraction)"
        sheet.Cells(1, 5) = "Biomass in habitat area(t/km^2)"
        sheet.Cells(1, 6) = "Biomass(t/km^2)"
        sheet.Cells(1, 7) = "Production/biomass(/year)"
        sheet.Cells(1, 8) = "Consumption/biomass(/year)"
        sheet.Cells(1, 9) = "Ecotrophic efficiency"
        sheet.Cells(1, 10) = "Production/Consumption"

        'Fill out core data
        For Row = 1 To m_core.nGroups
            sheet.Cells(1 + Row, 1) = m_core.EcoPathGroupOutputs(Row).Index
            sheet.Cells(1 + Row, 2) = m_core.EcoPathGroupOutputs(Row).Name
            sheet.Cells(1 + Row, 3) = m_core.EcoPathGroupOutputs(Row).TTLX
            sheet.Cells(1 + Row, 4) = m_core.EcoPathGroupOutputs(Row).Area
            sheet.Cells(1 + Row, 5) = m_core.EcoPathGroupOutputs(Row).BiomassArea
            sheet.Cells(1 + Row, 6) = m_core.EcoPathGroupOutputs(Row).Biomass
            sheet.Cells(1 + Row, 7) = m_core.EcoPathGroupOutputs(Row).PBOutput
            sheet.Cells(1 + Row, 8) = m_core.EcoPathGroupOutputs(Row).QBOutput
            sheet.Cells(1 + Row, 9) = m_core.EcoPathGroupOutputs(Row).EEOutput
            sheet.Cells(1 + Row, 10) = m_core.EcoPathGroupOutputs(Row).GEOutput
        Next

    End Sub

    Private Sub CreateKeyIndicesCSV(ByVal sheet As Excel.Worksheet)

        'Setup tab name
        sheet.Name = CheckName("KeyIndices", IndicatorsWB)

        'Setup titles
        sheet.Cells(1, 2) = "Group Name"
        sheet.Cells(1, 3) = "Biom. Accumul"
        sheet.Cells(1, 4) = "Biom. Acc. Rate"
        sheet.Cells(1, 5) = "Net Migration"
        sheet.Cells(1, 6) = "Flow to det."
        sheet.Cells(1, 7) = "Net Efficiency"
        sheet.Cells(1, 8) = "Omnivory Index"

        'Fill out main data
        For Row = 1 To m_core.nGroups
            sheet.Cells(1 + Row, 1) = m_core.EcoPathGroupOutputs(Row).Index
            sheet.Cells(1 + Row, 2) = m_core.EcoPathGroupOutputs(Row).Name
            sheet.Cells(1 + Row, 3) = m_core.EcoPathGroupOutputs(Row).BioAccum
            sheet.Cells(1 + Row, 4) = m_core.EcoPathGroupOutputs(Row).BioAccumRatePerYear
            sheet.Cells(1 + Row, 5) = m_core.EcoPathGroupOutputs(Row).NetMigration
            sheet.Cells(1 + Row, 6) = m_core.EcoPathGroupOutputs(Row).FlowToDet
            sheet.Cells(1 + Row, 7) = m_core.EcoPathGroupOutputs(Row).NetEfficiency
            sheet.Cells(1 + Row, 8) = m_core.EcoPathGroupOutputs(Row).OmnivoryIndex
        Next

    End Sub

    Private Sub CreateInitMortCoeffsCSV(ByVal sheet As Excel.Worksheet)

        'Setup tab name
        sheet.Name = CheckName("InitMortCoeffs", IndicatorsWB)

        'Setup titles
        sheet.Cells(1, 2) = "Group Name"
        sheet.Cells(1, 3) = "Prod/Biom or Z"
        sheet.Cells(1, 4) = "Fishing mort. rate"
        sheet.Cells(1, 5) = "Predat. mort. rate"
        sheet.Cells(1, 6) = "Biom. accum. rate"
        sheet.Cells(1, 7) = "Net Migration Rate"
        sheet.Cells(1, 8) = "Other mort. rate"
        sheet.Cells(1, 9) = "Fishing mort./Total mort"
        sheet.Cells(1, 10) = "Proportion nat. mort."

        For Row = 1 To m_core.nLivingGroups
            sheet.Cells(1 + Row, 1) = m_core.EcoPathGroupOutputs(Row).Index
            sheet.Cells(1 + Row, 2) = m_core.EcoPathGroupOutputs(Row).Name
            sheet.Cells(1 + Row, 3) = m_core.EcoPathGroupOutputs(Row).PBOutput
            sheet.Cells(1 + Row, 4) = m_core.EcoPathGroupOutputs(Row).MortCoFishRate
            sheet.Cells(1 + Row, 5) = m_core.EcoPathGroupOutputs(Row).MortCoPredMort
            sheet.Cells(1 + Row, 6) = m_core.EcoPathGroupOutputs(Row).BioAccumRatePerYear
            sheet.Cells(1 + Row, 7) = m_core.EcoPathGroupOutputs(Row).MortCoNetMig
            sheet.Cells(1 + Row, 8) = m_core.EcoPathGroupOutputs(Row).MortCoOtherMort
            sheet.Cells(1 + Row, 9) = m_core.EcoPathGroupOutputs(Row).FishMortPerTotMort
            sheet.Cells(1 + Row, 10) = m_core.EcoPathGroupOutputs(Row).NatMortPerTotMort
        Next

    End Sub

    Private Sub CreateInitPredMortCSV(ByVal sheet As Excel.Worksheet)

        Dim ColPoint As Integer
        Dim Pred As cCoreGroupBase
        Dim PredIndex(m_core.nGroups) As Integer

        'Setup tab name
        sheet.Name = CheckName("InitPredMortality", IndicatorsWB)

        'Write column headings
        sheet.Cells(1, 2) = "Prey\Predator"
        ColPoint = 3
        For x = 1 To m_core.nGroups
            Pred = m_core.EcoSimGroupOutputs(x)
            If Pred.PP < 1 Then
                sheet.Cells(1, ColPoint) = x
                PredIndex(ColPoint - 3) = x
                ColPoint += 1
            End If
        Next

        'Write row titles
        For y = 1 To m_core.nLivingGroups
            sheet.Cells(y + 1, 1) = m_core.EcoSimGroupOutputs(y).Index
            sheet.Cells(y + 1, 2) = m_core.EcoSimGroupOutputs(y).Name
        Next

        'Fill out consumption values
        For x = 3 To ColPoint - 1
            For y = 1 To m_core.nLivingGroups
                sheet.Cells(y + 1, x) = m_core.EcoPathGroupOutputs(y).PredMort(PredIndex(x - 3))
            Next
        Next

    End Sub

    Private Sub CreateInitFishingMortCSV(ByVal sheet As Excel.Worksheet)
        Dim slandings As Single
        Dim sDiscards As Single
        Dim sBiomass As Single

        'Setup tab name
        sheet.Name = CheckName("InitFishingMortality", IndicatorsWB)

        'Fill column titles row
        sheet.Cells(1, 2) = "Fleet\Group"
        For x = 1 To m_core.nFleets
            sheet.Cells(1, 2 + x) = m_core.FleetInputs(x).Name
        Next

        'Fill main data
        For y = 1 To m_core.nLivingGroups
            sheet.Cells(1 + y, 1) = m_core.EcoPathGroupOutputs(y).Index
            sheet.Cells(1 + y, 2) = m_core.EcoPathGroupOutputs(y).Name
            For x = 1 To m_core.nFleets
                slandings = m_core.FleetInputs(x).Landings(y)
                sDiscards = m_core.FleetInputs(x).Discards(y)
                sBiomass = m_core.EcoPathGroupOutputs(y).Biomass
                If sBiomass > 0 Then
                    sheet.Cells(1 + y, 2 + x) = (slandings + sDiscards) / sBiomass
                Else
                    sheet.Cells(1 + y, 2 + x) = 0
                End If
            Next
        Next

    End Sub

    Private Sub CreateInitConsumptionCSV(ByVal sheet As Excel.Worksheet)

        Dim ColPoint As Integer
        Dim Pred As cCoreGroupBase
        Dim TotalConsumption As Single
        Dim PredIndex(m_core.nGroups) As Integer

        'Setup tab name
        sheet.Name = CheckName("InitConsumption", IndicatorsWB)

        'Write column headings
        sheet.Cells(1, 2) = "Prey\Predator"
        ColPoint = 3
        For x = 1 To m_core.nGroups
            Pred = m_core.EcoSimGroupOutputs(x)
            If Pred.PP < 1 Or Pred.PP = 2 Then
                sheet.Cells(1, ColPoint) = x
                PredIndex(ColPoint - 3) = x
                ColPoint += 1
            End If
        Next

        'Write row headings
        For y = 1 To m_core.nGroups
            sheet.Cells(y + 1, 1) = m_core.EcoSimGroupOutputs(y).Index
            sheet.Cells(y + 1, 2) = m_core.EcoSimGroupOutputs(y).Name
        Next
        'Add Import row
        sheet.Cells(m_core.nGroups + 2, 1) = m_core.nGroups + 1
        sheet.Cells(m_core.nGroups + 2, 2) = "Import"
        'Add Sum row
        sheet.Cells(m_core.nGroups + 3, 1) = m_core.nGroups + 2
        sheet.Cells(m_core.nGroups + 3, 2) = "Sum"

        'Fill out consumption values
        For x = 3 To ColPoint - 1
            TotalConsumption = 0
            For y = 1 To m_core.nGroups
                sheet.Cells(y + 1, x) = m_core.EcoPathGroupOutputs(y).Consumption(PredIndex(x - 3))
                TotalConsumption += m_core.EcoPathGroupOutputs(y).Consumption(PredIndex(x - 3))
            Next
            sheet.Cells(m_core.nGroups + 2, x) = m_core.EcoPathGroupOutputs(PredIndex(x - 3)).ImportedConsumption
            TotalConsumption += m_core.EcoPathGroupOutputs(PredIndex(x - 3)).ImportedConsumption
            sheet.Cells(m_core.nGroups + 3, x) = TotalConsumption
        Next

    End Sub

    Private Sub CreateRespirationCSV(ByVal sheet As Excel.Worksheet)

        'Setup tab name
        sheet.Name = CheckName("Respiration", IndicatorsWB)

        'Set up titles
        sheet.Cells(1, 2) = "Group Name"
        sheet.Cells(1, 3) = "Respiration(t/km^2/year)"
        sheet.Cells(1, 4) = "Assimilation(t/hm^2/year)"
        sheet.Cells(1, 5) = "Respiration/assimilation"
        sheet.Cells(1, 6) = "Production/respiration"
        sheet.Cells(1, 7) = "Respiration/biomass(/year)"

        For Row = 1 To m_core.nGroups
            sheet.Cells(1 + Row, 1) = m_core.EcoPathGroupOutputs(Row).Index
            sheet.Cells(1 + Row, 2) = m_core.EcoPathGroupOutputs(Row).Name
            sheet.Cells(1 + Row, 3) = m_core.EcoPathGroupOutputs(Row).Respiration
            sheet.Cells(1 + Row, 4) = m_core.EcoPathGroupOutputs(Row).Assimilation
            sheet.Cells(1 + Row, 5) = m_core.EcoPathGroupOutputs(Row).RespAssim
            sheet.Cells(1 + Row, 6) = m_core.EcoPathGroupOutputs(Row).ProdResp
            sheet.Cells(1 + Row, 7) = m_core.EcoPathGroupOutputs(Row).RespBiom
        Next

    End Sub

    Private Sub CreateOverlapPreyCSV(ByVal sheet As Excel.Worksheet)

        'Setup tab name
        sheet.Name = CheckName("OverlapPrey", IndicatorsWB)

        'Write column headings
        sheet.Cells(1, 2) = "Group Name"
        For x = 1 To m_core.nLivingGroups
            sheet.Cells(1, 2 + x) = CStr(x)
        Next

        'Write body of data
        For Row = 1 To m_core.nLivingGroups
            sheet.Cells(1 + Row, 1) = m_core.EcoPathGroupOutputs(Row).Index
            sheet.Cells(1 + Row, 2) = m_core.EcoPathGroupOutputs(Row).Name
            For Col = 1 To Row
                sheet.Cells(1 + Row, 2 + Col) = m_core.EcoPathGroupOutputs(Row).Plap(Col)
            Next
        Next

    End Sub

    Private Sub CreateOverlapPredCSV(ByVal sheet As Excel.Worksheet)

        'Setup tab name
        sheet.Name = CheckName("OverlapPredator", IndicatorsWB)

        'Write column headings
        sheet.Cells(1, 2) = "Group Name"
        For x = 1 To m_core.nLivingGroups
            sheet.Cells(1, 2 + x) = CStr(x)
        Next

        'Write body of data
        For Row = 1 To m_core.nLivingGroups
            sheet.Cells(1 + Row, 1) = m_core.EcoPathGroupOutputs(Row).Index
            sheet.Cells(1 + Row, 2) = m_core.EcoPathGroupOutputs(Row).Name
            For Col = 1 To Row
                sheet.Cells(1 + Row, 2 + Col) = m_core.EcoPathGroupOutputs(Row).Hlap(Col)
            Next
        Next

    End Sub

    Private Sub CreateElectivityCSV(ByVal sheet As Excel.Worksheet)
        Dim ColPoint As Integer

        'Setup tab name
        sheet.Name = CheckName("Electivity", IndicatorsWB)

        'Write column headings
        sheet.Cells(1, 2) = "Prey\Predator"
        ColPoint = 1
        For x = 1 To m_core.nGroups
            If m_core.EcoPathGroupOutputs(x).PP < 1 Then
                sheet.Cells(1, 2 + ColPoint) = m_core.EcoPathGroupOutputs(x).Index
                ColPoint += 1
            End If
        Next

        'Write body of data
        For Row = 1 To m_core.nGroups
            sheet.Cells(1 + Row, 1) = m_core.EcoPathGroupOutputs(Row).Index
            sheet.Cells(1 + Row, 2) = m_core.EcoPathGroupOutputs(Row).Name
            For Col = 1 To m_core.nGroups
                If m_core.EcoPathGroupOutputs(Col).PP < 1 Then
                    sheet.Cells(1 + Row, 2 + Col) = m_core.EcoPathGroupOutputs(Col).Alpha(Row)
                End If
            Next
        Next

    End Sub

    Private Sub CreateInitFishingQuantitiesCSV(ByVal sheet As Excel.Worksheet)

        Dim TotalCatchGroup As Single
        Dim TotalCatchFleet(m_core.nFleets - 1) As Single
        Dim TotalTotalCatch As Single = 0
        Dim TTCatch As Single = 0
        Dim RowVals(m_core.nFleets - 1) As Single
        Dim RowPoint As Integer = 2
        Dim sourceGrpIntput As cCoreInputOutputBase = Nothing
        Dim sourceGrpIntputSec As cCoreInputOutputBase = Nothing
        Dim sourceGrpOutput As cCoreInputOutputBase = Nothing
        Dim propLandings As Single
        Dim propDiscards As Single
        Dim Quantities As Single
        Dim propTTLX As Single
        Dim QuantitiesTTLX As Single
        Dim FleetQuantities As Single
        Dim FleetQuantitiesTTLX As Single
        Dim AllQuantities As Single = 0
        Dim AllQuantitiesTTLX As Single = 0

        'Set tab title
        sheet.Name = "InitFishingQuantities"

        'Write column headings
        sheet.Cells(1, 2) = "Group Name"
        For x = 1 To m_core.nFleets
            sheet.Cells(1, 2 + x) = m_core.FleetInputs(x).Name
        Next
        sheet.Cells(1, 3 + m_core.nFleets) = "Total catch"

        'Write body of data
        For xGroup = 1 To m_core.nGroups
            TotalCatchGroup = 0
            For Col = 1 To m_core.nFleets
                RowVals(Col - 1) = m_core.FleetInputs(Col).Landings(xGroup) + m_core.FleetInputs(Col).Discards(xGroup)
                TotalCatchGroup += m_core.FleetInputs(Col).Landings(xGroup) + m_core.FleetInputs(Col).Discards(xGroup)
                TotalCatchFleet(Col - 1) += m_core.FleetInputs(Col).Landings(xGroup) + m_core.FleetInputs(Col).Discards(xGroup)
            Next
            If TotalCatchGroup > 0 Then
                sheet.Cells(RowPoint, 1) = m_core.EcoPathGroupOutputs(xGroup).Index
                sheet.Cells(RowPoint, 2) = m_core.EcoPathGroupOutputs(xGroup).Name
                For Col = 0 To m_core.nFleets - 1
                    sheet.Cells(RowPoint, 3 + Col) = RowVals(Col)
                Next
                sheet.Cells(RowPoint, 3 + m_core.nFleets) = TotalCatchGroup
                RowPoint += 1
            End If

        Next

        'Write the total line on the bottom
        sheet.Cells(RowPoint, 2) = "Total catch"
        For Col = 0 To m_core.nFleets - 1
            sheet.Cells(RowPoint, 3 + Col) = TotalCatchFleet(Col)
            TTCatch += TotalCatchFleet(Col)
        Next
        sheet.Cells(RowPoint, 3 + m_core.nFleets) = TTCatch
        RowPoint += 1

        'Write the trophic level line at bottom
        sheet.Cells(RowPoint, 2) = "Trophic Level"

        For fleetIndex As Integer = 1 To m_core.nFleets

            FleetQuantities = 0
            FleetQuantitiesTTLX = 0

            For GrpIndex As Integer = 1 To m_core.nGroups

                'Reset for each row
                Quantities = 0
                QuantitiesTTLX = 0

                'Calculate Quantity for each group
                propLandings = m_core.FleetInputs(fleetIndex).Landings(GrpIndex)
                propDiscards = m_core.FleetInputs(fleetIndex).Discards(GrpIndex)
                Quantities = (propLandings + propDiscards)

                'Get trophic level of group and multiply by quanity
                propTTLX = m_core.EcoPathGroupOutputs(GrpIndex).TTLX
                QuantitiesTTLX = Quantities * propTTLX

                'Keep running total of quanities and quantities*TTLX for each column
                FleetQuantities += Quantities
                FleetQuantitiesTTLX += QuantitiesTTLX

            Next

            sheet.Cells(RowPoint, 2 + fleetIndex) = FleetQuantitiesTTLX / FleetQuantities
            AllQuantities += FleetQuantities
            AllQuantitiesTTLX += FleetQuantitiesTTLX

        Next

        sheet.Cells(RowPoint, 3 + m_core.nFleets) = AllQuantitiesTTLX / AllQuantities

    End Sub

    Private Sub CreateInitFishingValuesCSV(ByVal sheet As Excel.Worksheet)
        Dim y As Integer = 1

        'Setup tab name
        sheet.Name = CheckName("InitFishingValues", IndicatorsWB)

        Dim ValueFleetGroup As Single
        Dim SumFixedCPUESailCost As Single

        Dim MarketValueSum As Single
        Dim NonMarketValueSum As Single
        Dim TotalValueSum As Single

        Dim TotalValueFleet(m_core.nFleets) As Single
        Dim TotalCostFleet(m_core.nFleets) As Single
        Dim TotalProfitFleet As Single

        'Write column headings for fleets
        sheet.Cells(y, 2) = "Group Names"
        For x = 1 To m_core.nFleets
            sheet.Cells(y, 2 + x) = m_core.FleetInputs(x).Name
        Next

        sheet.Cells(y, 3 + m_core.nFleets) = "Catch Value"
        sheet.Cells(y, 4 + m_core.nFleets) = "Non-market value(" & m_core.EwEModel.UnitMonetary.ToString & ")"
        sheet.Cells(y, 5 + m_core.nFleets) = "Total Value(" & m_core.EwEModel.UnitMonetary.ToString & ")"

        'Write body of data
        For Row = 1 To m_core.nGroups
            y += 1

            'Write Group Name
            sheet.Cells(y, 1) = m_core.EcoPathGroupOutputs(Row).Index
            sheet.Cells(y, 2) = m_core.EcoPathGroupOutputs(Row).Name

            'Reset totals(last 3 columns) to zero for start of each row
            MarketValueSum = 0
            NonMarketValueSum = 0
            TotalValueSum = 0

            For Col = 1 To m_core.nFleets
                ValueFleetGroup = m_core.FleetInputs(Col).Landings(Row) * m_core.FleetInputs(Col).OffVesselPrice(Row)
                sheet.Cells(y, 2 + Col) = ValueFleetGroup
                MarketValueSum += ValueFleetGroup
                TotalValueFleet(Col) += ValueFleetGroup
            Next

            'Calculate the sum for all fleets of the Non-market value
            NonMarketValueSum = m_core.EcoPathGroupInputs(Row).NonMarketValue * _
                m_core.EcoPathGroupOutputs(m_core.EcoPathGroupInputs(Row).Index).Biomass
            'Calculate the value total value for all fleets
            TotalValueSum = MarketValueSum + NonMarketValueSum

            'Fill last three columns of row
            sheet.Cells(y, 3 + m_core.nFleets) = MarketValueSum
            sheet.Cells(y, 4 + m_core.nFleets) = NonMarketValueSum
            sheet.Cells(y, 5 + m_core.nFleets) = TotalValueSum

        Next

        y += 1

        'Output total value for each fleet
        sheet.Cells(y, 2) = "Total Value(" & m_core.EwEModel.UnitMonetary.ToString & ")"
        MarketValueSum = 0
        For col = 1 To m_core.nFleets
            sheet.Cells(y, 2 + col) = TotalValueFleet(col)
            MarketValueSum += TotalValueFleet(col)
        Next
        sheet.Cells(y, 3 + m_core.nFleets) = MarketValueSum

        y += 1

        'Output total cost for each fleet
        sheet.Cells(y, 2) = "Total Cost(" & m_core.EwEModel.UnitMonetary.ToString & ")"
        MarketValueSum = 0
        For Col = 1 To m_core.nFleets
            SumFixedCPUESailCost = m_core.FleetInputs(Col).FixedCost + _
                                    m_core.FleetInputs(Col).CPUECost + _
                                    m_core.FleetInputs(Col).SailCost
            TotalCostFleet(Col) = SumFixedCPUESailCost * TotalValueFleet(Col) * CSng(0.01)
            MarketValueSum += TotalCostFleet(Col)
            sheet.Cells(y, 2 + Col) = TotalCostFleet(Col)
        Next
        sheet.Cells(y, 3 + m_core.nFleets) = MarketValueSum

        y += 1

        'Output profit row
        sheet.Cells(y, 2) = "Total Profit(" & m_core.EwEModel.UnitMonetary.ToString & ")"
        MarketValueSum = 0
        For Col = 1 To m_core.nFleets
            TotalProfitFleet = TotalValueFleet(Col) - TotalCostFleet(Col)
            MarketValueSum += TotalProfitFleet
            sheet.Cells(y, 2 + Col) = TotalProfitFleet
        Next
        sheet.Cells(y, 3 + m_core.nFleets) = MarketValueSum

    End Sub

    Private Sub CreateSearchRatesCSV(ByVal sheet As Excel.Worksheet)

        Dim ColPointer As Integer = 1

        'Setup tab name
        sheet.Name = CheckName("SearchRates", IndicatorsWB)

        'Write column headings
        sheet.Cells(1, 2) = "Prey \ predator"
        For x = 1 To m_core.nGroups
            If m_core.EcoPathGroupOutputs(x).PP < 1 Then
                sheet.Cells(1, 2 + ColPointer) = m_core.EcoPathGroupOutputs(x).Index
                ColPointer += 1
            End If
        Next

        'Write body of data
        For Row = 1 To m_core.nGroups
            ColPointer = 1
            sheet.Cells(1 + Row, 1) = m_core.EcoPathGroupOutputs(Row).Index
            sheet.Cells(1 + Row, 2) = m_core.EcoPathGroupOutputs(Row).Name
            For x = 1 To m_core.nGroups
                If m_core.EcoPathGroupOutputs(x).PP < 1 Then
                    sheet.Cells(1 + Row, 2 + ColPointer) = m_core.EcoPathGroupOutputs(Row).SearchRate(x)
                    ColPointer += 1
                End If
            Next
        Next

    End Sub

    Private Sub SetSaveResultsState()

        btnSaveResults.Enabled = False

        If ParentOnlySelection.CountSelected > 0 Then

            If chkBiomass.Checked Or chkBiomassInteg.Checked Or _
            chkPredationMortality.Checked Or chkFishingMortality.Checked Or _
            chkCatch.Checked Then
                btnSaveResults.Enabled = True
            End If

        ElseIf PredatorPreySelection.CountSelectedChild > 0 Then

            If chkConsumption.Checked Or chkDietProportions.Checked Then
                btnSaveResults.Enabled = True
            End If

        ElseIf PreyPredatorSelection.CountSelectedChild > 0 Then

            If chkPredationPerPredator.Checked Then
                btnSaveResults.Enabled = True
            End If

        ElseIf FleetPreySelection.CountSelectedChild > 0 Then

            If chkFishMortFleetToPrey.Checked Or chkCatchFleet.Checked Then
                btnSaveResults.Enabled = True
            End If

        ElseIf FleetOnlySelection.CountSelected > 0 Then

            If chkFleetValue.Checked Or chkEffort.Checked Then
                btnSaveResults.Enabled = True
            End If

        ElseIf chkBasicEstimates.Checked Or chkKeyIndices.Checked Or _
        chkMortalityCoefficients.Checked Or chkInitPredMort.Checked Or chkInitFishMort.Checked Or _
        chkInitConsumption.Checked Or chkRespiration.Checked Or _
        chkPreyOverlap.Checked Or chkPredOverlap.Checked Or _
        chkElectivity.Checked Or chkSearchRates.Checked Or _
        chkInitFishingQuantities.Checked Or chkInitFishingValues.Checked Then

            btnSaveResults.Enabled = True

        End If

    End Sub

    'Additions need making to this sub!!!
    Private Sub ResetForm()

        'Set all checkboxes to unchecked
        Me.chkBiomass.Checked = False
        Me.chkBiomassInteg.Checked = False
        Me.chkFishingMortality.Checked = False
        Me.chkPredationMortality.Checked = False
        Me.chkCatch.Checked = False
        Me.chkConsumption.Checked = False
        Me.chkDietProportions.Checked = False
        Me.chkPredationPerPredator.Checked = False
        Me.chkFishMortFleetToPrey.Checked = False
        Me.chkEffort.Checked = False
        Me.chkCatchFleet.Checked = False
        Me.chkFleetValue.Checked = False
        Me.chkBasicEstimates.Checked = False
        Me.chkKeyIndices.Checked = False
        Me.chkMortalityCoefficients.Checked = False
        Me.chkInitPredMort.Checked = False
        Me.chkInitFishMort.Checked = False
        Me.chkInitConsumption.Checked = False
        Me.chkRespiration.Checked = False
        Me.chkPreyOverlap.Checked = False
        Me.chkPredOverlap.Checked = False
        Me.chkElectivity.Checked = False
        Me.chkSearchRates.Checked = False
        Me.chkInitFishingQuantities.Checked = False
        Me.chkInitFishingValues.Checked = False

    End Sub

    Public Sub SendToFileTabbed(ByVal data(,) As Single, ByVal GroupNames As List(Of cCreatedObjects), _
                      ByVal TabName As String, ByVal FileName As String, _
                      ByVal sheet As Excel.Worksheet, ByVal wb As Excel.Workbook)

        sheet.Name = CheckName(TabName, wb)

        Dim simYears As Integer = CInt(m_core.nEcosimTimeSteps / cCore.N_MONTHS)
        Dim nGroups As Integer = data.GetLength(0) - 1
        Dim sum(nGroups) As Single

        Dim x As Integer = 1, y As Integer = 1 'Hold coordinates of cell underfocus

        'Create Super Headings
        For Each SuperGroup In GroupNames
            sheet.Cells(y, x) = SuperGroup.ParentName
            x += SuperGroup.CountChild
        Next

        'Move down start of next line
        x = 1
        y += 1
        For Each SuperGroup In GroupNames
            For Each SubGroup In SuperGroup.ChildNames
                sheet.Cells(y, x) = SubGroup
                x += 1
            Next
        Next

        For j As Integer = 0 To data.GetLength(1) - 1
            For i As Integer = 0 To nGroups
                sheet.Cells(j + y + 1, i + 1) = data(i, j)
            Next
        Next


    End Sub

    Public Sub SendToFileTabbed(ByVal data(,) As Single, ByVal strGroupNames As List(Of String), _
                          ByVal TabName As String, ByVal FileName As String, _
                          ByVal Sheet As Excel.Worksheet, ByVal wb As Excel.Workbook)

        Sheet.Name = CheckName(TabName, wb)

        Dim nGroups As Integer = data.GetLength(0) - 1

        For i = 0 To strGroupNames.Count - 1
            Sheet.Cells(1, i + 1) = strGroupNames(i)
        Next

        For j As Integer = 0 To data.GetLength(1) - 1
            For i As Integer = 0 To nGroups
                Sheet.Cells(j + 2, i + 1) = data(i, j)
            Next
        Next

    End Sub

    Public Sub ValidateObjectCreated()

        If ParentOnlySelection.SelectedNames.Count = 0 Then
            chkBiomass.Checked = False
            chkBiomassInteg.Checked = False
            chkFishingMortality.Checked = False
            chkPredationMortality.Checked = False
            chkCatch.Checked = False
            btnSetParentOnly.Enabled = False
        Else
            btnSetParentOnly.Enabled = True
        End If

        If PredatorPreySelection.CountSelectedChild = 0 Then
            chkConsumption.Checked = False
            chkDietProportions.Checked = False
            btnSetPredPrey.Enabled = False
        Else
            btnSetPredPrey.Enabled = True
        End If

        If PreyPredatorSelection.CountSelectedChild = 0 Then
            chkPredationPerPredator.Checked = False
            btnSetPreyPred.Enabled = False
        Else
            btnSetPreyPred.Enabled = True
        End If

        If FleetPreySelection.CountSelectedChild = 0 Then
            chkFishMortFleetToPrey.Checked = False
            chkCatchFleet.Checked = False
            btnSetFleetPrey.Enabled = False
        Else
            btnSetFleetPrey.Enabled = True
        End If

        If FleetOnlySelection.CountSelected = 0 Then
            chkFleetValue.Checked = False
            btnSetFleetOnly.Enabled = False
        Else
            btnSetFleetOnly.Enabled = True
        End If

        SetSaveResultsState()

    End Sub

    Public Sub DeleteObjects()

        If chkBiomass.Checked = False And chkBiomassInteg.Checked = False And _
            chkFishingMortality.Checked = False And chkPredationMortality.Checked = False And _
            chkCatch.Checked = False Then
            ParentOnlySelection.RemoveAll()
            btnSetParentOnly.Enabled = False
        End If

        If chkFishMortFleetToPrey.Checked = False And chkCatchFleet.Checked = False Then
            FleetPreySelection.RemoveAll()
            btnSetFleetPrey.Enabled = False
        End If

        If chkConsumption.Checked = False And chkDietProportions.Checked = False Then
            PredatorPreySelection.RemoveAll()
            btnSetPredPrey.Enabled = False
        End If

        If chkPredationPerPredator.Checked = False Then
            PreyPredatorSelection.RemoveAll()
            btnSetPreyPred.Enabled = False
        End If

        If chkFleetValue.Checked = False Then
            FleetOnlySelection.RemoveAll()
            btnSetFleetOnly.Enabled = False
        End If

    End Sub

    ' Subs that are executed in sequence when key-run is clicked
#Region "KeyRun"

    Private Sub PredatorPreyStage()

        'Check if previous selection performed correctly...
        If ParentOnlySelection.CountSelected = 0 Then
            If MsgBoxResult.Cancel = MsgBox("Your entry on the previous form was invalid. " & vbCrLf & _
                   "Would you like to retry your entry or cancel the " & vbCrLf & _
                    "process?", MsgBoxStyle.RetryCancel, "Invalid Selection") Then
                FireChecked = True
                Exit Sub
            End If
            btnAllOptions.PerformClick()
            Exit Sub
        End If

        '...and if they have tick all the relevant checkboxes
        chkBiomass.Checked = True
        chkBiomassInteg.Checked = True
        chkFishingMortality.Checked = True
        chkPredationMortality.Checked = True
        chkCatch.Checked = True

        'set delegate to next stage and load next form
        NextAction = AddressOf Me.PreyPredStage
        Dim a As New frmSelectPredatorPrey(PredatorPreySelection, m_core)
        a.Show()
        AddHandler a.FormExited, AddressOf ValidateObjectCreated

    End Sub

    Private Sub PreyPredStage()

        'Check if previous selection performed correctly...
        If PredatorPreySelection.CountSelectedChild = 0 Then
            If MsgBoxResult.Cancel = MsgBox("Your entry on the previous form was invalid. " & vbCrLf & _
       "Would you like to retry your entry or cancel the " & vbCrLf & _
        "process?", MsgBoxStyle.RetryCancel, "Invalid Selection") Then
                FireChecked = True
                Exit Sub
            End If
            PredatorPreyStage()
            Exit Sub
        End If

        '...and if they have tick all the relevant checkboxes
        chkDietProportions.Checked = True
        chkConsumption.Checked = True

        'set delegate to next stage and load next form
        NextAction = AddressOf Me.FleetPreyStage
        Dim a As New frmSelectPreyPredator(PreyPredatorSelection, m_core)
        a.Show()
        AddHandler a.FormExited, AddressOf ValidateObjectCreated


    End Sub

    Private Sub FleetPreyStage()

        'Check if previous selection performed correctly...
        If PreyPredatorSelection.CountSelectedChild = 0 Then
            If MsgBoxResult.Cancel = MsgBox("Your entry on the previous form was invalid. " & vbCrLf & _
       "Would you like to retry your entry or cancel the " & vbCrLf & _
        "process?", MsgBoxStyle.RetryCancel, "Invalid Selection") Then
                FireChecked = True
                Exit Sub
            End If
            PreyPredStage()
            Exit Sub
        End If

        '...and if they have tick all the relevant checkboxes
        chkPredationPerPredator.Checked = True

        'set delegate to next stage and load next form
        NextAction = AddressOf Me.FleetOnlyStage
        Dim a As New frmSelectFleetPrey(FleetPreySelection, m_core)
        a.Show()
        AddHandler a.FormExited, AddressOf ValidateObjectCreated


    End Sub

    Private Sub FleetOnlyStage()

        'Check if previous selection performed correctly...
        If FleetPreySelection.CountSelectedChild = 0 Then
            If MsgBoxResult.Cancel = MsgBox("Your entry on the previous form was invalid. " & vbCrLf & _
       "Would you like to retry your entry or cancel the " & vbCrLf & _
        "process?", MsgBoxStyle.RetryCancel, "Invalid Selection") Then
                FireChecked = True
                Exit Sub
            End If
            FleetPreyStage()
            Exit Sub
        End If

        '...and if they have tick all the relevant checkboxes
        chkFishMortFleetToPrey.Checked = True
        chkCatchFleet.Checked = True

        'set delegate to next stage and load next form
        NextAction = AddressOf Me.EcoPathValuesStage
        Dim a As New frmSelectFleetOnly(FleetOnlySelection, m_core)
        a.Show()
        AddHandler a.FormExited, AddressOf ValidateObjectCreated

    End Sub

    Private Sub EcoPathValuesStage()

        'Check if previous selection performed correctly...
        If FleetOnlySelection.CountSelected = 0 Then
            If MsgBoxResult.Cancel = MsgBox("Your entry on the previous form was invalid. " & vbCrLf & _
       "Would you like to retry your entry or cancel the " & vbCrLf & _
        "process?", MsgBoxStyle.RetryCancel, "Invalid Selection") Then
                FireChecked = True
                Exit Sub
            End If
            FleetOnlyStage()
            Exit Sub
        End If

        '...and if they have tick all the relevant checkboxes
        chkFleetValue.Checked = True
        chkEffort.Checked = True

        chkBasicEstimates.Checked = True
        chkKeyIndices.Checked = True
        chkMortalityCoefficients.Checked = True
        chkInitPredMort.Checked = True
        chkInitConsumption.Checked = True
        chkRespiration.Checked = True
        chkPreyOverlap.Checked = True
        chkPredOverlap.Checked = True
        chkElectivity.Checked = True
        chkSearchRates.Checked = True
        chkInitFishingQuantities.Checked = True
        chkInitFishingValues.Checked = True
        chkInitFishMort.Checked = True
        FireChecked = True

    End Sub

#End Region

#End Region


End Class

'Public Sub SendToFile(ByVal data(,) As Single, ByVal strGroupNames As String, _
'                      ByVal strPath As String, ByVal strFileName As String, _
'                      ByVal Yearly As Boolean)

'    Dim simYears As Integer = CInt(m_core.nEcosimTimeSteps / cCore.N_MONTHS)
'    Dim nGroups As Integer = data.GetLength(0) - 1
'    Dim sum(nGroups) As Single

'    'Overwritten the file
'    Dim sw As StreamWriter = New StreamWriter(strPath & "\" & strFileName, False)
'    sw.WriteLine(strGroupNames)


'    If Yearly = True Then
'        For j As Integer = 0 To simYears - 1
'            ReDim sum(nGroups)
'            For i As Integer = 0 To nGroups
'                For k As Integer = 1 To cCore.N_MONTHS
'                    sum(i) = sum(i) + data(i, j * cCore.N_MONTHS + k - 1)
'                Next
'                sw.Write(sum(i) / cCore.N_MONTHS)
'                sw.Write(",")
'            Next
'            sw.WriteLine()
'        Next
'    Else
'        For j As Integer = 0 To data.GetLength(1) - 1
'            For i As Integer = 0 To nGroups
'                sw.Write(data(i, j))
'                sw.Write(",")
'            Next
'            sw.WriteLine()
'        Next
'    End If

'    sw.Close()

'End Sub

'Network analysis attempt

'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
'    Dim NetAnalyWrapper As New cAccessNetAnaly(Me.m_core)
'    Dim PPTransfer As Single = NetAnalyWrapper.GetPPTransferEff(1)
'    MsgBox(PPTransfer * 100)
'End Sub