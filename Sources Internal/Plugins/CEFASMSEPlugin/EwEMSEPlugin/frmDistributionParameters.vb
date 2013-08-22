#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Windows.Forms
Imports ZedGraph
Imports ScientificInterfaceShared.Controls
Imports SourceGrid2
Imports System.IO
Imports LumenWorks.Framework.IO.Csv

#End Region ' Imports

Public Class frmDistributionParameters

    Enum ParameterSet
        Ecopath
        Ecosim
    End Enum

    Private Structure EcopathParam
        Public mGroupNo As Integer
        Public mGroupName As String
        Public mMean As Double
        Public mCV As Double
        Public mLowerBound As Double
        Public mUpperBound As Double
        Public Sub New(ByVal GroupNumber As Integer, ByVal GroupName As String, ByVal Mean As Single, ByVal CV As Double, ByVal LowerBound As Double, ByVal UpperBound As Double)
            Me.mGroupNo = GroupNumber
            Me.mGroupName = GroupName
            Me.mMean = Mean
            Me.mCV = CV
            Me.mLowerBound = LowerBound
            Me.mUpperBound = UpperBound
        End Sub

        Public Property CV() As Double
            Get
                Return mCV
            End Get
            Set(ByVal value As Double)
                mCV = value
            End Set
        End Property

        Public Property LowerBound() As Double
            Get
                Return mLowerBound
            End Get
            Set(ByVal value As Double)
                mLowerBound = value
            End Set
        End Property

        Public Property UpperBound() As Double
            Get
                Return mUpperBound
            End Get
            Set(ByVal value As Double)
                mUpperBound = value
            End Set
        End Property




    End Structure

    Private Structure EcosimParam
        Public GroupNo As Integer
        Public GroupName As String
        Public DistributionType As String
        Public LowerBound As Double
        Public UpperBound As Double
        Public MidPoint As Double
        Public Sub New(ByVal GroupNumber As Integer, ByVal GroupName As String, ByVal DistributionType As String, ByVal LowerBound As Double, ByVal UpperBound As Double, ByVal MidPoint As Double)
            Me.GroupNo = GroupNumber
            Me.GroupName = GroupName
            Me.DistributionType = DistributionType
            Me.LowerBound = LowerBound
            Me.UpperBound = UpperBound
            Me.MidPoint = MidPoint
        End Sub
    End Structure

    Private DataPath As String
    Private mCore As cCore
    Private m_MSEPlugin As cMSE

    Private B As List(Of EcopathParam)
    Private BA As List(Of EcopathParam)
    Private QB As List(Of EcopathParam)
    Private PB As List(Of EcopathParam)
    Private EE As List(Of EcopathParam)
    Private TGroupNumber As Integer
    Private TGroupName As String
    Private TMean As Single
    Private TCV As Double
    Private TLowerBound As Double
    Private TUpperBound As Double

    Private DenDepCatchability As List(Of EcosimParam)
    Private SwitchingPower As List(Of EcosimParam)
    Private QBMaxxQBio As List(Of EcosimParam)
    Private PredEffectFeedingTime As List(Of EcosimParam)
    Private OtherMortFeedingTime As List(Of EcosimParam)
    Private MaxRelFeedingTime As List(Of EcosimParam)
    Private FeedingTimeAdjustRate As List(Of EcosimParam)

    Private TDistributionType As String
    Private TMidPoint As Double

    Private BackColorOfDistableDataViewColumns As Color = Color.LightGray

    Public Sub Init(ByVal UI As cUIContext, ByVal Plugin As cMSE, ByVal PathToData As String, ByRef Core As cCore)

        Me.m_MSEPlugin = Plugin
        DataPath = PathToData & "\DistributionParameters"
        mCore = Core
        B = New List(Of EcopathParam)
        BA = New List(Of EcopathParam)
        QB = New List(Of EcopathParam)
        PB = New List(Of EcopathParam)
        EE = New List(Of EcopathParam)

        DenDepCatchability = New List(Of EcosimParam)
        SwitchingPower = New List(Of EcosimParam)
        QBMaxxQBio = New List(Of EcosimParam)
        PredEffectFeedingTime = New List(Of EcosimParam)
        OtherMortFeedingTime = New List(Of EcosimParam)
        MaxRelFeedingTime = New List(Of EcosimParam)
        FeedingTimeAdjustRate = New List(Of EcosimParam)

    End Sub

    Private Function ExtractEcosimParam(ByVal csv As CsvReader) As EcosimParam

        csv.ReadNextRecord()
        TGroupName = csv(0)
        TGroupNumber = Convert.ToInt16(csv(1))
        TDistributionType = csv(2)
        TLowerBound = Convert.ToDouble(csv(3))
        TUpperBound = Convert.ToDouble(csv(4))
        TMidPoint = Convert.ToDouble(csv(5))

        Return New EcosimParam(TGroupNumber, TGroupName, TDistributionType, TLowerBound, TUpperBound, TMidPoint)

    End Function

    Private Function ExtractEcopathParam(ByVal csv As CsvReader, ByVal ParameterType As String) As EcopathParam
        'Extracts distribution parameters for one group from csv and Ecopath

        csv.ReadNextRecord()
        TGroupNumber = Convert.ToInt16(csv(0))
        TGroupName = csv(1)
        TCV = Convert.ToDouble(csv(2))
        TLowerBound = Convert.ToDouble(csv(3))
        TUpperBound = Convert.ToDouble(csv(4))

        If ParameterType = "B" Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).BiomassAreaInput
        ElseIf ParameterType = "BA" Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).BioAccum
        ElseIf ParameterType = "QB" Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).QBInput
        ElseIf ParameterType = "PB" Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).PBInput
        ElseIf ParameterType = "EE" Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).EEInput
        End If

        Return New EcopathParam(TGroupNumber, TGroupName, TMean, TCV, TLowerBound, TUpperBound)

    End Function

    Private Function LoadEcosimParameters(ByVal Path As String) As Boolean
        Try
            Dim csv_DenDepCatchability, csv_SwitchingPower, csv_QBMaxxQBio, csv_PredEffectFeedingTime, _
                csv_OtherMortFeedingTime, csv_MaxRelFeedingTime, csv_FeedingTimeAdjustRate As CsvReader

            csv_DenDepCatchability = New CsvReader(New StreamReader(Path & "/DenDepCatchability.csv"), True)
            csv_SwitchingPower = New CsvReader(New StreamReader(Path & "/SwitchingPower.csv"), True)
            csv_QBMaxxQBio = New CsvReader(New StreamReader(Path & "/QBMaxxQBio.csv"), True)
            csv_PredEffectFeedingTime = New CsvReader(New StreamReader(Path & "/PredEffectFeedingTime.csv"), True)
            csv_OtherMortFeedingTime = New CsvReader(New StreamReader(Path & "/OtherMortFeedingTime.csv"), True)
            csv_MaxRelFeedingTime = New CsvReader(New StreamReader(Path & "/MaxRelFeedingTime.csv"), True)
            csv_FeedingTimeAdjustRate = New CsvReader(New StreamReader(Path & "/FeedingTimeAdjustRate.csv"), True)

            For igrp = 1 To mCore.nLivingGroups
                DenDepCatchability.Add(ExtractEcosimParam(csv_DenDepCatchability))
                SwitchingPower.Add(ExtractEcosimParam(csv_SwitchingPower))
                QBMaxxQBio.Add(ExtractEcosimParam(csv_QBMaxxQBio))
                PredEffectFeedingTime.Add(ExtractEcosimParam(csv_PredEffectFeedingTime))
                OtherMortFeedingTime.Add(ExtractEcosimParam(csv_OtherMortFeedingTime))
                MaxRelFeedingTime.Add(ExtractEcosimParam(csv_MaxRelFeedingTime))
                FeedingTimeAdjustRate.Add(ExtractEcosimParam(csv_FeedingTimeAdjustRate))
            Next

            csv_DenDepCatchability.Dispose()
            csv_SwitchingPower.Dispose()
            csv_QBMaxxQBio.Dispose()
            csv_PredEffectFeedingTime.Dispose()
            csv_OtherMortFeedingTime.Dispose()
            csv_MaxRelFeedingTime.Dispose()
            csv_FeedingTimeAdjustRate.Dispose()

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".LoadEcosimParameters() Exception: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadEcopathParameters(ByVal Path As String) As Boolean
        Try
            Dim csv_B, csv_PB, csv_QB, csv_EE, csv_BA As CsvReader

            csv_B = New CsvReader(New StreamReader(Path & "/B_Dist.csv"), True)
            csv_PB = New CsvReader(New StreamReader(Path & "/PB_Dist.csv"), True)
            csv_QB = New CsvReader(New StreamReader(Path & "/QB_Dist.csv"), True)
            csv_EE = New CsvReader(New StreamReader(Path & "/EE_Dist.csv"), True)
            csv_BA = New CsvReader(New StreamReader(Path & "/BA_Dist.csv"), True)

            For igrp = 1 To mCore.nLivingGroups

                'xgrp = 1

                B.Add(ExtractEcopathParam(csv_B, "B"))
                PB.Add(ExtractEcopathParam(csv_PB, "PB"))
                QB.Add(ExtractEcopathParam(csv_QB, "QB"))
                EE.Add(ExtractEcopathParam(csv_EE, "EE"))
                BA.Add(ExtractEcopathParam(csv_BA, "BA"))

            Next '========================================================================================================================================================

            'reset the connection to the csv files ready to be read from the beginning again
            csv_B.Dispose()
            csv_BA.Dispose()
            csv_EE.Dispose()
            csv_PB.Dispose()
            csv_QB.Dispose()

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".LoadEcopathParameters() Exception: " & ex.Message)
        End Try

        Return False
    End Function

    Private Sub frmDistributionParameters_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Dim Path As String = DataPath & "\DistributionParameters"

        If LoadEcopathParameters(DataPath) = False Then
            MsgBox("There was a problem loading the Ecopath parameters from csv")
        End If

        If LoadEcosimParameters(DataPath) = False Then
            MsgBox("There was a problem loading the Ecosim parameters from csv")
        End If

        cboPathOrSim.SelectedIndex = ParameterSet.Ecopath

    End Sub

    Private Sub ChangeGridtoEcopath()
        dgvParameters.Columns.Clear()
        dgvParameters.Columns.Add("GroupNumber", "Group Number")
        dgvParameters.Columns.Add("GroupName", "Group Name")
        dgvParameters.Columns.Add("Mean", "Mean")
        dgvParameters.Columns.Add("CV", "CV")
        dgvParameters.Columns.Add("Lower", "Lower Boundary")
        dgvParameters.Columns.Add("Upper", "Upper Boundary")
        dgvParameters.Columns(0).ReadOnly = True
        dgvParameters.Columns(1).ReadOnly = True
        dgvParameters.Columns(2).ReadOnly = True
        dgvParameters.Columns(0).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns
        dgvParameters.Columns(1).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns
        dgvParameters.Columns(2).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns
    End Sub

    Private Sub ChangeGridtoEcosim()
        dgvParameters.Columns.Clear()
        dgvParameters.Columns.Add("GroupNumber", "Group Number")
        dgvParameters.Columns.Add("GroupName", "Group Name")
        dgvParameters.Columns.Add("DistributionType", "Distribution Type")
        dgvParameters.Columns.Add("LowerBoundary", "Lower Boundary")
        dgvParameters.Columns.Add("UpperBoundary", "Upper Boundary")
        dgvParameters.Columns.Add("MidPoint", "MidPoint")
        dgvParameters.Columns(0).ReadOnly = True
        dgvParameters.Columns(1).ReadOnly = True
        dgvParameters.Columns(0).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns
        dgvParameters.Columns(1).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns
    End Sub


    Private Sub cboPathOrSim_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPathOrSim.SelectedIndexChanged

        If cboPathOrSim.SelectedIndex = ParameterSet.Ecopath Then
            ChangeGridtoEcopath()
            cboParamName.Items.Clear()
            cboParamName.Items.Add("Biomass")
            cboParamName.Items.Add("Biomass Accumulation")
            cboParamName.Items.Add("Consumption/Biomass")
            cboParamName.Items.Add("Production/Biomass")
            cboParamName.Items.Add("Ecotrophic Efficiency")
            cboParamName.SelectedIndex = 0
        ElseIf cboPathOrSim.SelectedIndex = ParameterSet.Ecosim Then
            ChangeGridtoEcosim()
            cboParamName.Items.Clear()
            cboParamName.Items.Add("DenDepCatchability")
            cboParamName.Items.Add("Switching Power")
            cboParamName.Items.Add("QBMaxxQBio")
            cboParamName.Items.Add("PredEffectFeedingTime")
            cboParamName.Items.Add("OtherMortFeedingTime")
            cboParamName.Items.Add("MaxRelFeedingTime")
            cboParamName.Items.Add("FeedingTimeAdjustRate")
            cboParamName.SelectedIndex = 0


        End If

    End Sub


    Private Sub cboParamName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboParamName.SelectedIndexChanged
        dgvParameters.Rows.Clear()
        If cboParamName.Text = "Biomass" Then
            FillDataGrid(B)
        ElseIf cboParamName.Text = "Biomass Accumulation" Then
            FillDataGrid(BA)
        ElseIf cboParamName.Text = "Consumption/Biomass" Then
            FillDataGrid(QB)
        ElseIf cboParamName.Text = "Production/Biomass" Then
            FillDataGrid(PB)
        ElseIf cboParamName.Text = "Ecotrophic Efficiency" Then
            FillDataGrid(EE)
        ElseIf cboParamName.Text = "DenDepCatchability" Then
            FillDataGrid(DenDepCatchability)
        ElseIf cboParamName.Text = "Switching Power" Then
            FillDataGrid(SwitchingPower)
        ElseIf cboParamName.Text = "QBMaxxQBio" Then
            FillDataGrid(QBMaxxQBio)
        ElseIf cboParamName.Text = "PredEffectFeedingTime" Then
            FillDataGrid(PredEffectFeedingTime)
        ElseIf cboParamName.Text = "OtherMortFeedingTime" Then
            FillDataGrid(OtherMortFeedingTime)
        ElseIf cboParamName.Text = "MaxRelFeedingTime" Then
            FillDataGrid(MaxRelFeedingTime)
        ElseIf cboParamName.Text = "FeedingTimeAdjustRate" Then
            FillDataGrid(FeedingTimeAdjustRate)
        End If

    End Sub

    Private Sub FillDataGrid(ByVal Parameters As Object)
        If cboPathOrSim.SelectedIndex = ParameterSet.Ecopath Then
            For Each iParameter In CType(Parameters, List(Of EcopathParam))
                dgvParameters.Rows.Add(iParameter.mGroupNo, iParameter.mGroupName, iParameter.mMean, iParameter.mCV, iParameter.mLowerBound, iParameter.mUpperBound)
            Next
        ElseIf cboPathOrSim.SelectedIndex = ParameterSet.Ecosim Then
            For Each iParameter In CType(Parameters, List(Of EcosimParam))
                dgvParameters.Rows.Add(iParameter.GroupNo, iParameter.GroupName, iParameter.DistributionType, iParameter.LowerBound, iParameter.UpperBound, iParameter.MidPoint)
            Next
        End If

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub


    Private Sub btnSaveAndClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveAndClose.Click

        SaveEcopathParameters2CSV(B, "B")
        SaveEcopathParameters2CSV(BA, "BA")
        SaveEcopathParameters2CSV(PB, "PB")
        SaveEcopathParameters2CSV(QB, "QB")
        SaveEcopathParameters2CSV(EE, "EE")

        SaveEcoSimParameters2CSV(DenDepCatchability, "DenDepCatchability")
        SaveEcoSimParameters2CSV(SwitchingPower, "SwitchingPower")
        SaveEcoSimParameters2CSV(QBMaxxQBio, "QBMaxxQBio")
        SaveEcoSimParameters2CSV(PredEffectFeedingTime, "PredEffectFeedingTime")
        SaveEcoSimParameters2CSV(OtherMortFeedingTime, "OtherMortFeedingTime")
        SaveEcoSimParameters2CSV(MaxRelFeedingTime, "MaxRelFeedingTime")
        SaveEcoSimParameters2CSV(FeedingTimeAdjustRate, "FeedingTimeAdjustRate")

        Me.Close()

    End Sub

    Private Sub SaveEcoSimParameters2CSV(ByRef EcosimParams As List(Of EcosimParam), ByRef FileName As String)

        Dim sw As StreamWriter = New StreamWriter(DataPath & "\" & FileName & ".csv", False)

        sw.WriteLine("GroupName, GroupNumber, DistributionType, Lower, Upper, Mid")

        For Each Param In EcosimParams
            sw.WriteLine("""" & Param.GroupName & """," & Param.GroupNo & ",""" & Param.DistributionType & """," & Param.LowerBound & "," & Param.UpperBound & "," & Param.MidPoint)
        Next

        sw.Dispose()

    End Sub

    Private Sub SaveEcopathParameters2CSV(ByRef EcopathParams As List(Of EcopathParam), ByRef FileName As String)

        Dim sw As StreamWriter = New StreamWriter(DataPath & "\" & FileName & "_Dist.csv", False)

        sw.WriteLine("Group Number, Name, CV, Lower Bound, Upper Bound")

        For Each Param In EcopathParams
            sw.WriteLine(Param.mGroupNo & ",""" & Param.mGroupName & """," & Param.mCV & "," & Param.mLowerBound & "," & Param.mUpperBound)
        Next

        sw.Dispose()

    End Sub

    Private Sub SetValueParameter(ByVal ParamSet As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Dim EcopathParamSet As List(Of EcopathParam)
        Dim EcosimParamSet As List(Of EcosimParam)
        Dim iEcopathParam As EcopathParam
        Dim iEcosimParam As EcosimParam

        If ParamSet.GetType Is GetType(List(Of EcopathParam)) Then
            EcopathParamSet = CType(ParamSet, List(Of EcopathParam))
            For i As Integer = 0 To EcopathParamSet.Count - 1
                iEcopathParam = EcopathParamSet(i)
                If EcopathParamSet(i).mGroupNo = CType(dgvParameters.Rows(e.RowIndex).Cells(0).Value, Integer) Then
                    If e.ColumnIndex = 3 Then
                        iEcopathParam.CV = CType(dgvParameters.CurrentCell.Value.ToString, Double)
                    ElseIf e.ColumnIndex = 4 Then
                        iEcopathParam.mLowerBound = CType(dgvParameters.CurrentCell.Value.ToString, Double)
                    ElseIf e.ColumnIndex = 5 Then
                        iEcopathParam.mUpperBound = CType(dgvParameters.CurrentCell.Value.ToString, Double)
                    End If
                    EcopathParamSet(i) = iEcopathParam
                End If
            Next
        Else
            EcosimParamSet = CType(ParamSet, List(Of EcosimParam))
            For i As Integer = 0 To EcosimParamSet.Count - 1
                iEcosimParam = EcosimParamSet(i)
                If iEcosimParam.GroupNo = CType(dgvParameters.Rows(e.RowIndex).Cells(0).Value, Integer) Then
                    If e.ColumnIndex = 2 Then
                        iEcosimParam.DistributionType = dgvParameters.CurrentCell.Value.ToString
                    ElseIf e.ColumnIndex = 3 Then
                        iEcosimParam.LowerBound = CType(dgvParameters.CurrentCell.Value.ToString, Double)
                    ElseIf e.ColumnIndex = 4 Then
                        iEcosimParam.UpperBound = CType(dgvParameters.CurrentCell.Value.ToString, Double)
                    ElseIf e.ColumnIndex = 5 Then
                        iEcosimParam.MidPoint = CType(dgvParameters.CurrentCell.Value.ToString, Double)
                    End If
                    EcosimParamSet(i) = iEcosimParam
                End If
            Next
        End If

    End Sub

    Private Sub dgvParameters_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvParameters.CellEndEdit

        If cboParamName.Text = "Biomass" Then
            SetValueParameter(B, e)
        ElseIf cboParamName.Text = "Biomass Accumulation" Then
            SetValueParameter(BA, e)
        ElseIf cboParamName.Text = "Consumption/Biomass" Then
            SetValueParameter(QB, e)
        ElseIf cboParamName.Text = "Production/Biomass" Then
            SetValueParameter(PB, e)
        ElseIf cboParamName.Text = "Ecotrophic Efficiency" Then
            SetValueParameter(EE, e)
        ElseIf cboParamName.Text = "DenDepCatchability" Then
            SetValueParameter(DenDepCatchability, e)
        ElseIf cboParamName.Text = "Switching Power" Then
            SetValueParameter(SwitchingPower, e)
        ElseIf cboParamName.Text = "QBMaxxQBio" Then
            SetValueParameter(QBMaxxQBio, e)
        ElseIf cboParamName.Text = "PredEffectFeedingTime" Then
            SetValueParameter(PredEffectFeedingTime, e)
        ElseIf cboParamName.Text = "OtherMortFeedingTime" Then
            SetValueParameter(OtherMortFeedingTime, e)
        ElseIf cboParamName.Text = "MaxRelFeedingTime" Then
            SetValueParameter(MaxRelFeedingTime, e)
        ElseIf cboParamName.Text = "FeedingTimeAdjustRate" Then
            SetValueParameter(FeedingTimeAdjustRate, e)
        End If

    End Sub

End Class