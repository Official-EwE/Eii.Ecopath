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
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

#End Region ' Imports 

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.18:</para>
''' <para>
''' Ensure default Pedigree levels
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_18
    Inherits cDBUpdate

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cDBUpdate6_60_00_18)()

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600018!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Ensured default pedigree levels"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim reader As IDataReader = db.GetReader("SELECT * FROM Pedigree ORDER BY Sequence ASC")
        Dim hasLevels(cEcopathDataStructures.PedigreeVariables.Length) As Boolean
        Dim bSuccess As Boolean = True

        Try

            While reader.Read()

                Dim var As eVarNameFlags = cin.GetVarName(CStr(reader("VarName")))
                ' fudge, no need to issue a database update
                If var = eVarNameFlags.Biomass Then var = eVarNameFlags.BiomassAreaInput
                Dim i As Integer = Array.IndexOf(cEcopathDataStructures.PedigreeVariables, var)
                If (i >= 0) Then hasLevels(i) = True
            End While
            db.ReleaseReader(reader)

            Dim iNextID As Integer = CInt(db.GetValue("SELECT MAX(LevelID) FROM Pedigree")) + 1

            For i As Integer = 0 To cEcopathDataStructures.PedigreeVariables.Length - 1
                Dim var As eVarNameFlags = cEcopathDataStructures.PedigreeVariables(i)
                If (Not hasLevels(i)) Then
                    bSuccess = bSuccess And Me.CreateDefaults(db, var, iNextID)
                End If
            Next
        Catch ex As Exception
            m_logger.LogError(ex, "DB update 65.60018")
        End Try

        Return bSuccess

    End Function

    Private Function CreateDefaults(db As cEwEDatabase, var As eVarNameFlags, ByRef iNextID As Integer) As Boolean

        Dim bSuccess As Boolean = True

        Select Case var

            Case eVarNameFlags.Biomass, eVarNameFlags.BiomassAreaInput

                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_ESTIMATED]", 0, 80)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_OTHERMODEL]", 0.0, 80)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_GUESSTIMATE]", 0.0, 80)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_APPROX_INDIRECT]", 0.4, 50)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_SAMPLING_LOW]", 0.7, 30)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_SAMPLING_HIGH]", 1.0, 10)

            Case eVarNameFlags.PBInput, eVarNameFlags.QBInput

                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_ESTIMATED]", 0, 80)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_GUESSTIMATE]", 0.1, 70)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_OTHERMODEL]", 0.2, 60)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_EMPERICAL]", 0.5, 50)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_SIM_SIM]", 0.6, 40)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_SIM_SAME]", 0.7, 30)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_SAME_SIM]", 0.8, 20)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_SAME_SAME]", 1.0, 10)

            Case eVarNameFlags.DietComp

                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_GENERAL_SIM]", 0.0, 80)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_OTHERMODEL]", 0.0, 80)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_GENERAL_SAME]", 0.2, 60)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_QUALDC]", 0.5, 50)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_QUANDC_LIM]", 0.7, 30)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_QUANDC_DET]", 1.0, 10)

            Case eVarNameFlags.TCatchInput

                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_GUESSTIMATE]", 0.1, 70)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_OTHERMODEL]", 0.1, 70)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_FAO]", 0.2, 80)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_NATIONAL]", 0.5, 50)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_LOCAL_LOW]", 0.7, 30)
                bSuccess = bSuccess And Me.Add(db, iNextID, var, "[PEDIGREE_DEFAULT_LOCAL_HIGH]", 1.0, 10)

        End Select
        Return bSuccess

    End Function

    Private Function Add(db As cEwEDatabase, ByRef iLevelID As Integer, var As eVarNameFlags, resName As String, sIndexValue As Single, iConfidenceInterval As Integer) As Boolean

        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim strSQL As String = String.Format("INSERT INTO Pedigree (LevelID, Sequence, LevelName, Description, VarName, IndexValue, Confidence) " &
                                             "VALUES ({0}, {1}, '{2}', '{3}', '{4}', {5}, {6})",
                                             iLevelID, iLevelID, resName, resName, CStr(cin.GetVarName(var)), cStringUtils.FormatSingle(sIndexValue), iConfidenceInterval)
        iLevelID += 1
        Return db.Execute(strSQL)

    End Function

End Class
