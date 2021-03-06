﻿using System;

namespace Showroom.Server.Controllers
{
    internal class NotFoundException : Exception
    {
        public NotFoundException(string entity, object id)
            : base($"{entity} with Id {id} was not found.")
        {
            this.Entity = entity;
            this.Id = id;
        }

        public string Entity { get; }

        public object Id { get; }
    }
}
