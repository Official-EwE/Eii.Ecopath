' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.09:</para>
''' <para>
''' Cleaned up pedigree table structure.
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
            Return "Cleaned up pedigree table structure"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True

        ' Remove several possible FKs. This has become a bit of a mess over time
        db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetFkKeyName("EcopathGroup", "EcopathGroupPedigree", "GroupID"))
        db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetFkKeyName("EcopathGroupPedigree", "EcopathGroup", "GroupID"))
        db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetFkKeyName("Pedigree", "EcopathGroupPedigree", "LevelID"))
        db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetFkKeyName("EcopathGroupPedigree", "Pedigree", "LevelID"))
        ' Drop PK
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupPedigree DROP CONSTRAINT " & db.GetPkKeyName("EcopathGroupPedigree"))
        ' Rebuild PK
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupPedigree ADD PRIMARY KEY (GroupID, VarName)")
        ' Rebuild FK on groups
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupPedigree ADD FOREIGN KEY (GroupID) REFERENCES EcopathGroup(GroupID)")

        Dim strIndex As String = db.GetIndexName("EcopathGroupPedigree", "LevelID")
        If Not String.IsNullOrWhiteSpace(strIndex) Then
            ' Remove index on LevelID, if still exists
            bSuccess = bSuccess And db.Execute("DROP INDEX " & db.GetIndexName("EcopathGroupPedigree", "LevelID") & " ON EcopathGroupPedigree")
        End If

        Return bSuccess

    End Function

End Class
