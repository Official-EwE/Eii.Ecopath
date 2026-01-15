' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports System.ComponentModel
Imports EwEUtils.Utilities



''' ===========================================================================
''' <summary>
''' This class holds defaults for <see cref="cProducerUnit">producer units</see>
''' in the Ecost model. Defaults are used as blueprints for spawning their base 
''' class objects.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)),
    Serializable()>
Public Class cProducerUnitDefault
    Inherits cProducerUnit

    <Browsable(False)>
    Public Overrides Property Name() As String
        Get
            Return "Default"
        End Get
        Set(value As String)
        End Set
    End Property

    <Browsable(False)>
    Public Overrides ReadOnly Property Category() As String
        Get
            Return ""
        End Get
    End Property

    <Browsable(False)>
    Public Overrides Property GearCode() As String
        Get
        End Get
        Set(value As String)
        End Set
    End Property

    <Browsable(False)>
    Public Overrides Property Fleet() As EwECore.cEcopathFleetInput
        Get
            Return Nothing
        End Get
        Friend Set(value As EwECore.cEcopathFleetInput)
        End Set
    End Property

End Class
