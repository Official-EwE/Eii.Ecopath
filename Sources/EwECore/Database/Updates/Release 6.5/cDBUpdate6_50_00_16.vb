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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Option Strict On
Imports EwEUtils.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.16:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Some old models created from development versions of 
''' <see cref="cDBUpdate6_01_01_010"/> contain incomplete taxonomy tables. This 
''' update re-applies some of the database schema changes as a precaution.
''' </description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_16
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500016!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed possible taxon table errors"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim astrColumns As String() = New String() {"CodeISCAAP", "CodeTaxon", "Code3A", "ClassName", "OrderName", "FamilyName", "GenusName", "SpeciesName", "CommonName", "SourceName", "SourceKey", "LastUpdated", "EcologyType", "OrganismType", "Exploited", "ConservationStatus", "OccurrenceStatus", "OccurenceStatus", "MeanWeight", "MeanLength", "MaxLength", "MeanLifeSpan", "VulnerabiltyIndex"}
        Dim bSucces As Boolean = True

        ' Remove obsolete fields from EcopathGroupTaxon
        For Each strColumn As String In astrColumns
            db.Execute("ALTER TABLE EcopathGroupTaxon DROP COLUMN " & strColumn)
        Next
        ' Make sure proportion of catch field is available in EcopathGroupTaxon
        db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN PropCatch SINGLE")

        ' Remove obsolete fields from EcopathStanzaTaxon
        db.Execute("ALTER TABLE EcopathStanzaTaxon DROP COLUMN Proportion")
        db.Execute("ALTER TABLE EcopathStanzaTaxon DROP COLUMN PropCatch")

        Return True

    End Function

End Class
