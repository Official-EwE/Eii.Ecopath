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
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports EwECore
Imports EwECore.Ecosim

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of core variables.
    ''' </summary>
    ''' <remarks>
    ''' <para>This class tries to obtain a string from the ScientificShared resources
    ''' to describe a <see cref="eVarNameFlags">core variable</see>. The string is
    ''' expected to be formatted as follows:</para>
    ''' <para>VARIABLE_[varname] = "[symbol]|[abbr]|[name]|[description]"</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cEcosimResultTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescriptor(ByVal value As Object, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter.GetDescriptor

            Select Case DirectCast(value, cEcosimResultWriter.eResultTypes)
                Case cEcosimResultWriter.eResultTypes.AvgWeightOrProdCons : Return My.Resources.HEADER_PRODCONS
                Case cEcosimResultWriter.eResultTypes.Biomass : Return My.Resources.HEADER_BIOMASS
                Case cEcosimResultWriter.eResultTypes.ConsumptionBiomass : Return My.Resources.HEADER_CONSUMPTION_OVER_BIOMASS
                Case cEcosimResultWriter.eResultTypes.FeedingTime : Return My.Resources.HEADER_FEEDINGTIME
                Case cEcosimResultWriter.eResultTypes.Mortality : Return "Mortality"
                Case cEcosimResultWriter.eResultTypes.PredationMortality : Return My.Resources.HEADER_PREDMORT
                Case cEcosimResultWriter.eResultTypes.Prey : Return My.Resources.HEADER_PREY_PERCENTAGE
                Case cEcosimResultWriter.eResultTypes.Value : Return My.Resources.HEADER_VALUE
                Case cEcosimResultWriter.eResultTypes.Yield : Return My.Resources.HEADER_CATCH
                Case cEcosimResultWriter.eResultTypes.TL : Return "Trophic levels"
            End Select
            Return ""

        End Function

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(cEcosimResultWriter.eResultTypes)
        End Function

    End Class

End Namespace ' Style
