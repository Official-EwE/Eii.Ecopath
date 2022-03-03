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
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEUtils.Utilities

#End Region ' Imports 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.09:</para>
''' <para>
''' Fixed potential ecosampler storage problem - reissued, because the on-board
''' database templates had the issue again.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_10
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.70001!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added fleet nominal effort"
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

        Return db.Execute("ALTER TABLE EcopathFleet ADD COLUMN NominalEffort SINGLE")

    End Function

End Class
