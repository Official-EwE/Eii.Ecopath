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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports EwEUtils.Utilities



Public Class cEcospaceASCMapContaminants
    Inherits cEcospaceASCBaseResultsWriter

    Public Sub New()
        MyBase.New()
        Me.vars = New eVarNameFlags() {eVarNameFlags.Concentration}
    End Sub

    Public Overrides Sub Init(theCore As Object)
        MyBase.Init(theCore)
    End Sub

    Protected Overrides Function GetFileName(varname As eVarNameFlags, iGrp As Integer, strExt As String, Optional iModelTimeStep As Integer = cCore.NULL_VALUE) As String
        Return Me.GetGroupFileName(varname, iGrp, strExt, iModelTimeStep)
    End Function

    Public Overrides Function GetGroupFileName(varname As eVarNameFlags, iGrp As Integer, strExt As String, Optional iModelTimeStep As Integer = cCore.NULL_VALUE) As String

        Dim fn As String
        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim timestep As String
        Dim grpName As String

        If iGrp > 0 Then
            grpName = Me.m_core.m_EcopathData.GroupName(iGrp)
        Else
            grpName = "Environment"
        End If

        timestep = cStringUtils.Localize("-{0:00000}", iModelTimeStep)
        fn = cFileUtils.ToValidFileName(cStringUtils.Localize("{0}-{1}{2}.{3}", cin.GetVarName(varname), grpName, timestep, strExt.Replace(".", "")), False)

        Return System.IO.Path.Combine(Me.OutputDirectory, fn.Replace("..", "."))

    End Function

    Protected Overrides Function FirstMap() As Integer
        Return 0
    End Function

    Public Overrides Sub WriteResults(SpaceTimeStepResults As Object)

        'Only if Contaminant Tracer is ON
        If Me.m_core.m_tracerData.EcoSpaceConSimOn Then
            MyBase.WriteResults(SpaceTimeStepResults)
        End If

    End Sub

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.CoreDefaults.ECOSPACE_WRITER_ASC_CONTAMINANTS
        End Get
    End Property

End Class
