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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports System.IO
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' 
''' </summary>
Public Class cEcospaceRegionResultWriter
    Inherits cEcospaceBaseResultsWriter

    Public Sub New()
    End Sub

    Protected Overrides Sub CreateOutputDir()
        If Me.m_core.m_EcoSpaceData.UseCoreOutputDir Then
            Me.m_TimeStampDirName = Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace)
        Else
            'Use the output directroy set by the user
            Me.m_TimeStampDirName = Me.m_core.OutputPath
        End If

        If (Not cFileUtils.IsDirectoryAvailable(Me.OutputDirectory, True)) Then
            Debug.Assert(False, Me.ToString & ".CreateTimeStampedDir() cannot create directory")
            cLog.Write("Ecospace output writer failed to create directory " & Me.OutputDirectory)
        End If
    End Sub

    Public Overrides Function FileExtension() As String
        Return ".csv"
    End Function

    Public Overrides Sub StartWrite()

        Try
            Me.CreateOutputDir()
        Catch ex As Exception

        End Try

    End Sub

    Public Overrides Sub WriteResults(SpaceTimeStepResults As Object)
        ' NOP - can only write when done
    End Sub

    Public Overrides Sub EndWrite()

        Try
            Me.WriteContent()
        Catch ex As Exception

        End Try

    End Sub

#Region " Internals "

    Private Sub WriteContent()

        Dim r As cEcospaceRegionOutput = Nothing
        Dim sw As StreamWriter = Nothing
        Dim strName As String = ""
        Dim strFile As String = ""
        Dim strDescriptor As String = ""
        Dim sValue As Single = 0

        For iGroup As Integer = 1 To Me.m_core.nGroups
            For iData As Integer = 0 To 1

                strName = Me.EcopathData.GroupName(iGroup)

                If (iData = 0) Then
                    strFile = cFileUtils.ToValidFileName(String.Format("Biomass_{0}.csv", strName), False)
                    strDescriptor = "Biomass by region"
                Else
                    strFile = cFileUtils.ToValidFileName(String.Format("Catch_{0}.csv", strName), False)
                    strDescriptor = "Catch by region"
                End If

                sw = New StreamWriter(Path.Combine(Me.OutputDirectory, strFile))
                sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
                sw.WriteLine("Data," & cStringUtils.ToCSVField(strDescriptor))
                sw.WriteLine("Group," & cStringUtils.ToCSVField(strName))
                sw.WriteLine()

                sw.Write("TimeStep")
                For iRegion As Integer = 0 To Me.m_core.nRegions
                    r = Me.m_core.EcospaceRegionOutput(iRegion)
                    sw.Write("," & cStringUtils.ToCSVField(r.Name))
                Next iRegion
                sw.WriteLine()

                For iTime As Integer = 1 To Me.m_core.nEcospaceTimeSteps
                    sw.Write(iTime)
                    For iRegion As Integer = 0 To Me.m_core.nRegions
                        r = Me.m_core.EcospaceRegionOutput(iRegion)
                        If (iData = 0) Then
                            sValue = r.BiomassByTime(iGroup, iTime)
                        Else
                            ' For now sum all catches. This needs to change so that each landing is reported by region
                            For ifleet As Integer = 1 To Me.m_core.nFleets
                                sValue += r.CatchFleetGroupTime(ifleet, iGroup, iTime)
                            Next
                        End If
                        sw.Write("," & cStringUtils.FormatNumber(sValue))
                    Next iRegion
                    sw.WriteLine()
                Next iTime
                sw.WriteLine()
                sw.Flush()
                sw.Close()
            Next iData
        Next iGroup
    End Sub

#End Region

End Class
