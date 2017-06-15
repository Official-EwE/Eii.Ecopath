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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Options "

Option Strict On
Imports System.Drawing

#End Region ' Options

Public Class cUnitImageFactory

    Public Shared Function GetImage(ByVal unitType As cUnitFactory.cUnitFormatter, _
                                    ByVal bLarge As Boolean) As Image
        Select Case unitType
            Case cUnitFactory.cUnitFormatter.Producer
                If bLarge Then Return My.Resources.producer
                Return My.Resources.producer_small
            Case cUnitFactory.cUnitFormatter.Processing
                If bLarge Then Return My.Resources.processing
                Return My.Resources.processing_small
            Case cUnitFactory.cUnitFormatter.Distribution
                If bLarge Then Return My.Resources.distribution
                Return My.Resources.distribution_small
            Case cUnitFactory.cUnitFormatter.Wholesaler
                If bLarge Then Return My.Resources.wholesaler
                Return My.Resources.wholesaler_small
            Case cUnitFactory.cUnitFormatter.Retailer
                If bLarge Then Return My.Resources.retailer
                Return My.Resources.retailer_small
            Case cUnitFactory.cUnitFormatter.Consumer
                If bLarge Then Return My.Resources.consumer
                Return My.Resources.consumer_small
        End Select
        Return Nothing
    End Function

End Class
