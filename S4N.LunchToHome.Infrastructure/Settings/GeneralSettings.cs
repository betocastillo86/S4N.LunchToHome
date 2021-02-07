using System;
using Microsoft.Extensions.Configuration;
using S4N.LunchToHome.Application.Common.Settings;

namespace S4N.LunchToHome.Infrastructure.Settings
{
    public class GeneralSettings : IGeneralSettings
    {
        private readonly IConfiguration configuration;

        public GeneralSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public int MaxRoutesPerDrone => Convert.ToInt32(this.configuration["MaxRoutesPerDrone"]);

        public string InputFilePath => this.configuration["InputFilePath"];

        public string OutputFilePath => this.configuration["OutputFilePath"];

        public string OutputHeaderText => this.configuration["OutputHeaderText"];
    }
}