﻿using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Domain.Entities
{
    public class Drone : BaseEntity
    {
        public string Name { get; set; }

        public Position CurrentPosition { get; set; }
    }
}