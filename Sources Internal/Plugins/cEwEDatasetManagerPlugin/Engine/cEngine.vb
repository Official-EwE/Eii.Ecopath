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
Imports EwECore
Imports EwECore.SpatialData

Public Class cEngine

    Private m_core As cCore = Nothing

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Public Function Switch(strFile As String) As Boolean

        Dim bSuccess As Boolean = True
        Dim man As cSpatialDataSetManager = Me.m_core.SpatialDataConnectionManager.DatasetManager
        Dim bHasModel As Boolean = (Me.m_core.DataSource IsNot Nothing)

        Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure)
        Try
            bSuccess = man.Load(strFile, True)
        Catch ex As Exception
            bSuccess = False
        End Try
        Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace, bSuccess)

        Return bSuccess
    End Function

End Class
