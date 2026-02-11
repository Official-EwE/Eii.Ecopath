' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.MonteCarlo

    Public Interface IMonteCarloPlugin
        Inherits IPlugin

        ''' <summary>
        ''' The MonteCarlo model has initialized. Passes the MonteCarlo model to a plugin.
        ''' </summary>
        ''' <param name="MonteCarloAsObject">The cEcosimMonteCarlo object as an Object </param>
        Sub MontCarloInitialized(MonteCarloAsObject As Object)

        ''' <summary>
        ''' The MonteCarlo model has initialized for a run. Ecosim SS has been calculated, and
        ''' the MC will start iterating next.
        ''' </summary>
        Sub MonteCarloRunInitialized()

        ''' <summary>
        ''' The MonteCarlo has found a balanced model and it about to run Ecosim.
        ''' </summary>
        ''' <param name="TrialNumber">Number of the current trial.</param>
        ''' <param name="nIterations">Number of iteration to find a balanced Ecopath model</param>
        ''' <remarks>The Plugin Manager will marshal the call to MonteCarloBalancedEcopathModel(...) from the MonteCarlo thread onto the main thread. 
        ''' If the plugin wants to thread its process it can't block the main thread so it has to use WaitLock.Reset() to set the signal state and return immediately. 
        ''' Then once the thread has completed call WaitLock.Set() to release the signal state and allow the MonteCarlo to continue.
        ''' If the Plugin ignores the WaitLock the MonteCarlo will continue running when MonteCarloBalancedEcopathModel(...) returns.
        '''  </remarks>
        Sub MonteCarloBalancedEcopathModel(TrialNumber As Integer, nIterations As Integer)

        ''' <summary>
        ''' The MonteCarlo has completed its Ecosim run and will start another trial.
        ''' </summary>
        Sub MonteCarloEcosimRunCompleted()

        ''' <summary>
        ''' The MonteCarlo has completed its run.
        ''' </summary>
        Sub MonteCarloRunCompleted()

    End Interface

End Namespace