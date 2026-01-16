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
''' <para>Database update 6.60.0.23:</para>
''' <para>
''' Fixed potential ecospace connection storage problem
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_23
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600023!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed potential pedigree saving problem"
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

        Dim dic As New Dictionary(Of Integer, String)
        Dim bSuccess As Boolean = True

        ' Copy Pedigree.Description to dict indexed by PedigreeID
        Dim reader As IDataReader = db.GetReader("SELECT * FROM Pedigree")
        While reader.Read()
            dic(CInt(reader("LevelID"))) = CStr(db.ReadSafe(reader, "Description", ""))
        End While
        db.ReleaseReader(reader)

        ' Delete Pedigree.Description column
        bSuccess = bSuccess And db.Execute("ALTER TABLE Pedigree DROP COLUMN Description")

        ' Create Pedigree.Description column
        bSuccess = bSuccess And db.Execute("ALTER TABLE Pedigree ADD COLUMN Description LONGTEXT")

        For Each id As Integer In dic.Keys
            db.Execute(String.Format("UPDATE Pedigree SET DESCRIPTION='{0}' WHERE LevelID={1}", dic(id), id))
        Next
        Return bSuccess

    End Function

End Class
