using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Common.Settings;
using S4N.LunchToHome.Application.Deliveries.Commands.CreateDelivery;
using S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery;
using S4N.LunchToHome.Application.Deliveries.Models;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Infrastructure.Helpers;

namespace S4N.LunchToHome.ConsoleApplication.Process
{
    public class ProcessDeliveryService : IProcessDeliveryService
    {
        private readonly IRepository<Drone> droneRepository;

        private readonly IFileHelper fileHelper;

        private readonly IGeneralSettings generalSettings;

        private readonly ISender sender;

        private readonly ILogger<ProcessDeliveryService> logger;

        public ProcessDeliveryService(
            IRepository<Drone> droneRepository,
            IFileHelper fileHelper,
            IGeneralSettings generalSettings,
            ISender sender,
            ILogger<ProcessDeliveryService> logger)
        {
            this.droneRepository = droneRepository;
            this.fileHelper = fileHelper;
            this.generalSettings = generalSettings;
            this.sender = sender;
            this.logger = logger;
        }

        public async Task ProcessAsync()
        {
            var deliveries = await this.CreateDeliveries();

            this.ProcessDeliveries(deliveries);
        }

        private void ProcessDeliveries(IList<Guid> deliveries)
        {
            Action<Guid> processDelivery = async deliveryId =>
            {
                try
                {
                    await this.sender.Send(new SendDeliveryCommand { DeliveryId = deliveryId });
                }
                catch (S4N.LunchToHome.Application.Common.Exceptions.NotFoundException e)
                {
                    this.logger.LogError(e, $"Delivery not found {deliveryId}");
                }
                catch (S4N.LunchToHome.Application.Common.Exceptions.ValidationException e)
                {
                    this.logger.LogError(e, $"Validation errors processing delivery");
                }
            };

            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 10, CancellationToken = new CancellationToken() };
            var result = Parallel.ForEach(deliveries, parallelOptions, processDelivery);
        }

        private async Task<IList<Guid>> CreateDeliveries()
        {
            var drones = this.droneRepository.Items.ToList();

            var deliveries = new List<Guid>();

            foreach (var drone in drones)
            {
                var routes = this.fileHelper.GetContentFile(string.Format(this.generalSettings.InputFilePath, drone.Name));

                if (routes != null)
                {
                    var devId = await this.sender.Send(new CreateDeliveryCommand { DroneId = drone.Id, Routes = routes.Select(c => new RouteModel { Path = c }).ToList() });
                    deliveries.Add(devId);
                }
            }

            return deliveries;
        }
    }
}