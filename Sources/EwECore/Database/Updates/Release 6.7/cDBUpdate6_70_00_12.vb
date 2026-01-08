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
Imports EwECore.Database

#End Region ' Imports 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.12:</para>
''' <para>
''' External data can now start and end at a specific date, not at a year.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_12
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700012!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "External data can now start and end at a specific date, not at a year"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' At some pont, the on-board EwE database templates received an erroneous
    ''' index on a value column. This update removes the index if it exists.
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns>Always true</returns>
    ''' <remarks>This update is re-issued because the index error returned in the
    ''' on-board database templates, thus re-instating the error. Good lord.</remarks>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        db.Execute("ALTER TABLE EcospaceScenarioDataConnection DROP COLUMN StartYear")
        db.Execute("ALTER TABLE EcospaceScenarioDataConnection DROP COLUMN EndYear")

        Return db.Execute("ALTER TABLE EcospaceScenarioDataConnection ADD COLUMN CustomDateStart TEXT(10)") And
               db.Execute("ALTER TABLE EcospaceScenarioDataConnection ADD COLUMN CustomDateEnd TEXT(10)")
    End Function

End Class
