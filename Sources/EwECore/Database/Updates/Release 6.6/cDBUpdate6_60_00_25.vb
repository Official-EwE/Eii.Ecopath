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
''' <para>Database update 6.60.0.25:</para>
''' <para>
''' This update addresses an issue present since the very start of EwE6, where 
''' deleted fleets left behind orphaned fishing effort shapes. This update does
''' not alter the structure of the EwE database.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_25
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600025!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed potential ecosampler saving problem"
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

        Dim bSucces As Boolean = True

        Try
            ' Get IDs for all orphaned shapes
            Dim reader As IDataReader = db.GetReader("SELECT ShapeID FROM EcosimShapeFishRate AS R WHERE NOT EXISTS (SELECT FishRateShapeID FROM EcosimScenarioFleet F WHERE F.FishRateShapeID = R.ShapeID)")
            Dim ShapeIDs As New List(Of Integer)
            While reader.Read
                ShapeIDs.Add(CInt(reader("ShapeID")))
            End While
            db.ReleaseReader(reader)

            ' Delete 'em all
            For Each iShapeID As Integer In ShapeIDs
                bSucces = bSucces And db.Execute(String.Format("DELETE FROM EcosimShapeFishRate WHERE ShapeID={0}", iShapeID))
                bSucces = bSucces And db.Execute(String.Format("DELETE FROM EcosimShape WHERE ShapeID={0}", iShapeID))
            Next

            cLog.Write("DBUpdate_" & CStr(Me.UpdateVersion), "Deleted " & CStr(ShapeIDs.Count) & " orphaned shapes")

        Catch ex As Exception
            ' Life is fantastic. Carry on.
        End Try
        Return True

    End Function

End Class
