namespace CalculatorApp;

public partial class MainPage
{
    /// <summary>
    /// Represents the state of the calculator during its operation.
    /// </summary>
    private enum CalculatorState
    {
        /// <summary>
        /// Indicates that the calculator is currently in the process of entering the first operand.
        /// </summary>
        EnteringFirstOperand = 1,

        /// <summary>
        /// Indicates that the calculator is currently in the process of entering the second operand.
        /// </summary>
        EnteringSecondOperand = 2,

        /// <summary>
        /// Indicates that the calculation process has been completed.
        /// </summary>
        CalculationComplete = -1,

        /// <summary>
        /// Indicates that an exception has been encountered during the calculation process.
        /// </summary>
        ExceptionFound = -1,

        /// <summary>
        /// Indicates that the calculator is waiting for the second operand.
        /// </summary>
        WaitingForSecondOperand = -2,
    }
}
