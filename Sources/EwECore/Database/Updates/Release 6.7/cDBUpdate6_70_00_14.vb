' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports EwECore.Database

 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.14:</para>
''' <para>
''' Added new effort attraction model.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_14
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700014!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added effort attraction model"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Go for it.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN UsePenaltySearch BYTE")
        db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN NoFishWeight SINGLE")
        db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN PenaltyPower SINGLE")
        db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN FirstPenaltyMonth SINGLE")
        db.Execute("ALTER TABLE EcospaceScenarioGroup ADD COLUMN FTarget SINGLE")
        Return True
    End Function

End Class
