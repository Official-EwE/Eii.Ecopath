' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.120012:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added data adapters scale.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_12_00012
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.120012!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added data adapters scale"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioDataAdapters ADD COLUMN Scale SINGLE")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioDataAdapters ADD COLUMN ScaleType BYTE")
        Return bSuccess

    End Function

End Class
