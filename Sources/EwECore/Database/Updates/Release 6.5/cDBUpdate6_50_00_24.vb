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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Utilities

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.24:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Shapes can have any number of function parameters</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_24
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500024!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Shapes can have any number of function parameters"
        End Get
    End Property

    Private s_tables As String() = New String() {"EcosimShapeTime", "EcosimShapeEggProd", "EcosimShapeMediation"}
    Private s_fields As String() = New String() {"YZero", "YBase", "YEnd", "Steep"}

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True

        For Each strTable As String In s_tables
            Try
                bSuccess = bSuccess And Me.UpdateTable(db, strTable)
            Catch ex As Exception
                bSuccess = False
            End Try
        Next

        Return bSuccess

    End Function

    Private Function UpdateTable(db As cEwEDatabase, strTableName As String) As Boolean

        Dim bSuccess As Boolean = db.Execute("ALTER TABLE " & strTableName & " ADD COLUMN FunctionParams MEMO")
        Dim writer As cEwEDatabase.cEwEDbWriter = db.GetWriter(strTableName)
        Dim dt As DataTable = writer.GetDataTable()
        Dim parms(3) As Single

        For Each drow As DataRow In dt.Rows
            For i As Integer = 0 To 3
                parms(i) = CSng(drow(s_fields(i)))
            Next
            drow.BeginEdit()
            drow("FunctionParams") = cStringUtils.ParamArrayToString(parms)
            drow.EndEdit()
        Next

        db.ReleaseWriter(writer, True)

        For i As Integer = 0 To 3
            bSuccess = bSuccess And db.Execute("ALTER TABLE " & strTableName & " DROP COLUMN " & s_fields(i))
        Next

        Return bSuccess

    End Function

End Class
