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
Option Explicit On

Imports System.IO
Imports System.Collections.ObjectModel
Imports EwECore
Imports EwEUtils.Utilities
Imports LumenWorks.Framework.IO.Csv
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Public Class cDiets
    Implements IMSEData

#Region " Internal Variables "

    Private m_core As cCore
    Private m_MSE As cMSE
    Private m_meanProportions(,) As Single
    Private m_interacts(,) As Integer
    Private m_dietPropMultipliers() As Double

#End Region

#Region " Construction initialiaztion"

    Public Sub New(MSE As cMSE, core As EwECore.cCore)
        Me.m_core = core
        Me.m_MSE = MSE
        ReDim m_meanProportions(m_core.nLivingGroups, m_core.nGroups)
        ReDim m_dietPropMultipliers(m_core.nLivingGroups)
        ReDim m_interacts(m_core.nLivingGroups, m_core.nGroups)
        Me.Defaults()
    End Sub

#End Region

#Region " Properties "

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
        For iPred As Integer = 1 To m_core.nLivingGroups
            For iPrey As Integer = 1 To m_core.nGroups
                mean = m_core.EcoPathGroupInputs(iPred).DietComp(iPrey)
                Me.m_meanProportions(iPred - 1, iPrey - 1) = mean
                Me.m_interacts(iPred - 1, iPrey - 1) = cSystemUtils.IIF(mean > 0, 1, 0)
            Next
            Me.m_dietPropMultipliers(iPred - 1) = 1.0
        Next

    End Sub

    Public Function IsChanged() As Boolean Implements IMSEData.IsChanged
        Return True
    End Function

    Public Function Load(Optional msg As cMessage = Nothing, _
                         Optional strFilename As String = "") As Boolean Implements IMSEData.Load

        ' Ignore filename param
        strFilename = Me.DefaultFileName("DietComposition.csv")

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim bSuccess As Boolean = True

        'Read in the values from the DietComposition.csv into each array
        If File.Exists(strFilename) Then
            reader = cMSEUtils.GetReader(strFilename)
            If (reader IsNot Nothing) Then
                csv = New CsvReader(reader, True)
                Try
                    While Not csv.EndOfStream
                        If csv.ReadNextRecord() Then

                            ' JS: diets are saved as follows:
                            '     Predator header row with imports
                            '     Rows for each prey
                            ' The reading logic does not reflect this

                            ' JS to discuss with MP. For now, Imports rows are ignored
                            If (String.Compare(csv(2), "imports", True) <> 0) Then
                                'Note about indices for interacts, lower and upper
                                'The 1st index for predator runs from 0 and each element is equal to the same element+1 in mcore.ecopathgroupinputs
                                'The 2nd index for prey runs from zero, where zero is the imports and then every other index is identical to mcore.ecopathgroupinputs
                                m_interacts(cStringUtils.ConvertToInteger(csv(2)) - 1, cStringUtils.ConvertToInteger(csv(3))) = cStringUtils.ConvertToInteger(csv(4))
                                m_meanProportions(cStringUtils.ConvertToInteger(csv(2)) - 1, cStringUtils.ConvertToInteger(csv(3))) = cStringUtils.ConvertToSingle(csv(5))
                            Else
                                ' Skip import row
                            End If
                        End If
                    End While
                Catch ex As Exception
                    cMSEUtils.LogError(msg, "DietComposition cannot load from " & strFilename & ". " & ex.Message)
                    bSuccess = False
                End Try
                csv.Dispose()
                cMSEUtils.ReleaseReader(reader)
            End If
        End If

        strFilename = Me.DefaultFileName("DietCompositionMultipliers.csv")
        reader = cMSEUtils.GetReader(strFilename)
        If (reader IsNot Nothing) Then
            'Read in the values from the DietCompositionMultipliers.csv
            csv = New CsvReader(reader, True)
            Try
                Do While Not csv.EndOfStream
                    If csv.ReadNextRecord() Then
                        m_dietPropMultipliers(cStringUtils.ConvertToInteger(csv(0)) - 1) = cStringUtils.ConvertToInteger(csv(2))
                    End If
                Loop
            Catch ex As Exception
                cMSEUtils.LogError(msg, "DietComposition multipliers cannot load from " & strFilename & ". " & ex.Message)
            End Try
            csv.Dispose()
            cMSEUtils.ReleaseReader(reader)
        End If

        Return bSuccess

    End Function

    Public Function Save(Optional strFilename As String = "") As Boolean Implements IMSEData.Save

        ' Ignore strFilename parameter
        strFilename = Me.DefaultFileName("DietComposition.csv")

        Dim writer As StreamWriter = cMSEUtils.GetWriter(strFilename, False)
        Dim bSuccess As Boolean = False

        If (writer Is Nothing) Then Return bSuccess

        writer.Write("Predator,Prey,PredIndex,PreyIndex,Interacts,Mean")
        writer.WriteLine()

        ' ToDo: write diets properly
        For iPred As Integer = 1 To m_core.nLivingGroups
            If m_core.EcoPathGroupInputs(iPred).ImpDiet > 0 Then
                Dim mean As Single = m_core.EcoPathGroupInputs(iPred).ImpDiet
                writer.WriteLine(cStringUtils.ToCSVField(m_core.EcoPathGroupInputs(iPred).Name) & ",Imports," & iPred & ",0,1," & cStringUtils.ToCSVField(mean))
            Else
                writer.WriteLine(cStringUtils.ToCSVField(m_core.EcoPathGroupInputs(iPred).Name) & ",Imports," & iPred & ",0,0,0")
            End If

            For iPrey As Integer = 1 To m_core.nGroups
                Dim mean As Single = Me.m_meanProportions(iPred - 1, iPrey - 1)
                Dim interact As Integer = Me.m_interacts(iPred - 1, iPrey - 1)
                writer.WriteLine(cStringUtils.ToCSVField(m_core.EcoPathGroupInputs(iPred).Name) & "," & cStringUtils.ToCSVField(m_core.EcoPathGroupInputs(iPrey).Name) & "," & iPred & "," & iPrey & "," & cStringUtils.ToCSVField(interact) & "," & cStringUtils.ToCSVField(mean))
            Next
        Next
        cMSEUtils.ReleaseWriter(writer)

        strFilename = Me.DefaultFileName("DietCompositionMultipliers.csv")
        writer = cMSEUtils.GetWriter(strFilename, False)
        If (writer IsNot Nothing) Then
            writer.WriteLine("PredatorIndexNumber,PredatorIndexName,Multiplier")
            For iPred As Integer = 1 To m_core.nLivingGroups
                writer.WriteLine("{0},{1},{2}", _
                                 cStringUtils.ToCSVField(iPred), _
                                 cStringUtils.ToCSVField(m_core.EcoPathGroupInputs(iPred).Name), _
                                 cStringUtils.ToCSVField(Me.DietPropMultipliers(iPred)))
            Next
        Else
            bSuccess = False
        End If
        cMSEUtils.ReleaseWriter(writer)
        Return bSuccess

    End Function

    Private Function DefaultFileName(strBit As String) As String
        Return cMSEUtils.MSEFile(m_MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, strBit)
    End Function

    Public Function FileExists(Optional strFilename As String = "") As Boolean Implements IMSEData.FileExists
        ' Ignore file name parameter
        Return File.Exists(Me.DefaultFileName("DietComposition.csv")) And _
               File.Exists(Me.DefaultFileName("DietCompositionMultipliers.csv"))
    End Function

End Class
