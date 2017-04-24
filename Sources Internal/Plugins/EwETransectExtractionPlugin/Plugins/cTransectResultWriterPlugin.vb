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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cTransectResultWriterPlugin
    Inherits cEcospaceBaseResultsWriter
    Implements IEcospaceResultWriterPlugin

    Private m_data As cTransectDatastructures = Nothing

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Init(theCore As Object) Implements IPlugin.Initialize
        MyBase.Init(theCore)
        Me.m_core = DirectCast(theCore, cCore)
        Me.m_data = cTransectDatastructures.Instance(Me.m_core)
    End Sub

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return "Transects writer"
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "TransectsResultWriter"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public Overrides Sub StartWrite()
        For Each t As cTransect In Me.m_data.Transects
            t.InitRun(Me.m_core)
        Next
    End Sub

    Public Overrides Sub WriteResults(SpaceTimeStepResults As Object)
        For Each t As cTransect In Me.m_data.Transects
            t.Record(DirectCast(SpaceTimeStepResults, cEcospaceTimestep))
        Next
    End Sub

    Public Overrides Sub EndWrite()

        Dim msg As cMessage = Nothing

        Try
            ' Create output dir
            Me.CreateOutputDir()
            ' Write it all
            Me.WriteResult()

            ' ToDo: globalize this method

            ' Notify user
            msg = New cMessage("Ecospace results across transects have been saved to '" & Me.OutputDirectory & "'",
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            msg.Hyperlink = Me.OutputDirectory
        Catch ex As Exception
            ' Notify user of error
            msg = New cMessage("Ecospace results across transects could not be saved to '" & Me.OutputDirectory & "'. " & ex.Message,
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
        End Try
        ' Done
        Me.m_core.Messages.SendMessage(msg)

    End Sub

    Protected Overrides Function FileExtension() As String
        Return ".csv"
    End Function

#Region " Internals "

    Private Sub WriteResult()

        Dim sw As StreamWriter = Nothing
        Dim strName As String = ""
        Dim strFile As String = ""
        Dim strDescriptor As String = ""
        Dim sValue As Single = 0

        For Each t As cTransect In Me.m_data.Transects
            For Each avg As eEcospaceResultsAverageType In [Enum].GetValues(GetType(eEcospaceResultsAverageType))

                strFile = Me.FileName(t, avg)

                Try
                    ' Start writing
                    sw = New StreamWriter(Path.Combine(Me.OutputDirectory, strFile))
                    If Me.m_core.SaveWithFileHeader Then
                        sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
                        sw.WriteLine(cStringUtils.ToCSVField("Transect") & "," & cStringUtils.ToCSVField(t.Name))
                        sw.WriteLine(cStringUtils.ToCSVField("Start-lat") + "," & cStringUtils.ToCSVField(t.Start.Y))
                        sw.WriteLine(cStringUtils.ToCSVField("Start-lon") + "," & cStringUtils.ToCSVField(t.Start.X))
                        sw.WriteLine(cStringUtils.ToCSVField("End-lat") + "," & cStringUtils.ToCSVField(t.End.Y))
                        sw.WriteLine(cStringUtils.ToCSVField("End-lon") + "," & cStringUtils.ToCSVField(t.End.X))
                        sw.WriteLine(cStringUtils.ToCSVField("Number of cells") & "," & cStringUtils.ToCSVField(t.NumCells))
                        sw.WriteLine()
                    End If

                    Me.WriteData(sw, t, avg)

                    ' Clean up
                    sw.Flush()
                    sw.Close()
                    sw.Dispose()

                Catch ex As Exception
                    cLog.Write(ex, "Failed to write Ecospace average biomass to file for data " + strDescriptor)
                End Try

            Next
        Next

    End Sub

    Private Function FileName(t As cTransect, avg As eEcospaceResultsAverageType) As String
        Dim strFileName As String = ""
        Select Case avg
            Case eEcospaceResultsAverageType.TimeStep
                strFileName = "Ecospace_Average_"
            Case eEcospaceResultsAverageType.Annual
                strFileName = "Ecospace_Annual_Average_"
        End Select
        Return cFileUtils.ToValidFileName(strFileName & t.Name & ".csv", False)
    End Function

    Private Sub WriteData(sw As StreamWriter, t As cTransect, avg As eEcospaceResultsAverageType)

        ' Write data header
        If avg = eEcospaceResultsAverageType.Annual Then
            sw.Write("Year")
        Else
            sw.Write("TimeStep")
        End If
        sw.WriteLine(",row,col,group,biomass,catch")

        Dim iYear As Integer = Me.m_core.EcosimFirstYear + CInt(Math.Truncate((Me.FirstOutputTimeStep - 1) / Me.m_core.m_EcoSpaceData.nTimeStepsPerYear))
        Dim bSave As Boolean

        ' Loop over all the time steps. 
        ' If in Annual mode then sum and average the at the end of the year
        '    For iTime As Integer = Me.FirstOutputTimeStep To Me.m_core.nEcospaceTimeSteps
        '        Dim cells As Point() = t.Cells(Me.m_core.EcospaceBasemap)
        '        Dim value(cells.Length, Me.m_core.nGroups, 2) As Single
        '        For iCell As Integer = 0 To cells.Length - 1
        '            Dim cell As Point = cells(iCell)
        '            For iGroup As Integer = 1 To Me.m_core.nGroups
        '                For iRslt As Integer = 0 To 1
        '                    value(iCell, iGroup, iRslt) += t.Result(iTime, iGroup, iRslt, iCell)
        '                    Select Case avg
        '                        Case eEcospaceResultsAverageType.Annual
        '                            If ((iTime Mod Me.m_core.m_EcoSpaceData.nTimeStepsPerYear) = 0) Then
        '                                value(iCell, iGroup, iRslt) /= 12
        '                                bSave = True
        '                            End If
        '                        Case eEcospaceResultsAverageType.TimeStep
        '                            bSave = True
        '                        Case Else
        '                            Debug.Assert(False)
        '                    End Select
        '                Next iRslt

        '                If bSave Then
        '                    iYear += 1
        '                    sw.WriteLine("{0},{1},{2},{3}", IIF(avg = eEcospaceResultsAverageType.Annual, iYear, iTime), cell.Y, cell.X, iGroup, cStringUtils.FormatNumber(value(iCell, iGroup, 0)), cStringUtils.FormatNumber(value(iCell, iGroup, 1)))
        '                    bSave = False
        '                End If

        '            Next iGroup
        '        Next iCell
        '    Next iTime
    End Sub

#End Region ' Internals

End Class
