#Region " Imports "

Option Strict On

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Plugin interface that defines all functionality required to receive a user
''' interface UI context.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IUIContextPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Implement this plug-in point to receive a user interface context. See
    ''' ScientificInterfaceShared > Controls > cUIContext for a full description
    ''' of this object.
    ''' </summary>
    ''' <param name="uic"></param>
    ''' -----------------------------------------------------------------------
    Sub UIContext(ByVal uic As Object)

End Interface
