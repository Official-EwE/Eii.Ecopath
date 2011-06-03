Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that is automatically invoked when
''' Ecopath has ran succesfully - after all IEcopathRunCompletedPlugin instances
''' have been called.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcopathRunCompletedPostPlugin
    Inherits IPlugin

    Sub EcopathRunCompletedPost(ByRef EcopathDataStructures As Object)

End Interface
