namespace CalculatorApp;

public partial class MainPage : ContentPage
{
    private string mathOperator;
    CalculatorState currentState;
    private double firstOperand, secondOperand;

    /// <summary>
    /// Constructor for the MainPage class.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
        mathOperator = "";
        currentState = CalculatorState.CalculationComplete;
        mainDisplay.Text = "0";
        currentCalculation.Text = "";
    }

    /// <summary>
    /// Event handler for the clear button click event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnAllClearButtonClicked(object sender, EventArgs e)
    {
        mathOperator = "";
        currentState = CalculatorState.CalculationComplete;
        firstOperand = 0;
        secondOperand = 0;

        mainDisplay.Text = "0";
        currentCalculation.Text = "";
    }

    /// <summary>
    /// Event handler for the numeric buttons click events.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnNumberButtonClicked(object sender, EventArgs e)
    {
        string value = ((Button)sender).Text;
        if (CanAppendToMainDisplay()) AppendToMainDisplay(value);
    }

    /// <summary>
    /// Determines whether text can be appended to the main display based on the length of the text
    /// in the main display and the current state of the calculator.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the length of the text in the main display is less than 16 characters 
    /// or if the calculator is in the state of an exception being found; <c>false</c> otherwise.
    /// </returns>
    private bool CanAppendToMainDisplay() => mainDisplay.Text.Length < 16 || currentState == CalculatorState.ExceptionFound;

    /// <summary>
    /// Appends the specified value to the main display.
    /// If necessary, clears the main display before appending.
    /// </summary>
    /// <param name="value">The value to be appended to the main display.</param>
    private void AppendToMainDisplay(string value)
    {
        if (ShouldClearMainDisplay()) ClearMainDisplay();
        mainDisplay.Text += value;
    }

    private bool ShouldClearMainDisplay() => mainDisplay.Text == "0" || IsWaitingForOperand();

    /// <summary>
    /// Determines whether the calculator is currently waiting for text to be entered.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the calculator is waiting for the first digit of a number; <c>false</c> otherwise.
    /// </returns>
    private bool IsWaitingForOperand() => currentState < 0;

    /// <summary>
    /// Clears the main display of the calculator.
    /// </summary>
    private void ClearMainDisplay()
    {
        mainDisplay.Text = "";
        if (IsWaitingForOperand()) ToggleCalculatorState();
    }

    /// <summary>
    /// Toggles the state of the calculator during its operation.
    /// </summary>
    /// <remarks>
    /// The method switches the calculator state between different states defined in the <see cref="CalculatorState"/>
    /// enumeration.
    /// </remarks>
    private void ToggleCalculatorState() => currentState = (CalculatorState)((int)currentState * -1);

    /// <summary>
    /// Event handler for the decimal button click event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <remarks>
    /// If the main display text does not already contain a decimal point, adds a decimal point to the end of the text.
    /// </remarks> 
    private void OnDecimalButtonClicked(object sender, EventArgs e) =>
        mainDisplay.Text = mainDisplay.Text.Contains('.') ? mainDisplay.Text : mainDisplay.Text + '.';

    /// <summary>
    /// Event handler for the operator buttons click events.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnOperatorButtonClicked(object sender, EventArgs e)
    {
        StoreMainDisplayValue();
        currentState = CalculatorState.WaitingForSecondOperand;
        mathOperator = ((Button)sender).Text;
        currentCalculation.Text = $"{Formatted(firstOperand)} {mathOperator}";
        mainDisplay.Text = "0";
    }

    /// <summary>
    /// Stores the value displayed on the main display if it can be parsed to a valid double.
    /// </summary>
    private void StoreMainDisplayValue() => PerformWithMainDisplayValue(SetOperand);

    /// <summary>
    /// Executes the specified action using the value displayed on the main display, if it can be parsed to a valid double.
    /// </summary>
    /// <param name="handle">The action to be performed with the main display value if it is a valid double.</param>
    private void PerformWithMainDisplayValue(Action<double> handle)
    {
        bool isValidDouble = double.TryParse(mainDisplay.Text, out double mainDisplayValue);
        if (isValidDouble) handle(mainDisplayValue);
    }

    /// <summary>
    /// Sets the operand value based on the current display context.
    /// </summary>
    /// <param name="value">The value to be set as the operand.</param>
    /// <remarks>
    /// If the calculator is displaying the first operand, the value provided is assigned as the first operand.
    /// Otherwise, it is assigned as the second operand.
    /// </remarks>
    private void SetOperand(double value)
    {
        if (IsDisplayingFirstOperand()) firstOperand = value;
        else secondOperand = value;
    }

    private bool IsDisplayingFirstOperand() =>
        currentState is CalculatorState.EnteringFirstOperand or CalculatorState.CalculationComplete;

    /// <summary>
    /// Event handler for the equals button click event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnEqualsButtonClicked(object sender, EventArgs e)
    {
        StoreMainDisplayValue();

        if (IsDisplayingSecondOperand())
        {
            if (IsDivisionByZero()) HandleDivisionByZero();
            else PerformCalculation();

            ResetCalculationFields();
        }
    }

    /// <summary>
    /// Determines whether the calculator is currently displaying the second operand.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if the calculator is displaying the second operand; <c>false</c> otherwise.
    /// </returns>
    private bool IsDisplayingSecondOperand() =>
        currentState is CalculatorState.EnteringSecondOperand or CalculatorState.WaitingForSecondOperand;

    /// <summary>
    /// Checks if the current operation involves division by zero.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if the division involves division by zero; <c>false</c> otherwise.
    /// </returns>
    private bool IsDivisionByZero() => secondOperand == 0 && mathOperator == "÷";

    /// <summary>
    /// Handles the scenario where division by zero occurs in the calculator.
    /// </summary>
    private void HandleDivisionByZero()
    {
        mainDisplay.Text = "Can't divide by 0";
        firstOperand = 0;
        currentState = CalculatorState.ExceptionFound;
    }

    /// <summary>
    /// Performs a calculation based on the current state and operands and updates the display.
    /// </summary>
    private void PerformCalculation()
    {
        double result = Calculate();
        mainDisplay.Text = Formatted(result);
        firstOperand = result;
        currentState = CalculatorState.CalculationComplete;
    }

    /// <summary>
    /// Calculates the result based on the specified math operator and operands.
    /// </summary>
    /// <returns>The result of the calculation.</returns>
    /// <remarks>
    /// Supported math operators are addition (+), subtraction (-), multiplication (x), and division (÷).
    /// If the provided operator is not recognized, the method returns 0.
    /// </remarks>
    private double Calculate() => mathOperator switch
    {
        "+" => firstOperand + secondOperand,
        "-" => firstOperand - secondOperand,
        "x" => firstOperand * secondOperand,
        "÷" => firstOperand / secondOperand,
        _ => 0,
    };

    private string Formatted(double value) => value.ToString("N8").TrimEnd('0').TrimEnd('.');

    /// <summary>
    /// Resets the fields specific to a calculation to their initial states.
    /// </summary>
    private void ResetCalculationFields()
    {
        currentCalculation.Text = "";
        secondOperand = 0;
    }

    /// <summary>
    /// Event handler for the change sign button click event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnChangeSignButtonClicked(object sender, EventArgs e) =>
        PerformWithMainDisplayValue(UpdateDisplayWithOpposite);

    /// <summary>
    /// Updates the display with the opposite of the current value.
    /// </summary>
    /// <param name="value">The numeric value to be negated.</param>
    private void UpdateDisplayWithOpposite(double value)
    {
        double oppositeValue = value * -1;
        SetOperand(oppositeValue);
        mainDisplay.Text = Formatted(oppositeValue);
    }

    /// <summary>
    /// Event handler for the percentage button click event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnPercentageButtonClicked(object sender, EventArgs e) =>
        PerformWithMainDisplayValue(UpdateDisplayWithPercentage);

    /// <summary>
    /// Updates the display with the percentage of the current value.
    /// </summary>
    /// <param name="value">The numeric value to be converted to a percentage.</param>
    private void UpdateDisplayWithPercentage(double value)
    {
        double percentage = value / 100;
        SetOperand(percentage);
        mainDisplay.Text = Formatted(percentage);
    }
}
