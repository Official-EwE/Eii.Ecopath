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
''' <para>Database update 6.60.0.24:</para>
''' <para>
''' Fixed potential ecosampler storage problem
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_24
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600024!
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
    ''' This update is pretty sad. In some cases we found Access database fields 
    ''' with AllowZeroLength = False flags set. This flag cannot be toggled through
    ''' SQL, and requires recreating the field. That happens below. Eeek.
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Try
            Dim name As String = db.GetIndexName("EcopathSample", "Hash")
            If (Not String.IsNullOrWhiteSpace(name)) Then
                ' Index name bracketed, just in case of conflicts with reserved words
                Return db.Execute("DROP INDEX [" & name & "] ON EcopathSample")
            End If
        Catch ex As Exception
            ' Life is fantastic. Carry on.
        End Try
        Return True

    End Function

End Class
