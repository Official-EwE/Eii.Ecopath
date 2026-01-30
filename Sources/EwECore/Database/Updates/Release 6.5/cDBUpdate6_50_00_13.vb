' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.13:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Add EcoBase metadata fields to the EwE model.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_13
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500013!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added EcoBase metadata fields"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True

        ' Add EcoBase metadata fields the EwE model
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathModel ADD COLUMN Country TEXT(64)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathModel ADD COLUMN EcosystemType TEXT(255)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathModel ADD COLUMN CodeEcobase TEXT(50)")

        Return bSuccess

    End Function

End Class
