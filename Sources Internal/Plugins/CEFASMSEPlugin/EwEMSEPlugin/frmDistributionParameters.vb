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

    ' ToDo_JS: use Sourcegrid grid instead, add QuickEditHandler

    Enum eParameterSet
        Ecopath
        Ecosim
    End Enum

    Enum eParamName
        B
        BA
        PB
        QB
        EE
        DenDepCatchability
        SwitchingPower
        QBMaxxQBio
        PredEffectFeedingTime
        OtherMortFeedingTime
        MaxRelFeedingTime
        FeedingTimeAdjustRate

    End Enum

    Private Structure EcopathParam
        'This holds one item in the list of any Ecopath parameters
        'Later in the code the entire list is grouped into a list of EcopathParam
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
        'Similar to EcopathParam this holds one item in the list of any Ecosim parameters
        Public GroupNo As Integer
        Public GroupName As String
        Public DistributionType As Integer
        Public LowerBound As Double
        Public UpperBound As Double
        Public MidPoint As Double
        Public Sub New(ByVal GroupNumber As Integer, ByVal GroupName As String, ByVal DistributionType As Integer, ByVal LowerBound As Double, ByVal UpperBound As Double, ByVal MidPoint As Double)
            Me.GroupNo = GroupNumber
            Me.GroupName = GroupName
            Me.DistributionType = DistributionType
            Me.LowerBound = LowerBound
            Me.UpperBound = UpperBound
            Me.MidPoint = MidPoint
        End Sub
    End Structure

    Private DataPath As String = ""
    Private mUIC As cUIContext = Nothing
    Private mCore As cCore = Nothing
    Private m_MSEPlugin As cMSE = Nothing

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

    Private TDistributionType As Integer
    Private TMidPoint As Double

    Private nPPers As Integer
    Private BackColorOfDistableDataViewColumns As Color = Color.LightGray
    Private EditsUnsaved As Boolean

    Public Sub Init(ByVal uic As cUIContext, ByVal Plugin As cMSE)

        Me.m_MSEPlugin = Plugin
        Me.mUIC = uic
        mCore = uic.Core

        DataPath = cMSEUtils.MSEFolder(Me.m_MSEPlugin.DataPath, cMSEUtils.eMSEPaths.DistrParams)

        'These are lists that contain all the Ecopath parameters for all functional groups 
        B = New List(Of EcopathParam)
        BA = New List(Of EcopathParam)
        QB = New List(Of EcopathParam)
        PB = New List(Of EcopathParam)
        EE = New List(Of EcopathParam)

        'These are lists that contain all the Ecosim parameters for all functional groups 
        DenDepCatchability = New List(Of EcosimParam)
        SwitchingPower = New List(Of EcosimParam)
        QBMaxxQBio = New List(Of EcosimParam)
        PredEffectFeedingTime = New List(Of EcosimParam)
        OtherMortFeedingTime = New List(Of EcosimParam)
        MaxRelFeedingTime = New List(Of EcosimParam)
        FeedingTimeAdjustRate = New List(Of EcosimParam)

        For i As Integer = 1 To mCore.nGroups
            If mCore.EcoPathGroupInputs(i).IsProducer Then nPPers += 1
        Next

    End Sub

    Private Function ExtractEcosimParam(ByVal csv As CsvReader) As EcosimParam
        'given a Ecosim csv object this extracts the data from the current line and uses it to return an EcosimParam structure object

        csv.ReadNextRecord()
        TGroupName = csv(0)
        TGroupNumber = Convert.ToInt16(csv(1))
        TDistributionType = CInt(csv(2))
        TLowerBound = Convert.ToDouble(csv(3))
        TUpperBound = Convert.ToDouble(csv(4))
        TMidPoint = Convert.ToDouble(csv(5))

        Return New EcosimParam(TGroupNumber, TGroupName, TDistributionType, TLowerBound, TUpperBound, TMidPoint)

    End Function

    Private Function ExtractEcopathParam(ByVal csv As CsvReader, ByVal ParameterType As eParamName) As EcopathParam
        'Extracts distribution parameters for one group from csv and Ecopath

        csv.ReadNextRecord()
        TGroupNumber = Convert.ToInt16(csv(0))
        TGroupName = csv(1)
        TCV = Convert.ToDouble(csv(2))
        TLowerBound = Convert.ToDouble(csv(3))
        TUpperBound = Convert.ToDouble(csv(4))

        If ParameterType = eParamName.B Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).BiomassAreaInput
        ElseIf ParameterType = eParamName.BA Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).BioAccum
        ElseIf ParameterType = eParamName.QB Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).QBInput
        ElseIf ParameterType = eParamName.PB Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).PBInput
        ElseIf ParameterType = eParamName.EE Then
            TMean = mCore.EcoPathGroupInputs(Convert.ToInt16(csv(0))).EEInput
        End If

        Return New EcopathParam(TGroupNumber, TGroupName, TMean, TCV, TLowerBound, TUpperBound)

    End Function

    Private Function LoadEcosimParamX(ByRef ParamList As List(Of EcosimParam), ByVal Path As String, ByVal ParamName As eParamName) As Boolean

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim bSuccess As Boolean = True

        If File.Exists(Path) Then

            reader = cMSEUtils.GetReader(Path)
            If (reader Is Nothing) Then Return False

            Try
                csv = New CsvReader(reader, True)
                For igrp = 1 To mCore.nLivingGroups
                    ParamList.Add(ExtractEcosimParam(csv))
                Next
                csv.Dispose()
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".LoadEcopathParameters() Exception: " & ex.Message)
                bSuccess = False
            End Try
            cMSEUtils.ReleaseReader(reader)

        Else
            For igrp = 1 To mCore.nGroups
                If mCore.EcoPathGroupOutputs(igrp).IsLiving Then
                    If ParamName = eParamName.DenDepCatchability Then
                        TMean = mCore.EcoSimGroupInputs(igrp).DenDepCatchability
                    ElseIf ParamName = eParamName.FeedingTimeAdjustRate Then
                        TMean = mCore.EcoSimGroupInputs(igrp).FeedingTimeAdjustRate
                    ElseIf ParamName = eParamName.MaxRelFeedingTime Then
                        TMean = mCore.EcoSimGroupInputs(igrp).MaxRelFeedingTime
                    ElseIf ParamName = eParamName.OtherMortFeedingTime Then
                        TMean = mCore.EcoSimGroupInputs(igrp).OtherMortFeedingTime
                    ElseIf ParamName = eParamName.PredEffectFeedingTime Then
                        TMean = mCore.EcoSimGroupInputs(igrp).PredEffectFeedingTime
                    ElseIf ParamName = eParamName.QBMaxxQBio Then
                        TMean = mCore.EcoSimGroupInputs(igrp).QBMaxQBio
                    ElseIf ParamName = eParamName.SwitchingPower Then
                        TMean = mCore.EcoSimGroupInputs(igrp).SwitchingPower
                    End If
                    ParamList.Add(New EcosimParam(igrp, mCore.EcoPathGroupInputs(igrp).Name, 2, TMean * (1 - 0.1), TMean * (1 + 0.1), TMean))
                End If

            Next

        End If
        Return bSuccess

    End Function

    Private Function LoadEcosimParameters(ByVal Folder As String) As Boolean
        'loads all the ecosim csv files up and creates instances of lists of structures that hold it all in memory

        Dim bOK As Boolean = LoadEcosimParamX(DenDepCatchability, Path.Combine(Folder, "DenDepCatchability.csv"), eParamName.DenDepCatchability) And _
            LoadEcosimParamX(SwitchingPower, Path.Combine(Folder, "SwitchingPower.csv"), eParamName.SwitchingPower) And _
            LoadEcosimParamX(QBMaxxQBio, Path.Combine(Folder, "QBMaxxQBio.csv"), eParamName.QBMaxxQBio) And _
            LoadEcosimParamX(PredEffectFeedingTime, Path.Combine(Folder, "PredEffectFeedingTime.csv"), eParamName.PredEffectFeedingTime) And _
            LoadEcosimParamX(OtherMortFeedingTime, Path.Combine(Folder, "OtherMortFeedingTime.csv"), eParamName.OtherMortFeedingTime) And _
            LoadEcosimParamX(MaxRelFeedingTime, Path.Combine(Folder, "MaxRelFeedingTime.csv"), eParamName.MaxRelFeedingTime) And _
            LoadEcosimParamX(FeedingTimeAdjustRate, Path.Combine(Folder, "FeedingTimeAdjustRate.csv"), eParamName.FeedingTimeAdjustRate)

        Return bOK

    End Function

    Private Sub LoadEcopathParamX(ByRef ParamList As List(Of EcopathParam), ByVal Path As String, ByVal ParamName As eParamName)
        Dim csv As CsvReader
        Dim MonteCarlo As cMonteCarloManager = mCore.EcosimMonteCarlo
        Dim MCGroup As cMonteCarloGroup

        ' ToDo_JS: use safe readers

        If File.Exists(Path) Then

            Try
                csv = New CsvReader(New StreamReader(Path), True)

                For igrp = 1 To mCore.nLivingGroups
                    ParamList.Add(ExtractEcopathParam(csv, ParamName))
                Next

                csv.Dispose()

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".LoadEcopathParameters() Exception: " & ex.Message)
            End Try

        Else
            For igrp = 1 To mCore.nGroups
                If mCore.EcoPathGroupOutputs(igrp).IsLiving Then
                    MCGroup = MonteCarlo.Groups(igrp)
                    If ParamName = eParamName.B Then
                        TMean = mCore.EcoPathGroupOutputs(igrp).Biomass
                        TCV = MCGroup.Bcv
                        TLowerBound = MCGroup.BLower
                        TUpperBound = MCGroup.BUpper
                    ElseIf ParamName = eParamName.BA Then
                        TMean = mCore.EcoPathGroupOutputs(igrp).BioAccum
                        TCV = MCGroup.BAcv
                        TLowerBound = MCGroup.BALower
                        TUpperBound = MCGroup.BAUpper
                    ElseIf ParamName = eParamName.EE Then
                        TMean = mCore.EcoPathGroupOutputs(igrp).EEOutput
                        TCV = MCGroup.EEcv
                        TLowerBound = MCGroup.EELower
                        TUpperBound = MCGroup.EEUpper
                    ElseIf ParamName = eParamName.PB Then
                        TMean = mCore.EcoPathGroupOutputs(igrp).PBOutput
                        TCV = MCGroup.PBcv
                        TLowerBound = MCGroup.PBLower
                        TUpperBound = MCGroup.PBUpper
                    ElseIf ParamName = eParamName.QB Then
                        TMean = mCore.EcoPathGroupOutputs(igrp).QBOutput
                        TCV = MCGroup.QBcv
                        TLowerBound = MCGroup.QBLower
                        TUpperBound = MCGroup.QBUpper
                    End If
                    ParamList.Add(New EcopathParam(igrp, mCore.EcoPathGroupInputs(igrp).Name, TMean, TCV, TLowerBound, TUpperBound))
                End If

            Next

        End If
    End Sub

    Private Function LoadEcopathParameters(ByVal Path As String) As Boolean

        ' ToDo_JS: use safe paths

        'If File.Exists(Path & "/B_Dist.csv") Then
        LoadEcopathParamX(B, Path & "/B_Dist.csv", eParamName.B)
        'End If
        'If File.Exists(Path & "/PB_Dist.csv") Then
        LoadEcopathParamX(PB, Path & "/PB_Dist.csv", eParamName.PB)
        'End If
        'If File.Exists(Path & "/QB_Dist.csv") Then
        LoadEcopathParamX(QB, Path & "/QB_Dist.csv", eParamName.QB)
        'End If
        'If File.Exists(Path & "/EE_Dist.csv") Then
        LoadEcopathParamX(EE, Path & "/EE_Dist.csv", eParamName.EE)
        'End If
        'If File.Exists(Path & "/BA_Dist.csv") Then
        LoadEcopathParamX(BA, Path & "/BA_Dist.csv", eParamName.BA)
        'End If
        Return True

        'loads all the ecopath csv's up and saves all the data to lists of structures

        'Try
        '    Dim csv_B, csv_PB, csv_QB, csv_EE, csv_BA As CsvReader

        '    csv_B = New CsvReader(New StreamReader(Path & "/B_Dist.csv"), True)
        '    csv_PB = New CsvReader(New StreamReader(Path & "/PB_Dist.csv"), True)
        '    csv_QB = New CsvReader(New StreamReader(Path & "/QB_Dist.csv"), True)
        '    csv_EE = New CsvReader(New StreamReader(Path & "/EE_Dist.csv"), True)
        '    csv_BA = New CsvReader(New StreamReader(Path & "/BA_Dist.csv"), True)

        '    For igrp = 1 To mCore.nLivingGroups

        '        'xgrp = 1

        '        B.Add(ExtractEcopathParam(csv_B, eParamName.B))
        '        PB.Add(ExtractEcopathParam(csv_PB, eParamName.PB))
        '        QB.Add(ExtractEcopathParam(csv_QB, eParamName.QB))
        '        EE.Add(ExtractEcopathParam(csv_EE, eParamName.EE))
        '        BA.Add(ExtractEcopathParam(csv_BA, eParamName.BA))

        '    Next '========================================================================================================================================================

        '    'reset the connection to the csv files ready to be read from the beginning again
        '    csv_B.Dispose()
        '    csv_BA.Dispose()
        '    csv_EE.Dispose()
        '    csv_PB.Dispose()
        '    csv_QB.Dispose()

        '    Return True

        'Catch ex As Exception
        '    Debug.Assert(False, Me.ToString & ".LoadEcopathParameters() Exception: " & ex.Message)
        'End Try

        'Return False
    End Function

    Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)

        ' ToDo_JS: globalize this method

        If EditsUnsaved = True Then
            Dim resultmessage As DialogResult = MessageBox.Show("You have made changes to the data in this form without saving. Are you sure you still want to close it?", _
                                                                    "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)
            If resultmessage = Windows.Forms.DialogResult.Cancel Then e.Cancel = True
        End If
        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        ' JS 30Sep13: globalized

        MyBase.OnLoad(e)

        If LoadEcopathParameters(DataPath) = False Then
            Me.m_MSEPlugin.SendMessage(My.Resources.ERROR_DISTRPAR_LOAD_ECOPATH, eMessageImportance.Warning)
        End If

        If LoadEcosimParameters(DataPath) = False Then
            Me.m_MSEPlugin.SendMessage(My.Resources.ERROR_DISTRPAR_LOAD_ECOSIM, eMessageImportance.Warning)
        End If

        'initialises the dropdown box to the Ecopath parameters
        cboPathOrSim.SelectedIndex = eParameterSet.Ecopath

        Me.CenterToParent()

    End Sub

    Private Sub ChangeGridtoEcopath()

        ' ToDo_JS: globalize this method

        'Modifies the grid to show ecopath parameters
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

        ' ToDo_JS: globalize this method

        'Modifies the grid to display Ecosim parameters
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
        'Everytime the user changes the parameter type combobox from Ecopath Parameters to Ecosim Parameters and vice versa 
        'this gets called to change all the options in the combobox used to specify the parameter name

        ' ToDo_JS: globalize this method

        If cboPathOrSim.SelectedIndex = eParameterSet.Ecopath Then
            ChangeGridtoEcopath()
            cboParamName.Items.Clear()
            cboParamName.Items.Add("Biomass")
            cboParamName.Items.Add("Biomass Accumulation")
            cboParamName.Items.Add("Consumption/Biomass")
            cboParamName.Items.Add("Production/Biomass")
            cboParamName.Items.Add("Ecotrophic Efficiency")
            cboParamName.SelectedIndex = 0
        ElseIf cboPathOrSim.SelectedIndex = eParameterSet.Ecosim Then
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

        ' ToDo_JS: globalize this method

        'Whenever the user specifies a given parameter this fills the grid with its values
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

        ' JS 30Sep13: speed up rendering
        dgvParameters.SuspendLayout()
        Try
            If cboPathOrSim.SelectedIndex = eParameterSet.Ecopath Then
                For Each iParameter In CType(Parameters, List(Of EcopathParam))
                    dgvParameters.Rows.Add(iParameter.mGroupNo, iParameter.mGroupName, iParameter.mMean, iParameter.mCV, iParameter.mLowerBound, iParameter.mUpperBound)
                Next
            ElseIf cboPathOrSim.SelectedIndex = eParameterSet.Ecosim Then
                For Each iParameter In CType(Parameters, List(Of EcosimParam))
                    dgvParameters.Rows.Add(iParameter.GroupNo, iParameter.GroupName, iParameter.DistributionType, iParameter.LowerBound, iParameter.UpperBound, iParameter.MidPoint)
                Next
            End If
        Catch ex As Exception

        End Try
        dgvParameters.ResumeLayout()

    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnCancel.Click
        Me.Close()
    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnOK.Click

        'Saves all the parameters to csv when user clicks to save

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

        EditsUnsaved = False

        Me.Close()

    End Sub

    Private Sub SaveEcoSimParameters2CSV(ByRef EcosimParams As List(Of EcosimParam), ByRef FileName As String)

        ' ToDo_JS: use proper path concatenation

        Dim sw As StreamWriter = New StreamWriter(DataPath & "\" & FileName & ".csv", False)

        sw.WriteLine("GroupName, GroupNumber, DistributionType, Lower, Upper, Mid")

        For Each Param In EcosimParams
            sw.WriteLine("""" & Param.GroupName & """," & Param.GroupNo & ",""" & Param.DistributionType & """," & Param.LowerBound & "," & Param.UpperBound & "," & Param.MidPoint)
        Next

        sw.Dispose()

    End Sub

    Private Sub SaveEcopathParameters2CSV(ByRef EcopathParams As List(Of EcopathParam), ByRef FileName As String)

        ' ToDo_JS: use proper path concatenation
        Dim sw As StreamWriter = New StreamWriter(DataPath & "\" & FileName & "_Dist.csv", False)

        sw.WriteLine("Group Number, Name, CV, Lower Bound, Upper Bound")

        For Each Param In EcopathParams
            sw.WriteLine(Param.mGroupNo & ",""" & Param.mGroupName & """," & Param.mCV & "," & Param.mLowerBound & "," & Param.mUpperBound)
        Next

        sw.Dispose()

    End Sub

    Private Sub SetValueParameter(ByVal ParamSet As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        'This saves the modifies the underlying structure with the value of the cell in the grid that has been modified

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
                        iEcosimParam.DistributionType = CType(dgvParameters.CurrentCell.Value.ToString, Integer)
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

        ' ToDo_JS: globalize this method

        EditsUnsaved = True

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