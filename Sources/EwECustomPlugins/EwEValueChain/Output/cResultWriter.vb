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
Imports EwECore

#End Region ' Imports

''' <summary>
''' CSV writer for Value Chain results.
''' </summary>
Public Class cResultWriter

#Region " Variables "

    Private m_data As cData = Nothing
    Private m_results As cResults = Nothing
    Private m_msg As cMessage = Nothing

#End Region ' Variables

    Public Sub New(ByVal data As cData, ByVal results As cResults)
        Me.m_data = data
        Me.m_results = results
    End Sub

    ''' <summary>
    ''' Write results to CSV file.
    ''' </summary>
    ''' <param name="agg">Data aggregation method in use during the run.</param>
    ''' <returns>True if succesful</returns>
    Public Function WriteResults(ByVal agg As cParameters.eAggregationModeType) As Boolean
        Return Me.WriteResults(agg, 0, "")
    End Function

    Public Function WriteResults(ByVal agg As cParameters.eAggregationModeType, iItem As Integer, strItem As String) As Boolean

        Dim strFile As String = Me.GetFileName(agg, strItem)
        Dim sw As StreamWriter = Nothing
        Dim vs As cVariableStatus = Nothing

        If String.IsNullOrWhiteSpace(strFile) Then Return False

        Try
            sw = New StreamWriter(strFile, False)
        Catch ex As Exception
            Me.m_msg = New cMessage(String.Format("Value chain results failed to save to '{0}': {1}", Path.GetDirectoryName(strFile), ex.Message), _
                               eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.Ecotracer, eMessageImportance.Warning)
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
                sw.Write(Me.m_results.GetTotal(v, New cUnit() {u}, iItem, cResults.GetVariableContributionType(v)))
            Next
            sw.WriteLine("")
        Next
        sw.Close()

        If (Me.m_msg Is Nothing) Then
            Me.m_msg = New cMessage(String.Format("Value chain results saved to '{0}'", Path.GetDirectoryName(strFile)), _
                               eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.Ecotracer, eMessageImportance.Information)
            Me.m_msg.Hyperlink = Path.GetDirectoryName(strFile)
        End If

        vs = New cVariableStatus(eStatusFlags.OK, String.Format("Saved '{0}'", strFile), EwEUtils.Core.eVarNameFlags.NotSet, EwEUtils.Core.eDataTypes.NotSet, EwEUtils.Core.eCoreComponentType.External, 0)
        Me.m_msg.AddVariable(vs)

        Return True

    End Function

    Private Function GetFileName(ByVal agg As cParameters.eAggregationModeType, ByVal strItem As String) As String

        Dim strPath As String = ""
        Dim strFile As String = ""

        Select Case m_results.RunType
            Case cModel.eRunTypes.Ecopath
                strPath = Me.m_data.Core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecopath)
            Case cModel.eRunTypes.Ecosim
                strPath = Me.m_data.Core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecosim)
            Case cModel.eRunTypes.Equilibrium
                Return ""
                'strPath = Me.m_data.Core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecopath, strPrefix:="ValueChain_")
        End Select

        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then Return ""

        If String.IsNullOrWhiteSpace(strItem) Then
            strFile = String.Format("valuechain_{0}.csv", agg.ToString())
        Else
            strFile = String.Format("valuechain_{0}_{1}.csv", agg.ToString(), strItem)
        End If

        Return Path.Combine(strPath, cFileUtils.ToValidFileName(strFile, False))

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the save results message.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property Message As cMessage
        Get
            Return Me.m_msg
        End Get
    End Property

End Class
