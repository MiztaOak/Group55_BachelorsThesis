using System;


public class IncorrectSimulationStepException : Exception
{
    public IncorrectSimulationStepException() { }

    public IncorrectSimulationStepException(int step) : base(String.Format("Incorrect simulation step: {0}", step.ToString()))
    {

    }
}
