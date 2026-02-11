' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

''' <summary>
''' Have the core do the data validation via its cCore.Validate() method
''' </summary>
''' <remarks>This is used for variables that need to use values from other parts of the core for data validation</remarks>
Public Class cValidatorCore
    Inherits cValidatorDefault

    Private m_core As cCore

    Public Sub New(theCore As cCore)
        Me.m_core = theCore
    End Sub

    Public Overrides Function Validate(ValueObject As cValue, MetaData As cVariableMetaData,
                                         Optional iSecondaryIndex As Integer = cCore.NULL_VALUE,
                                         Optional iThirdIndex As Integer = cCore.NULL_VALUE) As Boolean
        'Call Validate in the core to do the validation
        Return Me.m_core.Validate(ValueObject, MetaData, iSecondaryIndex, iThirdIndex)

    End Function

End Class