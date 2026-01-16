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

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    
    Public Class gridEcosimResultsIndices
        : Inherits cEwEGrid

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Define column headers
            Me.Redim(Me.Core.nEcosimTimeSteps + 1, 5)
            ' Time step
            Me(0, 0) = New cEwEColumnHeaderCell(SharedResources.HEADER_TIME)
            'FIB
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_FIB)
            'TL Catch
            Me(0, 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_TLC)
            'Total catch
            Me(0, 3) = New cEwEColumnHeaderCell(SharedResources.HEADER_TOTALCATCH)
            'Kemptons Q
            Me(0, 4) = New cEwEColumnHeaderCell(SharedResources.HEADER_KEMPTONSQ)

        End Sub

        Protected Overrides Sub FillData()

            Dim sg As cStyleGuide = Me.StyleGuide
            Dim src As cEcosimOutput = Me.Core.EcosimOutputs
            Dim styleVal As cStyleGuide.eStyleFlags = (cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed)

            For iTS As Integer = 1 To Me.Core.nEcosimTimeSteps
                Me(iTS, 0) = New cEwECell(iTS, GetType(Integer), cStyleGuide.eStyleFlags.Names)
                Me(iTS, 1) = New cEwECell(src.FIB(iTS), GetType(Single), styleVal)
                Me(iTS, 2) = New cEwECell(src.TLCatch(iTS), GetType(Single), styleVal)
                Me(iTS, 3) = New cEwECell(src.TotalCatch(iTS), GetType(Single), styleVal)
                Me(iTS, 4) = New cEwECell(src.DiversityIndex(iTS), GetType(Single), styleVal)
            Next

        End Sub

    End Class

End Namespace
