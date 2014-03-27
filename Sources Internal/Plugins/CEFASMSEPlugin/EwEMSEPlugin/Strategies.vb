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
Imports LumenWorks.Framework.IO.Csv
Imports EwECore
Imports EwEUtils.Utilities
Imports EwEUtils.Core

#End Region ' Imports 

''' <summary>
''' Class to wrap a list of Strategies into an object
''' </summary>
''' <remarks>Strategies "Is A" list of Strategy objects</remarks>
Public Class Strategies
    Inherits List(Of Strategy)

    Private mdataDir As String
    Private mName As String
    Private mMSE As cMSE
    Private mCore As cCore

    Sub New(MSE As cMSE, Core As cCore)
        mMSE = MSE
        mCore = Core
    End Sub

    Public Property DataDirectory As String
        Get
            Return mdataDir
        End Get
        Set(value As String)
            Me.mdataDir = value
        End Set
    End Property

    ''' <summary>
    ''' Overwrite default behaviour to delete the Strategy file when removing a Strategy from the list
    ''' </summary>
    ''' <param name="ZeroBasedIndex">Zero based index of the Strategy to remove</param>
    Public Shadows Sub RemoveAt(ByVal ZeroBasedIndex As Integer)
        Try
            Dim strategy As Strategy = Me.Item(ZeroBasedIndex)
            MyBase.RemoveAt(ZeroBasedIndex)

            If File.Exists(strategy.FileName) Then
                File.Delete(strategy.FileName)
            End If

        Catch ex As Exception
            Debug.Assert(False, Me.ToString + ".RemoveAt() Exception: " + ex.Message)
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resolve a name and index to a <see cref="cEcoPathGroupInput"/> instance.
    ''' </summary>
    ''' <param name="strName">The name to resolve.</param>
    ''' <param name="iIndex">The index to resolve.</param>
    ''' <returns>A <see cref="cEcoPathGroupInput"/> instance, or Nothing if
    ''' the index or name did not match any of the present groups.</returns>
    ''' <remarks>Note that name comparison is not case sensitive.</remarks>
    ''' -----------------------------------------------------------------------
    Private Function ResolveGroup(strName As String, iIndex As Integer) As cEcoPathGroupInput
        If (iIndex < 1) Or (iIndex > Me.mCore.nGroups) Then Return Nothing
        Dim grp As cEcoPathGroupInput = Me.mCore.EcoPathGroupInputs(iIndex)
        If String.Compare(grp.Name, strName, True) <> 0 Then
            Return Nothing
        End If
        Return grp
    End Function

    Public Function SaveHCRs() As Boolean
        Dim csvStrategyFile As StreamWriter = Nothing
        Dim strFile As String = ""
        Dim strPath As String = ""
        Dim msg As cMessage = Nothing
        Dim breturn As Boolean = True
        Try

            For Each Strategy In Me

                If msg Is Nothing Then
                    strPath = Path.GetDirectoryName(Strategy.FileName)
                    msg = New cMessage(String.Format(My.Resources.STATUS_SAVED_STRATEGIES, My.Resources.CAPTION, strPath), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                    msg.Hyperlink = strPath
                End If
                'Save the Strategy to file
                'The filename was passed into the Strategy in its constructor
                Strategy.Save()

                'Save the Regulations that are part of the Strategy
                'Done here instead of inside the Strategy.Save() for clarity 
                Strategy.Regulations.Save(Strategy.FileName)

            Next
        Catch ex As Exception
            breturn = False
            'Both the Strategy.Save() and  Strategy.Regulations.Save() will throw exceptions out to here
            Me.mCore.Messages.SendMessage(New cMessage("Exception saving Strategies to file.", eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Warning))
        End Try

        If msg IsNot Nothing Then
            Me.mCore.Messages.SendMessage(msg)
        End If

        Return breturn
    End Function


    Public Function LoadHCRsFromCSV() As Boolean

        Dim StrategiesFileNames As String()
        Dim Strategy As Strategy
        Dim datadir As String = cMSEUtils.MSEFolder(mMSE.DataPath, cMSEUtils.eMSEPaths.Strategies)
        Dim strVal As String = ""
        Dim StratCounter As Integer = 1
        Dim bReadStrat As Boolean
        Dim bReadReg As Boolean
        Dim lstFailedFiles As New List(Of String)
        'Get an array of strings giving the path to each HCR
        ' JS 30Sep13: Only read CSV files
        StrategiesFileNames = Directory.GetFiles(datadir, "*.csv")

        For Each StrategyFile As String In StrategiesFileNames 'loop through reading each HCR file

            Strategy = New Strategy(Path.GetFileNameWithoutExtension(StrategyFile), StratCounter, StrategyFile, mCore, mMSE)

            'Save the Strategy to the file pass into its constructor
            bReadStrat = Strategy.Read()
            bReadReg = Strategy.Regulations.Read(Strategy.FileName)

            If bReadStrat And bReadReg Then
                'Only add the Strategy if it read both strategy and regulations from file
                Me.Add(Strategy)
            Else
                'keep track for the files that failed to read
                lstFailedFiles.Add(StrategyFile)
            End If
            StratCounter += 1
        Next StrategyFile

        'Warn the user if anything failed
        If lstFailedFiles.Count > 0 Then
            Me.mCore.Messages.SetMessageLock()
            For Each File In lstFailedFiles
                Me.mCore.Messages.SendMessage(New cMessage("Cefas MSE Failed to read strategy file '" + File + "'",
                                                          eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Information))
            Next
            Me.mCore.Messages.RemoveMessageLock()
        End If

        Return True


        'Dim StrategiesFileNames As String()
        'Dim csvHCR As CsvReader
        'Dim tempHCRGroup As HCR_Group
        'Dim Strategy As Strategy = Nothing
        'Dim datadir As String = cMSEUtils.MSEFolder(mMSE.DataPath, cMSEUtils.eMSEPaths.Strategies)
        'Dim strVal As String = ""
        'Dim StratCounter As Integer = 1

        ''Get an array of strings giving the path to each HCR
        '' JS 30Sep13: Only read CSV files
        'StrategiesFileNames = Directory.GetFiles(datadir, "*.csv")

        'For Each HCRFileName As String In StrategiesFileNames 'loop through reading each HCR file

        '    ' ToDo_JS: make robust
        '    Dim reader As StreamReader = cMSEUtils.GetReader(HCRFileName)
        '    If (reader IsNot Nothing) Then
        '        csvHCR = New CsvReader(reader, True)
        '        'Create the new Strategy with the Filename as the strategy name
        '        Strategy = New Strategy(Path.GetFileNameWithoutExtension(HCRFileName), StratCounter, HCRFileName, mCore, mMSE)
        '        StratCounter += 1
        '        Try
        '            Do Until csvHCR.EndOfStream
        '                If csvHCR.ReadNextRecord() Then

        '                    'Data row

        '                    'Read all fields from csv and then add to the list that makes up the whole strategy
        '                    'csv.ReadNextRecord()
        '                    'Each HCR Group needs to be a new object
        '                    tempHCRGroup = New HCR_Group(mCore)

        '                    ' Resolve group
        '                    tempHCRGroup.GroupB = Me.ResolveGroup(csvHCR(0), cStringUtils.ConvertToInteger(csvHCR(1)))
        '                    tempHCRGroup.LowerLimit = cStringUtils.ConvertToDouble(csvHCR(2))
        '                    tempHCRGroup.UpperLimit = cStringUtils.ConvertToDouble(csvHCR(3))
        '                    tempHCRGroup.GroupF = Me.ResolveGroup(csvHCR(4), cStringUtils.ConvertToInteger(csvHCR(5)))
        '                    tempHCRGroup.MaxF = cStringUtils.ConvertToDouble(csvHCR(6))
        '                    'tempHCRGroup.CostFunction = HCR_Group.toCostFunctionEnum(csv(7))

        '                    'tempHCRGroup.GroupName4Biomass = csv(0)
        '                    'tempHCRGroup.GroupNumber4Biomass = csv(1)
        '                    'tempHCRGroup.GroupName4F = csv(4)
        '                    'tempHCRGroup.GroupNumber4F = csv(5)
        '                    'tempHCRGroup.CostFunctionOrg = csv(7)

        '                    ' Only add valid strategies!
        '                    If tempHCRGroup.isValid(strVal) Then
        '                        Strategy.Add(tempHCRGroup)
        '                    End If

        '                End If
        '            Loop
        '            Me.Add(Strategy)

        '        Catch ex As Exception
        '            ' ToDo: decide what to do when CSV data is malformed
        '        End Try
        '        csvHCR.Dispose()
        '    End If

        '    'End While


        '    cMSEUtils.ReleaseReader(reader)
        'Next

        'Return True

    End Function

    Public Shadows Sub Add(StrategyToAdd As Strategy)

        If Not Me.Contains(StrategyToAdd) Then
            MyBase.Add(StrategyToAdd)
        End If

    End Sub

    Public Shadows Function Contains(Item As Strategy) As Boolean

        For Each Strategy As Strategy In Me
            ' JS 30Sep13: made comparison case-insensitive
            If (String.Compare(Item.Name, Strategy.Name, True) = 0) And (String.Compare(Item.FileName, Strategy.FileName, True) = 0) Then
                Return True
            End If
        Next
        Return False

    End Function

End Class
