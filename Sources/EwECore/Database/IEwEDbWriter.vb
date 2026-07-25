' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Common interface implemented by every writer that can be returned from
    ''' <see cref="cEwEDatabase.GetWriter">GetWriter</see>: the original
    ''' DataAdapter-based <see cref="cEwEDatabase.cEwEDbWriter">cEwEDbWriter</see>
    ''' (Access/OleDb), the reflection-driven <see cref="cEwEEFDbWriter">cEwEEFDbWriter</see>
    ''' (Entity Framework / SQLite), and <see cref="cEwEVersusDbWriter">cEwEVersusDbWriter</see>
    ''' which drives both at once for migration validation.
    ''' </summary>
    ''' <remarks>
    ''' <para>Note that <c>Connect</c> is intentionally NOT part of this interface.
    ''' Each implementation connects in its own way as part of construction; nothing
    ''' outside the constructor ever calls Connect again, so there is no need to force
    ''' a common signature for it.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Interface IEwEDbWriter
        Inherits IDisposable

        ''' <summary>Reference count, mirroring the bookkeeping <see cref="cEwEDatabase.GetWriter">GetWriter</see>
        ''' and <see cref="cEwEDatabase.ReleaseWriter">ReleaseWriter</see> perform.</summary>
        Property RefCount As Integer

        ''' <summary>Commit all pending changes without closing the writer.</summary>
        Function Commit() As Boolean

        ''' <summary>Commits (optional) and releases the writer's underlying resources.</summary>
        Function Disconnect(Optional bSaveChanges As Boolean = True) As Boolean

        ''' <summary>Whether the writer is currently connected to its backing store.</summary>
        Function IsConnected() As Boolean

        ''' <summary>Whether the writer has been disposed.</summary>
        Function IsDisposed() As Boolean

        ''' <summary>Returns an empty row to populate values into (not yet added; call <see cref="AddRow"/>).</summary>
        Function NewRow() As DataRow

        ''' <summary>Adds a row previously obtained from <see cref="NewRow"/> to the pending set.</summary>
        Sub AddRow(drow As DataRow)

        ''' <summary>Marks a row for removal.</summary>
        Function RemoveRow(drow As DataRow) As Boolean

        ''' <summary>Returns an arbitrary row maintained by the writer.</summary>
        Function GetRow(nRow As Integer) As DataRow

        ''' <summary>Gets the DataTable backing this writer.</summary>
        Function GetDataTable() As DataTable

        ''' <summary>Gets the name of the table this writer is connected to.</summary>
        Function GetTableName() As String

    End Interface

End Namespace
