using NLog;

namespace ConsoleAppTemplate;

public class Configuration
{
    public string Option1   { get; set; }
    public string Option2   { get; set; }
    public string Option3	{ get; set; }

    // Hint: to make values optional, you can use the [Optional] attribute:
    // [Optional]
    // public string Option4	{ get; set; }


    public void LogOptions(ILogger logger)
    {
        //Note: To align the output in columns, set visual studio to use spaces instead of tabs!
        logger.Debug($"Option1                        : {Option1}");
        logger.Debug($"Option2                        : {Option2}");
        logger.Debug($"Option3                        : {Option3}");
    }
}
