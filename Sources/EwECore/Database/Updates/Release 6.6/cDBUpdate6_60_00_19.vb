' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database



''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.19:</para>
''' <para>
''' Added three more taxonomic codes
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_19
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600019!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added AquaMaps, OBIS, and WoRMS taxonomic code fields"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        ' This may fail, no worries
        db.Execute("ALTER TABLE EcopathTaxon DROP COLUMN CodeTaxon")
        ' But this has to work!
        Return db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN CodeAquaMaps TEXT(255)") And
               db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN CodeAphia TEXT(255)") And
               db.Execute("ALTER TABLE EcopathTaxon ADD COLUMN CodeOBIS LONG")

    End Function

End Class
