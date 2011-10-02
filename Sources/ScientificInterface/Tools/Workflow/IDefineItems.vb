Imports EwECore

Public Interface IItemInfo

    Property Item As cCoreInputOutputBase
    Property Status As eItemStatusTypes
    Property IsChanged As Boolean
    Property IsNew As Boolean
    Property IsRemoved As Boolean

    Property Name As String
    Property DBID As Integer

End Interface

''' <summary>
''' Administration for adding, removing and updating core objects that
''' will require data reload after modifications are done.
''' </summary>
Public Interface IDefineItems

    ''' <summary>The items that are being managed</summary>
    Function Items() As IItemInfo()
    ''' <summary>States whether the items are correctly configured, and that no data is missing</summary>
    Function CanApply() As Boolean
    ''' <summary>Create or delete item definitions</summary>
    Function ApplyAddRemove() As Boolean
    ''' <summary>Update items that have only been modified</summary>
    Function ApplyUpdate() As Boolean

    Function CreateItem(ByVal t As Type, ByVal strNameMask As String) As IItemInfo
    Function ToggleDeleteItem(ByVal item As IItemInfo) As Boolean

End Interface
