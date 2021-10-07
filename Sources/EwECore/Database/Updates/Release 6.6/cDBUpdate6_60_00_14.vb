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

#Region " Imports "

Option Strict On
Imports EwEUtils.Database

#End Region ' Imports 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.14:</para>
''' <para>
''' Brought back shared foraging arenas
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_14
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600014!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Brought back shared foraging arenas"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True
        bSuccess = bSuccess And db.Execute("CREATE TABLE EcosimScenarioArena (ScenarioID LONG, PreyID LONG, PredID LONG, PredSharedID LONG, PeatArena SINGLE)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioArena ADD PRIMARY KEY (ScenarioID, PreyID, PredID, PredSharedID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioArena ADD FOREIGN KEY (PreyID) REFERENCES EcosimScenarioGroup(GroupID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioArena ADD FOREIGN KEY (PredID) REFERENCES EcosimScenarioGroup(GroupID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioArena ADD FOREIGN KEY (PredSharedID) REFERENCES EcosimScenarioGroup(GroupID)")

        ' Clean up obsolete fields
        db.Execute("ALTER TABLE EcopathDietComp DROP COLUMN MTI")
        db.Execute("ALTER TABLE EcopathDietComp DROP COLUMN Electivity")
        db.Execute("ALTER TABLE EcosimScenarioForcingMatrix DROP COLUMN FlowType")

        Return bSuccess

    End Function


End Class
