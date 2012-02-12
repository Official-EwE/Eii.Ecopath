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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class EcosimResultsGridIndices
        : Inherits EwEGrid

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Define column headers
            Me.Redim(Me.Core.nEcosimTimeSteps + 1, 5)
            ' Time step
            Me(0, 0) = New EwEColumnHeaderCell("Time step")
            'FIB
            Me(0, 1) = New EwEColumnHeaderCell("FIB")
            'TL Catch
            Me(0, 2) = New EwEColumnHeaderCell("TL Catch")
            'Total catch
            Me(0, 3) = New EwEColumnHeaderCell("Total catch")
            'Kemptons Q
            Me(0, 4) = New EwEColumnHeaderCell("Kemptons Q")

        End Sub

        Protected Overrides Sub FillData()

            Dim sg As cStyleGuide = Me.StyleGuide
            Dim src As cEcosimOutput = Me.Core.EcosimOutputs
            Dim styleVal As cStyleGuide.eStyleFlags = (cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed)

            For iTS As Integer = 1 To Me.Core.nEcosimTimeSteps
                Me(iTS, 0) = New EwECell(iTS, GetType(Integer), cStyleGuide.eStyleFlags.Names)
                Me(iTS, 1) = New EwECell(src.FIB(iTS), GetType(Single), styleVal)
                Me(iTS, 2) = New EwECell(src.TLCatch(iTS), GetType(Single), styleVal)
                Me(iTS, 3) = New EwECell(src.TotalCatch(iTS), GetType(Single), styleVal)
                Me(iTS, 4) = New EwECell(src.KemptonsQ(iTS), GetType(Single), styleVal)
            Next

        End Sub

    End Class

End Namespace
