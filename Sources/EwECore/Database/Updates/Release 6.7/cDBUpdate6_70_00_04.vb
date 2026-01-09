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




Imports EwECore.Database

 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.04:</para>
''' <para>
''' Remember Ecosim and Ecospace capacity driver settings; Ecospace spin-up settings.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_04
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700004!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Remember Ecosim and Ecospace capacity driver settings; Ecospace spin-up settings"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delete orphaned fishing effort shapes
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return Me.AddEcospaceSpinup(db) And
               Me.AddEcospaceScenarioDriverDisabled(db)
    End Function

    Private Function AddEcospaceSpinup(db As cEwEDatabase) As Boolean
        Return db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN UseSpinup BYTE") And
               db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN SpinupYears INTEGER")
    End Function

    Private Function AddEcospaceScenarioDriverDisabled(db As cEwEDatabase) As Boolean
        ' Stored in a separate table instead of EcospaceScenarioDriverLayer because Depth layer needs considering too
        Return db.Execute("CREATE TABLE EcospaceScenarioDriverDisabled (ScenarioID LONG, LayerID LONG)") And
               db.Execute("ALTER TABLE EcospaceScenarioDriverDisabled ADD PRIMARY KEY(ScenarioID, LayerID)") And
               db.Execute("ALTER TABLE EcospaceScenarioDriverDisabled ADD FOREIGN KEY(ScenarioID) REFERENCES EcospaceScenario(ScenarioID)")
    End Function

End Class
