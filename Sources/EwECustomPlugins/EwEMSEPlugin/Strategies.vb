' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.IO
Imports EwECore
Imports EwECore.Common

''' <summary>
''' Class to wrap a list of Strategies into an object
''' </summary>
''' <remarks>Strategies "Is A" list of <see cref="Strategy">Strategy objects</see></remarks>
Public Class Strategies
    Inherits List(Of Strategy)
    Implements IMSEData

    Private m_strName As String = ""
    Private m_MSE As cMSE = Nothing
    Private m_Core As cCore = Nothing

    Sub New(MSE As cMSE, Core As cCore)
        Me.m_MSE = MSE
        Me.m_Core = Core
    End Sub

    ''' <summary>
    ''' Overwrite default behaviour to delete the Strategy file when removing a Strategy from the list
    ''' </summary>
    ''' <param name="ZeroBasedIndex">Zero based index of the Strategy to remove</param>
    Public Shadows Sub RemoveAt(ZeroBasedIndex As Integer)
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

    Public Function HCRExistsForGroup(iGrp As Integer) As Boolean

        For Each iStrategy In Me
            For iHCR = 1 To iStrategy.Count
                'If iStrategy.Item(iHCR - 1).GroupF.DBID = iGrp Then
                If iStrategy.Item(iHCR - 1).GroupF.Index = iGrp Then
                    Return True
                End If
            Next
        Next
        Return False

    End Function

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
        If (iIndex < 1) Or (iIndex > Me.m_Core.nGroups) Then Return Nothing
        Dim grp As cEcoPathGroupInput = Me.m_Core.EcopathGroupInputs(iIndex)
        If String.Compare(grp.Name, strName, True) <> 0 Then
            Return Nothing
        End If
        Return grp
    End Function

    Public Shadows Sub Add(StrategyToAdd As Strategy)

        If Not Me.Contains(StrategyToAdd) Then
            MyBase.Add(StrategyToAdd)
        End If

    End Sub

    Public Shadows Function Contains(Item As Strategy) As Boolean

        If Me.IndexOf(Item) > 0 Then
            Return True
        End If
        Return False

    End Function

    Public Sub Defaults() Implements IMSEData.Defaults
        Me.Clear()
    End Sub

    Public Function IsChanged() As Boolean _
        Implements IMSEData.IsChanged
        ' ToDo: return something useful here
        Return False
    End Function

    Public Function Load(Optional msg As cMessage = Nothing,
                         Optional strFilename As String = "") As Boolean _
        Implements IMSEData.Load

        Dim StrategiesFileNames As String()
        Dim Strategy As Strategy
        Dim strStategyDir As String = cMSEUtils.MSEFolder(Me.m_MSE.DataPath, cMSEUtils.eMSEPaths.Strategies)
        Dim strRegulationDir As String = cMSEUtils.MSEFolder(Me.m_MSE.DataPath, cMSEUtils.eMSEPaths.Regulations)
        Dim StratCounter As Integer = 1
        Dim bSuccess As Boolean = True
        Dim StrategyFileWithoutHCRReg As String
        Dim StrategyFileWithoutExtension As String

        If Not Directory.Exists(strStategyDir) Then Return False

        'Get an array of strings giving the path to each HCR
        ' JS 30Sep13: Only read CSV files
        StrategiesFileNames = Directory.GetFiles(strStategyDir, "*.csv")

        Me.Defaults()

        For Each StrategyFile As String In StrategiesFileNames 'loop through reading each HCR file

            StrategyFileWithoutHCRReg = StrategyFile.Replace("_hcr", "").Replace("_reg", "")
            StrategyFileWithoutExtension = Path.GetFileNameWithoutExtension(StrategyFile).Replace("_hcr", "").Replace("_reg", "")

            Strategy = New Strategy(StrategyFileWithoutExtension, StratCounter, StrategyFile, Me.m_Core, Me.m_MSE)

            'Only add the Strategy if it read both strategy and regulations from file
            If Strategy.Load(msg) And Strategy.Regulations.Load(msg, Path.Combine(strRegulationDir, Path.GetFileNameWithoutExtension(StrategyFile) & ".csv")) Then
                Me.Add(Strategy)
            Else
                bSuccess = False
            End If

            StratCounter += 1
        Next StrategyFile

        Return bSuccess

    End Function

    Public Function Save(Optional strFilename As String = "") As Boolean Implements IMSEData.Save

        Dim strRegulationDir As String = cMSEUtils.MSEFolder(Me.m_MSE.DataPath, cMSEUtils.eMSEPaths.Regulations)
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
                Strategy.Regulations.Save(Path.Combine(strRegulationDir, Path.GetFileName(Strategy.FileName.Replace("hcr", "reg"))))

            Next
        Catch ex As Exception
            breturn = False
            'Both the Strategy.Save() and  Strategy.Regulations.Save() will throw exceptions out to here
            Me.m_Core.Messages.SendMessage(New cMessage("Exception saving Strategies to file.", eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Warning))
        End Try

        If msg IsNot Nothing Then
            Me.m_Core.Messages.SendMessage(msg)
        End If

        Return breturn

    End Function

    Public Function FileExists(Optional strFilename As String = "") As Boolean _
        Implements IMSEData.FileExists
        Return File.Exists(strFilename)
    End Function

    Public Shadows Function IndexOf(Item As Strategy) As Integer
        Dim iPos As Integer = 0
        For Each Strategy As Strategy In Me
            'case-insensitive
            If (String.Compare(Item.Name, Strategy.Name, True) = 0) And (String.Compare(Item.FileName, Strategy.FileName, True) = 0) Then
                Return iPos
            End If
            iPos += 1
        Next
        Return -1

    End Function

    Public Shadows Sub Sort()
        Debug.Assert(False, Me.ToString + ".Sort() Sorry at this time you can't sort the list of Strategies because cMSE.Run() assumes they are in the same order as they were loaded.")
    End Sub

End Class
