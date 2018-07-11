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

#Region " Imports "

Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Utilities

#End Region ' Imports 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.09:</para>
''' <para>
''' Allow pedigree values outside predefined levels.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_09
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600009!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Allow pedigree values outside predefined levels"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        ' Add field and populate it
        Dim bSuccess As Boolean = db.Execute("ALTER TABLE EcopathGroupPedigree ADD COLUMN Confidence INTEGER")
        Dim s As String = "UPDATE EcopathGroupPedigree INNER JOIN Pedigree ON EcopathGroupPedigree.LevelID = Pedigree.LevelID SET EcopathGroupPedigree.Confidence = Pedigree.Confidence"
        bSuccess = bSuccess And db.Execute(s)

        ' Drop FKs - pffff
        db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetFkKeyName("EcopathGroup", "EcopathGroupPedigree", "GroupID"))
        db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetFkKeyName("EcopathGroupPedigree", "EcopathGroup", "GroupID"))
        db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetFkKeyName("Pedigree", "EcopathGroupPedigree", "LevelID"))
        db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetFkKeyName("EcopathGroupPedigree", "Pedigree", "LevelID"))
        ' Drop PK
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetPkKeyName("EcopathGroupPedigree"))
        ' Rebuild PK
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupPedigree ADD PRIMARY KEY (GroupID, VarName)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupPedigree ADD FOREIGN KEY (GroupID) REFERENCES EcopathGroup(GroupID)")

        ' Once LevelID is no longer used in any ey, remove it
        s = db.GetIndexName("EcopathGroupPedigree", "LevelID")
        If Not String.IsNullOrWhiteSpace(s) Then
            bSuccess = bSuccess And db.Execute("DROP INDEX " & db.GetIndexName("EcopathGroupPedigree", "LevelID") & " ON EcopathGroupPedigree")
        End If
        ' Access still thinks this field is part of indexes. Perhaps the transaction needs to be committed first?
        db.Execute("ALTER TABLE EcopathGroupPedigree DROP COLUMN LevelID")

        Return bSuccess

    End Function


End Class
