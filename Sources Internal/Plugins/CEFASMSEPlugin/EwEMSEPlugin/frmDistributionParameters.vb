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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports LumenWorks.Framework.IO.Csv
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmDistributionParameters

    ' ToDo_JS: use Sourcegrid grid instead, add QuickEditHandler

    Private Enum eParameterSet As Byte
        Ecopath
        Ecosim
    End Enum

    Private Enum eParamName As Byte
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

    Private Class ParamComboItem
        Public Sub New(paramname As eParamName, text As String)
            Me.ParamName = paramname : Me.Text = text
        End Sub
        Public Property ParamName As eParamName
        Public Property Text As String
        Public Overrides Function ToString() As String
            Return Me.Text
        End Function
    End Class

    ''' <summary>
    ''' This holds one item in the list of any Ecopath parameters. 
    ''' Later in the code the entire list is grouped into a list of EcopathParam
    ''' </summary>
    Private Structure EcopathParam

        Public Sub New(ByVal GroupNumber As Integer, ByVal GroupName As String, ByVal Mean As Single, ByVal CV As Double, ByVal LowerBound As Double, ByVal UpperBound As Double)
            Me.GroupNo = GroupNumber
            Me.GroupName = GroupName
            Me.Mean = Mean
            Me.CV = CV
            Me.LowerBound = LowerBound
            Me.UpperBound = UpperBound
        End Sub

        Public Property CV() As Double
        Public Property LowerBound() As Double
        Public Property UpperBound() As Double
        Public Property GroupNo As Integer
        Public Property GroupName As String
        Public Property Mean As Double

    End Structure

    ''' <summary>
    ''' Similar to <see cref="EcopathParam"/>, this holds one item 
    ''' in the list of any Ecosim parameters
    ''' </summary>
    Private Structure EcosimParam

        Public Sub New(ByVal GroupNumber As Integer, ByVal GroupName As String, ByVal DistributionType As Integer, ByVal LowerBound As Double, ByVal UpperBound As Double, ByVal MidPoint As Double)
            Me.GroupNo = GroupNumber
            Me.GroupName = GroupName
            Me.DistributionType = DistributionType
            Me.LowerBound = LowerBound
            Me.UpperBound = UpperBound
            Me.MidPoint = MidPoint
        End Sub

        Public Property GroupNo As Integer
        Public Property GroupName As String
        Public Property DistributionType As Integer
        Public Property LowerBound As Double
        Public Property UpperBound As Double
        Public Property MidPoint As Double

    End Structure

    Private mUIC As cUIContext = Nothing
    Private mCore As cCore = Nothing
    Private m_MSEPlugin As cMSE = Nothing

    Private B As List(Of EcopathParam)
    Private BA As List(Of EcopathParam)
    Private QB As List(Of EcopathParam)
    Private PB As List(Of EcopathParam)
    Private EE As List(Of EcopathParam)

    Private DenDepCatchability As List(Of EcosimParam)
    Private SwitchingPower As List(Of EcosimParam)
    Private QBMaxxQBio As List(Of EcosimParam)
    Private PredEffectFeedingTime As List(Of EcosimParam)
    Private OtherMortFeedingTime As List(Of EcosimParam)
    Private MaxRelFeedingTime As List(Of EcosimParam)
    Private FeedingTimeAdjustRate As List(Of EcosimParam)

    Private nPPers As Integer
    Private BackColorOfDistableDataViewColumns As Color = Color.LightGray
    Private EditsUnsaved As Boolean

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Sub Init(ByVal uic As cUIContext, ByVal Plugin As cMSE)

        Me.m_MSEPlugin = Plugin
        Me.mUIC = uic
        mCore = uic.Core

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

    Private ReadOnly Property DataPath As String
        Get
            ' JS 01Oct13: Datapath made dynamic
            Return cMSEUtils.MSEFolder(Me.m_MSEPlugin.DataPath, cMSEUtils.eMSEPaths.DistrParams)
        End Get
    End Property

    Private Function ExtractEcosimParam(ByVal csv As CsvReader) As EcosimParam

        ' ToDo_JS: make fail-proof
        ' ToDo_JS: use fixed CDV field reading

        'given a Ecosim csv object this extracts the data from the current line and uses it to return an EcosimParam structure object
        Dim TGroupName As String = ""
        Dim TGroupNumber As Integer
        Dim TLowerBound As Double
        Dim TUpperBound As Double
        Dim TDistributionType As Integer
        Dim TMidPoint As Double

        Try
            If (csv IsNot Nothing) Then
                csv.ReadNextRecord()

                TGroupName = cMSEUtils.FromCSVField(csv(0))
                TGroupNumber = cStringUtils.ConvertToInteger(csv(1))
                TDistributionType = cStringUtils.ConvertToInteger(csv(2))
                TLowerBound = cStringUtils.ConvertToDouble(csv(3))
                TUpperBound = cStringUtils.ConvertToDouble(csv(4))
                TMidPoint = cStringUtils.ConvertToDouble(csv(5))

            End If
        Catch ex As Exception
            ' ToDo_JS: respond to error
        End Try

        Return New EcosimParam(TGroupNumber, TGroupName, TDistributionType, TLowerBound, TUpperBound, TMidPoint)

    End Function

    Private Function ExtractEcopathParam(ByVal csv As CsvReader, ByVal ParameterType As eParamName) As EcopathParam
        'Extracts distribution parameters for one group from csv and Ecopath

        Dim TGroupName As String = ""
        Dim TGroupNumber As Integer
        Dim TMean As Single
        Dim TCV As Double
        Dim TLowerBound As Double
        Dim TUpperBound As Double

        Try
            If (csv IsNot Nothing) Then
                csv.ReadNextRecord()
                If (csv.FieldCount >= 5) Then

                    TGroupNumber = cStringUtils.ConvertToInteger(csv(0))
                    TGroupName = cMSEUtils.FromCSVField(csv(1))
                    TCV = cStringUtils.ConvertToDouble(csv(2))
                    TLowerBound = cStringUtils.ConvertToDouble(csv(3))
                    TUpperBound = cStringUtils.ConvertToDouble(csv(4))

                    ' JS 02Oct2013: Need to validate group number
                    If TGroupNumber < 1 Or TGroupNumber >= Me.mCore.nGroups Then
                        ' ToDo:_JS: report error somehow
                        Return Nothing
                    End If

                    If ParameterType = eParamName.B Then
                        TMean = mCore.EcoPathGroupInputs(TGroupNumber).BiomassAreaInput
                    ElseIf ParameterType = eParamName.BA Then
                        TMean = mCore.EcoPathGroupInputs(TGroupNumber).BioAccum
                    ElseIf ParameterType = eParamName.QB Then
                        TMean = mCore.EcoPathGroupInputs(TGroupNumber).QBInput
                    ElseIf ParameterType = eParamName.PB Then
                        TMean = mCore.EcoPathGroupInputs(TGroupNumber).PBInput
                    ElseIf ParameterType = eParamName.EE Then
                        TMean = mCore.EcoPathGroupInputs(TGroupNumber).EEInput
                    End If
                End If
            End If

        Catch ex As Exception
            ' ToDo:_JS: report error somehow
        End Try

        Return New EcopathParam(TGroupNumber, TGroupName, TMean, TCV, TLowerBound, TUpperBound)

    End Function

    Private Function LoadEcosimParamX(ByRef ParamList As List(Of EcosimParam), ByVal Path As String, ByVal ParamName As eParamName) As Boolean

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim TMean As Single
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

        Return LoadEcosimParamX(DenDepCatchability, Path.Combine(Folder, "DenDepCatchability.csv"), eParamName.DenDepCatchability) And _
               LoadEcosimParamX(SwitchingPower, Path.Combine(Folder, "SwitchingPower.csv"), eParamName.SwitchingPower) And _
               LoadEcosimParamX(QBMaxxQBio, Path.Combine(Folder, "QBMaxxQBio.csv"), eParamName.QBMaxxQBio) And _
               LoadEcosimParamX(PredEffectFeedingTime, Path.Combine(Folder, "PredEffectFeedingTime.csv"), eParamName.PredEffectFeedingTime) And _
               LoadEcosimParamX(OtherMortFeedingTime, Path.Combine(Folder, "OtherMortFeedingTime.csv"), eParamName.OtherMortFeedingTime) And _
               LoadEcosimParamX(MaxRelFeedingTime, Path.Combine(Folder, "MaxRelFeedingTime.csv"), eParamName.MaxRelFeedingTime) And _
               LoadEcosimParamX(FeedingTimeAdjustRate, Path.Combine(Folder, "FeedingTimeAdjustRate.csv"), eParamName.FeedingTimeAdjustRate)

    End Function

    Private Function LoadEcopathParamX(ByRef ParamList As List(Of EcopathParam), ByVal Path As String, ByVal ParamName As eParamName) As Boolean

        Dim csv As CsvReader
        Dim MonteCarlo As cMonteCarloManager = mCore.EcosimMonteCarlo
        Dim MCGroup As cMonteCarloGroup
        Dim TMean As Single
        Dim TCV As Double
        Dim TLowerBound As Double
        Dim TUpperBound As Double

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

        Return True

    End Function

    Private Function LoadEcopathParameters(ByVal Folder As String) As Boolean

        ' ToDo_JS: used safe path concatenation
        Return LoadEcopathParamX(B, Path.Combine(Folder, "B_Dist.csv"), eParamName.B) And _
               LoadEcopathParamX(PB, Path.Combine(Folder, "PB_Dist.csv"), eParamName.PB) And _
               LoadEcopathParamX(QB, Path.Combine(Folder, "QB_Dist.csv"), eParamName.QB) And _
               LoadEcopathParamX(EE, Path.Combine(Folder, "EE_Dist.csv"), eParamName.EE) And _
               LoadEcopathParamX(BA, Path.Combine(Folder, "BA_Dist.csv"), eParamName.BA)

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

        If (Me.EditsUnsaved = True) Then
            ' JS 02Oct13: globalized this method
            ' JS 02Oct13: replaced MsgBox with cFeedbackMessage
            Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_UNSAVED_CHANGES, _
                                 eCoreComponentType.External, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            fmsg.Reply = eMessageReply.YES
            Me.mCore.Messages.SendMessage(fmsg)
            e.Cancel = (fmsg.Reply <> eMessageReply.YES)
        End If

        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        ' JS 30Sep13: globalized this method

        MyBase.OnLoad(e)

        If LoadEcopathParameters(DataPath) = False Then
            Me.m_MSEPlugin.SendMessage(My.Resources.ERROR_DISTRPAR_LOAD_ECOPATH, eMessageImportance.Warning)
        End If

        If LoadEcosimParameters(DataPath) = False Then
            Me.m_MSEPlugin.SendMessage(My.Resources.ERROR_DISTRPAR_LOAD_ECOSIM, eMessageImportance.Warning)
        End If

        'initialises the dropdown box to the Ecopath parameters
        cboPathOrSim.SelectedIndex = eParameterSet.Ecopath

        Me.EditsUnsaved = False

        Me.CenterToParent()

    End Sub

    ''' <summary>
    ''' Modifies the grid to show ecopath parameters
    ''' </summary>
    Private Sub ChangeGridtoEcopath()

        ' JS 02Oct13: preserve unsaved changes flag
        Dim bSave As Boolean = Me.EditsUnsaved

        ' JS 02Oct13: globalized this method
        dgvParameters.Columns.Clear()
        dgvParameters.Columns.Add("GroupNumber", SharedResources.HEADER_INDEX) ' "Group Number"
        dgvParameters.Columns.Add("GroupName", SharedResources.HEADER_NAME) ' "Group Name"
        dgvParameters.Columns.Add("Mean", SharedResources.HEADER_MEAN) ' "Mean"
        dgvParameters.Columns.Add("CV", SharedResources.HEADER_CV) ' "CV"
        dgvParameters.Columns.Add("Lower", My.Resources.HEADER_LOWERBOUND) ' "Lower Boundary"
        dgvParameters.Columns.Add("Upper", My.Resources.HEADER_UPPERBOUND) ' "Upper Boundary"
        dgvParameters.Columns(0).ReadOnly = True
        dgvParameters.Columns(1).ReadOnly = True
        dgvParameters.Columns(2).ReadOnly = True
        dgvParameters.Columns(0).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns
        dgvParameters.Columns(1).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns
        dgvParameters.Columns(2).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns

        Me.EditsUnsaved = bSave

    End Sub

    ''' <summary>
    ''' Modifies the grid to display Ecosim parameters
    ''' </summary>
    Private Sub ChangeGridtoEcosim()

        ' JS 02Oct13: preserve unsaved changes flag
        Dim bSave As Boolean = Me.EditsUnsaved

        ' JS 02Oct13: globalized this method
        dgvParameters.Columns.Clear()
        dgvParameters.Columns.Add("GroupNumber", SharedResources.HEADER_INDEX) ' "Group Number"
        dgvParameters.Columns.Add("GroupName", SharedResources.HEADER_NAME) ' "Group Name"
        dgvParameters.Columns.Add("DistributionType", My.Resources.HEADER_DISTRIBUTIONTYPE) ' "Distribution Type"
        dgvParameters.Columns.Add("LowerBoundary", My.Resources.HEADER_LOWERBOUND) ' "Lower Boundary"
        dgvParameters.Columns.Add("UpperBoundary", My.Resources.HEADER_UPPERBOUND) ' "Upper Boundary"
        dgvParameters.Columns.Add("MidPoint", My.Resources.HEADER_MIDPOINT) ' "MidPoint"
        dgvParameters.Columns(0).ReadOnly = True
        dgvParameters.Columns(1).ReadOnly = True
        dgvParameters.Columns(0).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns
        dgvParameters.Columns(1).DefaultCellStyle.BackColor = BackColorOfDistableDataViewColumns

        Me.EditsUnsaved = bSave

    End Sub

    ''' <summary>
    ''' Everytime the user changes the parameter type combobox from Ecopath 
    ''' Parameters to Ecosim Parameters and vice versa. This gets called to 
    ''' change all the options in the combobox used to specify the parameter 
    ''' name.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub cboPathOrSim_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPathOrSim.SelectedIndexChanged

        ' JS 02Oct13: preserve unsaved changes flag
        Dim bSave As Boolean = Me.EditsUnsaved

        ' JS 02Oct13: globalized this method
        ' JS 02Oct13: used a class to encapsulate param instead of relying on item text

        If cboPathOrSim.SelectedIndex = eParameterSet.Ecopath Then
            ChangeGridtoEcopath()
            cboParamName.Items.Clear()
            cboParamName.Items.Add(New ParamComboItem(eParamName.B, SharedResources.HEADER_BIOMASS)) ' "Biomass"
            cboParamName.Items.Add(New ParamComboItem(eParamName.BA, SharedResources.HEADER_BIOMACCUM_ABBR)) ' "Biomass Accumulation"
            cboParamName.Items.Add(New ParamComboItem(eParamName.QB, SharedResources.HEADER_CONSUMPTION_OVER_BIOMASS)) ' "Consumption/Biomass"
            cboParamName.Items.Add(New ParamComboItem(eParamName.PB, SharedResources.HEADER_PRODUCTION_OVER_BIOMASS)) ' "Production/Biomass"
            cboParamName.Items.Add(New ParamComboItem(eParamName.EE, SharedResources.HEADER_EE)) ' "Ecotrophic Efficiency"
            cboParamName.SelectedIndex = 0
        ElseIf cboPathOrSim.SelectedIndex = eParameterSet.Ecosim Then
            ChangeGridtoEcosim()
            cboParamName.Items.Clear()
            cboParamName.Items.Add(New ParamComboItem(eParamName.DenDepCatchability, SharedResources.HEADER_DENDEPCATCHABILITY_ABBR)) ' "DenDepCatchability"
            cboParamName.Items.Add(New ParamComboItem(eParamName.SwitchingPower, SharedResources.HEADER_SWITCHINGPOWER)) ' "Switching Power"
            cboParamName.Items.Add(New ParamComboItem(eParamName.QBMaxxQBio, My.Resources.HEADER_QBMAX_X_PBMAX)) ' "QBMaxxQBio"
            cboParamName.Items.Add(New ParamComboItem(eParamName.PredEffectFeedingTime, My.Resources.HEADER_PREDEFFECTFEEDINGTIME)) ' "PredEffectFeedingTime"
            cboParamName.Items.Add(New ParamComboItem(eParamName.OtherMortFeedingTime, My.Resources.HEADER_OTHERMORTFEEDTIME)) ' "OtherMortFeedingTime"
            cboParamName.Items.Add(New ParamComboItem(eParamName.MaxRelFeedingTime, My.Resources.HEADER_MAXRELFEEDTIME)) ' "MaxRelFeedingTime"
            cboParamName.Items.Add(New ParamComboItem(eParamName.FeedingTimeAdjustRate, My.Resources.HEADER_FEEDTIMEADJUSTRATE)) ' "FeedingTimeAdjustRate"
            cboParamName.SelectedIndex = 0

        End If

        Me.EditsUnsaved = True

    End Sub

    Private Sub cboParamName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboParamName.SelectedIndexChanged

        ' ToDo_JS: globalize this method

        'Whenever the user specifies a given parameter this fills the grid with its values
        dgvParameters.Rows.Clear()

        ' JS 02Oct13: used a class to encapsulate param instead of relying on item text
        Dim item As ParamComboItem = DirectCast(cboParamName.SelectedItem, ParamComboItem)
        Select Case item.ParamName
            Case eParamName.B
                FillDataGrid(B)
            Case eParamName.BA
                FillDataGrid(BA)
            Case eParamName.QB
                FillDataGrid(QB)
            Case eParamName.PB
                FillDataGrid(PB)
            Case eParamName.EE
                FillDataGrid(EE)
            Case eParamName.DenDepCatchability
                FillDataGrid(DenDepCatchability)
            Case eParamName.SwitchingPower
                FillDataGrid(SwitchingPower)
            Case eParamName.QBMaxxQBio
                FillDataGrid(QBMaxxQBio)
            Case eParamName.MaxRelFeedingTime
                FillDataGrid(MaxRelFeedingTime)
            Case eParamName.PredEffectFeedingTime
                FillDataGrid(PredEffectFeedingTime)
            Case eParamName.OtherMortFeedingTime
                FillDataGrid(OtherMortFeedingTime)
            Case eParamName.FeedingTimeAdjustRate
                FillDataGrid(FeedingTimeAdjustRate)
            Case Else
                Debug.Assert(False, "ParamName not supported")
        End Select

    End Sub

    Private Sub FillDataGrid(ByVal Parameters As Object)

        ' JS 30Sep13: speed up rendering
        dgvParameters.SuspendLayout()
        Try
            If cboPathOrSim.SelectedIndex = eParameterSet.Ecopath Then
                For Each iParameter In CType(Parameters, List(Of EcopathParam))
                    dgvParameters.Rows.Add(iParameter.GroupNo, iParameter.GroupName, iParameter.Mean, iParameter.CV, iParameter.LowerBound, iParameter.UpperBound)
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

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnOK.Click

        If Not Me.m_MSEPlugin.IsDirectoryStructureAvailable(True) Then
            Return
        End If

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

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub SaveEcoSimParameters2CSV(ByRef EcosimParams As List(Of EcosimParam), ByRef FileName As String)

        ' ToDo_JS: use proper path concatenation
        ' ToDo_JS: use fixed CSV field formatting

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
            ' JS 02Oct13: used fixed CSV field formatting
            sw.WriteLine(cStringUtils.ToCSVField(Param.GroupNo) & "," & _
                         cStringUtils.ToCSVField(Param.GroupName) & "," & _
                         cStringUtils.ToCSVField(Param.CV) & "," & _
                         cStringUtils.ToCSVField(Param.LowerBound) & "," & _
                         cStringUtils.ToCSVField(Param.UpperBound))
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
                If EcopathParamSet(i).GroupNo = CType(dgvParameters.Rows(e.RowIndex).Cells(0).Value, Integer) Then
                    If e.ColumnIndex = 3 Then
                        iEcopathParam.CV = CType(dgvParameters.CurrentCell.Value.ToString, Double)
                    ElseIf e.ColumnIndex = 4 Then
                        iEcopathParam.LowerBound = CType(dgvParameters.CurrentCell.Value.ToString, Double)
                    ElseIf e.ColumnIndex = 5 Then
                        iEcopathParam.UpperBound = CType(dgvParameters.CurrentCell.Value.ToString, Double)
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

        ' JS 02Oct13: used a class to encapsulate param instead of relying on item text
        Dim item As ParamComboItem = DirectCast(cboParamName.SelectedItem, ParamComboItem)

        EditsUnsaved = True

        Select Case item.ParamName
            Case eParamName.B
                SetValueParameter(B, e)
            Case eParamName.BA
                SetValueParameter(BA, e)
            Case eParamName.QB
                SetValueParameter(QB, e)
            Case eParamName.PB
                SetValueParameter(PB, e)
            Case eParamName.EE
                SetValueParameter(EE, e)
            Case eParamName.DenDepCatchability
                SetValueParameter(DenDepCatchability, e)
            Case eParamName.SwitchingPower
                SetValueParameter(SwitchingPower, e)
            Case eParamName.QBMaxxQBio
                SetValueParameter(QBMaxxQBio, e)
            Case eParamName.PredEffectFeedingTime
                SetValueParameter(PredEffectFeedingTime, e)
            Case eParamName.OtherMortFeedingTime
                SetValueParameter(OtherMortFeedingTime, e)
            Case eParamName.MaxRelFeedingTime
                SetValueParameter(MaxRelFeedingTime, e)
            Case eParamName.FeedingTimeAdjustRate
                SetValueParameter(FeedingTimeAdjustRate, e)
            Case Else
                Debug.Assert(False, "ParamName not supported")
        End Select

    End Sub

End Class