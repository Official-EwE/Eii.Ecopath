' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.IO
Imports EwECore
Imports EwEUtils.Utilities
Imports LumenWorks.Framework.IO.Csv

#Region " Base classes "

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for maintaining distribution parameters for a single functional group.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IDistributionParamsData
    ' NOP
End Interface

''' ---------------------------------------------------------------------------
''' <summary>
''' Base class for organizing, loading, and saving distribution parameters.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class cDistributionParams

#Region " Internal Variables "

    Private m_core As EwECore.cCore
    Private m_MSE As cMSE
    Private m_bChangesToSave As Boolean

#End Region ' Internal Variables

#Region " Construction "

    Public Sub New(MSE As cMSE, core As EwECore.cCore)
        Me.m_core = core
        Me.m_MSE = MSE
        Me.Defaults()
    End Sub

#End Region ' Construction

#Region " Public bits "

    ''' <summary>
    ''' Enumerated type, providing all available distribution parameters.
    ''' </summary>
    Public Enum eDistrParamName As Byte
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

    Public Function IsChanged() As Boolean
        Return Me.m_bChangesToSave
    End Function

    ''' <summary>
    ''' Generate default values
    ''' </summary>
    Public MustOverride Sub Defaults()

    ''' <summary>
    ''' Generate entire parameter set from CSV.
    ''' </summary>
    ''' <param name="msg">Panic message, if any.</param>
    Public MustOverride Function Load(msg As cMessage) As Boolean

    ''' <summary>
    ''' Save entire parameter set to CSV.
    ''' </summary>
    Public MustOverride Function Save() As Boolean

#End Region ' Public bits

#Region " Internal bits "

    Protected ReadOnly Property MSE As cMSE
        Get
            Return Me.m_MSE
        End Get
    End Property

    Protected ReadOnly Property Core As cCore
        Get
            Return Me.m_core
        End Get
    End Property

#End Region ' Internal bits

End Class

#End Region ' Base classes

#Region " Ecopath "

''' ---------------------------------------------------------------------------
''' <summary>
''' Container for Ecopath distribution parameters for a single functional group.
''' <seealso cref="IDistributionParamsData"/>.
''' <seealso cref="cEcosimDistributionParamsData"/>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcopathDistributionParamsData
    Implements IDistributionParamsData

    Public Sub New(GroupNumber As Integer,
                   GroupName As String,
                   Mean As Single,
                   CV As Double,
                   LowerBound As Double,
                   UpperBound As Double)
        Me.GroupNo = GroupNumber
        Me.GroupName = GroupName
        Me.Mean = Mean
        Me.CV = CV
        Me.LowerBound = LowerBound
        Me.UpperBound = UpperBound
    End Sub

    Public Property CV As Double
    Public Property LowerBound As Double
    Public Property UpperBound As Double
    Public Property GroupNo As Integer
    Public Property GroupName As String
    Public Property Mean As Double

End Class

''' ---------------------------------------------------------------------------
''' <summary>
''' Manager for all <see cref="cEcopathDistributionParamsData"/> for the 
''' currently loaded model.
''' <seealso cref="cDistributionParams"/>
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcopathDistributionParams
    Inherits cDistributionParams

#Region " Internal variables "

    Private m_B As List(Of IDistributionParamsData)
    Private m_BA As List(Of IDistributionParamsData)
    Private m_QB As List(Of IDistributionParamsData)
    Private m_PB As List(Of IDistributionParamsData)
    Private m_EE As List(Of IDistributionParamsData)

#End Region ' Internal variables

#Region " Construction "

    Public Sub New(MSE As cMSE, core As EwECore.cCore)
        MyBase.New(MSE, core)
    End Sub

#End Region ' Construction

#Region " Public bits "

    Public Overrides Sub Defaults()
        If (Me.B Is Nothing) Then
            Me.m_B = New List(Of IDistributionParamsData)
            Me.m_BA = New List(Of IDistributionParamsData)
            Me.m_QB = New List(Of IDistributionParamsData)
            Me.m_PB = New List(Of IDistributionParamsData)
            Me.m_EE = New List(Of IDistributionParamsData)
        Else
            Me.m_B.Clear()
            Me.m_BA.Clear()
            Me.m_QB.Clear()
            Me.m_PB.Clear()
            Me.m_EE.Clear()
        End If
        Me.Load(Nothing)
    End Sub

    ''' <summary>
    ''' Get Ecopath <see cref="eDistrParamName.B">biomass</see> distribution parameters
    ''' </summary>
    Public ReadOnly Property B As List(Of IDistributionParamsData)
        Get
            Return Me.m_B
        End Get
    End Property

    Public ReadOnly Property BA As List(Of IDistributionParamsData)
        Get
            Return Me.m_BA
        End Get
    End Property

    Public ReadOnly Property QB As List(Of IDistributionParamsData)
        Get
            Return Me.m_QB
        End Get
    End Property

    Public ReadOnly Property PB As List(Of IDistributionParamsData)
        Get
            Return Me.m_PB
        End Get
    End Property

    Public ReadOnly Property EE As List(Of IDistributionParamsData)
        Get
            Return Me.m_EE
        End Get
    End Property

    ''' <inheritdocs cref="cDistributionParams.Load"/>
    Public Overrides Function Load(msg As cMessage) As Boolean
        Return Me.LoadEcopathParamX(msg, Me.B, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "B_Dist.csv"), eDistrParamName.B) And
               Me.LoadEcopathParamX(msg, Me.PB, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "PB_Dist.csv"), eDistrParamName.PB) And
               Me.LoadEcopathParamX(msg, Me.QB, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "QB_Dist.csv"), eDistrParamName.QB) And
               Me.LoadEcopathParamX(msg, Me.EE, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "EE_Dist.csv"), eDistrParamName.EE) And
               Me.LoadEcopathParamX(msg, Me.BA, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "BA_Dist.csv"), eDistrParamName.BA)
    End Function

    ''' <inheritdocs cref="cDistributionParams.Save"/>
    Public Overrides Function Save() As Boolean
        Return Me.SaveEcopathParameters2CSV(Me.B, "B_Dist") And
               Me.SaveEcopathParameters2CSV(Me.BA, "BA_Dist") And
               Me.SaveEcopathParameters2CSV(Me.PB, "PB_Dist") And
               Me.SaveEcopathParameters2CSV(Me.QB, "QB_Dist") And
               Me.SaveEcopathParameters2CSV(Me.EE, "EE_Dist")
    End Function

#End Region ' Public bits

#Region " Internals "

    Private Function LoadEcopathParamX(msg As cMessage,
                                       ParamList As List(Of IDistributionParamsData),
                                       strPath As String,
                                       ParamName As eDistrParamName) As Boolean

        Dim csv As CsvReader
        Dim MonteCarlo As cMonteCarloManager = Me.Core.EcosimMonteCarlo
        Dim MCGroup As cMonteCarloGroup
        Dim TMean As Single
        Dim TCV As Double
        Dim TLowerBound As Double
        Dim TUpperBound As Double
        Dim reader As StreamReader = Nothing
        Dim params(Me.Core.nLivingGroups) As cEcopathDistributionParamsData
        Dim param As cEcopathDistributionParamsData = Nothing
        Dim nGroups As Integer = 0
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
                            ' Only add with valid group indexes and names
                            Dim bOK As Boolean = False
                            If (param.GroupNo >= 1 And param.GroupNo <= Me.Core.nLivingGroups) Then
                                If (String.Compare(param.GroupName, Me.Core.EcopathGroupInputs(param.GroupNo).Name, True) = 0) Then
                                    params(param.GroupNo) = param
                                    nGroups += 1
                                    bOK = True
                                End If
                            End If
                            If (bOK = False) Then
                                cMSEUtils.LogError(msg, "Invalid group " & param.GroupNo & " encountered in " & Path.GetFileName(strPath))
                            End If
                            bSuccess = bSuccess And bOK
                        End If
                    End While
                    csv.Dispose()

                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".LoadEcopathParameters() Exception: " & ex.Message)
                    bSuccess = False
                End Try

                If bSuccess And (nGroups <> Me.Core.nLivingGroups) Then
                    If nGroups < Me.Core.nLivingGroups Then
                        cMSEUtils.LogError(msg, String.Format(My.Resources.ERROR_DISTRFILE_GROUPS_LIVING_MISSING, Path.GetFileName(strPath)))
                    ElseIf nGroups > Me.Core.nLivingGroups Then 'Check whether there are too many groups in the file
                        cMSEUtils.LogError(msg, String.Format(My.Resources.ERROR_DISTRFILE_GROUPS_HASNONLIVING, Path.GetFileName(strPath)))
                    End If
                    bSuccess = False
                End If
            End If
        End If
        cMSEUtils.ReleaseReader(reader)

        ' Complement list with defaults for missing groups
        For igrp = 1 To Me.Core.nLivingGroups
            If params(igrp) Is Nothing Then
                MCGroup = MonteCarlo.Groups(igrp)
                If ParamName = eDistrParamName.B Then
                    TMean = Me.Core.EcopathGroupOutputs(igrp).Biomass
                    TCV = MCGroup.Bcv
                    TLowerBound = MCGroup.BLower
                    TUpperBound = MCGroup.BUpper
                ElseIf ParamName = eDistrParamName.BA Then
                    TMean = Me.Core.EcopathGroupOutputs(igrp).BioAccum
                    TCV = MCGroup.BAcv
                    TLowerBound = MCGroup.BALower
                    TUpperBound = MCGroup.BAUpper
                ElseIf ParamName = eDistrParamName.EE Then
                    TMean = Me.Core.EcopathGroupOutputs(igrp).EEOutput
                    TCV = MCGroup.EEcv
                    TLowerBound = MCGroup.EELower
                    TUpperBound = MCGroup.EEUpper
                ElseIf ParamName = eDistrParamName.PB Then
                    TMean = Me.Core.EcopathGroupOutputs(igrp).PBOutput
                    TCV = MCGroup.PBcv
                    TLowerBound = MCGroup.PBLower
                    TUpperBound = MCGroup.PBUpper
                ElseIf ParamName = eDistrParamName.QB Then
                    TMean = Me.Core.EcopathGroupOutputs(igrp).QBOutput
                    TCV = MCGroup.QBcv
                    TLowerBound = MCGroup.QBLower
                    TUpperBound = MCGroup.QBUpper
                End If
                params(igrp) = New cEcopathDistributionParamsData(igrp, Me.Core.EcopathGroupInputs(igrp).Name, TMean, TCV, TLowerBound, TUpperBound)
            End If
        Next

        For Each param In params
            If (param IsNot Nothing) Then
                ParamList.Add(param)
            End If
        Next

        Return bSuccess

    End Function

    ''' <summary>
    ''' Extracts distribution parameters for one group from csv and Ecopath
    ''' </summary>
    ''' <param name="csv"></param>
    ''' <param name="ParameterType"></param>
    ''' <returns></returns>
    Private Function ExtractEcopathParam(csv As CsvReader, ParameterType As eDistrParamName) As cEcopathDistributionParamsData

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

            If ParameterType = eDistrParamName.B Then
                TMean = Me.Core.EcopathGroupInputs(TGroupNumber).BiomassAreaInput
            ElseIf ParameterType = eDistrParamName.BA Then
                TMean = Me.Core.EcopathGroupInputs(TGroupNumber).BioAccumInput
            ElseIf ParameterType = eDistrParamName.QB Then
                TMean = Me.Core.EcopathGroupInputs(TGroupNumber).QBInput
            ElseIf ParameterType = eDistrParamName.PB Then
                TMean = Me.Core.EcopathGroupInputs(TGroupNumber).PBInput
            ElseIf ParameterType = eDistrParamName.EE Then
                TMean = Me.Core.EcopathGroupInputs(TGroupNumber).EEInput
            End If

        Catch ex As Exception
            ' ToDo:_JS: report error somehow
            Return Nothing
        End Try

        Return New cEcopathDistributionParamsData(TGroupNumber, TGroupName, TMean, TCV, TLowerBound, TUpperBound)

    End Function

    Private Function SaveEcopathParameters2CSV(parms As List(Of IDistributionParamsData), strFileName As String) As Boolean

        Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, strFileName & ".csv"), False)
        Dim bSuccess As Boolean = False

        If (writer Is Nothing) Then Return bSuccess

        Try
            writer.WriteLine("Group Number,Name,CV,""Lower Bound"",""Upper Bound""")

            For Each entry As IDistributionParamsData In parms
                If (TypeOf (entry) Is cEcopathDistributionParamsData) Then
                    Dim param As cEcopathDistributionParamsData = DirectCast(entry, cEcopathDistributionParamsData)
                    writer.WriteLine(cStringUtils.ToCSVField(param.GroupNo) & "," &
                                 cStringUtils.ToCSVField(param.GroupName) & "," &
                                 cStringUtils.ToCSVField(param.CV) & "," &
                                 cStringUtils.ToCSVField(param.LowerBound) & "," &
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

End Class

#End Region ' Ecopath

#Region " Ecosim "

''' ---------------------------------------------------------------------------
''' <summary>
''' Container for Ecosim distribution parameters for a single functional group.
''' <seealso cref="IDistributionParamsData"/>.
''' <seealso cref="cEcopathDistributionParamsData"/>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcosimDistributionParamsData
    Implements IDistributionParamsData

    Public Sub New(GroupNumber As Integer,
                   GroupName As String,
                   DistributionType As cMSE.DistributionType,
                   LowerBound As Double,
                   UpperBound As Double,
                   MidPoint As Double)
        Me.GroupNo = GroupNumber
        Me.GroupName = GroupName
        Me.DistributionType = DistributionType
        Me.LowerBound = LowerBound
        Me.UpperBound = UpperBound
        Me.MidPoint = If(MidPoint < 0, cCore.NULL_VALUE, MidPoint)
    End Sub

    Public Property GroupNo As Integer
    Public Property GroupName As String
    Public Property DistributionType As cMSE.DistributionType
    Public Property LowerBound As Double
    Public Property UpperBound As Double
    Public Property MidPoint As Double

End Class

''' ---------------------------------------------------------------------------
''' <summary>
''' Manager for all <see cref="cEcosimDistributionParamsData"/> for the 
''' currently loaded model.
''' <seealso cref="cDistributionParams"/>
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcosimDistributionParams
    Inherits cDistributionParams

    Public DenDepCatchability As List(Of IDistributionParamsData)
    Public SwitchingPower As List(Of IDistributionParamsData)
    Public QBMaxxQBio As List(Of IDistributionParamsData)
    Public PredEffectFeedingTime As List(Of IDistributionParamsData)
    Public OtherMortFeedingTime As List(Of IDistributionParamsData)
    Public MaxRelFeedingTime As List(Of IDistributionParamsData)
    Public FeedingTimeAdjustRate As List(Of IDistributionParamsData)

    Public Sub New(MSE As cMSE, core As EwECore.cCore)
        MyBase.New(MSE, core)
    End Sub

    Public Overrides Sub Defaults()
        If (Me.DenDepCatchability Is Nothing) Then
            Me.DenDepCatchability = New List(Of IDistributionParamsData)
            Me.SwitchingPower = New List(Of IDistributionParamsData)
            Me.QBMaxxQBio = New List(Of IDistributionParamsData)
            Me.PredEffectFeedingTime = New List(Of IDistributionParamsData)
            Me.OtherMortFeedingTime = New List(Of IDistributionParamsData)
            Me.MaxRelFeedingTime = New List(Of IDistributionParamsData)
            Me.FeedingTimeAdjustRate = New List(Of IDistributionParamsData)
        Else
            Me.DenDepCatchability.Clear()
            Me.SwitchingPower.Clear()
            Me.QBMaxxQBio.Clear()
            Me.PredEffectFeedingTime.Clear()
            Me.OtherMortFeedingTime.Clear()
            Me.MaxRelFeedingTime.Clear()
            Me.FeedingTimeAdjustRate.Clear()
        End If
        Me.Load(Nothing)
    End Sub

    Public Overrides Function Load(msg As cMessage) As Boolean

        'loads all the ecosim csv files up and creates instances of lists of structures that hold it all in memory
        Return Me.LoadEcosimParamX(msg, Me.DenDepCatchability, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "DenDepCatchability.csv"), eDistrParamName.DenDepCatchability) And
               Me.LoadEcosimParamX(msg, Me.SwitchingPower, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "SwitchingPower.csv"), eDistrParamName.SwitchingPower) And
               Me.LoadEcosimParamX(msg, Me.QBMaxxQBio, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "QBMaxxQBio.csv"), eDistrParamName.QBMaxxQBio) And
               Me.LoadEcosimParamX(msg, Me.PredEffectFeedingTime, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "PredEffectFeedingTime.csv"), eDistrParamName.PredEffectFeedingTime) And
               Me.LoadEcosimParamX(msg, Me.OtherMortFeedingTime, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "OtherMortFeedingTime.csv"), eDistrParamName.OtherMortFeedingTime) And
               Me.LoadEcosimParamX(msg, Me.MaxRelFeedingTime, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "MaxRelFeedingTime.csv"), eDistrParamName.MaxRelFeedingTime) And
               Me.LoadEcosimParamX(msg, Me.FeedingTimeAdjustRate, cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "FeedingTimeAdjustRate.csv"), eDistrParamName.FeedingTimeAdjustRate)

    End Function

    Public Overrides Function Save() As Boolean

        Return Me.SaveEcoSimParameters2CSV(Me.DenDepCatchability, "DenDepCatchability") And
               Me.SaveEcoSimParameters2CSV(Me.SwitchingPower, "SwitchingPower") And
               Me.SaveEcoSimParameters2CSV(Me.QBMaxxQBio, "QBMaxxQBio") And
               Me.SaveEcoSimParameters2CSV(Me.PredEffectFeedingTime, "PredEffectFeedingTime") And
               Me.SaveEcoSimParameters2CSV(Me.OtherMortFeedingTime, "OtherMortFeedingTime") And
               Me.SaveEcoSimParameters2CSV(Me.MaxRelFeedingTime, "MaxRelFeedingTime") And
               Me.SaveEcoSimParameters2CSV(Me.FeedingTimeAdjustRate, "FeedingTimeAdjustRate")

    End Function

    ''' <summary>
    ''' Given a Ecosim csv object this extracts the data from the current line and uses it to return an EcosimParam structure object
    ''' </summary>
    ''' <param name="csv"></param>
    ''' <returns></returns>
    Private Function ExtractEcosimParam(csv As CsvReader) As cEcosimDistributionParamsData

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

        Return New cEcosimDistributionParamsData(TGroupNumber, TGroupName, TDistributionType, TLowerBound, TUpperBound, TMidPoint)

    End Function

    Private Function LoadEcosimParamX(msg As cMessage,
                                      ByRef ParamList As List(Of IDistributionParamsData),
                                      strPath As String,
                                      ParamName As eDistrParamName) As Boolean

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim TMean As Single
        Dim params(Me.Core.nLivingGroups) As cEcosimDistributionParamsData
        Dim param As cEcosimDistributionParamsData = Nothing
        Dim nGroups As Integer = 0
        Dim bSuccess As Boolean = True
        Dim nPrimaryProducers As Integer = 0

        'Count Primary producers
        For iGrp = 1 To Me.Core.nGroups
            If Me.Core.EcopathGroupInputs(iGrp).IsProducer Then
                nPrimaryProducers += 1
            End If
        Next

        If File.Exists(strPath) Then
            reader = cMSEUtils.GetReader(strPath)
            If (reader IsNot Nothing) Then
                Try
                    ParamList.Clear()
                    csv = New CsvReader(reader, True)
                    While Not csv.EndOfStream
                        param = Me.ExtractEcosimParam(csv)
                        If (param IsNot Nothing) Then
                            Dim bOK As Boolean = False
                            ' Only add with valid group indexes and names
                            If (param.GroupNo >= 1 And param.GroupNo <= Me.Core.nLivingGroups) Then
                                If (String.Compare(param.GroupName, Me.Core.EcopathGroupInputs(param.GroupNo).Name, True) = 0) Then
                                    If Me.Core.EcopathGroupInputs(param.GroupNo).IsProducer And param.DistributionType = cMSE.DistributionType.NotSet And
                                        param.LowerBound = -9999 And param.UpperBound = -9999 Then
                                        'This has been recognised at a primary producer that is correctly parameterised
                                        params(param.GroupNo) = param
                                        bOK = True
                                    ElseIf Me.Core.EcopathGroupInputs(param.GroupNo).IsProducer And (Not param.DistributionType = cMSE.DistributionType.NotSet Or
                                    param.LowerBound <> -9999 Or param.UpperBound <> -9999) Then
                                        'This is a primary producer that is incorrectly parameterised
                                        bOK = False
                                    ElseIf Not Me.Core.EcopathGroupInputs(param.GroupNo).IsProducer Then
                                        'This isn't a primary producer
                                        params(param.GroupNo) = param
                                        nGroups += 1
                                        bOK = True
                                    End If
                                End If
                            End If
                            If (bOK = False) Then
                                cMSEUtils.LogError(msg, "Invalid group " & param.GroupNo & " encountered in " & Path.GetFileName(strPath))
                            End If
                            bSuccess = bSuccess And bOK
                        End If
                    End While
                    csv.Dispose()

                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".LoadEcosimParameters() Exception: " & ex.Message)
                    bSuccess = False
                End Try

                If bSuccess And (nGroups <> Me.Core.nLivingGroups - nPrimaryProducers) Then
                    If nGroups < Me.Core.nLivingGroups - nPrimaryProducers Then
                        cMSEUtils.LogError(msg, String.Format(My.Resources.ERROR_DISTRPARAM_GROUPS_TOOFEW, Path.GetFileName(strPath)))
                    ElseIf nGroups > Me.Core.nLivingGroups - nPrimaryProducers Then 'Check whether there are too many groups in the file
                        cMSEUtils.LogError(msg, String.Format(My.Resources.ERROR_DISTRPARAM_GROUPS_TOOMANY, Path.GetFileName(strPath)))
                    End If
                    bSuccess = False
                End If
            End If
        End If
        cMSEUtils.ReleaseReader(reader)

        ' Complement list with defaults for missing groups
        For iGrp = 1 To Me.Core.nLivingGroups
            If (params(iGrp) Is Nothing) Then
                If ParamName = eDistrParamName.DenDepCatchability Then
                    TMean = Me.Core.EcosimGroupInputs(iGrp).DenDepCatchability
                ElseIf ParamName = eDistrParamName.FeedingTimeAdjustRate Then
                    TMean = Me.Core.EcosimGroupInputs(iGrp).FeedingTimeAdjustRate
                ElseIf ParamName = eDistrParamName.MaxRelFeedingTime Then
                    TMean = Me.Core.EcosimGroupInputs(iGrp).MaxRelFeedingTime
                ElseIf ParamName = eDistrParamName.OtherMortFeedingTime Then
                    TMean = Me.Core.EcosimGroupInputs(iGrp).OtherMortFeedingTime
                ElseIf ParamName = eDistrParamName.PredEffectFeedingTime Then
                    TMean = Me.Core.EcosimGroupInputs(iGrp).PredEffectFeedingTime
                ElseIf ParamName = eDistrParamName.QBMaxxQBio Then
                    TMean = Me.Core.EcosimGroupInputs(iGrp).QBMaxQBio
                ElseIf ParamName = eDistrParamName.SwitchingPower Then
                    TMean = Me.Core.EcosimGroupInputs(iGrp).SwitchingPower
                End If
                If Me.Core.EcopathGroupInputs(iGrp).IsProducer Then
                    params(iGrp) = New cEcosimDistributionParamsData(iGrp, Me.Core.EcopathGroupInputs(iGrp).Name, cMSE.DistributionType.NotSet, -9999, -9999, -9999)
                Else
                    params(iGrp) = New cEcosimDistributionParamsData(iGrp, Me.Core.EcopathGroupInputs(iGrp).Name, cMSE.DistributionType.Triangular, TMean * (1 - 0.1), TMean * (1 + 0.1), TMean)
                End If
            End If
        Next

        For Each param In params
            If (param IsNot Nothing) Then
                Dim grp As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(param.GroupNo)
                ' Only allow living non-producers
                If (grp.IsLiving) Then
                    ParamList.Add(param)
                End If
            End If
        Next

        Return bSuccess

    End Function

    Private Function SaveEcoSimParameters2CSV(params As List(Of IDistributionParamsData), strFileName As String) As Boolean

        Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, strFileName & ".csv"), False)
        Dim bSuccess As Boolean = False

        If (writer Is Nothing) Then Return bSuccess

        Try
            writer.WriteLine("GroupName,GroupNumber,DistributionType,Lower,Upper,Mid")

            For Each entry As IDistributionParamsData In params
                If (TypeOf (entry) Is cEcosimDistributionParamsData) Then
                    Dim param As cEcosimDistributionParamsData = DirectCast(entry, cEcosimDistributionParamsData)
                    writer.WriteLine(cStringUtils.ToCSVField(param.GroupName) & "," &
                                     cStringUtils.ToCSVField(param.GroupNo) & "," &
                                     cStringUtils.ToCSVField(param.DistributionType) & "," &
                                     cStringUtils.ToCSVField(param.LowerBound) & "," &
                                     cStringUtils.ToCSVField(param.UpperBound) & "," &
                                     cStringUtils.ToCSVField(param.MidPoint))
                End If
            Next

            bSuccess = True

        Catch ex As Exception

        End Try
        cMSEUtils.ReleaseWriter(writer)
        Return bSuccess

    End Function

End Class

#End Region ' Ecosim
