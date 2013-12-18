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

Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Imports EwEUtils.SystemUtilities.cSystemUtils

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.40.0.03:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added model exclusion layer</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_40_00_04
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.400004!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed capacity map constraints"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True

        If db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers DROP CONSTRAINT " & db.GetPkKeyName("GroupID")) Then
            bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers ADD CONSTRAINT fkGroupID FOREIGN KEY (GroupID) REFERENCES EcospaceScenarioGroup (GroupID)")
        End If

        Me.LogProgress("UpdateEcospaceScenarioCapacityDrivers", bSuccess)

        Return bSuccess

    End Function

End Class
