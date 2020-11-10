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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' A biomass-to-conversion rule
''' </summary>
''' -----------------------------------------------------------------------
Public Class cComplexityRule

#Region " Construction "

    Public Sub New()
        ' NOP
    End Sub

    Public Sub New(strName As String, a As Single, b As Single, c As Single)
        Me.Name = strName
        Me.A = a
        Me.B = b
        Me.C = c
    End Sub

#End Region ' Construction

#Region " Public bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of a rule.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the group in the rule
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Group As Integer = -1

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the A parameter of the rule, as in Ax²+bx+c
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property A As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the B parameter of the rule, as in ax²+Bx+c
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property B As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the C parameter of the rule, as in ax²+bx+C
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property C As Single

    Public Function IsValid() As Boolean
        Return (Group > 0)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculates the architectural complexisty of a cell based on a species biomass.
    ''' </summary>
    ''' <param name="Biomass"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Returned values are expressed in cubic cm per surface unit
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function ArchitecturalComplexity(Biomass As Single) As Single
        Return CSng(Math.Max(A * (Biomass ^ 2) + B * Biomass + C, 0))
    End Function

#End Region ' Public bits

#Region " Overrides "

    Public Overridable Function IsDefault() As Boolean
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Make default look pretty
    ''' </summary>
    ''' <returns>A pretty name for the default rule</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ToString() As String
        Return String.Format("{0}: {1}{2}x²{3}{4}x{5}{6}",
                             Me.Name,
                             If(Me.A < 0, "-", ""), Me.A,
                             If(Me.B < 0, "-", "+"), Me.B,
                             If(Me.C < 0, "-", "+"), Me.C)
    End Function

#End Region ' Overrides

End Class
