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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' CSV writer for Value Chain results.
''' </summary>
Public Class cResultWriter

    Private m_data As cData = Nothing
    Private m_results As cResults = Nothing

    Public Sub New(ByVal data As cData, ByVal results As cResults)
        Me.m_data = data
        Me.m_results = results
    End Sub

    Public Function WriteResults(iTimeStep As Integer) As Boolean
        Return Me.WriteResults(iTimeStep, 0, "")
    End Function

    Public Function WriteResults(iTimeStep As Integer, iItem As Integer, strItem As String) As Boolean

        Dim strFile As String = Me.GetFileName(iTimeStep, strItem)
        Dim sw As StreamWriter = Nothing

        If String.IsNullOrWhiteSpace(strFile) Then Return False

        Try
            sw = New StreamWriter(strFile, False)
        Catch ex As Exception
            Return False
        End Try

        ' Write header
        sw.Write("Unit, Type")
        For Each v As cResults.eVariableType In [Enum].GetValues(GetType(cResults.eVariableType))
            sw.Write(",")
            sw.Write(v.ToString)
        Next
        sw.WriteLine("")

        For Each u As cUnit In Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
            sw.Write(u.Name)
            sw.Write(",")
            sw.Write(u.UnitType.ToString)
            For Each v As cResults.eVariableType In [Enum].GetValues(GetType(cResults.eVariableType))
                sw.Write(",")
                sw.Write(Me.m_results.Result(u, v, iTimeStep, iItem, cResults.GetVariableContributionType(v)))
            Next
            sw.WriteLine("")
        Next
        sw.Close()

    End Function

    Private Function GetFileName(iTimeStep As Integer, strItem As String) As String

        Dim strPath As String = ""
        Dim strFile As String = ""

        Select Case m_results.RunType
            Case cModel.eRunTypes.Ecopath
                strPath = Me.m_data.Core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecopath, strPrefix:="ValueChain_")
                strFile = "results.csv"
            Case cModel.eRunTypes.Ecosim
                strPath = Me.m_data.Core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecosim, strPrefix:="ValueChain_")

                If String.IsNullOrWhiteSpace(strItem) Then
                    strFile = String.Format("results_{0}.csv", iTimeStep)
                Else
                    strFile = String.Format("results_{0}_{1}.csv", iTimeStep, strItem)
                End If
            Case cModel.eRunTypes.Equilibrium
                Return ""
                'strPath = Me.m_data.Core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecopath, strPrefix:="ValueChain_")
        End Select

        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then Return ""

        Return Path.Combine(strPath, cFileUtils.ToValidFileName(strFile, False))

    End Function

End Class
