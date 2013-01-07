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
