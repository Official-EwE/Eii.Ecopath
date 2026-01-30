' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources



Namespace Ecospace

    Public Class cRunEcospacePlotTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(frmRunEcospace.ePlotTypes)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            ' ToDo: globalize this properly
            Select Case DirectCast(value, frmRunEcospace.ePlotTypes)
                Case frmRunEcospace.ePlotTypes.RelB
                    Return SharedResources.HEADER_RELATIVEBIOMASS
                Case frmRunEcospace.ePlotTypes.F
                    Return SharedResources.HEADER_FISHMORT_OVER_TOTMORT
                Case frmRunEcospace.ePlotTypes.FOverB
                    Return "F over B"
                Case frmRunEcospace.ePlotTypes.Effort
                    Return SharedResources.HEADER_EFFORT
                Case frmRunEcospace.ePlotTypes.CoverB
                    Return "C over B"
                Case frmRunEcospace.ePlotTypes.Contaminant
                    Return "Rel. contaiminants"
            End Select
            Return ""

        End Function

    End Class

End Namespace
