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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Collections.Generic
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Core
Imports ValueChain

#End Region ' Imports

Public Interface IGraphView

    Sub SetData(strGraphTitle As String,
                strXAxisLabel As String,
                strYAxisLabel As String,
                aVars() As cValueChainResults.eVariableType)

End Interface
