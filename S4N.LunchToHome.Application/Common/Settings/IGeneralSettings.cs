namespace S4N.LunchToHome.Application.Common.Settings
{
    public interface IGeneralSettings
    {
        int MaxRoutesPerDrone { get; }

        string InputFilePath { get; }

        string OutputFilePath { get; }

        string OutputHeaderText { get; }
    }
}