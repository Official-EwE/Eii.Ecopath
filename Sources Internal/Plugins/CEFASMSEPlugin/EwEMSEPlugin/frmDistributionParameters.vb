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
    Implements IDisposable


    Public Enum eParameterSet As Integer
        Ecopath = 0
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
        Public Sub New(paramname As eParamName, text As String, ByRef data As List(Of cDistributionParamsData))
            Me.ParamName = paramname
            Me.Text = text
            Me.Data = data
        End Sub
        Public Property ParamName As eParamName
        Public Property Text As String
        Public Property Data As List(Of cDistributionParamsData)
        Public Overrides Function ToString() As String
            Return Me.Text
        End Function
    End Class

    Public Class cDistributionParamsData

    End Class

    ''' <summary>
    ''' This holds one item in the list of any Ecopath parameters. 
    ''' Later in the code the entire list is grouped into a list of EcopathParam
    ''' </summary>
    Public Class EcopathParam
        Inherits cDistributionParamsData

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

    End Class

    ''' <summary>
    ''' Similar to <see cref="EcopathParam"/>, this holds one item 
    ''' in the list of any Ecosim parameters
    ''' </summary>
    Public Class EcosimParam
        Inherits cDistributionParamsData

        Public Sub New(ByVal GroupNumber As Integer, ByVal GroupName As String, ByVal DistributionType As cMSE.DistributionType, ByVal LowerBound As Double, ByVal UpperBound As Double, ByVal MidPoint As Double)
            Me.GroupNo = GroupNumber
            Me.GroupName = GroupName
            Me.DistributionType = DistributionType
            Me.LowerBound = LowerBound
            Me.UpperBound = UpperBound
            Me.MidPoint = MidPoint
        End Sub

        Public Property GroupNo As Integer
        Public Property GroupName As String
        Public Property DistributionType As cMSE.DistributionType
        Public Property LowerBound As Double
        Public Property UpperBound As Double
        Public Property MidPoint As Double

    End Class

    Private m_plugin As cMSEPluginPoint = Nothing

    Private B As New List(Of cDistributionParamsData)
    Private BA As New List(Of cDistributionParamsData)
    Private QB As New List(Of cDistributionParamsData)
    Private PB As New List(Of cDistributionParamsData)
    Private EE As New List(Of cDistributionParamsData)

    Private DenDepCatchability As New List(Of cDistributionParamsData)
    Private SwitchingPower As New List(Of cDistributionParamsData)
    Private QBMaxxQBio As New List(Of cDistributionParamsData)
    Private PredEffectFeedingTime As New List(Of cDistributionParamsData)
    Private OtherMortFeedingTime As New List(Of cDistributionParamsData)
    Private MaxRelFeedingTime As New List(Of cDistributionParamsData)
    Private FeedingTimeAdjustRate As New List(Of cDistributionParamsData)

    Private nPPers As Integer
    Private m_bIsDirty As Boolean

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Sub Init(ByVal uic As cUIContext, ByVal Plugin As cMSEPluginPoint)

        Me.m_grid.UIContext = uic
        Me.UIContext = uic
        Me.m_plugin = Plugin

        For i As Integer = 1 To Me.Core.nGroups
            If Me.Core.EcoPathGroupInputs(i).IsProducer Then nPPers += 1
        Next

        ' JS: Item indexes should obviously correspond to eParameterSet enum values
        Me.m_tscmPathOrSim.Items.Add(SharedResources.HEADER_ECOPATH)
        Me.m_tscmPathOrSim.Items.Add(SharedResources.HEADER_ECOSIM)

    End Sub

    Private ReadOnly Property MSE As cMSE
        Get
            Return Me.m_plugin.MSE
        End Get
    End Property

#Region " Overrides "

    Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(value As ScientificInterfaceShared.Controls.cUIContext)
            MyBase.UIContext = value
        End Set
    End Property

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        ' JS 30Sep13: globalized this method
        MyBase.OnLoad(e)

        AddHandler Me.m_grid.onEdited, AddressOf OnGridEdited

        If LoadEcopathParameters() = False Then
            Me.m_plugin.InformUser(My.Resources.ERROR_DISTRPAR_LOAD_ECOPATH, eMessageImportance.Warning)
        End If

        If LoadEcosimParameters() = False Then
            Me.m_plugin.InformUser(My.Resources.ERROR_DISTRPAR_LOAD_ECOSIM, eMessageImportance.Warning)
        End If

        'initialises the dropdown box to the Ecopath parameters
        Me.m_tscmPathOrSim.SelectedIndex = eParameterSet.Ecopath

        Me.m_bIsDirty = False
        Me.CenterToParent()
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)

        If (Me.m_bIsDirty = True) Then
            ' JS 02Oct13: globalized this method
            ' JS 02Oct13: replaced MsgBox with cFeedbackMessage
            Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_UNSAVED_CHANGES, _
                                 eCoreComponentType.External, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            fmsg.Reply = eMessageReply.YES
            Me.Core.Messages.SendMessage(fmsg)
            e.Cancel = (fmsg.Reply <> eMessageReply.YES)
        End If

        MyBase.OnFormClosing(e)

    End Sub


    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_grid.onEdited, AddressOf OnGridEdited
        Me.m_grid.UIContext = Nothing

        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()
        ' Me.m_btnOK.Enabled = Me.m_bIsDirty
    End Sub

    Public Sub Clear()
        'HACK Should not have to do this. 
        'Clear out the data created when the form loaded this will release the memory
        'For some reason the framework is not releasing the form and allowing it to cleanup its memory
        'So do it manually
        Try
            B.Clear()
            BA.Clear()
            QB.Clear()
            EE.Clear()
            PB.Clear()
            DenDepCatchability.Clear()
            SwitchingPower.Clear()
            QBMaxxQBio.Clear()
            PredEffectFeedingTime.Clear()
            OtherMortFeedingTime.Clear()
            MaxRelFeedingTime.Clear()
            FeedingTimeAdjustRate.Clear()
        Catch ex As Exception

        End Try

    End Sub


#End Region ' Overrides

#Region " Internals "

    ''' <summary>
    ''' Given a Ecosim csv object this extracts the data from the current line and uses it to return an EcosimParam structure object
    ''' </summary>
    ''' <param name="csv"></param>
    ''' <returns></returns>
    Private Function ExtractEcosimParam(ByVal csv As CsvReader) As EcosimParam

        ' JS 12Oct13: made fail-proof
        ' JS 12Oct13: used fixed CSV field reading
        ' JS 02Dec13: added EndOfStream checks

        ' Sanity checks
        If (csv Is Nothing) Then Return Nothing
        If (csv.EndOfStream()) Then Return Nothing
        If (Not csv.ReadNextRecord()) Then Return Nothing

        Dim TGroupName As String = ""
        Dim TGroupNumber As Integer
        Dim TLowerBound As Double
        Dim TUpperBound As Double
        Dim TDistributionType As cMSE.DistributionType
        Dim TMidPoint As Double

        Try

            TGroupName = cMSEUtils.FromCSVField(csv(0))
            TGroupNumber = cStringUtils.ConvertToInteger(csv(1))
            Dim iDistr As Integer = cStringUtils.ConvertToInteger(csv(2))
            Try
                TDistributionType = DirectCast(iDistr, cMSE.DistributionType)
            Catch ex As Exception
                ' Default
                TDistributionType = cMSE.DistributionType.Uniform
            End Try
            TLowerBound = cStringUtils.ConvertToDouble(csv(3))
            TUpperBound = cStringUtils.ConvertToDouble(csv(4))
            TMidPoint = cStringUtils.ConvertToDouble(csv(5))

        Catch ex As Exception
            ' ToDo_JS: respond to error
            Return Nothing
        End Try

        Return New EcosimParam(TGroupNumber, TGroupName, TDistributionType, TLowerBound, TUpperBound, TMidPoint)

    End Function

    ''' <summary>
    ''' Extracts distribution parameters for one group from csv and Ecopath
    ''' </summary>
    ''' <param name="csv"></param>
    ''' <param name="ParameterType"></param>
    ''' <returns></returns>
    Private Function ExtractEcopathParam(ByVal csv As CsvReader, ByVal ParameterType As eParamName) As EcopathParam

        ' Sanity checks
        If (csv Is Nothing) Then Return Nothing
        If (Not csv.ReadNextRecord()) Then Return Nothing
        If (csv.FieldCount < 5) Then Return Nothing

        Dim TGroupName As String = ""
        Dim TGroupNumber As Integer
        Dim TMean As Single
        Dim TCV As Double
        Dim TLowerBound As Double
        Dim TUpperBound As Double

        Try
            TGroupNumber = cStringUtils.ConvertToInteger(csv(0))
            TGroupName = cMSEUtils.FromCSVField(csv(1))
            TCV = cStringUtils.ConvertToDouble(csv(2))
            TLowerBound = cStringUtils.ConvertToDouble(csv(3))
            TUpperBound = cStringUtils.ConvertToDouble(csv(4))

            ' JS 02Oct2013: Need to validate group number
            If TGroupNumber < 1 Or TGroupNumber >= Me.Core.nGroups Then
                ' ToDo:_JS: report error somehow
                Return Nothing
            End If

            If ParameterType = eParamName.B Then
                TMean = Me.Core.EcoPathGroupInputs(TGroupNumber).BiomassAreaInput
            ElseIf ParameterType = eParamName.BA Then
                TMean = Me.Core.EcoPathGroupInputs(TGroupNumber).BioAccum
            ElseIf ParameterType = eParamName.QB Then
                TMean = Me.Core.EcoPathGroupInputs(TGroupNumber).QBInput
            ElseIf ParameterType = eParamName.PB Then
                TMean = Me.Core.EcoPathGroupInputs(TGroupNumber).PBInput
            ElseIf ParameterType = eParamName.EE Then
                TMean = Me.Core.EcoPathGroupInputs(TGroupNumber).EEInput
            End If

        Catch ex As Exception
            ' ToDo:_JS: report error somehow
            Return Nothing
        End Try

        Return New EcopathParam(TGroupNumber, TGroupName, TMean, TCV, TLowerBound, TUpperBound)

    End Function

    Private Function LoadEcosimParamX(ByRef ParamList As List(Of cDistributionParamsData), ByVal Path As String, ByVal ParamName As eParamName) As Boolean

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim TMean As Single
        Dim params(Me.Core.nLivingGroups) As EcosimParam
        Dim param As EcosimParam = Nothing
        Dim bSuccess As Boolean = True

        If File.Exists(Path) Then

            reader = cMSEUtils.GetReader(Path)
            If (reader IsNot Nothing) Then
                Try
                    ParamList.Clear()
                    csv = New CsvReader(reader, True)
                    While Not csv.EndOfStream
                        param = Me.ExtractEcosimParam(csv)
                        If (param IsNot Nothing) Then
                            ' Only add with valid group indexes
                            If (param.GroupNo >= 1 And param.GroupNo <= Me.Core.nLivingGroups) Then
                                params(param.GroupNo) = param
                            Else
                                ' Not used: notify user?
                            End If
                        End If
                    End While
                    csv.Dispose()

                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".LoadEcosimParameters() Exception: " & ex.Message)
                    bSuccess = False
                End Try
                cMSEUtils.ReleaseReader(reader)
            End If
        End If

        ' Complement list with defaults for missing groups
        For igrp = 1 To Me.Core.nLivingGroups
            ' ToDo_JS: Exclude primary producers here?
            If params(igrp) Is Nothing Then
                If ParamName = eParamName.DenDepCatchability Then
                    TMean = Me.Core.EcoSimGroupInputs(igrp).DenDepCatchability
                ElseIf ParamName = eParamName.FeedingTimeAdjustRate Then
                    TMean = Me.Core.EcoSimGroupInputs(igrp).FeedingTimeAdjustRate
                ElseIf ParamName = eParamName.MaxRelFeedingTime Then
                    TMean = Me.Core.EcoSimGroupInputs(igrp).MaxRelFeedingTime
                ElseIf ParamName = eParamName.OtherMortFeedingTime Then
                    TMean = Me.Core.EcoSimGroupInputs(igrp).OtherMortFeedingTime
                ElseIf ParamName = eParamName.PredEffectFeedingTime Then
                    TMean = Me.Core.EcoSimGroupInputs(igrp).PredEffectFeedingTime
                ElseIf ParamName = eParamName.QBMaxxQBio Then
                    TMean = Me.Core.EcoSimGroupInputs(igrp).QBMaxQBio
                ElseIf ParamName = eParamName.SwitchingPower Then
                    TMean = Me.Core.EcoSimGroupInputs(igrp).SwitchingPower
                End If
                If Core.EcoPathGroupInputs(igrp).IsProducer Then
                    params(igrp) = New EcosimParam(igrp, Me.Core.EcoPathGroupInputs(igrp).Name, 0, -9999, -9999, -9999)
                Else
                    params(igrp) = New EcosimParam(igrp, Me.Core.EcoPathGroupInputs(igrp).Name, cMSE.DistributionType.Triangular, TMean * (1 - 0.1), TMean * (1 + 0.1), TMean)
                End If
            End If
        Next

        For Each param In params
            If (param IsNot Nothing) Then
                Dim grp As cEcoPathGroupInput = Core.EcoPathGroupInputs(param.GroupNo)
                ' Only allow living non-producers
                If (grp.IsLiving) Then
                    ParamList.Add(param)
                End If
            End If
        Next

        Return bSuccess

    End Function

    Private Function LoadEcosimParameters() As Boolean

        'loads all the ecosim csv files up and creates instances of lists of structures that hold it all in memory
        Return LoadEcosimParamX(DenDepCatchability, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "DenDepCatchability.csv"), eParamName.DenDepCatchability) And _
               LoadEcosimParamX(SwitchingPower, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "SwitchingPower.csv"), eParamName.SwitchingPower) And _
               LoadEcosimParamX(QBMaxxQBio, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "QBMaxxQBio.csv"), eParamName.QBMaxxQBio) And _
               LoadEcosimParamX(PredEffectFeedingTime, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "PredEffectFeedingTime.csv"), eParamName.PredEffectFeedingTime) And _
               LoadEcosimParamX(OtherMortFeedingTime, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "OtherMortFeedingTime.csv"), eParamName.OtherMortFeedingTime) And _
               LoadEcosimParamX(MaxRelFeedingTime, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "MaxRelFeedingTime.csv"), eParamName.MaxRelFeedingTime) And _
               LoadEcosimParamX(FeedingTimeAdjustRate, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "FeedingTimeAdjustRate.csv"), eParamName.FeedingTimeAdjustRate)

    End Function

    Private Function LoadEcopathParamX(ByVal ParamList As List(Of cDistributionParamsData), ByVal strPath As String, ByVal ParamName As eParamName) As Boolean

        Dim csv As CsvReader
        Dim MonteCarlo As cMonteCarloManager = Me.Core.EcosimMonteCarlo
        Dim MCGroup As cMonteCarloGroup
        Dim TMean As Single
        Dim TCV As Double
        Dim TLowerBound As Double
        Dim TUpperBound As Double
        Dim reader As StreamReader = cMSEUtils.GetReader(strPath)
        Dim params(Me.Core.nLivingGroups) As EcopathParam
        Dim param As EcopathParam = Nothing
        Dim bSuccess As Boolean = True

        If File.Exists(strPath) Then
            reader = cMSEUtils.GetReader(strPath)
            If (reader IsNot Nothing) Then
                Try
                    ParamList.Clear()
                    csv = New CsvReader(reader, True)
                    While Not csv.EndOfStream
                        param = Me.ExtractEcopathParam(csv, ParamName)
                        If (param IsNot Nothing) Then
                            ' Only add with valid group indexes
                            If (param.GroupNo >= 1 And param.GroupNo <= Me.Core.nLivingGroups) Then
                                params(param.GroupNo) = param
                            Else
                                ' Not used: notify user?
                            End If
                        End If
                    End While
                    csv.Dispose()

                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".LoadEcopathParameters() Exception: " & ex.Message)
                    bSuccess = False
                End Try
                cMSEUtils.ReleaseReader(reader)
            End If
        End If

        ' Complement list with defaults for missing groups
        For igrp = 1 To Me.Core.nLivingGroups
            If params(igrp) Is Nothing Then
                MCGroup = MonteCarlo.Groups(igrp)
                If ParamName = eParamName.B Then
                    TMean = Me.Core.EcoPathGroupOutputs(igrp).Biomass
                    TCV = MCGroup.Bcv
                    TLowerBound = MCGroup.BLower
                    TUpperBound = MCGroup.BUpper
                ElseIf ParamName = eParamName.BA Then
                    TMean = Me.Core.EcoPathGroupOutputs(igrp).BioAccum
                    TCV = MCGroup.BAcv
                    TLowerBound = MCGroup.BALower
                    TUpperBound = MCGroup.BAUpper
                ElseIf ParamName = eParamName.EE Then
                    TMean = Me.Core.EcoPathGroupOutputs(igrp).EEOutput
                    TCV = MCGroup.EEcv
                    TLowerBound = MCGroup.EELower
                    TUpperBound = MCGroup.EEUpper
                ElseIf ParamName = eParamName.PB Then
                    TMean = Me.Core.EcoPathGroupOutputs(igrp).PBOutput
                    TCV = MCGroup.PBcv
                    TLowerBound = MCGroup.PBLower
                    TUpperBound = MCGroup.PBUpper
                ElseIf ParamName = eParamName.QB Then
                    TMean = Me.Core.EcoPathGroupOutputs(igrp).QBOutput
                    TCV = MCGroup.QBcv
                    TLowerBound = MCGroup.QBLower
                    TUpperBound = MCGroup.QBUpper
                End If
                params(igrp) = New EcopathParam(igrp, Me.Core.EcoPathGroupInputs(igrp).Name, TMean, TCV, TLowerBound, TUpperBound)
            End If
        Next

        For Each param In params
            If (param IsNot Nothing) Then
                ParamList.Add(param)
            End If
        Next

        Return bSuccess

    End Function

    Private Function LoadEcopathParameters() As Boolean

        Return LoadEcopathParamX(B, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "B_Dist.csv"), eParamName.B) And _
               LoadEcopathParamX(PB, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "PB_Dist.csv"), eParamName.PB) And _
               LoadEcopathParamX(QB, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "QB_Dist.csv"), eParamName.QB) And _
               LoadEcopathParamX(EE, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "EE_Dist.csv"), eParamName.EE) And _
               LoadEcopathParamX(BA, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "BA_Dist.csv"), eParamName.BA)

    End Function


    Private Sub UpdateGrid(data As cDistributionParamsData(), strName As String)
        Me.m_grid.Data = data
        Me.m_grid.DataName = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, My.Resources.CAPTION, strName)
    End Sub

    Private Function SaveEcoSimParameters2CSV(ByVal params As List(Of cDistributionParamsData), ByVal strFileName As String) As Boolean

        Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, strFileName & ".csv"), False)
        Dim bSuccess As Boolean = False

        If (writer Is Nothing) Then Return bSuccess

        Try
            writer.WriteLine("GroupName,GroupNumber,DistributionType,Lower,Upper,Mid")

            For Each entry As cDistributionParamsData In params
                If (TypeOf (entry) Is EcosimParam) Then
                    Dim param As EcosimParam = DirectCast(entry, EcosimParam)
                    writer.WriteLine(cStringUtils.ToCSVField(param.GroupName) & "," & _
                                     cStringUtils.ToCSVField(param.GroupNo) & "," & _
                                     cStringUtils.ToCSVField(param.DistributionType) & "," & _
                                     cStringUtils.ToCSVField(param.LowerBound) & "," & _
                                     cStringUtils.ToCSVField(param.UpperBound) & "," & _
                                     cStringUtils.ToCSVField(param.MidPoint))
                End If
            Next

            bSuccess = True

        Catch ex As Exception

        End Try
        cMSEUtils.ReleaseWriter(writer)
        Return bSuccess

    End Function

    Private Function SaveEcopathParameters2CSV(ByVal parms As List(Of cDistributionParamsData), ByVal strFileName As String) As Boolean

        Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, strFileName & ".csv"), False)
        Dim bSuccess As Boolean = False

        If (writer Is Nothing) Then Return bSuccess

        Try
            writer.WriteLine("Group Number,Name,CV,""Lower Bound"",""Upper Bound""")

            For Each entry As cDistributionParamsData In parms
                If (TypeOf (entry) Is EcopathParam) Then
                    Dim param As EcopathParam = DirectCast(entry, EcopathParam)
                    writer.WriteLine(cStringUtils.ToCSVField(param.GroupNo) & "," & _
                                 cStringUtils.ToCSVField(param.GroupName) & "," & _
                                 cStringUtils.ToCSVField(param.CV) & "," & _
                                 cStringUtils.ToCSVField(param.LowerBound) & "," & _
                                 cStringUtils.ToCSVField(param.UpperBound))
                End If
            Next
            bSuccess = True

        Catch ex As Exception

        End Try

        cMSEUtils.ReleaseWriter(writer)
        Return bSuccess

    End Function

#End Region ' Internals

#Region " Control events "

    ''' <summary>
    ''' Everytime the user changes the parameter type combobox from Ecopath 
    ''' Parameters to Ecosim Parameters and vice versa. This gets called to 
    ''' change all the options in the combobox used to specify the parameter 
    ''' name.
    ''' </summary>
    Private Sub OnModelSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tscmPathOrSim.SelectedIndexChanged

        ' JS 02Oct13: preserve unsaved changes flag
        Dim bSave As Boolean = Me.m_bIsDirty

        ' JS 02Oct13: globalized this method
        ' JS 02Oct13: used a class to encapsulate param instead of relying on item text

        If m_tscmPathOrSim.SelectedIndex = eParameterSet.Ecopath Then
            Me.m_grid.Mode = eParameterSet.Ecopath
            Me.m_tscmParamName.Items.Clear()
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.B, SharedResources.HEADER_BIOMASS, B))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.BA, SharedResources.HEADER_BIOMACCUM_ABBR, BA))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.QB, SharedResources.HEADER_CONSUMPTION_OVER_BIOMASS, QB))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.PB, SharedResources.HEADER_PRODUCTION_OVER_BIOMASS, PB))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.EE, SharedResources.HEADER_EE, EE))
            Me.m_tscmParamName.SelectedIndex = 0
        ElseIf m_tscmPathOrSim.SelectedIndex = eParameterSet.Ecosim Then
            Me.m_grid.Mode = eParameterSet.Ecosim
            Me.m_tscmParamName.Items.Clear()
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.DenDepCatchability, SharedResources.HEADER_DENDEPCATCHABILITY_ABBR, DenDepCatchability))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.SwitchingPower, SharedResources.HEADER_SWITCHINGPOWER, SwitchingPower))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.QBMaxxQBio, My.Resources.HEADER_QBMAX_X_PBMAX, QBMaxxQBio))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.PredEffectFeedingTime, My.Resources.HEADER_PREDEFFECTFEEDINGTIME, PredEffectFeedingTime))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.OtherMortFeedingTime, My.Resources.HEADER_OTHERMORTFEEDTIME, OtherMortFeedingTime))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.MaxRelFeedingTime, My.Resources.HEADER_MAXRELFEEDTIME, MaxRelFeedingTime))
            Me.m_tscmParamName.Items.Add(New ParamComboItem(eParamName.FeedingTimeAdjustRate, My.Resources.HEADER_FEEDTIMEADJUSTRATE, FeedingTimeAdjustRate))
            Me.m_tscmParamName.SelectedIndex = 0

        End If

        Me.m_bIsDirty = True

    End Sub

    Private Sub OnGridEdited()
        Me.m_bIsDirty = True
        Me.Invoke(New MethodInvoker(AddressOf UpdateControls))
    End Sub

    Private Sub OnParamSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tscmParamName.SelectedIndexChanged

        Try
            Dim item As ParamComboItem = DirectCast(Me.m_tscmParamName.SelectedItem, ParamComboItem)
            Me.UpdateGrid(item.Data.ToArray, item.Text)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnOK.Click

        Dim lstrSubMessages As New List(Of String)
        Dim strFolder As String = cMSEUtils.MSEFolder(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams)

        If Not Me.mse.IsInputStructureAvailable(True) Then
            ' ToDo: report error
            Return
        End If

        'Saves all the parameters to csv when user clicks to save
        If SaveEcopathParameters2CSV(B, "B_Dist") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "B_Dist.csv"))
        If SaveEcopathParameters2CSV(BA, "BA_Dist") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "BA_Dist.csv"))
        If SaveEcopathParameters2CSV(PB, "PB_Dist") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "PB_Dist.csv"))
        If SaveEcopathParameters2CSV(QB, "QB_Dist") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "QB_Dist.csv"))
        If SaveEcopathParameters2CSV(EE, "EE_Dist") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "EE_Dist.csv"))

        If SaveEcoSimParameters2CSV(DenDepCatchability, "DenDepCatchability") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "DenDepCatchability.csv"))
        If SaveEcoSimParameters2CSV(SwitchingPower, "SwitchingPower") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "SwitchingPower.csv"))
        If SaveEcoSimParameters2CSV(QBMaxxQBio, "QBMaxxQBio") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "QBMaxxQBio.csv"))
        If SaveEcoSimParameters2CSV(PredEffectFeedingTime, "PredEffectFeedingTime") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "PredEffectFeedingTime.csv"))
        If SaveEcoSimParameters2CSV(OtherMortFeedingTime, "OtherMortFeedingTime") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "OtherMortFeedingTime.csv"))
        If SaveEcoSimParameters2CSV(MaxRelFeedingTime, "MaxRelFeedingTime") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "MaxRelFeedingTime.csv"))
        If SaveEcoSimParameters2CSV(FeedingTimeAdjustRate, "FeedingTimeAdjustRate") Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "FeedingTimeAdjustRate.csv"))

        Me.MSE.GenerateEmptyDietCSVs()
        Me.m_bIsDirty = False

        Me.m_plugin.InformUser(String.Format(My.Resources.STATUS_SAVED_DISTPARMS, My.Resources.CAPTION, strFolder), _
                                 eMessageImportance.Information, strFolder, lstrSubMessages.ToArray())

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

#End Region ' Control events



End Class