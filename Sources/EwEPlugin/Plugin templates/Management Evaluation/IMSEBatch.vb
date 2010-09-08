

Public Interface IMSEBatch
    Inherits IPlugin

    ''' <summary>
    ''' The MSE Batch Manager has been initialized
    ''' </summary>
    ''' <param name="MSEBatchManager">Instance of cMSEBatchManager as an object.</param>
    ''' <param name="MSEBatchManagerDataStrucures">Instance of cMSEBatchManagerDataStructures as an object.</param>
    ''' <remarks></remarks>
    Sub MSEBatchInitialized(ByVal MSEBatchManager As Object, ByVal MSEBatchManagerDataStrucures As Object)

End Interface
