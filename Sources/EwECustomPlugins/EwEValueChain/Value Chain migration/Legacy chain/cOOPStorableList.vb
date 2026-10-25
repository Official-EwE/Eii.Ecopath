Imports System.ComponentModel

Namespace ValueChainMigrator.LegacyData

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Strong-typed list for cOOPStorable instances.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cOOPStorableList
        Inherits cOOPStorable
        Implements IList(Of cOOPStorable)

        Private ReadOnly m_list As New List(Of cOOPStorable)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add an <see cref="cOOPStorable">object</see> to the list.
        ''' </summary>
        ''' <param name="item">The <see cref="cOOPStorable">object</see> to add.</param>
        ''' -------------------------------------------------------------------
        Public Sub Add(item As cOOPStorable) _
            Implements ICollection(Of cOOPStorable).Add
            Debug.Assert(Not Me.Contains(item), "Item already present in list")
            Me.m_list.Add(item)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clear the list.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Clear() _
            Implements ICollection(Of cOOPStorable).Clear
            Me.m_list.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the list contains a given <see cref="cOOPStorable">object</see>.
        ''' </summary>
        ''' <param name="item">The <see cref="cOOPStorable">object</see> to find.</param>
        ''' <returns>True if the list contains this <see cref="cOOPStorable">object</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Function Contains(item As cOOPStorable) As Boolean _
            Implements ICollection(Of cOOPStorable).Contains
            Return Me.m_list.Contains(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Copy the list to an array, starting at a given array index.
        ''' </summary>
        ''' <param name="array">The array to copy <see cref="cOOPStorable">object</see> into.</param>
        ''' <param name="arrayIndex">Index of the list item to start copying from.</param>
        ''' <remarks>Please make sure the receiving array is big enough 
        ''' to hold the list items.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub CopyTo(array() As cOOPStorable, arrayIndex As Integer) _
            Implements ICollection(Of cOOPStorable).CopyTo
            Me.m_list.CopyTo(array, arrayIndex)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the number of <see cref="cOOPStorable">objects</see> in the list.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(False)>
        Public ReadOnly Property Count() As Integer _
            Implements ICollection(Of cOOPStorable).Count
            Get
                Return Me.m_list.Count
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether the list can be modified.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(False)>
        Public ReadOnly Property IsReadOnly() As Boolean _
            Implements ICollection(Of cOOPStorable).IsReadOnly
            Get
                Return False
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove an <see cref="cOOPStorable">object</see> from the list.
        ''' </summary>
        ''' <param name="item">The <see cref="cOOPStorable">object</see> to remove.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Remove(item As cOOPStorable) As Boolean _
            Implements ICollection(Of cOOPStorable).Remove
            ' ToDo: remember this to actively erase item from DB?
            Me.m_list.Remove(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns an enumerator for cartwheeling though this list.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetEnumerator() As IEnumerator(Of cOOPStorable) _
            Implements IEnumerable(Of cOOPStorable).GetEnumerator
            Return Me.m_list.GetEnumerator()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the list index of a given <see cref="cOOPStorable">object</see>.
        ''' </summary>
        ''' <param name="item">The <see cref="cOOPStorable">object</see> to locate.</param>
        ''' <returns>An integer value representing the index of the 
        ''' <see cref="cOOPStorable">object</see> in the list, or -1 if this item
        ''' was not found.</returns>
        ''' -------------------------------------------------------------------
        Public Function IndexOf(item As cOOPStorable) As Integer _
            Implements IList(Of cOOPStorable).IndexOf
            Return Me.m_list.IndexOf(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Insert an <see cref="cOOPStorable">object</see> at a given position in the list.
        ''' </summary>
        ''' <param name="index">The list position to insert the item at.</param>
        ''' <param name="item">The <see cref="cOOPStorable">object</see> to insert.</param>
        ''' -------------------------------------------------------------------
        Public Sub Insert(index As Integer, item As cOOPStorable) _
            Implements IList(Of cOOPStorable).Insert
            Debug.Assert(Not Me.Contains(item), "Item already present in list")
            Me.m_list.Insert(index, item)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cOOPStorable">object</see> at a given position in the list.
        ''' </summary>
        ''' <param name="index">The list index of the <see cref="cOOPStorable">object</see> to retrieve.</param>
        ''' -------------------------------------------------------------------
        <Browsable(False)>
        Default Public Property Item(index As Integer) As cOOPStorable _
            Implements IList(Of cOOPStorable).Item
            Get
                Return Me.m_list.Item(index)
            End Get
            Set(value As cOOPStorable)
                Me.m_list.Item(index) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove an <see cref="cOOPStorable">object</see> at a given position in the list.
        ''' </summary>
        ''' <param name="index">The position of the <see cref="cOOPStorable">object</see> to remove.</param>
        ''' -------------------------------------------------------------------
        Public Sub RemoveAt(index As Integer) _
            Implements IList(Of cOOPStorable).RemoveAt
            Me.m_list.RemoveAt(index)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Neh
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Function GetEnumaarghAarghAargh() As System.Collections.IEnumerator _
            Implements System.Collections.IEnumerable.GetEnumerator
            Return Nothing
        End Function

    End Class

End Namespace