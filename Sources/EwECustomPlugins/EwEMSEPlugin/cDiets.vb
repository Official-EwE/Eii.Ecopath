' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.IO
Imports EwECore
Imports EwEUtils.Utilities
Imports LumenWorks.Framework.IO.Csv

Public Class cDiets
    Implements IMSEData

#Region " Internal Variables "

    Private m_core As cCore
    Private m_MSE As cMSE
    Private m_meanProportions(,) As Single
    Private m_interacts(,) As Integer '(Pred, Prey)
    Private m_meanProportions_imports() As Single
    Private m_interacts_imports() As Integer
    Private m_dietPropMultipliers() As Double

#End Region

#Region " Construction initialisation"

    Public Sub New(MSE As cMSE, core As EwECore.cCore)
        Me.m_core = core
        Me.m_MSE = MSE
        ReDim Me.m_meanProportions(Me.m_core.nLivingGroups - 1, Me.m_core.nGroups - 1)
        ReDim Me.m_dietPropMultipliers(Me.m_core.nLivingGroups - 1)
        ReDim Me.m_interacts(Me.m_core.nLivingGroups - 1, Me.m_core.nGroups - 1)
        ReDim Me.m_meanProportions_imports(Me.m_core.nLivingGroups - 1)
        ReDim Me.m_interacts_imports(Me.m_core.nLivingGroups - 1)
        Me.Defaults()
    End Sub

#End Region

#Region " Properties "

    Public ReadOnly Property Core As cCore
        Get
            Return Me.m_core
        End Get
    End Property

    ''' <summary>
    ''' Mean diet proportions (by predator x prey). Note that predator and prey indices are ZERO-based!
    ''' </summary>
    Public ReadOnly Property MeanProportions As Single(,)
        Get
            Return Me.m_meanProportions
        End Get
    End Property

    ''' <summary>
    ''' Number of diet interactions (by predator x prey). Note that predator and prey indices are ZERO-based!
    ''' </summary>
    Public ReadOnly Property Interacts As Integer(,)
        Get
            Return Me.m_interacts
        End Get
    End Property

    Public ReadOnly Property MeanProportionsImports As Single()
        Get
            Return Me.m_meanProportions_imports
        End Get
    End Property

    Public ReadOnly Property InteractsImports As Integer()
        Get
            Return Me.m_interacts_imports
        End Get
    End Property

    ''' <summary>
    ''' Diet proportion multipliers (by predator). Note that predator indices are ZERO-based!
    ''' </summary>
    Public ReadOnly Property DietPropMultipliers As Double()
        Get
            Return Me.m_dietPropMultipliers
        End Get
    End Property

#End Region

    Public Sub Defaults() _
        Implements IMSEData.Defaults

        Dim mean As Single = 0

        ' Set proper defaults in-memory
        For iPred As Integer = 1 To Me.m_core.nLivingGroups
            mean = Me.m_core.EcopathGroupInputs(iPred).ImpDiet
            Me.m_meanProportions_imports(iPred - 1) = mean
            'Me.m_meanProportions(iPred - 1, 0) = mean
            'Me.m_interacts(iPred - 1, 0) = IF(mean > 0, 1, 0)
            Me.m_interacts_imports(iPred - 1) = If(mean > 0, 1, 0)
            For iPrey As Integer = 1 To Me.m_core.nGroups
                mean = Me.m_core.EcopathGroupInputs(iPred).DietComp(iPrey)
                Me.m_meanProportions(iPred - 1, iPrey - 1) = mean
                Me.m_interacts(iPred - 1, iPrey - 1) = If(mean > 0, 1, 0)
            Next
            Me.m_dietPropMultipliers(iPred - 1) = 1.0
        Next

    End Sub

    Public Function IsChanged() As Boolean Implements IMSEData.IsChanged
        Return True
    End Function

    Public Function Load(Optional msg As cMessage = Nothing,
                         Optional strFilename As String = "") As Boolean Implements IMSEData.Load

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim bSuccess As Boolean = True

        strFilename = Me.DefaultFileName()
        reader = cMSEUtils.GetReader(strFilename)
        If (reader IsNot Nothing) Then
            'Read in the values from the DietCompositionMultipliers.csv
            csv = New CsvReader(reader, True)
            Try
                Do While Not csv.EndOfStream
                    If csv.ReadNextRecord() Then
                        Me.m_dietPropMultipliers(cStringUtils.ConvertToInteger(csv(0)) - 1) = cStringUtils.ConvertToInteger(csv(2))
                    End If
                Loop
            Catch ex As Exception
                cMSEUtils.LogError(msg, "DietComposition multipliers cannot load from " & strFilename & ". " & ex.Message)
            End Try
            csv.Dispose()
        End If
        cMSEUtils.ReleaseReader(reader)

        Return bSuccess

    End Function

    Public Function Save(Optional strFilename As String = "") As Boolean Implements IMSEData.Save

        Dim writer As StreamWriter = Nothing
        Dim bSuccess As Boolean = False

        strFilename = Me.DefaultFileName()
        writer = cMSEUtils.GetWriter(strFilename, False)
        If (writer IsNot Nothing) Then
            writer.WriteLine("PredatorIndexNumber,PredatorIndexName,Multiplier")
            For iPred As Integer = 1 To Me.m_core.nLivingGroups
                writer.WriteLine("{0},{1},{2}",
                                 cStringUtils.ToCSVField(iPred),
                                 cStringUtils.ToCSVField(Me.m_core.EcopathGroupInputs(iPred).Name),
                                 cStringUtils.ToCSVField(Me.DietPropMultipliers(iPred - 1)))
            Next
        Else
            bSuccess = False
        End If
        cMSEUtils.ReleaseWriter(writer)
        Return bSuccess

    End Function

    Private Function DefaultFileName() As String
        Return cMSEUtils.MSEFile(Me.m_MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "DietCompositionMultipliers.csv")
    End Function

    Public Function FileExists(Optional strFilename As String = "") As Boolean _
        Implements IMSEData.FileExists
        If (String.IsNullOrWhiteSpace(strFilename)) Then
            strFilename = Me.DefaultFileName
        End If
        Return File.Exists(strFilename)
    End Function

End Class
