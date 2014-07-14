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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cConsWriterPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcosimRunCompletedPostPlugin
    Implements EwEPlugin.IAutoSavePlugin

    Private m_core As cCore = Nothing
    Private m_simds As cEcosimDatastructures = Nothing
    ''' <summary>Array for averaging</summary>
    Private m_consumpt As Single(,) = Nothing

#Region " Generic plug-in bits "

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ndAutosaveSimConsumption"
        End Get
    End Property

#End Region ' Generic plug-in bits

#Region " Autosave implementation "

    Public Property AutoSave As Boolean _
        Implements EwEPlugin.IAutoSavePlugin.AutoSave
        Get
            Return My.Settings.Autosave
        End Get
        Set(value As Boolean)
            My.Settings.Autosave = value
            My.Settings.Save()
        End Set
    End Property

    Public Function AutoSaveName() As String _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveName
        Return "Consumption matrices"
    End Function

    Public Function AutoSaveSubPath() As String _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveSubPath
        Return ""
    End Function

    Public Function AutoSaveType() As EwEUtils.Core.eAutosaveTypes _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveType
        Return eAutosaveTypes.Ecosim
    End Function

#End Region ' Autosave implementation

#Region " Ecosim integration "

    Public Sub EcosimInitialized(EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        Try
            Me.m_simds = DirectCast(EcosimDatastructures, cEcosimDatastructures)
            ReDim Me.m_consumpt(Me.m_core.nGroups, Me.m_core.nGroups)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub EcosimRunCompletedPost(EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimRunCompletedPostPlugin.EcosimRunCompletedPost
        Me.m_consumpt = Nothing
    End Sub

    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer, Ecosimresults As Object) _
        Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        Try
            If (Me.AutoSave) Then
                Me.SaveDataToFile(iTime, True)
                Me.SaveDataToFile(iTime, False)
            End If
        Catch ex As Exception

        End Try

    End Sub

#End Region ' Ecosim integration

#Region " Internals "

    Private Function SaveDataToFile(ByVal iTime As Integer, _
                                    ByVal bAnnual As Boolean) As Boolean

        Dim strPath As String = Me.m_core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecosim)
        Dim strFileName As String = Me.GetOutputFileName(strPath, bAnnual, iTime)
        Dim strModelDetails As String = Me.GetModelDetails()
        Dim strDataDetails As String = "Data,Consumption"
        Dim data As Single(,) = Me.m_simds.Consumpt

        'Me.m_simds.ResultsOverTime()
        If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFileName)) Then Return False

        If bAnnual Then
            If (iTime Mod cCore.N_MONTHS) = 0 Then
                For i As Integer = 1 To Me.m_core.nGroups
                    For j As Integer = 1 To Me.m_core.nGroups
                        Me.m_consumpt(i, j) += data(i, j)
                    Next
                Next
                ' Exit
                Return True
            End If
            ' Calc mean and fall through
            For i As Integer = 1 To Me.m_core.nGroups
                For j As Integer = 1 To Me.m_core.nGroups
                    Me.m_consumpt(i, j) /= cCore.N_MONTHS
                    data = Me.m_consumpt
                Next
            Next
        End If

        Try
            'Overwritten the file
            Using sw As StreamWriter = New StreamWriter(strFileName, False)

                If Me.m_core.SaveWithFileHeader Then
                    sw.WriteLine(strModelDetails)
                    sw.WriteLine(strDataDetails)
                    sw.WriteLine()
                End If

                For i As Integer = 1 To Me.m_core.nGroups
                    If i > 1 Then sw.Write(",")
                    sw.Write(cStringUtils.ToCSVField(Me.m_core.EcoPathGroupInputs(i).Name))
                Next
                sw.WriteLine()
                For j As Integer = 1 To Me.m_core.nGroups
                    For i As Integer = 1 To Me.m_core.nGroups
                        If i > 1 Then sw.Write(", ")
                        sw.Write(cStringUtils.FormatSingle(data(j, i)))
                    Next
                    sw.WriteLine()
                Next
                sw.Close()

            End Using

        Catch ex As Exception
            Return False
        End Try
        Return True

    End Function

    Private Function GetOutputFileName(ByVal strPath As String, _
                                       ByVal bSaveAnnual As Boolean, _
                                       ByVal iTime As Integer) As String

        Dim strFileName As String = ""
        Dim strExt As String = ".csv"

        If bSaveAnnual Then
            strFileName = String.Format("Consumption_annual_{0:0000}", iTime)
        Else
            strFileName = String.Format("Consumption_{0:0000}", iTime)
        End If

        Return Path.Combine(strPath, cFileUtils.ToValidFileName(strFileName, False) & strExt)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get default model details to report in output file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetModelDetails() As String
        Return Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecosim)
    End Function

#End Region ' Internals

End Class
