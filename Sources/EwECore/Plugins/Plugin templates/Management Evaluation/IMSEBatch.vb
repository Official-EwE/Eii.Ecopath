' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.MSE

    Public Interface IMSEBatch
        Inherits IPlugin

        ''' <summary>
        ''' The MSE Batch Manager has been initialized
        ''' </summary>
        ''' <param name="MSEBatchManager">Instance of cMSEBatchManager as an object.</param>
        ''' <param name="MSEBatchManagerDataStrucures">Instance of cMSEBatchManagerDataStructures as an object.</param>
        ''' <remarks></remarks>
        Sub MSEBatchInitialized(MSEBatchManager As Object, MSEBatchManagerDataStrucures As Object)

    End Interface

End Namespace
